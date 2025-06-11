using System;

namespace Util {
    using Vector2 = UnityEngine.Vector2;

    public enum EasingType {
        Linear,
        QuadIn,
        QuadOut,
        QuadInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        QuartIn,
        QuartOut,
        QuartInOut,
        SineIn,
        SineOut,
        SineInOut,
        ExpoIn,
        ExpoOut,
        ExpoInOut,
        CircIn,
        CircOut,
        CircInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        BackIn,
        BackOut,
        BackInOut,
        BounceIn,
        BounceOut,
        BounceInOut,
    }

    public static class Easing {
        public static EasingType RandomType()
        {
            return (EasingType)RAND.RangeInt((int)EasingType.Linear, (int)EasingType.BounceInOut + 1);
        }

        public static Vector2 Calculation(EasingType type, float elapsedTime, Vector2 startPos, Vector2 distPos, float duration, bool safe = true)
        {
            if (Math.Abs(elapsedTime) <= float.Epsilon) {
                return startPos;
            } else if (safe && elapsedTime > duration) {
                return startPos + distPos;
            }

            switch (type) {
                case EasingType.Linear:
                    return Linear(elapsedTime, startPos, distPos, duration);
                case EasingType.QuadIn:
                    return EaseInQuad(elapsedTime, startPos, distPos, duration);
                case EasingType.QuadOut:
                    return EaseOutQuad(elapsedTime, startPos, distPos, duration);
                case EasingType.QuadInOut:
                    return EaseInOutQuad(elapsedTime, startPos, distPos, duration);
                case EasingType.CubicIn:
                    return EaseInCubic(elapsedTime, startPos, distPos, duration);
                case EasingType.CubicOut:
                    return EaseOutCubic(elapsedTime, startPos, distPos, duration);
                case EasingType.CubicInOut:
                    return EaseInOutCubic(elapsedTime, startPos, distPos, duration);
                case EasingType.QuartIn:
                    return EaseInQuart(elapsedTime, startPos, distPos, duration);
                case EasingType.QuartOut:
                    return EaseOutQuart(elapsedTime, startPos, distPos, duration);
                case EasingType.QuartInOut:
                    return EaseInOutQuart(elapsedTime, startPos, distPos, duration);
                case EasingType.SineIn:
                    return EaseInSine(elapsedTime, startPos, distPos, duration);
                case EasingType.SineOut:
                    return EaseOutSine(elapsedTime, startPos, distPos, duration);
                case EasingType.SineInOut:
                    return EaseInOutSine(elapsedTime, startPos, distPos, duration);
                case EasingType.ExpoIn:
                    return EaseInExpo(elapsedTime, startPos, distPos, duration);
                case EasingType.ExpoOut:
                    return EaseOutExpo(elapsedTime, startPos, distPos, duration);
                case EasingType.ExpoInOut:
                    return EaseInOutExpo(elapsedTime, startPos, distPos, duration);
                case EasingType.CircIn:
                    return EaseInCirc(elapsedTime, startPos, distPos, duration);
                case EasingType.CircOut:
                    return EaseOutCirc(elapsedTime, startPos, distPos, duration);
                case EasingType.CircInOut:
                    return EaseInOutCirc(elapsedTime, startPos, distPos, duration);
                case EasingType.ElasticIn:
                    return EaseInElastic(elapsedTime, startPos, distPos, duration);
                case EasingType.ElasticOut:
                    return EaseOutElastic(elapsedTime, startPos, distPos, duration);
                case EasingType.ElasticInOut:
                    return EaseInOutElastic(elapsedTime, startPos, distPos, duration);
                case EasingType.BackIn:
                    return EaseInBack(elapsedTime, startPos, distPos, duration);
                case EasingType.BackOut:
                    return EaseOutBack(elapsedTime, startPos, distPos, duration);
                case EasingType.BackInOut:
                    return EaseInOutBack(elapsedTime, startPos, distPos, duration);
                case EasingType.BounceIn:
                    return EaseInBounce(elapsedTime, startPos, distPos, duration);
                case EasingType.BounceOut:
                    return EaseOutBounce(elapsedTime, startPos, distPos, duration);
                case EasingType.BounceInOut:
                    return EaseInOutBounce(elapsedTime, startPos, distPos, duration);
            }

            return startPos;
        }

        public static float Calculation(EasingType type, float elapsedTime, float start, float dist, float duration, bool safe = true)
        {
            if (Math.Abs(elapsedTime) <= float.Epsilon) {
                return start;
            }

            if (safe && elapsedTime > duration) {
                return start + dist;
            }

            switch (type) {
                case EasingType.Linear:
                    return Linear(elapsedTime, start, dist, duration);
                case EasingType.QuadIn:
                    return EaseInQuad(elapsedTime, start, dist, duration);
                case EasingType.QuadOut:
                    return EaseOutQuad(elapsedTime, start, dist, duration);
                case EasingType.QuadInOut:
                    return EaseInOutQuad(elapsedTime, start, dist, duration);
                case EasingType.CubicIn:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.CubicOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.CubicInOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.QuartIn:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.QuartOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.QuartInOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.SineIn:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.SineOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.SineInOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.ExpoIn:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.ExpoOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.ExpoInOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.CircIn:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.CircOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.CircInOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.ElasticIn:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.ElasticOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.ElasticInOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.BackIn:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.BackOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.BackInOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.BounceIn:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.BounceOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
                case EasingType.BounceInOut:
                    return EaseInOutCubic(elapsedTime, start, dist, duration);
            }

            return start;
        }

        public static float Linear(float elapsedTime, float startPos, float distPos, float duration)
        {
            return distPos * elapsedTime / duration + startPos;
        }

        public static Vector2 Linear(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            return distPos * elapsedTime / duration + startPos;
        }

        // quadratic easing in - accelerating from zero velocity
        public static float EaseInQuad(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            return distPos * elapsedTime * elapsedTime + startPos;
        }

        // quadratic easing out - decelerating to zero velocity
        public static float EaseOutQuad(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            return -distPos * elapsedTime * (elapsedTime - 2) + startPos;
        }

        // quadratic easing in/out - acceleration until halfway, then deceleration
        public static float EaseInOutQuad(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return distPos * 0.5f * elapsedTime * elapsedTime + startPos;
            }

            elapsedTime--;
            return -distPos * 0.5f * (elapsedTime * (elapsedTime - 2) - 1) + startPos;
        }

        // quadratic easing in - accelerating from zero velocity
        public static Vector2 EaseInQuad(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            return distPos * elapsedTime * elapsedTime + startPos;
        }

        // quadratic easing out - decelerating to zero velocity
        public static Vector2 EaseOutQuad(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            return -distPos * elapsedTime * (elapsedTime - 2) + startPos;
        }

        // quadratic easing in/out - acceleration until halfway, then deceleration
        public static Vector2 EaseInOutQuad(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return distPos * 0.5f * elapsedTime * elapsedTime + startPos;
            }

            elapsedTime--;
            return -distPos * 0.5f * (elapsedTime * (elapsedTime - 2) - 1) + startPos;
        }

        // cubic easing in - accelerating from zero velocity
        public static float EaseInCubic(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            return distPos * elapsedTime * elapsedTime * elapsedTime + startPos;
        }

        // cubic easing out - decelerating to zero velocity
        public static float EaseOutCubic(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            elapsedTime--;
            return distPos * (elapsedTime * elapsedTime * elapsedTime + 1) + startPos;
        }

        // cubic easing in/out - acceleration until halfway, then deceleration
        public static float EaseInOutCubic(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return distPos * 0.5f * elapsedTime * elapsedTime * elapsedTime + startPos;
            }

            elapsedTime -= 2;
            return distPos * 0.5f * (elapsedTime * elapsedTime * elapsedTime + 2) + startPos;
        }

        // cubic easing in - accelerating from zero velocity
        public static Vector2 EaseInCubic(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            return distPos * elapsedTime * elapsedTime * elapsedTime + startPos;
        }

        // cubic easing out - decelerating to zero velocity
        public static Vector2 EaseOutCubic(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            elapsedTime--;
            return distPos * (elapsedTime * elapsedTime * elapsedTime + 1) + startPos;
        }

        // cubic easing in/out - acceleration until halfway, then deceleration
        public static Vector2 EaseInOutCubic(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return distPos * 0.5f * elapsedTime * elapsedTime * elapsedTime + startPos;
            }

            elapsedTime -= 2;
            return distPos * 0.5f * (elapsedTime * elapsedTime * elapsedTime + 2) + startPos;
        }

        // quartic easing in - accelerating from zero velocity
        public static float EaseInQuart(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            return distPos * elapsedTime * elapsedTime * elapsedTime * elapsedTime + startPos;
        }

        // quartic easing out - decelerating to zero velocity
        public static float EaseOutQuart(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            elapsedTime--;
            return -distPos * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1) + startPos;
        }

        // quartic easing in/out - acceleration until halfway, then deceleration
        public static float EaseInOutQuart(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return distPos * 0.5f * elapsedTime * elapsedTime * elapsedTime * elapsedTime + startPos;
            }

            elapsedTime -= 2;
            return -distPos * 0.5f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2) + startPos;
        }

        // quartic easing in - accelerating from zero velocity
        public static Vector2 EaseInQuart(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            return distPos * elapsedTime * elapsedTime * elapsedTime * elapsedTime + startPos;
        }

        // quartic easing out - decelerating to zero velocity
        public static Vector2 EaseOutQuart(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            elapsedTime--;
            return -distPos * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1) + startPos;
        }

        // quartic easing in/out - acceleration until halfway, then deceleration
        public static Vector2 EaseInOutQuart(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return distPos * 0.5f * elapsedTime * elapsedTime * elapsedTime * elapsedTime + startPos;
            }

            elapsedTime -= 2;
            return -distPos * 0.5f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2) + startPos;
        }

        // quintic easing in - accelerating from zero velocity
        public static float EaseInQuint(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            return distPos * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + startPos;
        }

        // quintic easing out - decelerating to zero velocity
        public static float EaseOutQuint(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            elapsedTime--;
            return distPos * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1) + startPos;
        }

        // quintic easing in/out - acceleration until halfway, then deceleration
        public static float EaseInOutQuint(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return distPos * 0.5f * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + startPos;
            }

            elapsedTime -= 2;
            return distPos * 0.5f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2) + startPos;
        }

        // quintic easing in - accelerating from zero velocity
        public static Vector2 EaseInQuint(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            return distPos * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + startPos;
        }

        // quintic easing out - decelerating to zero velocity
        public static Vector2 EaseOutQuint(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            elapsedTime--;
            return distPos * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1) + startPos;
        }

        // quintic easing in/out - acceleration until halfway, then deceleration
        public static Vector2 EaseInOutQuint(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return distPos * 0.5f * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + startPos;
            }

            elapsedTime -= 2;
            return distPos * 0.5f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2) + startPos;
        }

        // sinusoidal easing in - accelerating from zero velocity
        public static float EaseInSine(float elapsedTime, float startPos, float distPos, float duration)
        {
            return (float)(-distPos * Math.Cos(elapsedTime / duration * (Math.PI * 0.5f)) + distPos + startPos);
        }

        // sinusoidal easing out - decelerating to zero velocity
        public static float EaseOutSine(float elapsedTime, float startPos, float distPos, float duration)
        {
            return (float)(distPos * Math.Sin(elapsedTime / duration * (Math.PI * 0.5f)) + startPos);
        }

        // sinusoidal easing in/out - accelerating until halfway, then decelerating
        public static float EaseInOutSine(float elapsedTime, float startPos, float distPos, float duration)
        {
            return (float)(-distPos * 0.5f * (Math.Cos(Math.PI * elapsedTime / duration) - 1) + startPos);
        }

        // sinusoidal easing in - accelerating from zero velocity
        public static Vector2 EaseInSine(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            return -distPos * (float)Math.Cos(elapsedTime / duration * (Math.PI * 0.5)) + distPos + startPos;
        }

        // sinusoidal easing out - decelerating to zero velocity
        public static Vector2 EaseOutSine(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            return distPos * (float)Math.Sin(elapsedTime / duration * (Math.PI * 0.5)) + startPos;
        }

        // sinusoidal easing in/out - accelerating until halfway, then decelerating
        public static Vector2 EaseInOutSine(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            return -distPos * 0.5f * (float)(Math.Cos(Math.PI * elapsedTime / duration) - 1) + startPos;
        }

        // exponential easing in - accelerating from zero velocity
        public static float EaseInExpo(float elapsedTime, float startPos, float distPos, float duration)
        {
            return (float)(distPos * Math.Pow(2, 10 * (elapsedTime / duration - 1)) + startPos);
        }

        // exponential easing out - decelerating to zero velocity
        public static float EaseOutExpo(float elapsedTime, float startPos, float distPos, float duration)
        {
            return (float)(distPos * (-Math.Pow(2, -10 * elapsedTime / duration) + 1) + startPos);
        }

        // exponential easing in/out - accelerating until halfway, then decelerating
        public static float EaseInOutExpo(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return (float)(distPos * 0.5f * Math.Pow(2, 10 * (elapsedTime - 1)) + startPos);
            }

            elapsedTime--;
            return (float)(distPos * 0.5f * (-Math.Pow(2, -10 * elapsedTime) + 2) + startPos);
        }

        // exponential easing in - accelerating from zero velocity
        public static Vector2 EaseInExpo(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            return distPos * (float)Math.Pow(2, 10 * (elapsedTime / duration - 1)) + startPos;
        }

        // exponential easing out - decelerating to zero velocity
        public static Vector2 EaseOutExpo(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            return distPos * (float)(-Math.Pow(2, -10 * elapsedTime / duration) + 1) + startPos;
        }

        // exponential easing in/out - accelerating until halfway, then decelerating
        public static Vector2 EaseInOutExpo(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return distPos * 0.5f * (float)Math.Pow(2, 10 * (elapsedTime - 1)) + startPos;
            }

            elapsedTime--;
            return distPos * 0.5f * (float)(-Math.Pow(2, -10 * elapsedTime) + 2) + startPos;
        }

        // circular easing in - accelerating from zero velocity
        public static float EaseInCirc(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            return (float)(-distPos * (Math.Sqrt(1 - elapsedTime * elapsedTime) - 1) + startPos);
        }

        // circular easing out - decelerating to zero velocity
        public static float EaseOutCirc(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration;
            elapsedTime--;
            return (float)(distPos * Math.Sqrt(1 - elapsedTime * elapsedTime) + startPos);
        }

        // circular easing in/out - acceleration until halfway, then deceleration
        public static float EaseInOutCirc(float elapsedTime, float startPos, float distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return (float)(-distPos * 0.5f * (Math.Sqrt(1 - elapsedTime * elapsedTime) - 1) + startPos);
            }

            elapsedTime -= 2;
            return (float)(distPos * 0.5 * (Math.Sqrt(1 - elapsedTime * elapsedTime) + 1) + startPos);
        }

        // circular easing in - accelerating from zero velocity
        public static Vector2 EaseInCirc(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            return -distPos * (float)(Math.Sqrt(1 - elapsedTime * elapsedTime) - 1) + startPos;
        }

        // circular easing out - decelerating to zero velocity
        public static Vector2 EaseOutCirc(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration;
            elapsedTime--;
            return distPos * (float)Math.Sqrt(1 - elapsedTime * elapsedTime) + startPos;
        }

        // circular easing in/out - acceleration until halfway, then deceleration
        public static Vector2 EaseInOutCirc(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            elapsedTime /= duration * 0.5f;
            if (elapsedTime < 1) {
                return -distPos * 0.5f * (float)(Math.Sqrt(1 - elapsedTime * elapsedTime) - 1) + startPos;
            }

            elapsedTime -= 2;
            return distPos * 0.5f * (float)(Math.Sqrt(1 - elapsedTime * elapsedTime) + 1) + startPos;
        }

        public static float EaseInElastic(float elapsedTime, float start, float dist, float duration)
        {
            elapsedTime /= duration * 0.5f;
            var p = duration * 0.3;
            var a = dist;
            var s = p * 0.25;
            var postFix = a * Math.Pow(2, 10 * (elapsedTime -= 1)); // this is a fix, again, with post-increment operators
            return (float)(-(postFix * Math.Sin((elapsedTime * duration - s) * (2 * Math.PI) / p)) + start);
        }

        public static float EaseOutElastic(float elapsedTime, float start, float dist, float duration)
        {
            elapsedTime /= duration * 0.5f;
            var p = duration * .3f;
            var a = dist;
            var s = p * 0.25;
            return (float)(a * Math.Pow(2, -10 * elapsedTime) * Math.Sin((elapsedTime * duration - s) * (2 * Math.PI) / p) + dist + start);
        }

        public static float EaseInOutElastic(float elapsedTime, float start, float dist, float duration)
        {
            elapsedTime /= duration * 0.5f;
            var p = duration * (.3f * 1.5f);
            var a = dist;
            var s = p * 0.25;

            if (elapsedTime < 1) {
                var postFix = a * Math.Pow(2, 10 * (elapsedTime -= 1));
                return (float)(-0.5 * (postFix * Math.Sin((elapsedTime * duration - s) * (2 * Math.PI) / p)) + start);
            } else {
                var postFix = a * Math.Pow(2, -10 * (elapsedTime -= 1));
                return (float)(postFix * Math.Sin((elapsedTime * duration - s) * (2 * Math.PI) / p) * 0.5 + dist + start);
            }
        }

        public static Vector2 EaseInElastic(float elapsedTime, Vector2 start, Vector2 dist, float duration)
        {
            return new Vector2(EaseInElastic(elapsedTime, start.x, dist.x, duration), EaseInElastic(elapsedTime, start.y, dist.y, duration));
        }

        public static Vector2 EaseOutElastic(float elapsedTime, Vector2 start, Vector2 dist, float duration)
        {
            return new Vector2(EaseOutElastic(elapsedTime, start.x, dist.x, duration), EaseOutElastic(elapsedTime, start.y, dist.y, duration));
        }

        public static Vector2 EaseInOutElastic(float elapsedTime, Vector2 start, Vector2 dist, float duration)
        {
            return new Vector2(EaseInOutElastic(elapsedTime, start.x, dist.x, duration), EaseInOutElastic(elapsedTime, start.y, dist.y, duration));
        }

        public static float EaseInBack(float elapsedTime, float start, float dist, float duration)
        {
            const float s = 1.70158f;
            return dist * (elapsedTime /= duration) * elapsedTime * ((s + 1) * elapsedTime - s) + start;
        }

        public static float EaseOutBack(float elapsedTime, float start, float dist, float duration)
        {
            const float s = 1.70158f;
            return dist * ((elapsedTime = elapsedTime / duration - 1) * elapsedTime * ((s + 1) * elapsedTime + s) + 1) + start;
        }

        public static float EaseInOutBack(float elapsedTime, float start, float dist, float duration)
        {
            var s = 1.70158f;
            if ((elapsedTime /= duration * 0.5f) < 1) {
                return dist * 0.5f * (elapsedTime * elapsedTime * (((s *= 1.525f) + 1) * elapsedTime - s)) + start;
            }

            return dist * 0.5f * ((elapsedTime -= 2) * elapsedTime * (((s *= 1.525f) + 1) * elapsedTime + s) + 2) + start;
        }

        public static Vector2 EaseInBack(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            const float s = 1.70158f;
            return distPos * (elapsedTime /= duration) * elapsedTime * ((s + 1) * elapsedTime - s) + startPos;
        }

        public static Vector2 EaseOutBack(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            const float s = 1.70158f;
            return distPos * ((elapsedTime = elapsedTime / duration - 1) * elapsedTime * ((s + 1) * elapsedTime + s) + 1) + startPos;
        }

        public static Vector2 EaseInOutBack(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            var s = 1.70158f;
            if ((elapsedTime /= duration * 0.5f) < 1) {
                return distPos * 0.5f * (elapsedTime * elapsedTime * (((s *= 1.525f) + 1) * elapsedTime - s)) + startPos;
            }

            return distPos * 0.5f * ((elapsedTime -= 2) * elapsedTime * (((s *= 1.525f) + 1) * elapsedTime + s) + 2) + startPos;
        }

        public static float EaseInBounce(float elapsedTime, float start, float dist, float duration)
        {
            return dist - EaseOutBounce(duration - elapsedTime, 0, dist, duration) + start;
        }

        public static float EaseOutBounce(float elapsedTime, float start, float dist, float duration)
        {
            if ((elapsedTime /= duration) < 1f / 2.75f) {
                return dist * (7.5625f * elapsedTime * elapsedTime) + start;
            } else if (elapsedTime < (2f / 2.75f)) {
                return dist * (7.5625f * (elapsedTime -= 1.5f / 2.75f) * elapsedTime + 0.75f) + start;
            } else if (elapsedTime < (2.5f / 2.75f)) {
                return dist * (7.5625f * (elapsedTime -= 2.25f / 2.75f) * elapsedTime + 0.9375f) + start;
            } else {
                return dist * (7.5625f * (elapsedTime -= 2.625f / 2.75f) * elapsedTime + 0.984375f) + start;
            }
        }

        public static float EaseInOutBounce(float elapsedTime, float start, float dist, float duration)
        {
            if (elapsedTime < duration * 0.5) {
                return EaseInBounce(elapsedTime * 2, 0, dist, duration) * 0.5f + start;
            }

            return EaseOutBounce(elapsedTime * 2 - duration, 0, dist, duration) * 0.5f + dist * 0.5f + start;
        }

        public static Vector2 EaseInBounce(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            return distPos - EaseOutBounce(duration - elapsedTime, Vector2.zero, distPos, duration) + startPos;
        }

        public static Vector2 EaseOutBounce(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            if ((elapsedTime /= duration) < (1f / 2.75f)) {
                return distPos * (7.5625f * elapsedTime * elapsedTime) + startPos;
            } else if (elapsedTime < (2f / 2.75f)) {
                return distPos * (7.5625f * (elapsedTime -= (1.5f / 2.75f)) * elapsedTime + 0.75f) + startPos;
            } else if (elapsedTime < (2.5f / 2.75f)) {
                return distPos * (7.5625f * (elapsedTime -= (2.25f / 2.75f)) * elapsedTime + 0.9375f) + startPos;
            } else {
                return distPos * (7.5625f * (elapsedTime -= (2.625f / 2.75f)) * elapsedTime + 0.984375f) + startPos;
            }
        }

        public static Vector2 EaseInOutBounce(float elapsedTime, Vector2 startPos, Vector2 distPos, float duration)
        {
            if (elapsedTime < duration * 0.5f) {
                return EaseInBounce(elapsedTime * 2, Vector2.zero, distPos, duration) * 0.5f + startPos;
            }

            return EaseOutBounce(elapsedTime * 2 - duration, Vector2.zero, distPos, duration) * 0.5f + distPos * 0.5f + startPos;
        }
    }
}
