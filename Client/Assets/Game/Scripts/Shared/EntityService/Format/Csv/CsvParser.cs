using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EntityService.Format {
    public class CsvParser : IParser {
        public FileFormat Format => FileFormat.CSV;
        public string Encrypt_key;

        public ESTable ParseFromText(string filename, string text)
        {
            ESTable table;
            try {
                table = ParseText(filename, text);
            } catch (Exception e) {
                throw new Exception($"({typeof(CsvParser).Name}.ParseFromFile: {e.Message}");
            }
            return table;
        }

        private ESTable ParseText(string filename, string text)
        {
            // Begin parsing 구문 분석 시작
            var tableId = Path.GetFileNameWithoutExtension(filename);
            var table = ESTableManager.GetTableCanBeNull(tableId);
            if (table == null) {
                table = new ESTable();
                table.Init(tableId, filename);
            }

            if (!ParseDataText(table, tableId, text)) {
                throw new Exception($"테이블({tableId})에 구문 분석하는 중에 오류가 발생했습니다.");
            }

            return table;
        }

        private bool ParseDataText(ESTable table, string idspace, string text)
        {
            if (string.IsNullOrEmpty(idspace)) {
                var msg = $"{table.FileName}에 Idspace가 없다. \n{new StackTrace()}";
                throw new NullReferenceException(msg);
            }

            var index = table.Count;
            var stringReader = new StringReader(text);
            var line = stringReader.ReadLine();
            if (line == null) {
                return false;
            }

            var schemas = line.Split(',');
            line = stringReader.ReadLine();
            if (line == null) {
                return false;
            }

            var names = line.Split(',');
            while ((line = stringReader.ReadLine()) != null) {
                var values = line.Split(',');

                var id = ++index;
                var data = new ESClass {
                    Idspace = idspace,
                    ClassId = id,
                    AutoGenClassId = true,
                };

                if (ParseAttributes(data, names, schemas, values) == false) {
                    continue;
                }
                table.AddClass(data);
            }

            return true;
        }

        public ESTable ParseFromFile(string path)
        {
            ESTable table;
            try {
                table = Parse(path);
            } catch (Exception e) {
                throw new Exception($"({nameof(CsvParser)}.ParseFromFile: {e.Message}");
            }
            return table;
        }

        private ESTable Parse(string path)
        {
            // Begin parsing 구문 분석 시작
            var tableId = Path.GetFileNameWithoutExtension(path);
            var table = ESTableManager.GetTableCanBeNull(tableId);
            if (table == null) {
                table = new ESTable();
                table.Init(tableId, path);
            }

            if (!ParseData(table, tableId, path)) {
                throw new Exception($"테이블({tableId})에 구문 분석하는 중에 오류가 발생했습니다.");
            }

            return table;
        }


        private bool ParseData(ESTable table, string idspace, string path)
        {
            if (string.IsNullOrEmpty(idspace)) {
                var msg = $"{table.FileName}에 Idspace가 없다. \n{new StackTrace()}";
                throw new NullReferenceException(msg);
            }

            string[] lines;
            if (string.IsNullOrEmpty(Encrypt_key) == false) {
                var encryptedText = File.ReadAllText(path);
                var decryptedText = CSVUtil.EncryptOrDecrypt(encryptedText, Encrypt_key);
                lines = CSVUtil.CSVToLine(decryptedText);
            } else {
                lines = File.ReadAllLines(path);
            }

            if (lines == null) {
                return false;
            }

            table.SetSchemas(lines[0], lines[1]);
            var schemas = CSVUtil.LineToColumn(lines[0]);
            var names = CSVUtil.LineToColumn(lines[1]);
            var index = 0;
            for (var i = 2; i < lines.Length; i++) {
                var values = CSVUtil.LineToColumn(lines[i]);
                if (string.IsNullOrEmpty(values[0])) {
                    continue;
                }

                var id = ++index;
                var data = new ESClass {
                    Idspace = idspace,
                    ClassId = id,
                    AutoGenClassId = true,
                };

                if (ParseAttributes(data, names, schemas, values) == false) {
                    continue;
                }

                table.AddClass(data);
                //var replaceIndex = line.IndexOf('\"');
                //while (replaceIndex != -1) {
                //    var originText = line.Substring(replaceIndex, line.IndexOf('\"', replaceIndex + 1) - replaceIndex + ("\"").Length);
                //    var replaceText = originText.Replace(",", "{#$}");
                //    replaceText = replaceText.Replace("\"", "");
                //    line = line.Replace(originText, replaceText);
                //    replaceIndex = line.IndexOf('\"');
                //}
            }

            return true;
        }

        private bool ParseAttributes(ESClass data, string[] names, string[] schemas, string[] values)
        {
            for (var j = 0; j < values.Length; j++) {
                var name = names[j];
                var schema = schemas[j];
                var value = values[j];
                if (name == ESDataSchema.ClassId) {
                    if (int.TryParse(value, out var result) == false) {
                        return false;
                    }

                    data.ClassId = result;
                    data.AutoGenClassId = false;
                    continue;
                }

                if (name == ESDataSchema.ClassName) {
                    if (string.IsNullOrWhiteSpace(value)) {
                        return false;
                    }

                    data.ClassName = value;
                    continue;
                }

                if (name == ESDataSchema.CategoryName) {
                    if (string.IsNullOrEmpty(value)) {
                        return false;
                    }

                    data.Category = value;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(value) || value == "_") {
                    continue;
                }

                var valueType = EnumHelper<ValueType>.SafetyParse(schema, ValueType.String);
                var info = ESProperty.Empty;
                try {
                    switch (valueType) {
                        case ValueType.Enum:
                        case ValueType.String:
                            info = new ESProperty(value);
                            break;

                        case ValueType.EnumArray:
                        case ValueType.StringArray:
                            info = new ESProperty(value.ToStringArray());
                            break;
                        case ValueType.StringArray2:
                            info = new ESProperty(value.ToStringArray2());
                            break;

                        case ValueType.StringIntArray2:
                            info = new ESProperty(value.ToStringIntArray2());
                            break;

                        case ValueType.Bool:
                            info = new ESProperty(value.ToBool());
                            break;
                        case ValueType.BoolArray:
                            info = new ESProperty(value.ToBoolArraySafe());
                            break;

                        case ValueType.Int:
                            info = new ESProperty(value.ToInt());
                            break;
                        case ValueType.IntArray:
                            info = new ESProperty(value.ToIntArray());
                            break;
                        case ValueType.IntRange:
                            info = new ESProperty(value.ToIntRange());
                            break;
                        case ValueType.IntRangeArray:
                            info = new ESProperty(value.ToIntRangeArray());
                            break;

                        case ValueType.Float:
                            info = new ESProperty(value.ToFloat());
                            break;
                        case ValueType.FloatArray:
                            info = new ESProperty(value.ToFloatArray());
                            break;
                        case ValueType.FloatRange:
                            info = new ESProperty(value.ToFloatRange());
                            break;
                        case ValueType.FloatRangeArray:
                            info = new ESProperty(value.ToFloatRangeArray());
                            break;

                        case ValueType.Vector2:
                            info = new ESProperty(value.ToVector2());
                            break;
                        case ValueType.Vector2Array:
                            info = new ESProperty(value.ToVector2Array());
                            break;

                        case ValueType.Vector3:
                            info = new ESProperty(value.ToVector3());
                            break;
                        case ValueType.Vector3Array:
                            info = new ESProperty(value.ToVector3Array());
                            break;

                        case ValueType.Time:
                            info = new ESProperty(value.ToTimeSpan());
                            break;
                    }
                } catch (Exception) {
                    throw new Exception($"데이터({data.Idspace}.{data.ClassName})를 구문 분석하는 중에 오류가 발생했습니다. [schema({schema}):name({name}):value({value})]");
                }

                data.Set(name, info);
            }
            return true;
        }

        public static string EncryptOrDecrypt(string text, string key)
        {
            var result = new StringBuilder();

            for (var c = 0; c < text.Length; c++) {
                result.Append((char)(text[c] ^ (uint)key[c % key.Length]));
            }

            return result.ToString();
        }
    }

    public class CSVUtil {
        //행으로 나눈다.
        public static string[] CSVToLine(string str)
        {
            var lineArr = str.Split('\n');
            return lineArr;
        }

        //열으로 나눈다.
        public static string[] LineToColumn(string line)
        {
            var columnArr = System.Text.RegularExpressions.Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            for (var i = 0; i < columnArr.Length; i++) {
                columnArr[i] = columnArr[i].TrimStart(' ', '"').TrimEnd('"', '\r');
                columnArr[i] = columnArr[i].Replace("#n", "\n");
            }
            return columnArr;
        }

        public static string[] LineToColumn2(string line)
        {
            var columnArr = System.Text.RegularExpressions.Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            return columnArr;
        }

        public static string EncryptOrDecrypt(string text, string key)
        {
            var result = new StringBuilder();
            for (var c = 0; c < text.Length; c++) {
                result.Append((char)(text[c] ^ (uint)key[c % key.Length]));
            }

            return result.ToString();
        }
    }
}
