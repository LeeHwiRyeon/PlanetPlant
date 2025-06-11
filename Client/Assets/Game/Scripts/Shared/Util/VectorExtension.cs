
namespace Util {
    using System;
    using System.Text;
    using Vector2 = UnityEngine.Vector2;
    using Vector3 = UnityEngine.Vector3;
    using Color = UnityEngine.Color;

    public static class VectorExtension {
        public static float DistanceTo(this in Vector2 self, in Vector2 target)
        {
            return (self - target).magnitude;
        }
        public static float SqrDistanceTo(this in Vector2 self, in Vector2 target)
        {
            return (self - target).sqrMagnitude;
        }
        public static float DotProduct(this in Vector2 self, in Vector2 target)
        {
            return self.x * target.x + self.y * target.y;
        }
        public static Vector2 RotateAngle(this in Vector2 self, float angle)
        {
            var radian = angle * MATH.Deg2Rad;
            return self.Rotate(radian);
        }
        public static Vector2 Rotate(this in Vector2 self, float radian)
        {
            return self.Rotate((float)Math.Sin(radian), (float)Math.Cos(radian));
        }
        public static Vector2 Rotate(this in Vector2 self, float sin, float cos)
        {
            return new Vector2(cos * self.x + sin * self.y, -sin * self.x + cos * self.y);
        }
        /// <summary> angle 만큼 반시계 방향으로 돌립니다. (양각) </summary>
        public static Vector2 RotateAngleAntiClockwise(this in Vector2 self, float angle)
        {
            var radian = angle * MATH.Deg2Rad;
            return self.RotateAntiClockwise(radian);
        }
        /// <summary> radian 만큼 반시계 방향으로 돌립니다. (양각) </summary>
        public static Vector2 RotateAntiClockwise(this in Vector2 self, float radian)
        {
            return self.RotateAntiClockwise((float)Math.Sin(radian), (float)Math.Cos(radian));
        }
        /// <summary> radian 만큼 반시계 방향으로 돌립니다. (양각) </summary>
        public static Vector2 RotateAntiClockwise(this in Vector2 self, float sin, float cos)
        {
            return new Vector2(cos * self.x + -sin * self.y, sin * self.x + cos * self.y);
        }
        public static string ToStringFormat(this in Vector2 self, StringBuilder builder, string format = null)
        {
            return builder.Append("(").Append(self.x.ToString(format)).Append("_").Append(self.y.ToString(format)).Append(")").ToString();
        }
        public static string ToStringFormat(this in Vector3 self, StringBuilder builder, string format = null)
        {
            return builder.Append("(").Append(self.x.ToString(format)).Append("_").Append(self.y.ToString(format)).Append("_").Append(self.z.ToString(format)).Append(")").ToString();
        }
        public static Color ToColor(this in Vector3 self, float alpha = 1f)
        {
            return new Color(self.x, self.y, self.z, alpha);
        }
    }
}
