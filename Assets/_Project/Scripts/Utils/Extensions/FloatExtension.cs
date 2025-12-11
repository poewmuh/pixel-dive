using UnityEngine;

namespace Utils.Extensions
{
    public static class FloatExtension
    {
        public static float Sqr(this float f)
        {
            return f * f;
        }

        public static float Abs(this float f)
        {
            return Mathf.Abs(f);
        }
        
        public static bool ApproxZero(this float f)
        {
            return Mathf.Approximately(0, f);
        }
    }
}
