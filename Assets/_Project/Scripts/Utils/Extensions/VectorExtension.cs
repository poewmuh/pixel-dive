using UnityEngine;

namespace Utils._Project.Utils.Extensions
{
    public static class VectorExtension
    {
        public static Vector3 OnlyXZ(this Vector3 v3)
        {
            v3.y = 0f;
            return v3;
        }
    }
}
