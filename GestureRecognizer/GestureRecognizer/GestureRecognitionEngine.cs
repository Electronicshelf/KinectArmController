using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Microsoft.Kinect;
using System.IO.Ports;
using System.IO;


//Adapted from Abhijit Jana Approach on  Kinect for windows SDk programming
namespace GestureRecognizer
{
   public class GestureRecognitionEngine
    {

       int SkipFramesAfterGestureIsDetected = 0;
       public bool IsGestureDetected { get; set; }

       public List<GestureBase> gestureCollection = null;
       
       public GestureRecognitionEngine()
       {
           this.InitilizeGesture();
       }

        public void InitilizeGesture()
       {     
            this.gestureCollection = new List<GestureBase>();
            this.gestureCollection.Add(new ArmTriggerRight());
            //this.gestureCollection.Add(new ZoomInGesture());
            //this.gestureCollection.Add(new ZoomOutGesture());
            this.gestureCollection.Add(new SwipeToRightGesture());
            this.gestureCollection.Add(new SwipeToLeftGesture());
        }

       
      
       
        public event EventHandler<GestureEventArgs>GestureRecognized;
      //public event EventHandler<GestureEventArgs>GestureNotRecognized;
        public GestureType  GestureType { get; set; }
        public Skeleton Skeleton { get; set; }






                    


        public void StartRecognise()
        {

           
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
           
                
        
       

    }

}

