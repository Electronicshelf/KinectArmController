using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Microsoft.Kinect;
using System.IO.Ports;
using System.IO;


//Adapted from Abhijit Jana Kinect for windows SDk programming
namespace GestureRecognizer
{
   public class GestureRecognitionEngine
    {

       int SkipFramesAfterGestureIsDetected = 0;
       public bool IsGestureDetected { get; set; }

       public List<GestureBase> gestureCollection = null;

        public GestureRecognitionEngine()
    {
      InitilizeGesture();
    }

        public void InitilizeGesture()
        {
            this.gestureCollection = new List<GestureBase>();
            this.gestureCollection.Add(new ZoomInGesture());
            this.gestureCollection.Add(new ZoomOutGesture());
            this.gestureCollection.Add(new SwipeToRightGesture());
            this.gestureCollection.Add(new SwipeToLeftGesture());
        }

       
       // private  SerialPort port = new SerialPort("COM15", 19200, Parity.None, 8, StopBits.One);
       
        public event EventHandler<GestureEventArgs>GestureRecognized;
      //public event EventHandler<GestureEventArgs>GestureNotRecognized;
        public GestureType  GestureType { get; set; }
        public Skeleton Skeleton;


      

       

      

     
        public void StartRecognise()
        {
            switch (GestureType)
            {
                case GestureType.HandClapping:
                    this.MatchClappingGesture(this.Skeleton);
                    break;
                default:
                    break;
            }
           
            if (this.IsGestureDetected)
            {
                while (this.SkipFramesAfterGestureIsDetected <= 30)
                {
                    this.SkipFramesAfterGestureIsDetected++;
                }
                this.RestGesture();
                return;
            }

            foreach (var item in this.gestureCollection)
            {
                if (item.CheckForGesture(this.Skeleton))
                {
                    if (this.GestureRecognized != null)
                    {
                        this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success, item.GestureType));
                        this.IsGestureDetected = true;
                    }

                }
            }
           
        }
           
                   private void RestGesture()
                   {
                       this.gestureCollection = null;
                       this.InitilizeGesture();
                       this.SkipFramesAfterGestureIsDetected = 0;
                       this.IsGestureDetected = false;
                   }
           
                   float previousDistance = 0.0f;
                  
                   private void MatchClappingGesture(Skeleton skeleton)
                   {
                      
                       if (skeleton == null)
                       {
                           return;
                       }
           
                       if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked &&
                           skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
                       {
           
                        float  currentDistance = equationBox.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.HandLeft]);
                           {
           
           
                               if (currentDistance < 0.1f && previousDistance > 0.1f)
                               {
                                   
                                  if (this.GestureRecognized != null)
                                   {
                                       this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success, GestureType.HandClapping));
                                       
                                   }
                                
                                 
                               }
                              
                               previousDistance = currentDistance;
           
                           }
           
                           
                       }
                       
                   }
                   
        
       

    }

}

