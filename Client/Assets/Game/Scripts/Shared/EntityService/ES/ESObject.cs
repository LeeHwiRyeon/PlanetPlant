using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EntityService {
    using Vector2 = UnityEngine.Vector2;
    using Vector3 = UnityEngine.Vector3;
    public class ESObject : IEntity, IPropertyTable, ICP, IEnumerable<KeyValuePair<string, ESProperty>> {
        private const string Logger = nameof(ESObject);

        protected ESObject() { }
        protected ESObject(ESClass baseClass)
        {
            System.Diagnostics.Debug.Assert(baseClass != null, "'baseClass'가 null입니다. ", "Parameterless 생성자를 사용해야합니다");
            m_baseClass = baseClass;
        }

        private ESObject m_parent;
        private ESClass m_baseClass;
        private readonly List<ESObject> m_children = new List<ESObject>();

        private readonly Dictionary<string, ESProperty> m_properties = new Dictionary<string, ESProperty>();
        private readonly Dictionary<string, CP> m_calculatedProperties = new Dictionary<string, CP>();
        public event Action<string, ESProperty, ESProperty>PropertyValueChanged;

        public string Idspace => m_baseClass.Idspace;
        public int ClassId => m_baseClass.ClassId;
        public string ClassName => m_baseClass.ClassName;
        public string Category => m_baseClass?.Category ?? string.Empty;
        public IReadOnlyDictionary<string, ESProperty> Properties => m_properties;

        public void SetInfo(ESClass info)
        {
            m_baseClass = info;
        }

        public void SetParent(ESObject parent)
        {
            m_parent = parent;
        }

        public void ClearProperties()
        {
            m_properties.Clear();
        }

        public void Clear()
        {
            m_baseClass = null;
            m_parent = null;
            m_properties.Clear();
            PropertyValueChanged -= PropertyValueChanged;
        }

        public void Set<Key, T>(Key key, T vaule) where Key : struct, IConvertible
        {
            if (EnumHelper<Key>.TryWrite(key, out var name)) {
                Set(name, new ESProperty(vaule));
            }
        }

        public void Set<T>(string key, T value)
        {
            Set(key, new ESProperty(value));
        }

        public void Set(string key, in ESProperty prop)
        {
            if (m_calculatedProperties.ContainsKey(key)) {
                ESCallback.Error(this, "CP({0})에는 값을 넣을 수 없음", key);
            }

            //  런타임 프로퍼티 테이블에 저장
            if (!m_properties.TryGetValue(key, out var prev)) {
                m_properties.Add(key, prop);
            } else if (prev.Equals(prop)) {
                //  이미 같은값이 Set 되어있는 경우 종료
                return;
            } else {
                m_properties[key] = prop;
            }

            PropertyValueChanged?.Invoke(key, prev, prop);
        }

        public void ReplaceProperty(string key, ESProperty vaule)
        {
            // TODO 아직 필요없음.
            throw new NotImplementedException();
        }

        public ESProperty GetProperty(string key)
        {
            if (m_calculatedProperties.TryGetValue(key, out var cp)) {
                if (!cp.m_isValid) {
                    var prevProp = cp.m_cachedProp;
                    var calcValue = cp.m_cpCallback(this);
                    if (float.IsNaN(calcValue) || double.IsInfinity(calcValue)) {
                        ESCallback.Error(this, "The formula for value '{0}' is no correct", key);
                    } else {
                        cp.m_cachedProp = new ESProperty(calcValue);
                    }

                    cp.m_isValid = true;
                    if (Math.Abs(prevProp.GetFloat() - cp.m_cachedProp.GetFloat()) > float.Epsilon) {
                        PropertyValueChanged?.Invoke(key, prevProp, cp.m_cachedProp);
                    }
                }
                return cp.m_cachedProp;
            }

            if (m_properties.TryGetValue(key, out var property)) {
                return property;
            }

            if (m_baseClass != null && m_baseClass.TryGetProperty(key, out property)) {
                return property;
            }

            return ESProperty.Empty;
        }

        public T Get<T, Key>(Key key) where Key : struct, IConvertible
        {
            if (EnumHelper<Key>.TryWrite(key, out var strKey)) {

            }

            var prop = GetProperty(strKey);
            if (prop.Type != ValueType.Invalid) {
                return prop.Get<T>();
            }

            return m_baseClass.Get<T>(strKey);
        }

        public T Get<T>(string key)
        {
            var prop = GetProperty(key);
            if (prop.Type != ValueType.Invalid) {
                return prop.Get<T>();
            }

            return m_baseClass.Get<T>(key);
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue;
            }
            return prop.GetBool();
        }

        public bool[] GetBoolArray(string key, bool[] defaultValue)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue == null ? Array.Empty<bool>() : defaultValue;
            }

            return prop.GetBoolArray();
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue;
            }

            if (prop.Type != ValueType.Int && prop.Type != ValueType.Float) {
                GameLogger.Log.Debug(Logger, $"{Idspace}.{key} : ValueType({prop.Type}) 프로퍼티 값({prop.GetString()}) 확인 필요.");
                return defaultValue;
            }

            return prop.GetInt();
        }

        public int[] GetIntArray(string key, int[] defaultValue = null)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue == null ? Array.Empty<int>() : defaultValue;
            }

            return prop.GetIntArray();
        }

        public float GetFloat(string key, float defaultValue = 0)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue;
            }

            if (prop.Type != ValueType.Int && prop.Type != ValueType.Float) {
                GameLogger.Log.Debug(Logger, $"{Idspace}.{key} : ValueType({prop.Type}) 프로퍼티 값({prop.GetString()}) 확인 필요.");
                return defaultValue;
            }

            return prop.GetFloat();
        }

        public float[] GetFloatArray(string key, float[] defaultValue = null)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue == null ? Array.Empty<float>() : defaultValue;
            }

            return prop.GetFloatArray();
        }

        public string GetString(string key, string defaultValue = null)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue;
            }

            return prop.GetString();
        }

        public string[] GetStringArray(string key, string[] defaultValue = null)
        {
            var prop = GetProperty(key);
            if (prop.Type == ValueType.Invalid) {
                return defaultValue == null ? Array.Empty<string>() : defaultValue;
            }

            return prop.GetStringArray();
        }

        public Vector2 GetVector2(string key, in Vector2 defaultVaule)
        {
            var prop = GetProperty(key);
            var type = prop.Type;
            if (type == ValueType.String) {
                var value = prop.GetString().ToVector2();
                m_properties[key] = new ESProperty(value);
                return value;
            }

            if (type != ValueType.Vector2) {
                return defaultVaule;
            }

            return prop.GetVector2();
        }

        public Vector2[] GetVector2Array(string key, in Vector2[] defaultVaule = null)
        {
            var prop = GetProperty(key);
            var type = prop.Type;
            if (type == ValueType.String) {
                var value = prop.GetString().ToVector2Array();
                m_properties[key] = new ESProperty(value);
                return value;
            }

            if (type != ValueType.Vector2Array) {
                return defaultVaule == null ? Array.Empty<Vector2>() : defaultVaule;
            }

            return prop.GetVector2Array();
        }


        public Vector3 GetVector3(string key, in Vector3 defaultValue)
        {
            var prop = GetProperty(key);
            var t = prop.Type;
            if (t == ValueType.String) {
                var value = prop.GetString().ToVector3();
                m_properties[key] = new ESProperty(value);
                return value;
            }

            if (t != ValueType.Vector3) {
                return defaultValue;
            }

            return prop.GetVector3();
        }

        public Vector3[] GetVector3Array(string key, in Vector3[] defaultValue = null)
        {
            var prop = GetProperty(key);
            var type = prop.Type;
            if (type == ValueType.String) {
                var value = prop.GetString().ToVector3Array();
                m_properties[key] = new ESProperty(value);
                return value;
            }

            if (type != ValueType.Vector3Array) {
                return defaultValue == null ? Array.Empty<Vector3>() : defaultValue;
            }

            return prop.GetVector3Array();
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
            } else if (v != ValueType.EnumArray) {
                return defaultValue;
            }

            return prop.GetEnumArray<EnumType>();
        }

        public bool ContainsKey(string key)
        {
            return m_properties.ContainsKey(key);
        }

        public void AddCP(string key, CPDelegate cpmethod)
        {
            var cp = new CP {
                m_cachedProp = ESProperty.Empty,
                m_isValid = false,
                m_cpCallback = cpmethod,
            };
            m_calculatedProperties.Add(key, cp);
        }

        public void RefreshCP()
        {
            // TODO: 호출이 많아지면 나중에 한번에 최적화 작업 진행
            foreach (var cp in m_calculatedProperties.Values) {
                cp.m_isValid = false;
            }
        }

        public void RemoveCP(string key)
        {
            key = string.Intern(key);

            if (m_calculatedProperties.ContainsKey(key)) {
                throw new InvalidOperationException("cp는 초기화할 수 없음");
            }

        }

        public bool IsCP(string propNames)
        {
            return m_calculatedProperties.ContainsKey(propNames);
        }

        public float GetInChildrenSum(string key, string[] filters)
        {
            return GetInChildren(key, filters, CalcGetInChildrenSum);
        }

        protected float GetInChildren(string key, string[] filters, Func<string, string[], float> func)
        {
            if (func == null) {
                throw new ArgumentNullException(nameof(func));
            }

            var cachedValue = func.Invoke(key, filters);
            return cachedValue;
        }

        private float CalcGetInChildrenSum(string key, string[] filters)
        {
            if (filters != null && filters.Length > 0) {
                float sum = 0;
                foreach (var c in m_children) {
                    if (filters.Contains(c.Idspace) == false) {
                        continue;
                    }

                    if (c.ContainsKey(key)) {
                        sum += c.GetFloat(key);
                    }
                }
                return sum;
            } else {
                float sum = 0;
                foreach (var c in m_children) {
                    if (c.ContainsKey(key) == false) {
                        continue;
                    }
                    sum += c.GetFloat(key);
                }
                return sum;
            }
        }

        public void AddChild(ESObject obj)
        {
            m_children.Add(obj);
            RefreshCP();
        }

        public void RemoveChild(ESObject obj)
        {
            m_children.Remove(obj);
            RefreshCP();
        }

        public ESObject[] GetChildren(string idspace)
        {
            return m_children.Where(obj => obj.Idspace == idspace).ToArray();
        }

        public ESObject[] GetChildren()
        {
            return m_children.ToArray();
        }

        public ESObject GetParent()
        {
            return m_parent;
        }

        public ESClass GetInfo()
        {
            return m_baseClass;
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
