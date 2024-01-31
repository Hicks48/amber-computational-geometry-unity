using UnityEngine;

namespace AmberScience.MotionPlanning {
    
    public class MovementConfiguration {
        public Vector2 Position { get; set; }

        public Vector2 Heading { get; set; }

        public MovementConfiguration(Vector2 position, Vector2 heading) {
            this.Position = position;
            this.Heading = heading;
        }

        public override string ToString() {
            return $"Movement Configuration: [position: {this.Position}, heading: {this.Heading}]";
        }
    }
}
