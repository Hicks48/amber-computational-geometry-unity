namespace AmberScience.MotionPlanning {
    
    public class MovementConstraints {
        public float TurningRadius { get; private set; }

        public MovementConstraints(float turningRadius) {
            this.TurningRadius = turningRadius;
        }
    }
}
