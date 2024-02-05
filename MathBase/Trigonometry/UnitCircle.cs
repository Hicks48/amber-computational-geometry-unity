using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmberScience.MathBase.Trigonometry {
    
    public class UnitCircle {
        public static readonly float FullCircle = 2.0f * Mathf.PI;

        public enum RotationDirection {
            Clockwise = -1,
            Counterclockwise = 1,
        }

        public static Vector2 CircumferencePointForAngle(float angleRad) {
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float VectorToAngle(Vector2 vec) {
            return NormalizeAngle(Mathf.Atan2(vec.y, vec.x));
        }

        public static Vector2 AngleToVector(float angleRad) {
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float AngleBetweenTwoAgles(float startAngleRad, float endAngleRad, RotationDirection directionOfRotation) {
            var needsToCrossZero = directionOfRotation == RotationDirection.Counterclockwise
                ? endAngleRad < startAngleRad
                : endAngleRad > startAngleRad;

            return needsToCrossZero
                ? (
                    directionOfRotation == RotationDirection.Counterclockwise
                        ? endAngleRad + FullCircle - startAngleRad
                        : -1.0f * (startAngleRad + FullCircle - endAngleRad)
                    )
                : Mathf.Abs(endAngleRad - startAngleRad);
        }

        public static bool IsBetweenAngles(float startAngleRad, float endAngleRad, float angleRad, RotationDirection directionOfRotation) {
            return AngleBetweenTwoAgles(startAngleRad, angleRad, directionOfRotation) < AngleBetweenTwoAgles(startAngleRad, endAngleRad, directionOfRotation);
        }

        public static float MinAngleBetweenTwoAngles(float aRad, float bRad) {
            return Mathf.Min(
                Mathf.Abs(AngleBetweenTwoAgles(aRad, bRad, RotationDirection.Counterclockwise)),
                Mathf.Abs(AngleBetweenTwoAgles(aRad, bRad, RotationDirection.Clockwise))
            );
        }

        public static RotationDirection MinRotationDirectionBetweenAngles(float aRad, float bRad) {
            return Mathf.Abs(AngleBetweenTwoAgles(aRad, bRad, RotationDirection.Counterclockwise)) <= Mathf.Abs(AngleBetweenTwoAgles(aRad, bRad, RotationDirection.Clockwise))
                ? RotationDirection.Counterclockwise
                : RotationDirection.Clockwise;
        }

        public static float NormalizeAngle(float angleRad) {
            var singleRound = angleRad % FullCircle;
            return singleRound > 0.0f
                ? singleRound
                : FullCircle - Mathf.Abs(singleRound);
        }
    }
}
