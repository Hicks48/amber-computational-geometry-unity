using AmberScience.MathBase.Trigonometry;
using AmberScience.MathBase.Geometry;
using UnityEngine;

namespace AmberScience.MotionPlanning.Trajectories {

    public class Arc : ITrajectory {
        public float AngleLengthRad {
            get {
                return Mathf.Abs(UnitCircle.AngleBetweenTwoAgles(this.StartAngleRad, this.EndAngleRad, this.DirectionOfRotation));
            }
        }

        public float Length {
            get { return this.AngleLengthRad * this.TurningBase.Radius; }
        }

        public Circle TurningBase { get; }

        public float StartAngleRad { get; }

        public float EndAngleRad { get; }

        public Vector2 StartHeading {
            get { return ((int) this.DirectionOfRotation) * Vector2.Perpendicular(this.StartPosition - this.TurningBase.Center); }
        }

        public Vector2 EndHeading {
            get { return ((int) this.DirectionOfRotation) * Vector2.Perpendicular(this.EndPosition - this.TurningBase.Center); }
        }

        public UnitCircle.RotationDirection DirectionOfRotation { get; }

        public Vector2 StartPosition {
            get { return this.TurningBase.Center + this.TurningBase.Radius * UnitCircle.CircumferencePointForAngle(this.StartAngleRad); }
        }

        public Vector2 EndPosition {
            get { return this.TurningBase.Center + this.TurningBase.Radius * UnitCircle.CircumferencePointForAngle(this.EndAngleRad); }
        }

        public Arc(Circle turningBase, float startAngleRad, float endAngleRad, UnitCircle.RotationDirection directionOfRotation) {
            this.TurningBase = turningBase;
            this.StartAngleRad = startAngleRad;
            this.EndAngleRad = endAngleRad;
            this.DirectionOfRotation = directionOfRotation;
        }

        public MovementConfiguration GetMovementConfiguration(float travelDistance) {
            var newAngle = this.StartAngleRad + ((int) this.DirectionOfRotation) * this.TurningBase.GetDeltaAngleRad(travelDistance);

            var newPosition = this.TurningBase.GetCircumferencePoint(newAngle);
            var newHeading = ((int) this.DirectionOfRotation) * Vector2.Perpendicular(newPosition - this.TurningBase.Center);

            return new MovementConfiguration(newPosition, newHeading);
        }
    }
}
