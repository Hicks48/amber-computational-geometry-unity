using UnityEngine;

namespace AmberScience.MotionPlanning.Trajectories {

    public class Straight : ITrajectory {
        public Vector2 StartPosition { get; private set; }

        public Vector2 EndPosition { get; private set; }

        public Vector2 StartHeading {
            get { return (this.EndPosition - this.StartPosition).normalized; }
        }

        public Vector2 EndHeading {
            get { return (this.EndPosition - this.StartPosition).normalized; }
        }

        public float Length {
            get { return (this.EndPosition - this.StartPosition).magnitude; }
        }

        public Straight(Vector2 startPosition, Vector2 endPosition) {
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
        }

        public MovementConfiguration GetMovementConfiguration(float travelDistance) {
            Vector2 direction = (this.EndPosition - this.StartPosition).normalized;
            return new MovementConfiguration(
                this.StartPosition + direction * Mathf.Min(travelDistance, this.Length),
                direction
            );
        }
    }
}