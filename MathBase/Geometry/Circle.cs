using AmberScience.MathBase.Trigonometry;
using UnityEngine;

namespace AmberScience.MathBase.Geometry {

    public class Circle {
        public float Radius { get; }
        public Vector2 Center { get; }

        public Circle(float radius, Vector2 center) {
            this.Radius = radius;
            this.Center = center;
        }

        public float GetPerimiter() {
            return 2.0f * Mathf.PI * Radius;
        }

        public float GetDeltaAngleRad(float deltaArc) {
            return deltaArc / this.Radius;
        }

        public float GetDeltaArc(float deltaAngleRad) {
            return deltaAngleRad * this.Radius;
        }

        public Vector2 GetCircumferencePoint(float angleRad) {
            return this.Center + this.Radius * UnitCircle.CircumferencePointForAngle(angleRad);
        }
    }
}
