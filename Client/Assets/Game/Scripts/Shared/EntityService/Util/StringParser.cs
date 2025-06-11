using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Util;

namespace EntityService {
    using Vector2 = UnityEngine.Vector2;
    using Vector3 = UnityEngine.Vector3;

    public static class EmptyDictionary<TKey, TValue> {
        public static readonly Dictionary<TKey, TValue> Empty = new Dictionary<TKey, TValue>();
    }

    public static class StringParser {
        private static readonly char[] separators = { ',', ';' };
        private static readonly char[] rangeOuterTrimChars = { '[', ']', ' ' };
        private static readonly char[] vectorOuterTrimChars = { '(', ')', ' ' };
        private static readonly char[] vectorSeparators = { '_' };
        private static readonly char[] rangeSeparators = { ',', ';', '-' };

        public static string[] ToStringArray(this string property)
        {
            return property.AsSpan().ToStringArray();
        }

        public static string[][] ToStringArray2(this string property)
        {
            return property.AsSpan().ToStringArray2();
        }
        public static KeyValuePair<string, int>[][] ToStringIntArray2(this string property)
        {
            return property.AsSpan().ToStringIntArray2();
        }

        public static string[] ToStringArraySafe(this string property, params char[] s)
        {
            return property.AsSpan().ToStringArraySafe(s);
        }

        public static Vector3[] ToVector3Array(this string property)
        {
            return property.AsSpan().ToVector3Array();
        }

        public static Vector2[] ToVector2Array(this string property)
        {
            return property.AsSpan().ToVector2Array();
        }

        public static int[] ToIntArrayOrDefault(this string property)
        {
            return property.AsSpan().ToIntArrayOrDefault();
        }

        public static int[] ToIntArray(this string property)
        {
            return property.AsSpan().ToIntArray();
        }

        public static float[] ToFloatArray(this string property)
        {
            return property.AsSpan().ToFloatArray();
        }

        public static float[] ToFloatArrayOrDefault(this string property)
        {
            return property.AsSpan().ToFloatArrayOrDefault();
        }

        public static Range<int> ToIntRange(this string property)
        {
            return property.AsSpan().ToIntRange();
        }

        public static Range<int>[] ToIntRangeArray(this string property)
        {
            return property.AsSpan().ToIntRangeArray();
        }

        public static Range<float> ToFloatRange(this string property)
        {
            return property.AsSpan().ToFloatRange();
        }

        public static Range<float>[] ToFloatRangeArray(this string property)
        {
            return property.AsSpan().ToFloatRangeArray();
        }

        public static Vector2 ToVector2(this string property)
        {
            return property.AsSpan().ToVector2();
        }

        public static Vector3 ToVector3(this string property)
        {
            return property.AsSpan().ToVector3();
        }

        public static Vector3 ToHexColor(this string property)
        {
            return property.AsSpan().ToHexColor();
        }
        public static KeyValuePair<string, int> ToStringInt(this string property, int defaultValue = 1)
        {
            return property.AsSpan().ToStringInt(defaultValue);
        }

        public static KeyValuePair<int, string> ToIntString(this string property, string defaultValue = "")
        {
            return property.AsSpan().ToIntString(defaultValue);
        }

        public static KeyValuePair<int, int> ToIntInt(this string property, int defaultValue = 1)
        {
            return property.AsSpan().ToIntInt(defaultValue);
        }

        public static KeyValuePair<string, string> ToStringString(this string property)
        {
            return property.AsSpan().ToStringString();
        }

        public static KeyValuePair<string, Vector2>[] ToStringVector2Array(this string property)
        {
            return property.AsSpan().ToStringVector2Array();
        }

        public static KeyValuePair<string, Vector2> ToStringVector2(this string property)
        {
            return property.AsSpan().ToStringVector2();
        }

        public static KeyValuePair<string, bool> ToStringBool(this string property, bool defValue = true)
        {
            return property.AsSpan().ToStringBool(defValue);
        }

        public static KeyValuePair<string, float> ToStringFloat(this string property, float defaultValue = 0.0f)
        {
            return property.AsSpan().ToStringFloat(defaultValue);
        }

        public static KeyValuePair<float, int> ToFloatInt(this string property, int defaultValue = 0)
        {
            return property.AsSpan().ToFloatInt(defaultValue);
        }

        public static Dictionary<string, KeyValuePair<string, float>> ToStringFloatPairDictionary(this string property, float defaultValue = 0.0f)
        {
            return property.AsSpan().ToStringFloatPairDictionary(defaultValue);
        }

        public static Dictionary<string, KeyValuePair<string, bool>> ToStringBoolPairDictionary(this string property)
        {
            return property.AsSpan().ToStringBoolPairDictionary();
        }

        public static Dictionary<string, string> ToStringDictionary(this string property)
        {
            return property.AsSpan().ToStringDictionary();
        }

        public static Dictionary<string, List<string>> ToStringListDictionary(this string property)
        {
            return property.AsSpan().ToStringListDictionary();
        }

        public static Dictionary<string, int> ToStringIntDictionary(this string property, int defValue = 1)
        {
            return property.AsSpan().ToStringIntDictionary(defValue);
        }

        public static Dictionary<int, string> ToIntStringDictionary(this string property, string defValue = "")
        {
            return property.AsSpan().ToIntStringDictionary(defValue);
        }

        public static Dictionary<string, List<int>> ToStringIntListDictionary(this string property, int defValue = 1)
        {
            return property.AsSpan().ToStringIntListDictionary(defValue);
        }

        public static Dictionary<int, int> ToIntIntDictionary(this string property, int defValue = 1)
        {
            return property.AsSpan().ToIntIntDictionary(defValue);
        }

        public static Dictionary<string, float> ToStringFloatDictionary(this string property, float defValue = 0.0f)
        {
            return property.AsSpan().ToStringFloatDictionary(defValue);
        }

        public static Dictionary<string, bool> ToStringBoolDictionary(this string property, bool defValue = true)
        {
            return property.AsSpan().ToStringBoolDictionary(defValue);
        }

        public static Dictionary<string, List<KeyValuePair<string, string>>> ToKeyValueListDictionary(this string property)
        {
            return property.AsSpan().ToKeyValueListDictionary();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int _IntParseOrDef(ReadOnlySpan<char> s)
        {
            return int.TryParse(s.ToString(), out var val) ? val : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float _FloatParseOrDef(ReadOnlySpan<char> s)
        {
            return float.TryParse(s.ToString(), out var val) ? val : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool _BoolParseOrDef(ReadOnlySpan<char> s)
        {
            return bool.TryParse(s.ToString(), out var val) ? val : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int _IntParse(ReadOnlySpan<char> s)
        {
            return int.Parse(s.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float _FloatParse(ReadOnlySpan<char> s)
        {
            return float.Parse(s.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool _BoolParse(ReadOnlySpan<char> s)
        {
            return bool.Parse(s.ToString());
        }

        public static int ToInt(this string value)
        {
            return int.Parse(value);
        }

        public static int ToIntOrDefault(this string value, int @default = 0)
        {
            if (int.TryParse(value, out var v)) {
                return v;
            }
            return @default;
        }

        public static float ToFloat(this string value)
        {
            return float.Parse(value);
        }

        public static bool ToBool(this string value)
        {
            return bool.Parse(value);
        }

        public static bool ToBoolOrDefault(this string value, bool def = false)
        {
            if (bool.TryParse(value, out var v)) {
                return v;
            }
            return def;
        }

        public static bool[] ToBoolArraySafe(this string value)
        {
            return value.AsSpan()._ToArraySafe(_BoolParseOrDef);
        }

        /// <summary> true or false 무조건 반환 </summary>
        public static bool ToTryBool(this string value, bool defaultValue = false)
        {
            if (bool.TryParse(value, out var result)) {
                return result;
            } else {
                return defaultValue;
            }
        }

        public static E ToEnum<E>(this string value) where E : struct, IConvertible
        {
            return EnumHelper<E>.Parse(value);
        }

        public static E ToEnumSafty<E>(this string value, E defValue) where E : struct, IConvertible
        {
            if (!EnumHelper<E>.TryParse(value, out var type)) {
                type = defValue;
            }
            return type;
        }

        public static Range<int> ToIntRange(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                throw new ArgumentException($"{nameof(ToIntRange)}::'property' was null or empty");
            }

            //  괄호 하나를 받음
            var start = property.IndexOf('[');
            if (start == -1) {
                throw new FormatException($"{nameof(ToIntRange)}::[ expected in IntRange");
            }

            var end = property.LastIndexOf(']');
            if (end == -1) {
                throw new FormatException($"{nameof(ToIntRange)}::] expected in IntRange");
            }

            var idx = 0;
            var elem0 = 0;
            var elem1 = 0;
            foreach (var entry in property.Trim(rangeOuterTrimChars).Split(rangeSeparators)) {
                if (idx == 0) {
                    if (!int.TryParse(entry.ToString(), out elem0)) {
                        throw new FormatException($"{nameof(ToIntRange)}::fail to parse min value as integer:" + entry.ToString());
                    }

                    elem1 = elem0;
                } else if (idx == 1) {
                    if (!int.TryParse(entry.ToString(), out elem1)) {
                        throw new FormatException($"{nameof(ToIntRange)}::fail to parse max value as integer:" + entry.ToString());
                    }

                } else {
                    break;
                }

                ++idx;
            }

            return new Range<int>(elem0, elem1);
        }

        public static Range<int>[] ToIntRangeArray(this ReadOnlySpan<char> property)
        {
            return property._ToArray(ToIntRange, '[', ']');
        }

        public static Range<float> ToFloatRange(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                throw new ArgumentException("'property' was null or empty");
            }

            //  괄호 하나를 받음
            var start = property.IndexOf('[');
            if (start == -1) {
                throw new FormatException($"{nameof(ToFloatRange)}:: [ expected in FloatRange");
            }

            var end = property.LastIndexOf(']');
            if (end == -1) {
                throw new FormatException($"{nameof(ToFloatRange)}:: ] expected in FloatRange");
            }

            var idx = 0;
            float elem0 = 0;
            float elem1 = 0;
            foreach (var entry in property.Trim(rangeOuterTrimChars).Split(rangeSeparators)) {
                if (idx == 0) {
                    if (!float.TryParse(entry.ToString(), out elem0)) {
                        throw new FormatException($"{nameof(ToFloatRange)}::fail to parse min value as float:" + entry.ToString());
                    }

                    elem1 = elem0;
                } else if (idx == 1) {
                    if (!float.TryParse(entry.ToString(), out elem1)) {
                        throw new FormatException($"{nameof(ToFloatRange)}::fail to parse max value as float:" + entry.ToString());
                    }

                } else {
                    break;
                }

                ++idx;
            }

            return new Range<float>(elem0, elem1);
        }

        public static Range<float>[] ToFloatRangeArray(this ReadOnlySpan<char> property)
        {
            return property._ToArray(ToFloatRange, '[', ']');
        }

        public static Vector2 ToVector2(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                throw new ArgumentException($"{nameof(ToVector2)}::property was null or empty");
            }

            //  괄호 하나를 받음
            var start = property.IndexOf('(');
            if (start == -1) {
                throw new FormatException($"{nameof(ToVector2)}::( expected in Vector2");
            }

            var end = property.IndexOf(')');
            if (end == -1) {
                throw new FormatException($"{nameof(ToVector2)}::) expected in Vector2");
            }

            var idx = 0;
            float elem0 = 0;
            float elem1 = 0;
            foreach (var entry in property.Trim(vectorOuterTrimChars).Split(vectorSeparators)) {
                if (idx == 0) {
                    elem0 = _FloatParseOrDef(entry);
                } else if (idx == 1) {
                    elem1 = _FloatParseOrDef(entry);
                } else {
                    throw new FormatException($"{nameof(ToVector2)}::element count invalid in ToVector2");
                }

                ++idx;
            }

            return idx == 2 ? new Vector2(elem0, elem1) : throw new FormatException($"{nameof(ToVector2)}::Invalid element count in Vector2");
        }

        public static Vector3 ToVector3(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                throw new ArgumentException($"{nameof(ToVector3)}::property was null or empty");
            }

            if (property.Contains("#".AsSpan(), StringComparison.Ordinal)) {
                return property.ToHexColor();
            }

            //  괄호 하나를 받음
            var start = property.IndexOf('(');
            if (start == -1) {
                throw new FormatException($"{nameof(ToVector3)}::( expected in Vector3");
            }

            var end = property.IndexOf(')');
            if (end == -1) {
                throw new FormatException($"{nameof(ToVector3)}::) expected in Vector3");
            }

            var idx = 0;
            float elem0 = 0;
            float elem1 = 0;
            float elem2 = 0;
            foreach (var entry in property.Trim(vectorOuterTrimChars).Split(vectorSeparators)) {
                if (idx == 0) {
                    elem0 = _FloatParseOrDef(entry);
                } else if (idx == 1) {
                    elem1 = _FloatParseOrDef(entry);
                } else if (idx == 2) {
                    elem2 = _FloatParseOrDef(entry);
                } else {
                    throw new FormatException($"{nameof(ToVector3)}::element count invalid in Vector3");
                }

                ++idx;
            }

            return idx == 3 ? new Vector3(elem0, elem1, elem2) : throw new FormatException($"{nameof(ToVector3)}::Invalid element count in Vector3");
        }

        public static Vector3 ToHexColor(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                throw new ArgumentException("property was null or empty");
            }

            var start = property.IndexOf('#');
            if (start == -1) {
                throw new FormatException("# expected in Hex Color Vector3");
            }

            start += 1;
            var r = property.Slice(start, 2).ToString();
            var g = property.Slice(start + 2, 2).ToString();
            var b = property.Slice(start + 4, 2).ToString();

            return new Vector3(Convert.ToInt32(r, 16) / 255f,
                               Convert.ToInt32(g, 16) / 255f,
                               Convert.ToInt32(b, 16) / 255f);
        }

        public static int ToColorRGB(this string property)
        {
            if (string.IsNullOrEmpty(property)) {
                throw new ArgumentException("'property' was null or empty");
            }

            var tokens = property.Split(vectorSeparators);
            if (tokens.Length < 3) {
                throw new FormatException("element count was not enough.");
            }

            var r = Convert.ToByte(tokens[0]);
            var g = Convert.ToByte(tokens[1]);
            var b = Convert.ToByte(tokens[2]);
            return ((r & 0xff) << 16) + ((g & 0xff) << 8) + (b & 0xff);
        }

        /// <summary>
        /// d.hh:mm:ss
        /// https://docs.microsoft.com/ko-kr/dotnet/api/system.timespan.parse?view=netframework-4.8 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string str)
        {
            return TimeSpan.Parse(str);
        }

        /// <summary>
        /// yy/MM/dd hh:mm:ss
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str)
        {
            return DateTime.Parse(str);
        }


        #region 복수 항목
        /// <summary> 특정한 룰을 따라 스트링 어레이로 전환합니다. 예외를 던지지 않습니다. </summary>
        /// <returns>
        ///     프로퍼티가 중괄호로 싸여져 있고 콤마로 분리된 스트링 리스트일 경우 갯수 만큼 어레이를 반환합니다. 
        ///     아닌 경우 그냥 하나만 반환합니다. 
        ///     빈 프로퍼티인 경우 길이가 0인 어레이를 반환합니다.
        /// </returns>
        public static string[] ToStringArraySafe(this ReadOnlySpan<char> property, params char[] s)
        {
            return property._ToArraySafe(s1 => s1.Trim().ToString(), s);
        }

        /// <summary> 특정한 룰을 따라 스트링 어레이로 전환합니다. 예외를 던지지 않습니다. </summary>
        /// <returns>
        ///     프로퍼티가 중괄호로 싸여져 있고 콤마로 분리된 스트링 리스트일 경우 갯수 만큼 어레이를 반환합니다. 
        ///     아닌 경우 그냥 하나만 반환합니다. 
        ///     빈 프로퍼티인 경우 길이가 0인 어레이를 반환합니다.
        /// </returns>
        public static string[] ToStringArray(this ReadOnlySpan<char> property)
        {
            return property._ToArray(s => s.Trim().ToString());
        }

        public static string[][] ToStringArray2(this ReadOnlySpan<char> property)
        {
            return property._ToArray2(s => s.Trim().ToString());
        }

        public static KeyValuePair<string, int>[][] ToStringIntArray2(this ReadOnlySpan<char> property)
        {
            return property._ToArray2(ToStringInt, ':');
        }

        public static Vector3[] ToVector3Array(this ReadOnlySpan<char> property)
        {
            return property._ToArray(ToVector3, '(', ')');
        }

        public static Vector2[] ToVector2Array(this ReadOnlySpan<char> property)
        {
            return property._ToArray(ToVector2, '(', ')');
        }

        public static KeyValuePair<bool, string>[][] ToBoolStringArray(this string property)
        {
            if (string.IsNullOrEmpty(property)) {
                throw new ArgumentException("property was null or empty");
            }

            //  괄호 하나를 받음
            var start = property.IndexOf('{');
            if (start == -1) {
                throw new FormatException("{ expected in Vector2List");
            }

            var end = property.LastIndexOf('}');
            if (end == -1) {
                throw new FormatException("} expected in Vector2List");
            }

            var result = new List<KeyValuePair<bool, string>[]>();
            var substring = property.Substring(start + 1, end - start - 1);
            substring = substring.Trim();
            var i = 0;
            while (i < substring.Length) {
                var start2 = substring.IndexOf('(', i);
                if (start2 == -1) {
                    throw new FormatException("( expected in Vector2List");
                }

                var end2 = substring.IndexOf(')', i);
                if (end2 == 0) {
                    throw new FormatException(") expected in Vector2List");
                }

                i = end2 + 1;

                var separsators2 = substring.Substring(start2 + 1, end2 - start2 - 1);
                var elements = separsators2.Split(separators);
                if (elements.Length > 0) {
                    var items = new List<KeyValuePair<bool, string>>(elements.Length);
                    foreach (var element in elements) {
                        var strs = element.Split(':');
                        if (strs.Length != 2) {
                            continue;
                        }

                        items.Add(new KeyValuePair<bool, string>(bool.Parse(strs[0]), strs[1]));
                    }

                    if (items.Count > 0) {
                        result.Add(items.ToArray());
                    }
                }
            }

            return result.ToArray();
        }


        /// <summary> int 배열을 반환 합니다. property 가 비어(string empty or null) 있다면, 빈 int 배열을 반환 합니다. 예외를 던지지 않습니다. </summary>
        public static int[] ToIntArrayOrDefault(this ReadOnlySpan<char> property)
        {
            return property._ToArraySafe(_IntParseOrDef);
        }

        public static int[] ToIntArray(this ReadOnlySpan<char> property)
        {
            return property._ToArray(_IntParse);
        }

        public static float[] ToFloatArray(this ReadOnlySpan<char> property)
        {
            return property._ToArray(_FloatParse);
        }

        public static float[] ToFloatArrayOrDefault(this ReadOnlySpan<char> property)
        {
            return property._ToArraySafe(_FloatParseOrDef);
        }
        #endregion

        /// <summary>  콜론으로 분리된 스트링을 keyValue 로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static KeyValuePair<string, int> ToStringInt(this ReadOnlySpan<char> property, int defaultValue = 1)
        {
            return property.ToKeyValuePair(defaultValue, s => s.Trim().ToString(), _IntParseOrDef);
        }

        public static KeyValuePair<string, int> ToStringInt(this ReadOnlySpan<char> property)
        {
            return property.ToKeyValuePair(0, s => s.Trim().ToString(), _IntParseOrDef);
        }

        public static KeyValuePair<int, string> ToIntString(this ReadOnlySpan<char> property, string defaultValue = "")
        {
            return property.ToKeyValuePair(defaultValue, _IntParseOrDef, s => s.Trim().ToString());
        }

        public static KeyValuePair<int, int> ToIntInt(this ReadOnlySpan<char> property, int defaultValue = 1)
        {
            return property.ToKeyValuePair(defaultValue, _IntParseOrDef, _IntParseOrDef);
        }

        /// <summary>  콜론으로 분리된 스트링을 keyValue 로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static KeyValuePair<string, string> ToStringString(this ReadOnlySpan<char> property)
        {
            return property.ToKeyValuePair(string.Empty, s => s.Trim().ToString(), s => s.Trim().ToString());
        }

        public static KeyValuePair<string, Vector2>[] ToStringVector2Array(this ReadOnlySpan<char> property)
        {
            return property._ToArray(ToStringVector2, '(', ')');
        }

        public static KeyValuePair<string, Vector2> ToStringVector2(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                return new KeyValuePair<string, Vector2>();
            }

            var idx = 0;
            var key = string.Empty;
            var val = Vector2.zero;
            foreach (var entry in property.Trim(vectorOuterTrimChars).Split(':')) {
                if (idx == 0) {
                    key = entry.ToString();
                } else if (idx == 1) {
                    var idxVector2 = 0;
                    float elem0 = 0;
                    float elem1 = 0;
                    foreach (var entry2 in entry.Split(separators)) {
                        if (idxVector2 == 0) {
                            elem0 = _FloatParseOrDef(entry2);
                        } else if (idxVector2 == 1) {
                            elem1 = _FloatParseOrDef(entry2);
                        } else {
                            throw new FormatException("_ToStringVector2 - element count invalid in ToVector2");
                        }

                        ++idxVector2;
                    }

                    val = new Vector2(elem0, elem1);
                } else {
                    break;
                }

                ++idx;
            }

            return new KeyValuePair<string, Vector2>(key, val);
        }

        /// <summary> 특정한 룰을 따라 keyValue 로 전환합니다. 예외를 던지지 않습니다. </summary>
        /// <returns>
        ///     콜론으로 분리된 스트링을 keyValue 로 반환합니다.
        /// </returns>
        public static KeyValuePair<string, bool> ToStringBool(this ReadOnlySpan<char> property, bool defValue = true)
        {
            return property.ToKeyValuePair(defValue, s => s.Trim().ToString(), _BoolParseOrDef);
        }

        public static KeyValuePair<string, float> ToStringFloat(this ReadOnlySpan<char> property, float defaultValue = 0.0f)
        {
            return property.ToKeyValuePair(defaultValue, s => s.Trim().ToString(), _FloatParseOrDef);
        }

        public static KeyValuePair<float, int> ToFloatInt(this ReadOnlySpan<char> property, int defaultValue = 0)
        {
            return property.ToKeyValuePair(defaultValue, _FloatParseOrDef, _IntParseOrDef);
        }

        public static Dictionary<string, KeyValuePair<string, float>> ToStringFloatPairDictionary(this ReadOnlySpan<char> property, float defaultValue = 0.0f)
        {
            if (property.IsEmpty) {
                return EmptyDictionary<string, KeyValuePair<string, float>>.Empty;
            }
            var stringArr = property.ToStringArraySafe();
            if (stringArr.Length == 0) {
                return EmptyDictionary<string, KeyValuePair<string, float>>.Empty;
            }

            var dic = new Dictionary<string, KeyValuePair<string, float>>(stringArr.Length, StringComparer.Ordinal);
            foreach (var stringData in stringArr) {
                var keyValue = stringData.Split(':');
                if (keyValue.Length != 3) {
                    continue;
                }

                if (dic.ContainsKey(keyValue[0])) {
                    continue;
                }

                if (float.TryParse(keyValue[2], out var value)) {
                    dic.Add(keyValue[0], new KeyValuePair<string, float>(keyValue[1], value));
                }
            }

            return dic;
        }

        public static Dictionary<string, KeyValuePair<string, bool>> ToStringBoolPairDictionary(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                return EmptyDictionary<string, KeyValuePair<string, bool>>.Empty;
            }
            var stringArr = property.ToStringArraySafe();
            if (stringArr.Length == 0) {
                return EmptyDictionary<string, KeyValuePair<string, bool>>.Empty;
            }

            var dic = new Dictionary<string, KeyValuePair<string, bool>>(stringArr.Length, StringComparer.Ordinal);
            foreach (var stringData in stringArr) {
                var keyValue = stringData.Split(':');
                if (keyValue.Length != 3) {
                    continue;
                }

                if (dic.ContainsKey(keyValue[0])) {
                    continue;
                }

                if (bool.TryParse(keyValue[2], out var value)) {
                    dic.Add(keyValue[0], new KeyValuePair<string, bool>(keyValue[1], value));
                }
            }

            return dic;
        }

        /// <summary>  콜론으로 분리된 스트링들을 Dictionary로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static Dictionary<string, string> ToStringDictionary(this ReadOnlySpan<char> property)
        {
            return property._ToDictionary(string.Empty, s => s.Trim().ToString(), StringComparer.Ordinal, s => s.Trim().ToString(), (s, s1) => s1);
        }

        /// <summary> 
        /// 콜론으로 분리된 스트링들을 Dictionary로 전환합니다. 예외를 던지지 않습니다.
        /// 중복된 키를 입력하면 리스트에 추가합니다.
        /// </summary>
        public static Dictionary<string, List<string>> ToStringListDictionary(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                return EmptyDictionary<string, List<string>>.Empty;
            }

            var stringArr = ToStringArraySafe(property);
            if (stringArr.Length == 0) {
                return EmptyDictionary<string, List<string>>.Empty;
            }

            var stringDic = new Dictionary<string, List<string>>(stringArr.Length, StringComparer.Ordinal);
            foreach (var stringData in stringArr) {
                var keyVal = ToStringString(stringData.AsSpan());
                if (stringDic.TryGetValue(keyVal.Key, out var list)) {
                    list.Add(keyVal.Value);
                } else {
                    list = new List<string>(stringArr.Length) { keyVal.Value };
                    stringDic.Add(keyVal.Key, list);
                }
            }
            return stringDic;
        }

        /// <summary>
        /// 중복된 키가 존재한다면 int 값을 더해줍니다.
        /// </summary>
        public static Dictionary<string, int> ToStringIntDictionary(this ReadOnlySpan<char> property, int defValue = 1)
        {
            return property._ToDictionary(defValue, s => s.Trim().ToString(), StringComparer.Ordinal, _IntParseOrDef, (i, i1) => i + i1);
        }

        /// <summary>
        /// 중복된 키가 존재한다면 덮어 마지막 값으로 덮어 씌워줍니다.
        /// </summary>
        public static Dictionary<int, string> ToIntStringDictionary(this ReadOnlySpan<char> property, string defValue = "")
        {
            return property._ToDictionary(defValue, _IntParseOrDef, EqualityComparer<int>.Default, s => s.ToString(), (s, s1) => s1);
        }

        /// <summary> string,List<int> </int> </summary>
        public static Dictionary<string, List<int>> ToStringIntListDictionary(this ReadOnlySpan<char> property, int defValue = 1)
        {
            if (property.IsEmpty) {
                return EmptyDictionary<string, List<int>>.Empty;
            }

            var stringArr = ToStringArraySafe(property);
            if (stringArr.Length == 0) {
                return EmptyDictionary<string, List<int>>.Empty;
            }

            var stringDic = new Dictionary<string, List<int>>(stringArr.Length, StringComparer.Ordinal);
            foreach (var stringData in stringArr) {
                var keyVal = ToStringInt(stringData.AsSpan(), defValue);
                if (stringDic.TryGetValue(keyVal.Key, out var list)) {
                    list.Add(keyVal.Value);
                } else {
                    list = new List<int> { keyVal.Value };
                    stringDic.Add(keyVal.Key, list);
                }
            }
            return stringDic;
        }

        public static Dictionary<int, int> ToIntIntDictionary(this ReadOnlySpan<char> property, int defValue = 1)
        {
            return property._ToDictionary(defValue, _IntParseOrDef, EqualityComparer<int>.Default, _IntParseOrDef, (i, i1) => i1);
        }

        public static Dictionary<string, float> ToStringFloatDictionary(this ReadOnlySpan<char> property, float defValue = 0.0f)
        {
            return property._ToDictionary(defValue, s => s.ToString(), StringComparer.Ordinal, _FloatParseOrDef, (i, i1) => i1);
        }

        /// <summary>  콜론으로 분리된 스트링을 keyValue 로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static Dictionary<string, bool> ToStringBoolDictionary(this ReadOnlySpan<char> property, bool defValue = true)
        {
            return property._ToDictionary(defValue, s => s.ToString(), StringComparer.Ordinal, _BoolParseOrDef, (i, i1) => i1);
        }

        /// <summary>  콜론으로 분리된 스트링들을 Dictionary로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static Dictionary<string, List<KeyValuePair<string, string>>> ToKeyValueListDictionary(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                return EmptyDictionary<string, List<KeyValuePair<string, string>>>.Empty;
            }

            var stringArr = ToStringArraySafe(property);
            if (stringArr.Length == 0) {
                return EmptyDictionary<string, List<KeyValuePair<string, string>>>.Empty;
            }

            var stringDic = new Dictionary<string, List<KeyValuePair<string, string>>>(stringArr.Length, StringComparer.Ordinal);
            foreach (var stringData in stringArr) {
                var tuple = stringData.Split(':');
                if (tuple.Length != 3) {
                    continue;
                }
                if (!stringDic.ContainsKey(tuple[0])) {
                    stringDic.Add(tuple[0], new List<KeyValuePair<string, string>>(stringArr.Length));
                }
                stringDic[tuple[0]].Add(new KeyValuePair<string, string>(tuple[1], tuple[2]));
            }
            return stringDic;
        }
    }
}
