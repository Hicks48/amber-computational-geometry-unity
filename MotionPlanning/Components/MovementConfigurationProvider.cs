using System;
using UnityEngine;

namespace AmberScience.MotionPlanning.Components {

    public class MovementConfigurationProvider : MonoBehaviour {
        public EventHandler<MovementConfiguration> OnConfigurationChanged;

        public void SetMovementConfiguration(MovementConfiguration movementConfiguration) {
            this.transform.position = movementConfiguration.Position;

            // Forward direction is the positive X axis.
            this.transform.right = movementConfiguration.Heading;

            this.OnConfigurationChanged?.Invoke(this, this.GetMovementConfiguration());
        }

        public MovementConfiguration GetMovementConfiguration() {
            return new MovementConfiguration(
                this.transform.position,
                this.transform.right // Forward direction is the positive X axis.
            );
        }
    }
}
