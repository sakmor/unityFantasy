using UnityEngine;
namespace myMath
{
    //http://gizma.com/easing/
    public static class MathS
    {
        // t: current time (second/milliseconds)
        // b: start value
        // c: change in value
        // d:duration

        // simple linear tweening - no easing, no acceleration
        public static float staticlinearTween(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }


        // quadratic easing in - accelerating from zero velocity
        public static float easeInQuad(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t + b;
        }

        // quadratic easing out - decelerating to zero velocity
        public static float easeOutQuad(float t, float b, float c, float d)
        {
            t /= d;
            return -c * t * (t - 2) + b;
        }

        // quadratic easing in/out - acceleration until halfway, then deceleration
        public static float easeInOutQuad(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t + b;
            t--;
            return -c / 2 * (t * (t - 2) - 1) + b;
        }

        // cubic easing in - accelerating from zero velocity
        public static float easeInCubic(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t * t + b;
        }

        // cubic easing out - decelerating to zero velocity
        public static float easeOutCubic(float t, float b, float c, float d)
        {
            t /= d;
            t--;
            return c * (t * t * t + 1) + b;
        }

        // cubic easing in/out - acceleration until halfway, then deceleration
        public static float easeInOutCubic(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t + b;
            t -= 2;
            return c / 2 * (t * t * t + 2) + b;
        }


        // quartic easing in - accelerating from zero velocity
        public static float easeInQuart(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t * t * t + b;
        }

        // quartic easing out - decelerating to zero velocity
        public static float easeOutQuart(float t, float b, float c, float d)
        {
            t /= d;
            t--;
            return -c * (t * t * t * t - 1) + b;
        }

        // quartic easing in/out - acceleration until halfway, then deceleration
        public static float easeInOutQuart(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t + b;
            t -= 2;
            return -c / 2 * (t * t * t * t - 2) + b;
        }

        // quintic easing in - accelerating from zero velocity
        public static float easeInQuint(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t * t * t * t + b;
        }

        // quintic easing out - decelerating to zero velocity
        public static float easeOutQuint(float t, float b, float c, float d)
        {
            t /= d;
            t--;
            return c * (t * t * t * t * t + 1) + b;
        }

        // quintic easing in/out - acceleration until halfway, then deceleration
        public static float easeInOutQuint(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t * t + b;
            t -= 2;
            return c / 2 * (t * t * t * t * t + 2) + b;
        }

        // sinusoidal easing in - accelerating from zero velocity
        public static float easeInSine(float t, float b, float c, float d)
        {
            return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
        }

        // sinusoidal easing out - decelerating to zero velocity
        public static float easeOutSine(float t, float b, float c, float d)
        {
            return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
        }

        // sinusoidal easing in/out - accelerating until halfway, then decelerating
        public static float easeInOutSine(float t, float b, float c, float d)
        {
            return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
        }

        // exponential easing in - accelerating from zero velocity
        public static float easeInExpo(float t, float b, float c, float d)
        {
            return c * Mathf.Pow(2, 10 * (t / d - 1)) + b;
        }

        // exponential easing out - decelerating to zero velocity
        public static float easeOutExpo(float t, float b, float c, float d)
        {
            return c * (-Mathf.Pow(2, -10 * t / d) + 1) + b;
        }

        // exponential easing in/out - accelerating until halfway, then decelerating
        public static float easeInOutExpo(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;
            t--;
            return c / 2 * (-Mathf.Pow(2, -10 * t) + 2) + b;
        }

        // circular easing in - accelerating from zero velocity
        public static float easeInCirc(float t, float b, float c, float d)
        {
            t /= d;
            return -c * (Mathf.Sqrt(1 - t * t) - 1) + b;
        }

        // circular easing out - decelerating to zero velocity
        public static float easeOutCirc(float t, float b, float c, float d)
        {
            t /= d;
            t--;
            return c * Mathf.Sqrt(1 - t * t) + b;
        }

        // circular easing in/out - acceleration until halfway, then deceleration
        public static float easeInOutCirc(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1) return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;
            t -= 2;
            return c / 2 * (Mathf.Sqrt(1 - t * t) + 1) + b;
        }

        public static Vector3 normalized(Vector3 pos)
        {
            Vector3 temp;
            //正規化生物座標
            temp.x = Mathf.Floor(pos.x);
            temp.z = Mathf.Floor(pos.z);
            temp.y = Mathf.Floor(pos.y);

            return temp;
        }
    }
}
