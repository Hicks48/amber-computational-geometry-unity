using AmberScience.MathBase.Trigonometry;
using UnityEngine;

namespace AmberScience.MathBase.Geometry {

    public class Circle {
        public float Radious { get; }
        public Vector2 Center { get; }

        public Circle(float radious, Vector2 center) {
            this.Radious = radious;
            this.Center = center;
        }

        public float GetPerimiter() {
            return 2.0f * Mathf.PI * Radious;
        }

        public float GetDeltaAngleRad(float deltaArc) {
            return deltaArc / this.Radious;
        }

        public float GetDeltaArc(float deltaAngleRad) {
            return deltaAngleRad * this.Radious;
        }

        public Vector2 GetCircumferencePoint(float angleRad) {
            return this.Center + this.Radious * UnitCircle.CircumferencePointForAngle(angleRad);
        }
    }
}
