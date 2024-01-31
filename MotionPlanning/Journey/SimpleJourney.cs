using AmberScience.MotionPlanning.Trajectories;
using UnityEngine;

namespace AmberScience.MotionPlanning {

    public class SimpleJourney {

        public bool IsAtEnd {
            get {

                if (this.Trajectory == null) {
                    return true;
                }

                return this.CurrentDistanceOnTrajectory >= this.Trajectory.Length - float.Epsilon;
            }
        }

        public float Length {
            get { return this.Trajectory != null ? this.Trajectory.Length : 0.0f; }
        }

        public ITrajectory Trajectory { get; private set; }
        public float CurrentDistanceOnTrajectory { get; private set; }

        public SimpleJourney(ITrajectory trajectory) {
            this.Trajectory = trajectory;
            this.CurrentDistanceOnTrajectory = 0.0f;
        }

        public MovementConfiguration Advance(float deltaDistance) {

            if (this.Trajectory == null) {
                return null;
            }

            this.CurrentDistanceOnTrajectory = Mathf.Min(this.CurrentDistanceOnTrajectory + deltaDistance, this.Trajectory.Length);
            return this.Trajectory.GetMovementConfiguration(this.CurrentDistanceOnTrajectory);
        }

        public float GetRemainingDistance() {
            return this.Trajectory.Length - this.CurrentDistanceOnTrajectory;
        }
    }
}
