using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util {
    using Vector2 = UnityEngine.Vector2;
    using Vector3 = UnityEngine.Vector3;

    public static class StringFormatter {
        public static readonly ObjectPool<StringBuilder> s_pool;

        static StringFormatter()
        {
            var provider = new DefaultObjectPoolProvider();
            s_pool = provider.CreateStringBuilderPool();

        }

        public static string ToStringList(this IEnumerable<string> list)
        {
            var sb = s_pool.Get();
            try {
                sb.Append('{');
                var enumerable = list as string[] ?? list.ToArray();
                if (enumerable.Any()) {
                    sb.Append(enumerable.Aggregate((a, b) => $"{a},{b}"));
                }
                sb.Append('}');

                return sb.ToString();
            } finally {
                s_pool.Return(sb);
            }
        }

        public static string ToStringArray2(this IEnumerable<string[]> list)
        {
            var sb = s_pool.Get();
            try {
                sb.Append('{');

                var enumerable = list;
                if (enumerable.Any()) {
                    foreach (var items in enumerable) {
                        if (items.Any() == false) {
                            continue;
                        }

                        sb.Append('{');
                        foreach (var item in items) {
                            sb.Append(item);
                            sb.Append(';');
                        }
                        sb.Append("}");
                    }
                }
                sb.Append('}');

                return sb.ToString();
            } finally {
                s_pool.Return(sb);
            }
        }

        public static string ToStringList(this List<int> intList, string format = null)
        {
            var sb = s_pool.Get();
            try {
                sb.Append('{');
                var requiredSplitChar = false;
                var count = intList.Count;
                for (var i = 0; i < count; i++) {
                    if (requiredSplitChar) {
                        sb.Append(';');
                    }

                    sb.Append(intList[i].ToString(format));
                    requiredSplitChar = true;
                }
                sb.Append('}');

                return sb.ToString();

            } finally {
                s_pool.Return(sb);
            }
        }

        public static string ToStringList(this List<float> floatList, string format = null)
        {
            var sb = s_pool.Get();
            try {
                sb.Append('{');
                var requiredSplitChar = false;
                var count = floatList.Count;
                for (var i = 0; i < count; i++) {
                    if (requiredSplitChar) {
                        sb.Append(';');
                    }

                    sb.Append(floatList[i].ToString(format));
                    requiredSplitChar = true;
                }
                sb.Append('}');

                return sb.ToString();
            } finally {
                s_pool.Return(sb);
            }
        }

        public static string ToStringList(this Vector2[] vector2List)
        {
            var sb = s_pool.Get();
            var temp = s_pool.Get();
            try {
                sb.Append('{');
                var requiredSplitChar = false;
                var count = vector2List.Length;
                for (var i = 0; i < count; i++) {
                    temp.Clear();
                    if (requiredSplitChar) {
                        sb.Append(';');
                    }
                    sb.Append(vector2List[i].ToStringFormat(temp));
                    requiredSplitChar = true;
                }
                sb.Append('}');

                return sb.ToString();
            } finally {
                s_pool.Return(sb);
                s_pool.Return(temp);
            }
        }

        public static string ToStringList(this Vector3[] vector3List)
        {
            var sb = s_pool.Get();
            var temp = s_pool.Get();
            try {
                sb.Append('{');
                var requiredSplitChar = false;
                var count = vector3List.Length;
                for (var i = 0; i < count; i++) {
                    temp.Clear();
                    if (requiredSplitChar) {
                        sb.Append(';');
                    }
                    sb.Append(vector3List[i].ToStringFormat(temp));
                    requiredSplitChar = true;
                }
                sb.Append('}');

                return sb.ToString();
            } finally {
                s_pool.Return(sb);
                s_pool.Return(temp);
            }
        }

        public static string ToStringList(this Range<int>[] intRangeList)
        {
            var sb = s_pool.Get();
            try {
                sb.Append('{');
                var requiredSplitChar = false;
                var count = intRangeList.Length;
                for (var i = 0; i < count; i++) {
                    if (requiredSplitChar) {
                        sb.Append(';');
                    }

                    sb.Append(intRangeList[i].ToString());
                    requiredSplitChar = true;
                }
                sb.Append('}');

                return sb.ToString();
            } finally {
                s_pool.Return(sb);
            }
        }

        public static string ToStringList(this Range<float>[] floatRangeList)
        {
            var sb = s_pool.Get();
            try {
                sb.Append('{');
                var requiredSplitChar = false;
                var count = floatRangeList.Length;
                for (var i = 0; i < count; i++) {
                    if (requiredSplitChar) {
                        sb.Append(';');
                    }

                    sb.Append(floatRangeList[i].ToString());
                    requiredSplitChar = true;
                }
                sb.Append('}');

                return sb.ToString();
            } finally {
                s_pool.Return(sb);
            }
        }
    }
}