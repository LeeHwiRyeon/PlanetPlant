using System;

namespace Util {
    using Vector2 = UnityEngine.Vector2;
    using Vector3 = UnityEngine.Vector3;

    public struct OBB {
        public double[] Origins;
        public Vector2[] Corners;
        public Vector2[] Axes;

        public OBB(Vector2 center, float width, float height, float angle)
        {
            Origins = new double[2];
            Corners = new Vector2[4];
            Axes = new Vector2[2];

            var cosAngle = (float)Math.Cos(angle);
            var sinAngle = (float)Math.Sin(angle);
            var x = new Vector2(cosAngle, sinAngle);
            var y = new Vector2(-sinAngle, cosAngle);

            x *= width * 0.5f;
            y *= height * 0.5f;

            Corners[0] = center - x - y;
            Corners[1] = center + x - y;
            Corners[2] = center + x + y;
            Corners[3] = center - x + y;

            CalcAxes();
        }

        private void CalcAxes()
        {
            Axes[0] = Corners[1] - Corners[0];
            Axes[1] = Corners[3] - Corners[0];

            for (var i = 0; i < 2; ++i) {
                Axes[i] /= Axes[i].sqrMagnitude;
                Origins[i] = Corners[i].DotProduct(Axes[i]);
            }
        }

        public void MoveTo(Vector2 center)
        {
            var sum = Vector2.zero;
            for (var i = 0; i < 4; ++i) {
                sum += Corners[i];
            }

            var centroid = sum / 4;
            var translationV = center - centroid;
            for (var i = 0; i < 4; ++i) {
                Corners[i] += translationV;
            }
            CalcAxes();
        }
    }

    public class MATH {
        public const float PI = 3.141592654f;
        public const float Deg2Rad = 0.0174532924f;
        public const float Rad2Deg = 57.29578f;

        /// <summary> 내적 </summary>
        public static float InnerProduct(Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public static float AngleWithTwoDirection(Vector2 v1, Vector2 v2)
        {
            var cross = v1.x * v2.y - v1.y * v2.x;
            var dot = InnerProduct(v1, v2);
            var degree = Atan2Approximation(cross, dot) * Rad2Deg;
            return degree;
        }

        /// <summary> Atan2 근사값 참조 https://developer.download.nvidia.com/cg/atan2.html </summary>
        public static float Atan2Approximation(float y, float x)
        {
            float t0, t1, t3, t4;
            t3 = Math.Abs(x);
            t1 = Math.Abs(y);
            t0 = Math.Max(t3, t1);
            t1 = Math.Min(t3, t1);
            t3 = t1 * (1f / t0);
            t4 = t3 * t3;
            t0 = -0.013480470f;
            t0 = t0 * t4 + 0.057477314f;
            t0 = t0 * t4 - 0.121239071f;
            t0 = t0 * t4 + 0.195635925f;
            t0 = t0 * t4 - 0.332994597f;
            t0 = t0 * t4 + 0.999995630f;
            t3 = t0 * t3;
            t3 = (Math.Abs(y) > Math.Abs(x)) ? (PI / 2) - t3 : t3;
            t3 = (x < 0) ? PI - t3 : t3;
            t3 = (y < 0) ? -t3 : t3;
            return t3;
        }

        public static float AngleToRadian(double angle)
        {
            return (float)(Deg2Rad * angle);
        }

        public static float RadianToAngle(double rad)
        {
            return (float)(rad * Rad2Deg);
        }

        public static float VecToDegree(Vector2 vec)
        {
            return (float)(Math.Atan2(vec.y, vec.x) * Rad2Deg);
        }

        public static float VecToDegree(float x, float y)
        {
            const double Rad2Deg = 180 / Math.PI;
            return (float)(Math.Atan2(y, x) * Rad2Deg);
        }

        public static Vector2 AngleToVec(float degree)
        {
            var radian = degree * Deg2Rad;
            return new Vector2((float)Math.Cos(radian), (float)Math.Sin(radian));
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static float Clamp01(float value)
        {
            if ((double)value < 0.0)
                return 0.0f;
            return (double)value > 1.0 ? 1f : value;
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Clamp01(t);
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        public static bool Approximately(float a, float b)
        {
            return Math.Abs(b - a) < Math.Max(1E-06f * Math.Max(Math.Abs(a), Math.Abs(b)), float.Epsilon * 8f);
        }
    }
}

