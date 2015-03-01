using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.IO.Ports;
using System.IO;


namespace GestureRecognizer
{
   public class GestureRecognitionEngine
    {


        public GestureRecognitionEngine()
    {
    }
       
       private  SerialPort port = new SerialPort("COM15", 19200, Parity.None, 8, StopBits.One);
       Vector3 Hand = Vector3.Zero;
       public event EventHandler<GestureEventArgs>GestureRecognized;
      //public event EventHandler<GestureEventArgs>GestureNotRecognized;
        public GestureType  GestureType { get; set; }
        public Skeleton skeleton;
     
       
  

          //write value of joints to texbox  
        void wait(int a) { for (int v = 0; v < a; a++);}
      
        


       public double[] HandXYZ(Skeleton skeleton , int T)
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

       

       private float GetJointDistance(Joint firstJoint, Joint secondJoint)
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
        public void StartRecognise()
        {
            switch (GestureType)
            {
                case GestureType.HandClapping:
                    this.MatchClappingGesture(this.skeleton);
                    break;
                case GestureType.TrackArm:
                    this.armControl(this.skeleton);
                    break;
               default:
                    break;
            }
           
        }
        public int forward(int fwd)
        {

            return fwd += 1;

        }

        void count()
        {
            for (int e = 0; e < 200; e++) ;
               // Console.WriteLine(e);
        }


        

        float previousDistance = 0.0f;
        float currentDistance = 0.0f;
       
       // DateTime finalDate;
    //float delay = 0;
    void reset()
    {
        previousDistance = 0.0f;
        currentDistance = 0.0f;

    }
        private void MatchClappingGesture(Skeleton skeleton)
        {
           
            if (skeleton == null)
            {
                return;
            }

            if (skeleton.Joints[JointType.WristRight].TrackingState == JointTrackingState.Tracked && skeleton.Joints[JointType.WristLeft].TrackingState == JointTrackingState.Tracked)
            {

               currentDistance = GetJointDistance(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.WristLeft]);
                {


                    if (currentDistance < 0.1f && previousDistance > 0.1f)
                    {
                        int triggerTime = DateTime.Now.Millisecond ;
                       if (this.GestureRecognized != null)
                        {
                            this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success));
                          
                        }
                       count();

                       int claptTime = DateTime.Now.Millisecond;
                        if ( claptTime > triggerTime + 100 )
                        {
                           GestureRecognized = null;
                           //this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Failed));
                            reset();
                            
                        }
                        
                    }
                   
                    previousDistance = currentDistance;

                }

                currentDistance = GetJointDistance(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.WristLeft]);
            }
            
        }
       
        
        float armInitialPoint = 0.1f;
        float armFinalPoint;
           private void armControl (Skeleton skeleton) 
            {

                
                if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked )
                {
                    Hand = Vector3.ToVector3(skeleton.Joints.Where(j => j.JointType == JointType.HandRight).First().Position);
                armFinalPoint = GetJointMagnitude(skeleton.Joints[JointType.HandRight]);
                    

                }
               // else
               //     if (skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
               //     {
               //         Hand = Vector3.ToVector3(skeleton.Joints.Where(j => j.JointType == JointType.HandLeft).First().Position);
               //       armFinalPoint = GetJointMagnitude(skeleton.Joints[JointType.HandLeft]);
               //     }


                if (armInitialPoint > Hand.Y  )


                {

                    if (this.GestureRecognized != null)
                    {

                        this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success));
                    }

                }
                armInitialPoint = Hand.Y;
        
            }
    

    }

}

