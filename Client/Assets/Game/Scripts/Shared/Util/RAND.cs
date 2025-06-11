using System;

namespace Util {
    using Vector2 = UnityEngine.Vector2;

    /// <summary> 광역적으로 간단하고 빠르게 임의의 수를 구하기 위해 사용하는 클래스. </summary>
    public static class RAND {
        private static readonly Random _global = new Random();
        [ThreadStatic]
        private static Random _local;

        private static Random Inst {
            get {
                var inst = _local;
                if (inst == null) {
                    int seed;
                    lock (_global) seed = _global.Next();
                    _local = inst = new Random(seed);
                }

                return inst;
            }
        }

        public static int RandomMax(int max)
        {
            return Inst.Next(max + 1);
        }

        /// <summary> min, max 사이의 임의의 수를 구합니다. (min <= x < max) </summary>
        public static int RangeInt(int min, int max)
        {
            return Inst.Next(min, max);
        }

        /// <summary> min, max 사이의 임의의 수를 구합니다. (min <= x < max) </summary>
        public static double Range(double min, double max)
        {
            return Inst.NextDouble() * (max - min) + min;
        }

        public static Vector2 RandomNormalizedVector2()
        {
            var rad = Range(0, 360);
            return new Vector2((float)Math.Sin(rad), (float)Math.Cos(rad));
        }

        public static bool Bool()
        {
            return RangeInt(0, 2) == 0;
        }
    }

    /// <summary> 임의의 수를 구하기 위해 사용하는 인스턴스. </summary>
    public class RANDInstance {
        private readonly Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public int RandomMax(int max)
        {
            return rand.Next(max + 1);
        }

        /// <summary> min, max 사이의 임의의 수를 구합니다. (min <= x < max) </summary>
        public int RangeInt(int min, int max)
        {
            return rand.Next(min, max);
        }

        /// <summary> min, max 사이의 임의의 수를 구합니다. (min <= x < max)</summary>
        public double Range(double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }

        public Vector2 RandomNormalizedVector2()
        {
            var rad = Range(0, 360);
            return new Vector2((float)Math.Sin(rad), (float)Math.Cos(rad));
        }

        public bool Bool()
        {
            return RangeInt(0, 2) == 0;
        }
    }
}
