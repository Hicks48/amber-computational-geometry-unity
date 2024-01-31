using UnityEngine;

namespace AmberScience.MotionPlanning.Components {

    public class MovementConstraintsProvider : MonoBehaviour {
        [SerializeField] private float turningRadious;

        public MovementConstraints GetMovementConstraints() {
            return new MovementConstraints(this.turningRadious);
        }
    }
}
