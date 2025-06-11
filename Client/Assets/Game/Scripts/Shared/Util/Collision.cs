using System;
using UnityEngine;

namespace Util {
    using Vector2 = UnityEngine.Vector2;

    public static class Collision {
        public static bool IsInCircle(in Vector2 sourcePos, float minRange, float maxRange,
                                      in Vector2 targetPos, float targetRadius)
        {
            var dist = sourcePos.DistanceTo(targetPos);
            // 타겟이 maxRange 바깥에 있는 상태
            maxRange += targetRadius;
            var isInsideMaxRange = dist < maxRange || Mathf.Approximately(dist, maxRange);
            if (isInsideMaxRange == false)
                return false;
            
            // 타겟이 minRange 안쪽으로 포함된 상태
            var isInsideMinRange = dist + targetRadius < minRange;
            if (isInsideMinRange)
                return false;

            return true;
        }

        public static bool IsInSquare(Vector2 sourcePos,
                                      Vector2 targetPos,
                                      float targetRadius,
                                      float minRange, // range = 높이
                                      float maxRange,
                                      float height,
                                      float angle)
        {
            var width = maxRange - minRange;
            var offset = (maxRange + minRange) * 0.5f;
            var center = sourcePos + (Vector2.right * offset).RotateAntiClockwise(angle);

            // 거리 벡터
            var dist = (targetPos - center).Rotate(angle);

            // 타겟(원) 에서 가장 가까운 사각형 위의 점.
            var clamped = new Vector2((float)(Math.Sign(dist.x) * Math.Min(Math.Abs(dist.x), width * 0.5f)),
                                      (float)(Math.Sign(dist.y) * Math.Min(Math.Abs(dist.y), height * 0.5f)));

            // 그 점이 원 안에 위치하는가를 검사.
            return !((dist - clamped).sqrMagnitude - targetRadius * targetRadius > 0.0f);
        }

        public static bool IsInFan(in Vector2 sourcePos, in Vector2 direction,
                                   in Vector2 targetPos, float targetRadius,
                                   float minRange, float maxRange, float angle)
        {
            // 타겟이 원형 범위에 포함되는지 확인
            var isInsideCircle = IsInCircle(sourcePos, minRange, maxRange, targetPos, targetRadius);
            if (isInsideCircle == false)
                return false;

            // 타겟이 유효 각도 범위내에 있는지 확인
            var sourceDir = direction;
            var targetDir = (targetPos - sourcePos);

            var degree = MATH.AngleWithTwoDirection(sourceDir, targetDir);    // [-180, 180]
            var half_angle = angle * 0.5f;
            if (Math.Abs(degree) > half_angle)
                return false;

            return true;
        }
    }
}
