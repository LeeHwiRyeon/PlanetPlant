using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Util {

    public struct Range<T> where T : struct, IConvertible {
        public static readonly Range<T> Empty = new Range<T>();

        public T Min;
        public T Max;

        public Range(T min_)
        {
            Min = min_;
            Max = min_;
            Validate();
        }
        public Range(T min, T max)
        {
            Min = min;
            Max = max;
            Validate();
        }
        public void SetRange(T min_, T max_)
        {
            Min = min_;
            Max = max_;
            Validate();
        }
        public bool Inside(T val)
        {
            if (Comparer<T>.Default.Compare(Min, val) > 0) {
                return false;
            }

            if (Comparer<T>.Default.Compare(val, Max) > 0) {
                return false;
            }

            return true;
        }
        public bool SqrInside(T val)
        {
            if (Comparer<T>.Default.Compare(Multiply(Min, Min), val) > 0) {
                return false;
            }

            if (Comparer<T>.Default.Compare(val, Multiply(Max, Max)) > 0) {
                return false;
            }

            return true;
        }
        private static T Multiply(T a, double b)
        {
            return (T)Convert.ChangeType(a.ToDouble(CultureInfo.CurrentCulture) * b, typeof(T));
        }
        private static T Multiply(T a, T b)
        {
            return (T)Convert.ChangeType(a.ToDouble(CultureInfo.CurrentCulture) * b.ToDouble(CultureInfo.CurrentCulture), typeof(T));
        }
        private static T Add(T a, double b)
        {
            return (T)Convert.ChangeType(a.ToDouble(CultureInfo.CurrentCulture) + b, typeof(T));
        }
        private static T Sum(T a, T b)
        {
            return (T)Convert.ChangeType(a.ToDouble(CultureInfo.CurrentCulture) + b.ToDouble(CultureInfo.CurrentCulture), typeof(T));
        }

        private void Validate()
        {
            if (Comparer<T>.Default.Compare(Min, Max) > 0) {
                var t = Min;
                Min = Max;
                Max = t;
            }
        }
        public override string ToString()
        {
            return new StringBuilder().Append("[").Append(Min).Append("_").Append(Max).Append("]").ToString();
        }
        public static Range<T> operator *(Range<T> range, double multiplier)
        {
            return new Range<T>(Multiply(range.Min, multiplier), Multiply(range.Max, multiplier));
        }
        public static Range<T> operator *(Range<T> range1, Range<T> range2)
        {
            return new Range<T>(Multiply(range1.Min, range2.Min), Multiply(range1.Max, range2.Max));
        }
        public static Range<T> operator +(Range<T> range, double addValue)
        {
            return new Range<T>(Add(range.Min, addValue), Add(range.Max, addValue));
        }
        public static Range<T> operator +(Range<T> range1, Range<T> range2)
        {
            return new Range<T>(Sum(range1.Min, range2.Min), Sum(range1.Max, range2.Max));
        }
    }
}
