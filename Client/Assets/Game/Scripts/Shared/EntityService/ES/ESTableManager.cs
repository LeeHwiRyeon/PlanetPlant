using EntityService.Format;
using EntityService.Util;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace EntityService {
    public static class ESTableManager {
        private static readonly Dictionary<string, ESTable> m_tables = new Dictionary<string, ESTable>();

        private static IParser m_loader;
        private static readonly Watcher m_watcher = new Watcher();

        public static void Watcher(string path)
        {
            m_watcher.Init(path);
        }

        public static bool Save(string idspace)
        {
            if (!m_tables.TryGetValue(idspace, out var table)) {
                table.Save();
                return true;
            }
            return false;
        }


        public static bool LoadFile(string fileName, string format, string encrypt_key = "")
        {
            switch (format) {
                case ".csv": {
                    if (m_loader == null || m_loader.Format != FileFormat.CSV) {
                        m_loader = new CsvParser {
                            Encrypt_key = encrypt_key,
                        };
                    }
                    var loadTable = m_loader.ParseFromFile(fileName);
                    if (loadTable == null) {
                        throw new Exception($"{nameof(CsvParser)} 구문 분석 - 파일 이름 '{fileName}' 없다.");
                    }

                    m_tables[loadTable.Idspace] = loadTable;
                }
                    break;
            }

            return true;
        }

        public static bool LoadText(string fileName, string format, string text)
        {
            switch (format) {
                case ".csv": {
                    if (m_loader == null || m_loader.Format != FileFormat.CSV) {
                        m_loader = new CsvParser();
                    }
                    var loadTable = m_loader.ParseFromText(fileName, text);
                    if (loadTable == null) {
                        throw new Exception($"{nameof(CsvParser)} 구문 분석 - 파일 이름 '{fileName}' 없다.");
                    }

                    m_tables[loadTable.Idspace] = loadTable;
                }
                    break;
            }

            return true;
        }

        public static void Drop(string table_name)
        {
            if (m_tables.ContainsKey(table_name)) {
                m_tables.Remove(table_name);
            }
        }

        public static bool ContainsTable(string idSpace)
        {
            return m_tables.ContainsKey(idSpace);
        }

        internal static ESTable GetTableCanBeNull(string idSpace)
        {
            m_tables.TryGetValue(idSpace, out var table);
            return table;
        }

        public static ESTable GetTable(string idSpace)
        {
            if (!m_tables.TryGetValue(idSpace, out var table)) {
                throw new NullReferenceException($"{idSpace} 테이블은 없습니다.");
            }

            return table;
        }

        [CanBeNull]
        public static ESClass GetClass(string idspace, int classId)
        {
            return m_tables.TryGetValue(idspace, out var table) == false ? null : table.GetClass(classId);
        }

        [CanBeNull]
        public static ESClass GetClass(string idspace, string className)
        {
            return m_tables.TryGetValue(idspace, out var table) == false ? null : table.GetClass(className);
        }

        [CanBeNull]
        public static List<ESClass> GetCategory(string idspace, string categoryName)
        {
            return m_tables.TryGetValue(idspace, out var table) == false ? null : table.GetCategory(categoryName);
        }

        public static void Clear()
        {
            m_tables.Clear();
        }

        public static IEnumerator<ESTable> GetEnumerator()
        {
            return m_tables.Values.GetEnumerator();
        }

        public static Dictionary<string, ESTable>.ValueCollection GetValueCollection()
        {
            return m_tables.Values;
        }

        public static Dictionary<string, ESTable>.KeyCollection GetNames()
        {
            return m_tables.Keys;
        }
    }
}