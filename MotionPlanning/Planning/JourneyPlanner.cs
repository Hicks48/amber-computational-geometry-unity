using System.Collections.Generic;
using System.Linq;
using AmberScience.MathBase.Geometry;
using AmberScience.MathBase.Trigonometry;
using AmberScience.MathBase.Vectors;
using AmberScience.MotionPlanning.Trajectories;
using UnityEngine;

namespace AmberScience.MotionPlanning { 

    public static class JourneyPlanner {

        // Plan for CS paths for which only the start configuration is provided and the end point is only a postion without the heading.
        public static Journey Plan(MovementConfiguration start, Vector2 end, MovementConstraints constraints) {
            // Get turning bases for both turn directions from the start configuration.
            var (startLeftTurningBase, startRightTurningBase) = GeometryQueries.CircleFromCircumferencePointTangentAndRadius(start.Position, start.Heading, constraints.TurningRadius);

            // For both turning bases calculate the distance for the journey to the end point.
            var leftTurningBaseJourney = CalculateCSPath(start, end, startLeftTurningBase);
            var rightTurningBaseJourney = CalculateCSPath(start, end, startRightTurningBase);

            // Return the shortest of the two journeys.
            return leftTurningBaseJourney.Length < rightTurningBaseJourney.Length ? leftTurningBaseJourney : rightTurningBaseJourney;
        }

        private static Journey CalculateCSPath(MovementConfiguration start, Vector2 end, Circle startTurningBase) {
            // Calculate rotation direction at start.
            var rotationDirection = GeometryQueries.GetRotationDirection(start.Heading, start.Position, startTurningBase.Center);

            // Calculate vector from the center of turning base to the end point.
            var fromTurningBaseToEnd = end - startTurningBase.Center;

            // The vectrors from center to end, from center to tangent point and from tangent point to end form a right triangle.
            // The angle between the vector from center to end and from center to tangent point is 90 degrees.
            // The angle alpha angle between the vectors from center to end and center to tangent is arc cosine of the ratio of the length of the two vectors.
            var alpha = Mathf.Acos(startTurningBase.Radius / fromTurningBaseToEnd.magnitude);

            // Calculate unit circle angle for the tangent point by getting the angle of the vector from center to end and rotating it by alpha against the rotation direction.
            var centerToEndAngle = UnitCircle.VectorToAngle(fromTurningBaseToEnd);
            var tangentPointAngle = UnitCircle.NormalizeAngle(centerToEndAngle + alpha * -1 * ((int) rotationDirection));

            // Create the arc for the CS path.
            var arc = new Arc(
                startTurningBase,
                UnitCircle.VectorToAngle(start.Position - startTurningBase.Center),
                tangentPointAngle,
                rotationDirection
            );

            // Calculate the tangent point.
            var tangentPoint = UnitCircle.AngleToVector(tangentPointAngle) * startTurningBase.Radius + startTurningBase.Center;

            // Create straight from the tangent to the end point.
            var straight = new Straight(tangentPoint, end);

            // Create the journey.
            return new Journey(
                new SimpleJourney(arc),
                new SimpleJourney(straight)
            );
        }

        // Plan for CSC and CCC paths for which both end and start movement configurations are allowed.
        public static Journey Plan(MovementConfiguration start, MovementConfiguration end, MovementConstraints constraints) {
            // Compute all turning base combinations.
            var (startLeftTurningBase, startRightTurningBase) = GeometryQueries.CircleFromCircumferencePointTangentAndRadius(start.Position, start.Heading, constraints.TurningRadius);
            var (endLeftTurningBase, endRightTurningBase) = GeometryQueries.CircleFromCircumferencePointTangentAndRadius(end.Position, end.Heading, constraints.TurningRadius);

            // Calculate correct path option based on travel direction of start and end configuration on their respective turning bases.
            var dubinsCSCCurves = new List<Journey>() {
                CalculateCSCPath(start, end, startLeftTurningBase, endLeftTurningBase),
                CalculateCSCPath(start, end, startLeftTurningBase, endRightTurningBase),
                CalculateCSCPath(start, end, startRightTurningBase, endLeftTurningBase),
                CalculateCSCPath(start, end, startRightTurningBase, endRightTurningBase),
            }.Where(journey => journey != null).ToList();

            var dubinsCCCCurves = new List<Journey>() {
                CalculateCCCPath(start, end, startLeftTurningBase, endLeftTurningBase),
                CalculateCCCPath(start, end, startLeftTurningBase, endRightTurningBase),
                CalculateCCCPath(start, end, startRightTurningBase, endLeftTurningBase),
                CalculateCCCPath(start, end, startRightTurningBase, endRightTurningBase),
            }.Where(journey => journey != null).ToList();

            var dubinsCurves = dubinsCSCCurves.Concat(dubinsCCCCurves).ToList();

            // Choose the journey with smallest distance
            Journey shortestJourney = null;
            dubinsCurves.ForEach(journey => {
                
                if (shortestJourney == null || shortestJourney.Length > journey.Length) {
                    shortestJourney = journey;
                }
            });

            return shortestJourney;
        }

        private static Journey CalculateCCCPath(MovementConfiguration start, MovementConfiguration end, Circle startTurningBase, Circle endTurningBase) {
            var fromStartToEnd = end.Position - start.Position;

            // If the distance between start and end is more than 4 x turning base radius then the CCC path can not be shortest and can skip.
            if (fromStartToEnd.magnitude > 4 * startTurningBase.Radius) {
                return null;
            }

            // Check that the provided turning bases match either RLR or LRL since those are only options.
            // Meaning the turning direction has to be the same for start and end turning bases.
            var startRotationDirection = GeometryQueries.GetRotationDirection(start.Heading, start.Position, startTurningBase.Center);
            var endRotationDirection = GeometryQueries.GetRotationDirection(end.Heading, end.Position, endTurningBase.Center);

            if (startRotationDirection != endRotationDirection) {
                return null;
            }

            // Calculate the center of the middle circle in the CCC path and create the middle circle.
            var fromStartTurningBaseCenterToEndTurningBaseCenter = endTurningBase.Center - startTurningBase.Center;

            var angleBetweenStartToEndAndStartToMiddleCircle = Mathf.Acos((0.5f * fromStartTurningBaseCenterToEndTurningBaseCenter.magnitude) / (2 * startTurningBase.Radius));
            var fromStartToCenterOfMiddleCircle = 2 * startTurningBase.Radius * VectorTransformations.RotateVector(fromStartTurningBaseCenterToEndTurningBaseCenter.normalized, angleBetweenStartToEndAndStartToMiddleCircle);
            var centerOfThirdCircle = startTurningBase.Center + fromStartToCenterOfMiddleCircle;

            var middleCircle = new Circle(startTurningBase.Radius, centerOfThirdCircle);

            // Calculate intersections between the three circles.
            var startToMiddleCrossPoint = startTurningBase.Center + 0.5f * (middleCircle.Center - startTurningBase.Center);
            var middleToEndCrossPoint = endTurningBase.Center + 0.5f * (middleCircle.Center - endTurningBase.Center);

            // Create arcs for the journey.
            var startCircleArc = new Arc(
                startTurningBase,
                UnitCircle.VectorToAngle(start.Position - startTurningBase.Center),
                UnitCircle.VectorToAngle(startToMiddleCrossPoint - startTurningBase.Center),
                startRotationDirection
            );

            var middleCircleArc = new Arc(
                middleCircle,
                UnitCircle.VectorToAngle(startToMiddleCrossPoint - middleCircle.Center),
                UnitCircle.VectorToAngle(middleToEndCrossPoint - middleCircle.Center),
                // The middle circles rotation direction is always opposite to the start circles rotation direction.
                (UnitCircle.RotationDirection) (-1 * (int) startRotationDirection)
            );

            var endCircleArc = new Arc(
                endTurningBase,
                UnitCircle.VectorToAngle(middleToEndCrossPoint - endTurningBase.Center),
                UnitCircle.VectorToAngle(end.Position - endTurningBase.Center),
                endRotationDirection
            );

            // Create the combined journey from start to end.
            return new Journey(
                new SimpleJourney(startCircleArc),
                new SimpleJourney(middleCircleArc),
                new SimpleJourney(endCircleArc)
            );
        }

        private static Journey CalculateCSCPath(MovementConfiguration start, MovementConfiguration end, Circle startTurningBase, Circle endTurningBase) {
            var option = (
                GeometryQueries.GetRotationDirection(start.Heading, start.Position, startTurningBase.Center),
                GeometryQueries.GetRotationDirection(end.Heading, end.Position, endTurningBase.Center)
            );

            return option switch {
                (UnitCircle.RotationDirection.Clockwise, UnitCircle.RotationDirection.Clockwise) => CalculateStraightPath(start.Position, end.Position, startTurningBase, endTurningBase, UnitCircle.RotationDirection.Clockwise),
                (UnitCircle.RotationDirection.Clockwise, UnitCircle.RotationDirection.Counterclockwise) => CalculateCrossPath(start.Position, end.Position, startTurningBase, endTurningBase, UnitCircle.RotationDirection.Clockwise),
                (UnitCircle.RotationDirection.Counterclockwise, UnitCircle.RotationDirection.Clockwise) => CalculateCrossPath(start.Position, end.Position, startTurningBase, endTurningBase, UnitCircle.RotationDirection.Counterclockwise),
                (UnitCircle.RotationDirection.Counterclockwise, UnitCircle.RotationDirection.Counterclockwise) => CalculateStraightPath(start.Position, end.Position, startTurningBase, endTurningBase, UnitCircle.RotationDirection.Counterclockwise),
                _ => new Journey(),
            };
        }

        private static Journey CalculateStraightPath(Vector2 start, Vector2 end, Circle startTurningBase, Circle endTurningBase, UnitCircle.RotationDirection initialRotationDirection) {
            var fromTurningBaseToTurningBase = CalculateStraightTrajectoryBetweenTurningBases(startTurningBase, endTurningBase, initialRotationDirection);
            return CreateJourney(start, end, startTurningBase, endTurningBase, initialRotationDirection, initialRotationDirection, fromTurningBaseToTurningBase);
        }

        private static Journey CalculateCrossPath(Vector2 start, Vector2 end, Circle startTurningBase, Circle endTurningBase, UnitCircle.RotationDirection initialRotationDirection) {
            var fromTurningBaseToTurningBase = CalculateCrossTrajectoryBetweenTurningBases(startTurningBase, endTurningBase, initialRotationDirection);

            if (fromTurningBaseToTurningBase == null) {
                return null;
            }

            return CreateJourney(start, end, startTurningBase, endTurningBase, initialRotationDirection, (UnitCircle.RotationDirection)(-1 * (int)initialRotationDirection), fromTurningBaseToTurningBase);
        }

        private static Journey CreateJourney(Vector2 start, Vector2 end, Circle startTurningBase, Circle endTurningBase, UnitCircle.RotationDirection startRotationDirection, UnitCircle.RotationDirection endRotationDirection, ITrajectory fromTurningBaseToTurningBase) {
            var startArc = new Arc(
                startTurningBase,
                UnitCircle.VectorToAngle(start - startTurningBase.Center),
                UnitCircle.VectorToAngle(fromTurningBaseToTurningBase.StartPosition - startTurningBase.Center),
                startRotationDirection
            );

            var endArc = new Arc(
                endTurningBase,
                UnitCircle.VectorToAngle(fromTurningBaseToTurningBase.EndPosition - endTurningBase.Center),
                UnitCircle.VectorToAngle(end - endTurningBase.Center),
                endRotationDirection
            );

            var simpleJournies = new List<SimpleJourney>() {
                new SimpleJourney(startArc),
                new SimpleJourney(fromTurningBaseToTurningBase),
                new SimpleJourney(endArc)
            };

            return new Journey(simpleJournies);
        }

        private static ITrajectory CalculateStraightTrajectoryBetweenTurningBases(Circle startTurningBase, Circle endTurningBase, UnitCircle.RotationDirection initialRotationDirection) {
            var turningBaseToTurningBase = endTurningBase.Center - startTurningBase.Center;
            var fromCenterToIntersection = startTurningBase.Radius * -1.0f * ((int)initialRotationDirection) * Vector2.Perpendicular(turningBaseToTurningBase).normalized;
            return new Straight(
                startTurningBase.Center + fromCenterToIntersection,
                endTurningBase.Center + fromCenterToIntersection
            );
        }

        private static ITrajectory CalculateCrossTrajectoryBetweenTurningBases(Circle startTurningBase, Circle endTurningBase, UnitCircle.RotationDirection initialRotationDirection) {
            var turningBaseToTurningBase = endTurningBase.Center - startTurningBase.Center;
            var fromStartToMidpoint = 0.5f * turningBaseToTurningBase;
            var fromEndToMidpoint = -0.5f * turningBaseToTurningBase;

            var cosine = startTurningBase.Radius / fromStartToMidpoint.magnitude;

            // Cosine should always be between -1 and 1. If it is not then return null to indicate that trajectory is not valid.
            if (cosine > 1.0f || cosine < -1.0f) {
                return null;
            }

            var alpha = Mathf.Acos(cosine);
            return new Straight(
                startTurningBase.Center + startTurningBase.Radius * VectorTransformations.RotateVector(fromStartToMidpoint.normalized, -1.0f * ((int)initialRotationDirection) * alpha),
                endTurningBase.Center + endTurningBase.Radius * VectorTransformations.RotateVector(fromEndToMidpoint.normalized, -1.0f * ((int)initialRotationDirection) * alpha)
            );
        }
    }
}
