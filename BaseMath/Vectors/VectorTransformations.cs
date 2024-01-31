using UnityEngine;

namespace AmberScience.MathBase.Vectors {

    public static class VectorTransformations {

        public static Vector2 RotateVector(Vector2 v, float radians) {
            return new Vector2(
                v.x * Mathf.Cos(radians) - v.y * Mathf.Sin(radians),
                v.x * Mathf.Sin(radians) + v.y * Mathf.Cos(radians)
            );
        }
    }
}
