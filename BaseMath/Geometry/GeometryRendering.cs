using System.Collections.Generic;
using AmberScience.MathBase.Trigonometry;
using UnityEngine;

namespace AmberScience.MathBase.Geometry {

    public static class GeometryRendering {

        public static List<Vector2> GetRenderingPointsForCircle(Circle circle, float pointsPerUnit) {
            var nPoints = (int) Mathf.Ceil(circle.GetPerimiter() / pointsPerUnit);

            var points = new List<Vector2>();

            for (int i = 0; i < nPoints; i++) {
                var circumferenceProgress = i / (float) nPoints;
                var currentRadians = circumferenceProgress * 2 * Mathf.PI;

                var circumferencePoint = circle.GetCircumferencePoint(currentRadians);
                points.Add(circumferencePoint);
            }

            return points;
        }
    }
}
