
namespace GestureRecognizer
{
    using Microsoft.Kinect;

    /// <summary>
    /// 
    /// </summary>
    class ZoomInGesture : GestureBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomInGesture" /> class.
        /// </summary>
        public ZoomInGesture() : base(GestureType.ZoomIn) { }

        /// <summary>
        /// 
        /// </summary>
        private float lastDistance = 0;

        /// <summary>
        /// Validates the gesture start condition.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns></returns>
        protected override bool ValidateGestureStartCondition(Skeleton skeleton)
        {
            var handRightPoisition = skeleton.Joints[JointType.HandRight].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var hipCenterPosition = skeleton.Joints[JointType.HipCenter].Position;

            float dif = equationBox.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.HandLeft]);

            if ((handRightPoisition.Y < shoulderRightPosition.Y) && (handLeftPosition.Y < shoulderRightPosition.Y) &&
                 (handRightPoisition.Y > hipCenterPosition.Y) &&
                (handLeftPosition.Y > hipCenterPosition.Y) && dif <= 0.5)
            {
                lastDistance = dif;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is gesture valid] [the specified skeleton].
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>
        ///   <c>true</c> if [is gesture valid] [the specified skeleton]; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsGestureValid(Skeleton skeleton)
        {
            float dif = equationBox.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.HandLeft]);

            if (dif >= lastDistance)
            {
                lastDistance = dif;
                return true;
            }
            return true;
        }

        /// <summary>
        /// Validates the gesture end condition.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns></returns>
        protected override bool ValidateGestureEndCondition(Skeleton skeleton)
        {
            float dif = equationBox.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.HandLeft]);
            if (dif > 1.0 && dif > lastDistance)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validates the base condition.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns></returns>
        protected override bool ValidateBaseCondition(Skeleton skeleton)
        {
            var handRightPoisition = skeleton.Joints[JointType.HandRight].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var hipCenterPosition = skeleton.Joints[JointType.HipCenter].Position;

            if ((handRightPoisition.Y < shoulderRightPosition.Y) && (handLeftPosition.Y < shoulderRightPosition.Y) &&
                (handRightPoisition.Y > hipCenterPosition.Y) &&
               (handLeftPosition.Y > hipCenterPosition.Y))
            {
                return true;
            }
            return false;
        }
    }
}
