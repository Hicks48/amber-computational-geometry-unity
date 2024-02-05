using UnityEngine;

namespace AmberScience.MotionPlanning.Components {

    public class MovementConstraintsProvider : MonoBehaviour {
        [SerializeField] private float turningRadius;

        public MovementConstraints GetMovementConstraints() {
            return new MovementConstraints(this.turningRadius);
        }
    }
}
