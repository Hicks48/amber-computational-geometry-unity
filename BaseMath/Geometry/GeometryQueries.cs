using AmberScience.MathBase.Vectors;
using AmberScience.MathBase.Trigonometry;
using UnityEngine;

namespace AmberScience.MathBase.Geometry {

    public static class GeometryQueries {

        public static (Circle, Circle) CircleFromCircumferencePointTangentAndRadious(Vector2 circumferencePoint, Vector2 tangent, float radious) {
            Vector2 fromTangentToCircleCenter = Vector2.Perpendicular(tangent).normalized * radious;

            Vector2 circleACenter = circumferencePoint + fromTangentToCircleCenter;
            Vector2 circleBCenter = circumferencePoint - fromTangentToCircleCenter;

            return VectorLocationQueries.IsLeftOf(tangent, circleACenter)
                ? (new Circle(radious, circleACenter), new Circle(radious, circleBCenter))
                : (new Circle(radious, circleBCenter), new Circle(radious, circleACenter));
        }

        public static UnitCircle.RotationDirection GetRotationDirection(Vector2 tangent, Vector2 circumferencePoint, Vector2 circleCenter) {
            return VectorLocationQueries.IsLeftOf(tangent, circumferencePoint - circleCenter)
                ? UnitCircle.RotationDirection.Clockwise
                : UnitCircle.RotationDirection.Counterclockwise;
        }
    }
}
