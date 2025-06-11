using System;
using System.Collections.Generic;

namespace EntityService {
    internal static class StringParserInternal {
        private static readonly char[] separators = { ',', ';' };
        private static readonly char[] trimChars = { '{', '}', ' ' };

        public delegate T SpanConverter<out T>(ReadOnlySpan<char> arg);

        internal static T[] _ToArray<T>(this ReadOnlySpan<char> property, SpanConverter<T> converter, params char[] s)
        {
            if (property.IsEmpty) {
                throw new ArgumentException("property was null or empty");
            }

            //  괄호 하나를 받음
            var start = property.IndexOf('{');
            if (start == -1) {
                throw new FormatException("{ expected in _ToArray");
            }

            var end = property.LastIndexOf('}');
            if (end == -1) {
                throw new FormatException("} expected in _ToArray");
            }

            if (s == null || s.Length == 0) {
                s = separators;
            }

            var result = new List<T>();
            foreach (var entry in property.Trim(trimChars).Split(s, StringSplitOptions.RemoveEmptyEntries)) {
                var converted = converter.Invoke(entry);
                result.Add(converted);
            }

            return result.ToArray();
        }

        internal static T[][] _ToArray2<T>(this ReadOnlySpan<char> property, SpanConverter<T> converter, params char[] s)
        {
            if (property.IsEmpty) {
                throw new ArgumentException("property was null or empty");
            }

            //  괄호 하나를 받음
            var start = property.IndexOf('{');
            if (start == -1) {
                throw new FormatException("{ expected in _ToArray2");
            }

            var end = property.LastIndexOf('}');
            if (end == -1) {
                throw new FormatException("} expected in _ToArray2");
            }

            if (s == null || s.Length == 0) {
                s = separators;
            }

            var result = new List<T[]>();
            var substring = property.Slice(start + 1, end - start - 1);
            const char bs = '{';
            const char be = '}';
            do {
                var rangeStart = substring.IndexOf(bs);
                if (rangeStart == -1) {
                    break;
                }

                var rangeEnd = substring.IndexOf(be) + 1;
                if (rangeEnd == 0) {
                    throw new FormatException($"{be} expected in _ToArray2");
                }

                var rangeString = substring.Slice(rangeStart, rangeEnd - rangeStart);
                var array = rangeString._ToArray(converter, s);
                substring = substring.Slice(rangeEnd);
                result.Add(array);
            } while (!(substring.IsEmpty || substring.IsWhiteSpace()));

            return result.ToArray();
        }

        internal static T[] _ToArraySafe<T>(this ReadOnlySpan<char> property, SpanConverter<T> converter, params char[] s)
        {
            if (property.IsEmpty) {
                return Array.Empty<T>();
            }

            if (s == null || s.Length == 0) {
                s = separators;
            }

            var result = new List<T>();
            foreach (var entry in property.Trim(trimChars).Split(s, StringSplitOptions.RemoveEmptyEntries)) {
                var converted = converter.Invoke(entry);
                result.Add(converted);
            }

            return result.ToArray();
        }

        internal static T[] _ToArray<T>(this ReadOnlySpan<char> property, SpanConverter<T> converter, char bs, char be)
        {
            if (property.IsEmpty) {
                throw new ArgumentException("property was null or empty");
            }

            //  괄호 하나를 받음
            var start = property.IndexOf('{');
            if (start == -1) {
                throw new FormatException("{ expected in _ToArray");
            }

            var end = property.LastIndexOf('}');
            if (end == -1) {
                throw new FormatException("} expected in _ToArray");
            }

            var result = new List<T>();
            var substring = property.Slice(start + 1, end - start - 1);

            do {
                var rangeStart = substring.IndexOf(bs);
                if (rangeStart == -1) {
                    throw new FormatException($"{bs} expected in _ToArray");
                }

                var rangeEnd = substring.IndexOf(be) + 1;
                if (rangeEnd == 0) {
                    throw new FormatException($"{be} expected in _ToArray");
                }

                var rangeString = substring.Slice(rangeStart, rangeEnd - rangeStart);
                result.Add(converter(rangeString));
                substring = substring.Slice(rangeEnd);
            } while (!(substring.IsEmpty || substring.IsWhiteSpace()));

            return result.ToArray();
        }

        internal static T[] ToArraySafe<T>(this ReadOnlySpan<char> property, SpanConverter<T> converter, char bs, char be)
        {
            if (property.IsEmpty) {
                return Array.Empty<T>();
            }

            var result = new List<T>();
            var substring = property.Trim(trimChars);

            do {
                var rangeStart = substring.IndexOf(bs);
                if (rangeStart == -1) {
                    break;
                }

                var rangeEnd = substring.IndexOf(be) + 1;
                if (rangeEnd == 0) {
                    break;
                }

                var rangeString = substring.Slice(rangeStart, rangeEnd - rangeStart);
                result.Add(converter(rangeString));
                substring = substring.Slice(rangeEnd);
            } while (!(substring.IsEmpty || substring.IsWhiteSpace()));

            return result.ToArray();
        }

        internal static KeyValuePair<A, B> ToKeyValuePair<A, B>(this ReadOnlySpan<char> property, B def, SpanConverter<A> keyConverter, SpanConverter<B> valConverter)
        {
            if (property.IsEmpty) {
                return new KeyValuePair<A, B>();
            }

            var idx = 0;
            A elem0 = default;
            var elem1 = def;
            foreach (var entry in property.Split(':')) {
                if (idx == 0) {
                    elem0 = keyConverter.Invoke(entry);
                } else if (idx == 1) {
                    elem1 = valConverter.Invoke(entry);
                } else {
                    break;
                }

                ++idx;
            }

            return new KeyValuePair<A, B>(elem0, elem1);
        }

        internal static Dictionary<A, B> _ToDictionary<A, B>(
            this ReadOnlySpan<char> property,
            B defaultValue,
            SpanConverter<A> keyConverter,
            IEqualityComparer<A> keyComparer,
            SpanConverter<B> valConverter,
            Func<B, B, B> valMerger)
        {
            if (property.IsEmpty) {
                return new Dictionary<A, B>();
            }

            var dic = new Dictionary<A, B>(keyComparer);
            foreach (var entry in property.Trim(trimChars).Split(separators, StringSplitOptions.RemoveEmptyEntries)) {
                var idx = 0;
                A elem0 = default;
                var elem1 = defaultValue;
                foreach (var pairToken in entry.Trim().Split(':')) {
                    if (idx == 0) {
                        elem0 = keyConverter(pairToken);
                    } else if (idx == 1) {
                        elem1 = valConverter(pairToken);
                    } else {
                        break;
                    }

                    ++idx;
                }

                if (elem0 == null) {
                    continue;
                }

                if (dic.TryGetValue(elem0, out var prev)) {
                    dic[elem0] = valMerger(prev, elem1);
                } else {
                    dic.Add(elem0, elem1);
                }
            }
            return dic;
        }

        internal static ValueTuple<A, B> _ToValueTuple<A, B>(this ReadOnlySpan<char> property,
            ReadOnlySpan<char> separators,
            SpanConverter<A> item1Selector, A defA,
            SpanConverter<B> item2Selector, B defB)
            where A : struct
            where B : struct
        {
            if (property.IsEmpty) {
                return ValueTuple.Create(defA, defB);
            }

            var idx = 0;
            var elem0 = defA;
            var elem1 = defB;
            foreach (var entry in property.Split(separators)) {
                if (idx == 0) {
                    elem0 = item1Selector.Invoke(entry);
                } else if (idx == 1) {
                    elem1 = item2Selector.Invoke(entry);
                } else {
                    break;
                }

                ++idx;
            }

            return ValueTuple.Create(elem0, elem1);
        }

    }

}
