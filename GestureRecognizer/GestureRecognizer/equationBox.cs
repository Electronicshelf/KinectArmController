using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace GestureRecognizer
{
    /// <summary>
    /// Gesture Helper
    /// </summary>
     
   public class equationBox
    {
      public  equationBox()
        {
        }
        Vector3 Hand = Vector3.Zero;

       
        public static float GetJointDistance(Joint firstJoint, Joint secondJoint)
        {
            float distanceX = firstJoint.Position.X - secondJoint.Position.X;
            float distanceY = firstJoint.Position.Y - secondJoint.Position.Y;
            float distanceZ = firstJoint.Position.Z - secondJoint.Position.Z;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) + Math.Pow(distanceZ, 2));
        }

        private float GetJointMagnitude(Joint handJoint)
        {
            float distanceX = handJoint.Position.X;
            float distanceY = handJoint.Position.Y;
            float distanceZ = handJoint.Position.Z;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) + Math.Pow(distanceZ, 2));
        }

        public double[] HandXYZ(Skeleton skeleton, int T)
        {


            if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked && T > 1)
            {
                Hand = Vector3.ToVector3(skeleton.Joints.Where(j => j.JointType == JointType.HandRight).First().Position);

            }
            else
                if (skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
                {
                    Hand = Vector3.ToVector3(skeleton.Joints.Where(j => j.JointType == JointType.HandLeft).First().Position);
                }


            double[] cord3D = new double[] { Hand.X, Hand.Y, Hand.Z };

            return cord3D;
        }
    }
}
