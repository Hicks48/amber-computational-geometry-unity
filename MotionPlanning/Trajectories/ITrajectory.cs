using UnityEngine;

namespace AmberScience.MotionPlanning.Trajectories {

    public interface ITrajectory {
        public Vector2 StartPosition { get; }
        public Vector2 EndPosition { get; }
        public float Length { get; }

        public MovementConfiguration GetMovementConfiguration(float travelDistance);
    }
}
