using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EntityService {
    public class ESTable : IEnumerable<KeyValuePair<int, ESClass>>, ICloneable {
        public string FileName { get; private set; }
        public string Idspace { get; private set; }
        public int Count => m_classById.Count;

        private readonly Dictionary<int, ESClass> m_classById;
        private readonly Dictionary<string, ESClass> m_classByName;
        private Dictionary<string, List<ESClass>> m_classListByCategoryName;

        private StringBuilder[] m_keyValuePair = new StringBuilder[2];
        private string[] m_schemas;
        public int LastClassId { get; private set; }

        public ESTable()
        {
            m_classById = new Dictionary<int, ESClass>();
            m_classByName = new Dictionary<string, ESClass>();
        }

        public void Init(string idSpace, string fileName)
        {
            Idspace = idSpace;
            FileName = fileName;
        }

        public bool ContainsKey(string key)
        {
            return m_classByName.ContainsKey(key);
        }

        public bool ContainsKey(int key)
        {
            return m_classById.ContainsKey(key);
        }

        public bool AddClass(ESClass esClass, bool inspectionPass = false)
        {
            esClass.Idspace = Idspace;
            if (inspectionPass == false) {
                if ((string.IsNullOrWhiteSpace(esClass.ClassName) && string.IsNullOrWhiteSpace(esClass.Category))
                    || esClass.ClassId < 0) {
                    var msg = $"{ESDataSchema.Idspace}:{esClass.Idspace}({!string.IsNullOrWhiteSpace(esClass.Idspace)}), "
                            + $"{ESDataSchema.ClassName}:{esClass.ClassName}({!string.IsNullOrWhiteSpace(esClass.ClassName)}), "
                            + $"{ESDataSchema.ClassId}:{esClass.ClassId}({esClass.ClassId > 0})"
                            + $"\n{new StackTrace()}";
                    throw new NullReferenceException(msg);
                }

                if (m_classById.ContainsKey(esClass.ClassId)) {
                    throw new NullReferenceException($"파일명[{FileName}]:{Idspace} 테이블 {ESDataSchema.ClassId} 중복 {esClass.ClassId} \n{new StackTrace()}");
                }

                if (m_classByName.ContainsKey(esClass.ClassName)) {
                    throw new NullReferenceException($"파일명[{FileName}]:{Idspace} 테이블 {ESDataSchema.ClassName} 중복 {esClass.ClassName} \n{new StackTrace()}");
                }

                if (LastClassId < esClass.ClassId) {
                    LastClassId = esClass.ClassId;
                }

                m_classById.Add(esClass.ClassId, esClass);
                if (string.IsNullOrWhiteSpace(esClass.ClassName) == false) {
                    m_classByName.Add(esClass.ClassName, esClass);
                }

                if (string.IsNullOrWhiteSpace(esClass.Category) == false) {
                    m_classListByCategoryName ??= new Dictionary<string, List<ESClass>>();
                    if (m_classListByCategoryName.TryGetValue(esClass.Category, out var list) == false) {
                        list = new List<ESClass>();
                        m_classListByCategoryName.Add(esClass.Category, list);
                    }
                    list.Add(esClass);
                }
            }

            return true;
        }

        public bool RemoveClass(in int claasId)
        {
            if (m_classById.TryGetValue(claasId, out var es)) {
                m_classById.Remove(claasId);
            }

            if (m_classByName.ContainsKey(es.ClassName)) {
                m_classByName.Remove(es.ClassName);
            }

            return true;
        }

        public bool RemoveClass(ESClass entityClass)
        {
            if (m_classById.ContainsKey(entityClass.ClassId)) {
                m_classById.Remove(entityClass.ClassId);
            }

            if (m_classByName.ContainsKey(entityClass.ClassName)) {
                m_classByName.Remove(entityClass.ClassName);
            }

            return true;
        }

        [CanBeNull]
        public List<ESClass> GetCategory(string key)
        {
            if (m_classListByCategoryName == null) {
                return null;
            }

            if (m_classListByCategoryName.TryGetValue(key, out var list) == false) {

            }

            return list;
        }

        [CanBeNull]
        public ESClass GetClass(string key)
        {
            if (string.IsNullOrEmpty(key)) {
                return null;
            }
            return m_classByName.TryGetValue(key, out var info) == false ? null : info;
        }

        [CanBeNull]
        public ESClass GetClass(in int key)
        {
            return m_classById.TryGetValue(key, out var info) == false ? null : info;
        }

        [CanBeNull]
        public ESClass GetClassByIndex(int index)
        {
            return index >= 0 && index < Count ? m_classById.Values.ElementAt(index) : null;
        }

        public IEnumerator<KeyValuePair<int, ESClass>> GetEnumerator()
        {
            return m_classById.GetEnumerator();

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_classById.GetEnumerator();
        }

        public object Clone()
        {
            var table = new ESTable {
                Idspace = Idspace,
                FileName = FileName
            };

            for (var i = 0; i < Count; i++) {
                if (GetClassByIndex(i)?.Clone() is ESClass cls) {
                    table.AddClass(cls);
                }
            }

            return table;
        }


        public void SetSchemas(string type, string name)
        {
            if (m_schemas == null) {
                m_schemas = new string[2];
            }
            m_schemas[0] = type;
            m_schemas[1] = name;
        }

        public void Save()
        {
            var text1 = new StringBuilder();
            text1.Append(m_schemas[0]);
            text1.AppendLine();
            text1.Append(m_schemas[1]);

            foreach (var info in m_classByName) {
                //text1.Append(info.Value.ClassId);
                //text1.Append(",");
                text1.Append(info.Value.ClassName);
                text1.Append(",");
                foreach (var prop in info.Value) {
                    var type = prop.Value.Type;

                    if (type == ValueType.StringArray) {
                        text1.Append(prop.Value.GetString());
                    } else {
                        text1.Append(prop.Value.ValueObject);
                    }

                    text1.Append(",");
                }
                text1.AppendLine();
            }
            ;
            File.WriteAllText(FileName, text1.ToString());
        }
    }
}