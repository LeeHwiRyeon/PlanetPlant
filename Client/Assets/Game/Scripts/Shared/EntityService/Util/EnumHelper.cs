using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityService {
    public static class EnumHelper<EnumType> where EnumType : struct, IConvertible {
        private static EnumCache s_cache;

        private class EnumCache {
            public struct Token {
                public class TokenValueComparer : IEqualityComparer<Token> {
                    public bool Equals(Token x, Token y)
                    {
                        return x.Value.Equals(y.Value);
                    }

                    public int GetHashCode(Token obj)
                    {
                        return obj.Value.GetHashCode();
                    }
                }
                public Token(string name, EnumType value)
                {
                    Name = name;
                    Value = value;
                }
                public string Name { get; }
                public EnumType Value { get; }
            }

            /// <remarks>
            ///     http://stackoverflow.com/questions/105372/how-do-i-enumerate-an-enum
            /// </remarks>
            public EnumCache()
            {
                if (!typeof(EnumType).IsEnum) {
                    throw new NotSupportedException($"Only Enum type is supported by EnumHelper. Given type argument was <{typeof(EnumType)}>).");
                }

                var infos = typeof(EnumType).GetFields(BindingFlags.Public | BindingFlags.Static);
                var ary = infos.Select(x => new Token(x.Name, (EnumType)x.GetValue(null))).ToArray();
                ValueToName = ary.Select(x => x.Value)
                                 .Distinct() // as dictionary key cannot be duplicate
                                 .ToDictionary(x => x, x => ary.Where(y => y.Value.Equals(x))
                                                               .Select(y => y.Name)
                                                               .ToArray());
                NameToValue = ary.ToDictionary(x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
            }

            public Dictionary<EnumType, string[]> ValueToName { get; }
            public Dictionary<string, EnumType> NameToValue { get; }
        }

        private static EnumCache Cache => s_cache ?? (s_cache = new EnumCache());
        public static EnumType SafetyParse(string str, EnumType defValue)
        {
            EnumType t;
            try {
                TryParse(str, out t);
            } catch {
                t = defValue;
            }
            return t;
        }

        public static EnumType Parse(string str)
        {
            if (!TryParse(str, out var t)) {
                throw new InvalidOperationException($"value {str} is not a valid '{typeof(EnumType).Name}' value");
            }

            return t;
        }

        public static string Write(EnumType t)
        {
            if (!TryWrite(t, out var str)) {
                throw new InvalidOperationException($"value {t} is not a valid '{typeof(EnumType).Name}' value");
            }

            return str;
        }

        public static bool TryParse(string str, out EnumType t)
        {
            return Cache.NameToValue.TryGetValue(str, out t);
        }

        public static bool TryWrite(EnumType t, out string str)
        {
            if (Cache.ValueToName.TryGetValue(t, out var list)) {
                str = list[0];
                return true;
            }
            str = null;
            return false;
        }

        public static Dictionary<string, EnumType>.KeyCollection GetKeyCollection()
        {
            return Cache.NameToValue.Keys;
        }
        public static Dictionary<string, EnumType>.ValueCollection ValueCollection()
        {
            return Cache.NameToValue.Values;
        }
    }
}
