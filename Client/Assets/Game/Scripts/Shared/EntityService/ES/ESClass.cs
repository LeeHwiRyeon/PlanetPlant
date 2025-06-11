using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EntityService {
    using Vector2 = UnityEngine.Vector2;
    using Vector3 = UnityEngine.Vector3;

    public class ESClass : IEntity, IPropertyTable, ICloneable, IEnumerable<KeyValuePair<string, ESProperty>> {
        public static ESClass Create(string category, int claasId, string className)
        {
            var info = new ESClass {
                ClassId = claasId,
                ClassName = className,
                Category = category
            };
            return info;
        }

        public string Idspace { get; internal set; }
        public string Category { get; internal set; }
        public string SubCategory { get; internal set; }
        public int ClassId { get; internal set; }
        public string ClassName { get; internal set; }
        private ESClass[] m_extendParent;
        public bool AutoGenClassId { get; set; }

        private readonly Dictionary<string, ESProperty> m_properties;

        public struct NotifyProperty {
            public NotifyProperty(ESClass sender, string name, ESProperty value)
            {
                Sender = sender;
                Name = name;
                Value = value;
            }
            public readonly ESClass Sender;
            public readonly string Name;
            public readonly ESProperty Value;
        }
        public event Action<NotifyProperty> PropertyAdded;
        public event Action<NotifyProperty> PropertyRemoved;

        public ESClass()
        {
            Idspace = string.Empty;
            Category = string.Empty;
            SubCategory = string.Empty;
            ClassId = -1;
            ClassName = string.Empty;
            m_properties = new Dictionary<string, ESProperty>();
        }

        public bool ContainsKey(string key)
        {
            return m_properties.ContainsKey(key);
        }

        public void Set(string key, ESProperty property)
        {
            if (m_properties.ContainsKey(key)) {
                new Exception($"클래스는 프로퍼티({key})를 재정의 할 수 없습니다.");
            }

            m_properties.Add(key, property);
        }

        public void Set<T>(string key, T value)
        {
            if (m_properties.ContainsKey(key)) {
                new Exception($"클래스는 프로퍼티({key})를 재정의 할 수 없습니다.");
            }

            m_properties.Add(key, new ESProperty(value));
        }

        public void ReplaceProperty(string key, ESProperty value)
        {
            if (m_properties.ContainsKey(key)) {
                PropertyRemoved?.Invoke(new NotifyProperty(this, key, ESProperty.Empty));
                m_properties[key] = value;
            } else {
                m_properties.Add(key, value);
            }

            PropertyAdded?.Invoke(new NotifyProperty(this, key, value));
        }

        public ESProperty GetProperty(string key)
        {
            if (m_properties.TryGetValue(key, out var output)) {
                return output;
            }

            return ESProperty.Empty;
        }

        public Dictionary<string, ESProperty> GetProperties()
        {
            return m_properties;
        }

        internal bool TryGetProperty(string key, out ESProperty result)
        {
            if (m_properties.TryGetValue(key, out result)) {
                return true;
            }

            if (m_extendParent != null) {
                foreach (var extend in m_extendParent) {
                    if (extend.TryGetProperty(key, out result)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public T Get<T>(string key)
        {
            if (m_properties.TryGetValue(key, out var property) == false) {
                new NullReferenceException($"{key}에 프로퍼티가 없습니다.");
            }

            return property.Get<T>();
        }

        public EnumType GetEnum<EnumType>(string key, EnumType defaultValue) where EnumType : struct, IConvertible
        {
            var prop = GetProperty(key);
            var v = prop.Type;
            if (prop.Type == ValueType.Invalid) {
                return defaultValue;
            }

            if (v == ValueType.String) {
                EnumHelper<EnumType>.TryParse(prop.GetString(), out var type);
                m_properties[key] = new ESProperty(type);
                return type;
            }

            return prop.GetEnum<EnumType>();
        }

        public EnumType[] GetEnumArray<EnumType>(string key, in EnumType[] defaultValue) where EnumType : struct, IConvertible
        {
            var prop = GetProperty(key);
            var v = prop.Type;
            if (prop.Type == ValueType.Invalid) {
                return defaultValue;
            }

            if (v == ValueType.StringArray) {
                var items = prop.GetStringArray();
                var enums = new EnumType[items.Length];
                for (var i = 0; i < items.Length; i++) {
                    var item = items[i];
                    if (EnumHelper<EnumType>.TryParse(item, out var type) == false) {
                        throw new InvalidOperationException($"{Idspace}.{ClassName}.{key} 프로퍼티 접근 실패 EnumType({item})이 존재하지 않습니다.");
                    }
                    enums[i] = type;
                }

                m_properties[key] = new ESProperty(enums);
                return enums;
            } else if (v != ValueType.EnumArray && v != ValueType.IntArray) {
                return defaultValue;
            }

            return prop.GetEnumArray<EnumType>();
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                return property.GetBool();
            }

            var boolString = GetString(key);
            if (!string.IsNullOrEmpty(boolString)) {
                bool.TryParse(boolString, out defaultValue);
            }
            return defaultValue;
        }

        public bool[] GetBoolArray(string key, bool[] defaultValue = null)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                return property.GetBoolArray();
            }
            return defaultValue;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                return property.GetInt();
            }
            return defaultValue;
        }

        public int[] GetIntArray(string key, int[] defaultValue = null)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                return property.GetIntArray();
            }
            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                return property.GetFloat();
            }
            return defaultValue;
        }

        public float[] GetFloatArray(string key, float[] defaultValue = null)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                return property.GetFloatArray();
            }
            return defaultValue;
        }

        public string GetString(string key, string defaultValue = null)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                return property.GetString();
            }
            return defaultValue;
        }

        public string[] GetStringArray(string key, string[] defaultValue = null)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue ?? Array.Empty<string>();
            }

            if (prop.Type != ValueType.StringArray) {
                if (prop.Type != ValueType.String) {
                    return defaultValue ?? Array.Empty<string>();
                }
                var obj = prop.GetString().ToStringArraySafe();
                prop = new ESProperty(obj);
                ReplaceProperty(key, prop);
            }

            return (string[])prop.ValueObject;
        }
        public string[][] GetStringArray2(string key, string[][] defaultValue = null)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue ?? Array.Empty<string[]>();
            }

            if (prop.Type != ValueType.StringArray2) {
                if (prop.Type != ValueType.String) {
                    return defaultValue ?? Array.Empty<string[]>();
                }
                var obj = prop.GetString().ToStringArray2();
                prop = new ESProperty(obj);
                ReplaceProperty(key, prop);
            }

            return (string[][])prop.ValueObject;
        }

        public Vector2 GetVector2(string key, in Vector2 defaultVaule)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                var t = property.Type;
                if (t == ValueType.String) {
                    var vaule = property.GetString().ToVector3();
                    m_properties[key] = new ESProperty(vaule);
                    return vaule;
                }

                if (t != ValueType.Vector2) {
                    return defaultVaule;
                }

                return property.GetVector3();
            }
            return defaultVaule;
        }

        public Vector2[] GetVector2Array(string key, in Vector2[] defaultVaule = null)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                var t = property.Type;
                if (t == ValueType.String) {
                    var vaule = property.GetString().ToVector2Array();
                    m_properties[key] = new ESProperty(vaule);
                    return vaule;
                }

                if (t != ValueType.Vector2Array) {
                    return defaultVaule ?? Array.Empty<Vector2>();
                }

                return property.GetVector2Array();
            }

            return defaultVaule ?? Array.Empty<Vector2>();
        }

        public Vector3 GetVector3(string key, in Vector3 defaultValue)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                var t = property.Type;
                if (t == ValueType.String) {
                    var vaule = property.GetString().ToVector3();
                    m_properties[key] = new ESProperty(vaule);
                    return vaule;
                }

                if (t != ValueType.Vector3) {
                    return defaultValue;
                }

                return property.GetVector3();
            }
            return defaultValue;
        }

        public Vector3[] GetVector3Array(string key, in Vector3[] defaultValue = null)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                var t = property.Type;
                if (t == ValueType.String) {
                    var vaule = property.GetString().ToVector3Array();
                    m_properties[key] = new ESProperty(vaule);
                    return vaule;
                }

                if (t != ValueType.Vector3Array) {
                    return defaultValue ?? Array.Empty<Vector3>();
                }

                return property.GetVector3Array();
            }

            return defaultValue ?? Array.Empty<Vector3>();
        }

        public TimeSpan GetTimeSpan(string key, in TimeSpan defaultVaule)
        {
            if (m_properties.TryGetValue(key, out var property)) {
                return property.GetTimeSpan();
            }
            return defaultVaule;
        }

        public object Clone()
        {
            var esClass = new ESClass {
                ClassName = ClassName,
                ClassId = ClassId,
                Category = Category,
            };

            foreach (var property in m_properties) {
                esClass.Set(property.Key, property.Value);
            }

            return esClass;
        }

        public void SortedForeach(Action<string, ESProperty> callback)
        {
            var list = m_properties.Keys.ToList();
            list.Sort(StringComparer.Ordinal);
            foreach (var key in list) {
                callback(key, m_properties[key]);
            }
        }

        public void Foreach(Action<string, ESProperty> callback)
        {
            foreach (var pair in m_properties) {
                callback(pair.Key, pair.Value);
            }
        }

        public void SetClassId(int claasId)
        {
            ClassId = claasId;
        }

        public void SetClassName(string className)
        {
            ClassName = className;
        }

        public void SetCategory(string category)
        {
            Category = category;
        }

        internal void Extend(ESClass[] parents)
        {
            m_extendParent = parents;
        }

        public void ForeachWithExtends(Action<string, ESProperty> callback)
        {
            if (m_extendParent != null) {
                foreach (var ext in m_extendParent) {
                    ext.Foreach(callback);
                }
            }

            foreach (var pair in m_properties) {
                callback(pair.Key, pair.Value);
            }
        }

        public IEnumerator<KeyValuePair<string, ESProperty>> GetEnumerator()
        {
            return m_properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
