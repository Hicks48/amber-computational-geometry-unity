namespace AmberScience.MotionPlanning {
    
    public class MovementConstraints {
        public float TurningRadious { get; private set; }

        public MovementConstraints(float turningRadious) {
            this.TurningRadious = turningRadious;
        }
    }
}
