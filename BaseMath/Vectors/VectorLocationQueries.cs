using UnityEngine;

namespace AmberScience.MathBase.Vectors {

    public static class VectorLocationQueries {

        public static bool IsLeftOf(Vector2 a, Vector2 b) {
            return Vector3.Cross(new Vector3(a.x, 0.0f, a.y), new Vector3(b.x, 0.0f, b.y)).y < 0.0f;
        }

        public static bool IsRightOf(Vector2 a, Vector2 b) {
            return !IsLeftOf(a, b);
        }
    }
}