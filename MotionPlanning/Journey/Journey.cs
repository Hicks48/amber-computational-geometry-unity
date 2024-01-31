using System.Linq;
using System.Collections.Generic;

namespace AmberScience.MotionPlanning {

    public class Journey {

        public bool IsAtTheEnd {
            get {
                if (this.SimpleJourneys == null || this.SimpleJourneys.Count == 0) {
                    return true;
                }

                if (this.currentJourneyIndex < this.SimpleJourneys.Count - 1) {
                    return false;
                }

                return this.SimpleJourneys[this.currentJourneyIndex].IsAtEnd;
            }
        }

        public float Length {
            get {
                float length = 0.0f;
                foreach (SimpleJourney simpleJourney in this.SimpleJourneys) {
                    length += simpleJourney.Length;
                }

                return length;
            }
        }

        public List<SimpleJourney> SimpleJourneys { get; private set; }
        private int currentJourneyIndex;

        public Journey() : this(new List<SimpleJourney>() { }) { }

        public Journey(params SimpleJourney[] journeys) : this(journeys.ToList()) { }

        public Journey(List<SimpleJourney> journeys) {
            this.SimpleJourneys = journeys;
            this.currentJourneyIndex = 0;
        }

        public List<SimpleJourney> GetSimpleJourneys() {
            return this.SimpleJourneys;
        }

        public MovementConfiguration Advance(float deltaDistance) {

            if (this.SimpleJourneys == null || this.SimpleJourneys.Count == 0) {
                return null;
            }

            SimpleJourney currentSimpleJourney = this.SimpleJourneys[this.currentJourneyIndex];

            bool isOverCurrentJourney = currentSimpleJourney.GetRemainingDistance() < deltaDistance;
            if (!isOverCurrentJourney) {
                return currentSimpleJourney.Advance(deltaDistance);
            }

            if (this.HasNextJourney()) {
                this.currentJourneyIndex++;
                SimpleJourney nextJourney = this.SimpleJourneys[this.currentJourneyIndex];
                return nextJourney.Advance(deltaDistance - currentSimpleJourney.GetRemainingDistance());
            }

            // Is at the end since is over current trajectory and does not have a next one so just advance the last one.
            return currentSimpleJourney.Advance(deltaDistance);
        }

        public float GetRemainingDistance() {
            float remainingDistance = 0.0f;
            for (int i = this.currentJourneyIndex; i < this.SimpleJourneys.Count; i++) {
                remainingDistance += this.SimpleJourneys[i].GetRemainingDistance();
            }

            return remainingDistance;
        }

        public List<SimpleJourney> GetRemainingSimpleJourneys() {

            if (this.SimpleJourneys.Count == 0 || this.IsAtTheEnd) {
                return new List<SimpleJourney>();
            }

            var nRemainingSimpleJourneys = this.SimpleJourneys.Count - this.currentJourneyIndex;
            return this.SimpleJourneys.TakeLast(nRemainingSimpleJourneys).ToList();
        }

        private bool HasNextJourney() {
            return this.currentJourneyIndex < this.SimpleJourneys.Count - 1;
        }
    }
}
