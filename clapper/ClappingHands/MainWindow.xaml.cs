using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.IO;
using System.IO.Ports;

using Arm_Controller;
using GestureRecognizer;


namespace ClappingHands
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
   
    public partial class MainWindow : Window
    {

        
        
        SerialPort port = new SerialPort("COM16", 19200, Parity.None, 8, StopBits.One);
        GestureRecognitionEngine recognitionEngine;
        private KinectSensor sensor;
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        double[] raMove = new double[6] { 7, 75, 160, 250, 165, 160 };
        string[] servoId = new string[6] { "INIT", "BASE", "SHLD", "ELBW", "WRST", "GRPR" };
        ArmControllerEngine armEngine;
        equationBox equation;
       // private CoordinateMapper myMapper;
        double[] cord3d  = new double[3];
        
        
        public MainWindow()
        
        {     
            InitializeComponent();
           //Loaded += new RoutedEventHandler(Window_Loaded);          
        }

        public void readFile(double a)
        {
            string Filename = "baseAngle.txt";
            TextReader fileRead = new StreamReader(Filename);
            a = double.Parse( fileRead.ReadLine());
            fileRead.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            this.sensor = KinectSensor.KinectSensors[0];

            this.sensor.SkeletonStream.Enable();
            this.sensor.DepthStream.Enable();
            this.sensor.ColorStream.Enable();

            armEngine = new ArmControllerEngine();
            this.sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(Kinect_SkeletonAllFramesReady);
            recognitionEngine = new GestureRecognitionEngine();
            //readFile(raMove[1]);
            equation = new equationBox();
            armEngine.setArm(raMove, port, servoId);
            recognitionEngine.GestureRecognized += new EventHandler<GestureEventArgs>(recognitionEngine_GestureRecognized);
            this.sensor.Start();
            
           
        }
         
        void wait(int a) 
        {
            for (int i = 0; i < a; i++) ;
           
        }
       public double CheckAngle(double a)
        {
            if (a < 165)
                a -= 90-75;
            else
                a -= 90;
            return a;
           
        }
      
         public double[]  GetArmCord()
         {
        
             foreach (Skeleton skel in allSkeletons)
             {
     
                 int a = 0;
                 if (LeftArmButton.IsChecked.Value)
                 {
                     a = 1;
                 }
                 else if (RightArmButton.IsChecked.Value) 
                 {
                     a = 5;
                 }
                                 cord3d = equation.HandXYZ(skel, a);
                 xAxes.Text = string.Format(" {0:0.0} ", cord3d[0]); yAxes.Text = string.Format(" {0:0.0} ", cord3d[1]); zAxes.Text = string.Format(" {0:0.0} ", cord3d[2]);
                 double[] raMove = armEngine.anglesINV_G(cord3d[0], cord3d[2], cord3d[1]);
                 iValue.Text = string.Format("{0:0} ", raMove[0]); bValue.Text = string.Format("{0:0} ", raMove[1]); sValue.Text = string.Format("{0:0} ", raMove[2]);
                 eValue.Text = string.Format("{0:0} ", raMove[3]); wValue.Text = string.Format("{0:0} ", raMove[4]); gValue.Text = string.Format("{0:0} ", raMove[5]);
                 double BaseValue = CheckAngle(raMove[1]);
                 bValueAngle.Text = string.Format("{0:0} ", BaseValue); sValueAngle.Text = string.Format("{0:0} ", raMove[2]- 90); eValueAngle.Text = string.Format("{0:0} ", raMove[3]-90);
             }
             return cord3d;
         }
               
      //   public double[] findAngles()
      //   {
      //       //wait(200);
      //       
      //       //Note the Z-axis of the kinect is represented as the Y-axis on the RA01 Robotic Arm
      //       double x1 = cord3d[0];
      //       double y1 = cord3d[2];
      //       double z1 = cord3d[1];
      //      
      //       return raMove;
      //   }

         
        // int count= 0;
        
        void recognitionEngine_GestureRecognized(object sender, GestureEventArgs e)
         {

             switch (e.GestureType) 
             {
                 case   (GestureType.SwipeToLeft):
                     this.armEngine.baseDynamicLeft(raMove, port, servoId);
                     break;
                 case (GestureType.SwipeToRight):
                     this.armEngine.baseDynamicRight(raMove, port, servoId);
                     break;
                 case (GestureType.ArmTriggerRight):
                     MessageBox.Show("ArmTriggered");
                     break;
                 default:
                     //this.armEngine.setArm(raMove, port, servoId);
                     break;
             }

            gestureList.Items.Add(e.GestureType.ToString());
             
            //wait(3);
           // clapShow(); 
        //   if (e.GestureType == GestureType.SwipeToLeft) { clapShow(); }
        // else
        //        MessageBox.Show(e.GestureType.ToString());
         }
       
         void clapShow() 
          {
             // armEngine.setArm(raMove, port, servoId);
           //count += 1;
           // if (count == 1)
           // {
           //   
           MessageBox.Show(string.Format(" X = {0:0.0}  Y= {1:0.0}   Z = {2:0.0} ", cord3d[0],  cord3d[1], cord3d[2]));
           //        
           // }
           // count = 0 ; 
            } 
      

        private void Window_Closed(object sender, EventArgs e)
        {
            sensor.Stop();
        }
        private void Kinect_SkeletonAllFramesReady(object source, AllFramesReadyEventArgs e)
        {
           
            ColorImageFrame colorImageFrame = null;
            DepthImageFrame depthImageFrame = null;
            SkeletonFrame skeletonFrame = null;
            
            try
            {
                
                colorImageFrame = e.OpenColorImageFrame();
                depthImageFrame = e.OpenDepthImageFrame();
                skeletonFrame   = e.OpenSkeletonFrame();

                 if (DepthImageRadioButton.IsChecked.Value)
                     image.Source = depthImageFrame.ToBitmapSource();
               
                 else
                     if (ColorImageRadioButton.IsChecked.Value)
                         image.Source = colorImageFrame.ToBitmapSource();
                     else

                        image.Source = null;
               
                
               
              
                 if (skeletonFrame == null)
                {
                    return;
                }
                // copy the frame data in to the collection
                skeletonFrame.CopySkeletonDataTo(allSkeletons);

                // get the first Tracked skeleton
                Skeleton firstSkeleton = (from trackskeleton in allSkeletons
                                          where trackskeleton.TrackingState == SkeletonTrackingState.Tracked
                                          select trackskeleton).FirstOrDefault();

                // if the first skeleton returns null
                if (firstSkeleton == null)
                {
                    return;
                }

                   myCanvas.Children.Clear(); 
                   recognitionEngine.Skeleton = firstSkeleton;
                     
                   GetArmCord();
                  // findAngles();
                   this.drawSkeleton(firstSkeleton, myCanvas, Colors.Chartreuse);
                   recognitionEngine.StartRecognise();
                  
               }
                                 
            finally
            {
                 
                if (colorImageFrame != null)
                   colorImageFrame.Dispose();
                if (depthImageFrame != null)
                    depthImageFrame.Dispose();
                if (skeletonFrame != null)
                    skeletonFrame.Dispose();
                
            }
        }

      

        private void ColorImageRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

    //   void drawBone(Joint trackedJoint1, Joint trackedJoint2)
    //   {
    //       Line bone = new Line();
    //       bone.Stroke = Brushes.Red;
    //       bone.StrokeThickness = 3;
    //       ColorImagePoint j1p = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(trackedJoint1.Position, ColorImageFormat.RgbResolution1280x960Fps12);
    //       Point joint1 = this.ScalePosition(trackedJoint1.Position);
    //       bone.X1 = joint1.X;
    //       bone.Y1 = joint1.Y;
    //      
    //       //Point mappedPoint11 = this.ScalePosition(trackedJoint1.Position);
    //       //
    //       //TextBlock textBlock = new TextBlock();
    //       //textBlock.Text = trackedJoint1.JointType.ToString();
    //       //textBlock.Foreground = Brushes.Black;
    //       //Canvas.SetLeft(textBlock, mappedPoint11.X + 5);
    //       //Canvas.SetTop(textBlock, mappedPoint11.Y + 5);
    //       //myCanvas.Children.Add(textBlock);
    //       //
    //       //Point mappedPoint1 = this.ScalePosition(trackedJoint1.Position);
    //       //Rectangle r = new Rectangle(); r.Height = 10; r.Width = 10;
    //       //r.Fill = Brushes.Red;
    //       //Canvas.SetLeft(r, mappedPoint1.X - 2);
    //       //Canvas.SetTop(r, mappedPoint1.Y - 2);
    //       //myCanvas.Children.Add(r);
    //
    //
    //       Point joint2 = this.ScalePosition(trackedJoint2.Position);
    //       bone.X2 = joint2.X;
    //       bone.Y2 = joint2.Y;
    //
    //       Point mappedPoint2 = this.ScalePosition(trackedJoint2.Position);
    //
    //      // if (LeafJoint(trackedJoint2))
    //      // {
    //      //     Rectangle r1 = new Rectangle(); r1.Height = 10; r1.Width = 10;
    //      //     r1.Fill = Brushes.Red;
    //      //     Canvas.SetLeft(r1, mappedPoint2.X - 2);
    //      //     Canvas.SetTop(r1, mappedPoint2.Y - 2);
    //      //     myCanvas.Children.Add(r1);
    //      // }
    //      //
    //      // if (LeafJoint(trackedJoint2))
    //      // {
    //      //     Point mappedPoint = this.ScalePosition(trackedJoint2.Position);
    //      //     TextBlock textBlock1 = new TextBlock();
    //      //     textBlock1.Text = trackedJoint2.JointType.ToString();
    //      //     textBlock1.Foreground = Brushes.Black;
    //      //     Canvas.SetLeft(textBlock1, mappedPoint.X + 5);
    //      //     Canvas.SetTop(textBlock1, mappedPoint.Y + 5);
    //      //     myCanvas.Children.Add(textBlock1);
    //      // }
    //
    //       myCanvas.Children.Add(bone);
    //   }

    //    private void plotOnColorImage(Joint myJoint, ColorImageFormat myFormat, UIElement myObject, Canvas tgtCanvas)
    //    {
    //        //Transform the joint position from skeleton coordinates to color image position
    //        ColorImagePoint colP = myMapper.MapSkeletonPointToColorPoint(myJoint.Position, myFormat);
    //        //Define the UI element (ellipse) top left corner position inside the canvas
    //        Canvas.SetTop(myObject, (double)colP.Y / kinect.ColorStream.FrameHeight * tgtCanvas.Height - myObject.RenderSize.Height / 2);
    //        Canvas.SetLeft(myObject, (double)colP.X / kinect.ColorStream.FrameWidth * tgtCanvas.Width - myObject.RenderSize.Width / 2);
    //    }
    //    private void plotOnDepthImage(Joint myJoint, DepthImageFormat myFormat, UIElement myObject, Canvas tgtCanvas)
    //    {
    //        //Transform the joint position from skeleton coordinates to color image position
    //        DepthImagePoint colP = myMapper.MapSkeletonPointToDepthPoint(myJoint.Position, myFormat);
    //        //Define the UI element (ellipse) top left corner position inside the canvas
    //        Canvas.SetTop(myObject, (double)colP.Y / kinect.DepthStream.FrameHeight * tgtCanvas.Height - myObject.RenderSize.Height / 2);
    //        Canvas.SetLeft(myObject, (double)colP.X / kinect.DepthStream.FrameWidth * tgtCanvas.Width - myObject.RenderSize.Width / 2);
    //    }
        public void addLine(Joint j1, Joint j2, Color color, KinectSensor sensor, Canvas canvas)
        {
            Line boneLine = new Line();
            boneLine.Stroke = new SolidColorBrush(color);
            boneLine.StrokeThickness = 5;
            DepthImagePoint j1p = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(j1.Position, DepthImageFormat.Resolution640x480Fps30);
            //Rescale points to canvas size
            boneLine.X1 = (double)j1p.X / sensor.DepthStream.FrameWidth * myCanvas.Width ;
            boneLine.Y1 = (double)j1p.Y / sensor.DepthStream.FrameHeight* myCanvas.Height;
            DepthImagePoint j2p = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(j2.Position, DepthImageFormat.Resolution640x480Fps30);
            boneLine.X2 = (double)j2p.X / sensor.DepthStream.FrameWidth * myCanvas.Width ;
            boneLine.Y2 = (double)j2p.Y / sensor.DepthStream.FrameHeight* myCanvas.Height;
           // myCanvas.Children.Add(boneLine);
        }
//        private Point ScalePosition(SkeletonPoint skeletonPoint)
//        {
//            DepthImagePoint depthPoint = this.kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);
//            return new Point((double)depthPoint.X / kinect.ColorStream.FrameWidth * myCanvas.Width, depthPoint.Y/kinect.DepthStream.FrameHeight * myCanvas.Height);
//        }
//
       

        public void drawSkeleton(Skeleton skeleton, Canvas target, Color color)
        {
            //Spine
            addLine(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter], color, sensor, target);
            addLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine], color, sensor, target);
            //Left leg
            addLine(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter], color, sensor, target);
            addLine(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft], color, sensor, target);
            addLine(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft], color, sensor, target);
            addLine(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft], color, sensor, target);
            addLine(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft], color, sensor, target);
            //Right leg 
            addLine(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter], color, sensor, target);
            addLine(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight], color, sensor, target);
            addLine(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight], color, sensor, target);
            addLine(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight], color, sensor, target);
            addLine(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight], color, sensor, target);
            //Left arm 
            addLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft], color, sensor, target);
            addLine(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft], color, sensor, target);
            addLine(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft], color, sensor, target);
            addLine(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft], color, sensor, target);
            //Right arm 
            addLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight], color, sensor, target);
            addLine(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight], color, sensor, target);
            addLine(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight], color, sensor, target);
            addLine(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight], color, sensor, target);
        }
       
        }




          
       
        }
        
        





    

