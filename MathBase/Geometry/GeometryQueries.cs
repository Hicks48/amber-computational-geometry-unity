using AmberScience.MathBase.Vectors;
using AmberScience.MathBase.Trigonometry;
using UnityEngine;

namespace AmberScience.MathBase.Geometry {

    public static class GeometryQueries {

        public static (Circle, Circle) CircleFromCircumferencePointTangentAndRadius(Vector2 circumferencePoint, Vector2 tangent, float radius) {
            var fromTangentToCircleCenter = Vector2.Perpendicular(tangent).normalized * radius;

            var circleACenter = circumferencePoint + fromTangentToCircleCenter;
            var circleBCenter = circumferencePoint - fromTangentToCircleCenter;

            return VectorLocationQueries.IsLeftOf(tangent, circleACenter)
                ? (new Circle(radius, circleACenter), new Circle(radius, circleBCenter))
                : (new Circle(radius, circleBCenter), new Circle(radius, circleACenter));
        }

        public static UnitCircle.RotationDirection GetRotationDirection(Vector2 tangent, Vector2 circumferencePoint, Vector2 circleCenter) {
            return VectorLocationQueries.IsLeftOf(tangent, circumferencePoint - circleCenter)
                ? UnitCircle.RotationDirection.Clockwise
                : UnitCircle.RotationDirection.Counterclockwise;
        }
    }
}
