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
        private KinectSensor kinect;
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        double[] raMove = new double[6] { 7, 160, 80, 250, 165, 160 };
        string[] servoId = new string[6] { "INIT", "BASE", "SHLD", "ELBW", "WRST", "GRPR" };
        ArmControllerEngine armEngine;
        equationBox equation;
        private CoordinateMapper myMapper;
        double[] cord3d;
        
        
        public MainWindow()
        
        {
            
            InitializeComponent();
           // Loaded += new RoutedEventHandler(Window_Loaded);
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                kinect = KinectSensor.KinectSensors[0];
                myMapper = new CoordinateMapper(kinect);
                kinect.Start();
               

            }
            catch (Exception ex)
            {
                 System.Windows.MessageBox.Show("Could not find Kinect Camera: " + ex.Message);
                }

                kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                kinect.ColorStream.Enable(ColorImageFormat.RgbResolution1280x960Fps12);
                kinect.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Correction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.05f,
                    Prediction = 0.5f,
                    Smoothing = 0.5f
                });
                recognitionEngine = new GestureRecognitionEngine();
                
                kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(Kinect_SkeletonAllFramesReady);
                //recognitionEngine.GestureType = GestureType.HandClapping;
                recognitionEngine.GestureRecognized += new EventHandler<GestureEventArgs>(recognitionEngine_GestureRecognized);
                equation = new equationBox();
                armEngine = new ArmControllerEngine();
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
                 iValue.Text = string.Format("{0:0} ", raMove[0]); bValue.Text = string.Format("{0:0} ", raMove[1]); sValue.Text = string.Format("{0:0} ", raMove[2]);
                 eValue.Text = string.Format("{0:0} ", raMove[3]); wValue.Text = string.Format("{0:0} ", raMove[4]); gValue.Text = string.Format("{0:0} ", raMove[5]);
                 double BaseValue = CheckAngle(raMove[1]);
                 bValueAngle.Text = string.Format("{0:0} ", BaseValue); sValueAngle.Text = string.Format("{0:0} ", raMove[2]- 90); eValueAngle.Text = string.Format("{0:0} ", raMove[3]-90);
             }
             return cord3d;
         }
               
         public double[] findAngles()
         {
             wait(200);
             
             //Note the Z-axis of the kinect is represented as the Y-axis on the RA01 Robotic Arm
             double x1 = cord3d[0];
             double y1 = cord3d[2];
             double z1 = cord3d[1];
             double[] newRamove = armEngine.calcIK(x1, y1, z1);
           // double[] newRamove = armEngine.anglesINV_DH(x1, y1, z1);
             raMove = newRamove; 
             return raMove;
         }

         DateTime time = DateTime.Now;
         int count= 0;
        
        void recognitionEngine_GestureRecognized(object sender, GestureEventArgs e)
         {
            wait(3);
             clapShow();
            
             
         }
         
         void clapShow() 
          {

           count += 1;
            if (count == 1)
            {
                MessageBox.Show(string.Format(" X = {0:0.0}  Y= {1:0.0}   Z = {2:0.0} ", cord3d[0],  cord3d[1], cord3d[2]));
                   
            }
            count = 0 ; 
            } 
  

        private void Window_Closed(object sender, EventArgs e)
        {
            kinect.Stop();
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
                    HumanTracking.Source = depthImageFrame.ToBitmapSource();
                else
                    if (ColorImageRadioButton.IsChecked.Value)
                        HumanTracking.Source = colorImageFrame.ToBitmapSource();
                    else
                        HumanTracking.Source = null;
               
                
                if (skeletonFrame != null)
                {
                    if ((this.allSkeletons == null) || (this.allSkeletons.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.allSkeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    skeletonFrame.CopySkeletonDataTo(this.allSkeletons);
                }
                myCanvas.Children.Clear(); 
                foreach(Skeleton sd in allSkeletons)
                {
                   if (sd == null)
                   {
                       return;
                   }
                  
                   GetArmCord();
                   findAngles();
                   recognitionEngine.Skeleton = sd;
                   this.drawSkeleton(sd, myCanvas, Colors.Beige);
                   recognitionEngine.StartRecognise();
                   
                   
 
                   //determine Joint coordinates
                   
                     // 
                //}
               }
                 
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

        void drawBone(Joint trackedJoint1, Joint trackedJoint2)
        {
            Line bone = new Line();
            bone.Stroke = Brushes.Red;
            bone.StrokeThickness = 3;
            ColorImagePoint j1p = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(trackedJoint1.Position, ColorImageFormat.RgbResolution1280x960Fps12);
            Point joint1 = this.ScalePosition(trackedJoint1.Position);
            bone.X1 = joint1.X;
            bone.Y1 = joint1.Y;
           
            //Point mappedPoint11 = this.ScalePosition(trackedJoint1.Position);
            //
            //TextBlock textBlock = new TextBlock();
            //textBlock.Text = trackedJoint1.JointType.ToString();
            //textBlock.Foreground = Brushes.Black;
            //Canvas.SetLeft(textBlock, mappedPoint11.X + 5);
            //Canvas.SetTop(textBlock, mappedPoint11.Y + 5);
            //myCanvas.Children.Add(textBlock);
            //
            //Point mappedPoint1 = this.ScalePosition(trackedJoint1.Position);
            //Rectangle r = new Rectangle(); r.Height = 10; r.Width = 10;
            //r.Fill = Brushes.Red;
            //Canvas.SetLeft(r, mappedPoint1.X - 2);
            //Canvas.SetTop(r, mappedPoint1.Y - 2);
            //myCanvas.Children.Add(r);


            Point joint2 = this.ScalePosition(trackedJoint2.Position);
            bone.X2 = joint2.X;
            bone.Y2 = joint2.Y;

            Point mappedPoint2 = this.ScalePosition(trackedJoint2.Position);

           // if (LeafJoint(trackedJoint2))
           // {
           //     Rectangle r1 = new Rectangle(); r1.Height = 10; r1.Width = 10;
           //     r1.Fill = Brushes.Red;
           //     Canvas.SetLeft(r1, mappedPoint2.X - 2);
           //     Canvas.SetTop(r1, mappedPoint2.Y - 2);
           //     myCanvas.Children.Add(r1);
           // }
           //
           // if (LeafJoint(trackedJoint2))
           // {
           //     Point mappedPoint = this.ScalePosition(trackedJoint2.Position);
           //     TextBlock textBlock1 = new TextBlock();
           //     textBlock1.Text = trackedJoint2.JointType.ToString();
           //     textBlock1.Foreground = Brushes.Black;
           //     Canvas.SetLeft(textBlock1, mappedPoint.X + 5);
           //     Canvas.SetTop(textBlock1, mappedPoint.Y + 5);
           //     myCanvas.Children.Add(textBlock1);
           // }

            myCanvas.Children.Add(bone);
        }

        private void plotOnColorImage(Joint myJoint, ColorImageFormat myFormat, UIElement myObject, Canvas tgtCanvas)
        {
            //Transform the joint position from skeleton coordinates to color image position
            ColorImagePoint colP = myMapper.MapSkeletonPointToColorPoint(myJoint.Position, myFormat);
            //Define the UI element (ellipse) top left corner position inside the canvas
            Canvas.SetTop(myObject, (double)colP.Y / kinect.ColorStream.FrameHeight * tgtCanvas.Height - myObject.RenderSize.Height / 2);
            Canvas.SetLeft(myObject, (double)colP.X / kinect.ColorStream.FrameWidth * tgtCanvas.Width - myObject.RenderSize.Width / 2);
        }
        private void plotOnDepthImage(Joint myJoint, DepthImageFormat myFormat, UIElement myObject, Canvas tgtCanvas)
        {
            //Transform the joint position from skeleton coordinates to color image position
            DepthImagePoint colP = myMapper.MapSkeletonPointToDepthPoint(myJoint.Position, myFormat);
            //Define the UI element (ellipse) top left corner position inside the canvas
            Canvas.SetTop(myObject, (double)colP.Y / kinect.DepthStream.FrameHeight * tgtCanvas.Height - myObject.RenderSize.Height / 2);
            Canvas.SetLeft(myObject, (double)colP.X / kinect.DepthStream.FrameWidth * tgtCanvas.Width - myObject.RenderSize.Width / 2);
        }
        public void addLine(Joint j1, Joint j2, Color color, KinectSensor sensor, Canvas canvas)
        {
            Line boneLine = new Line();
            boneLine.Stroke = new SolidColorBrush(color);
            boneLine.StrokeThickness = 5;
            ColorImagePoint j1p = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution1280x960Fps12);
            //Rescale points to canvas size
            boneLine.X1 = (double)j1p.X / kinect.ColorStream.FrameWidth * canvas.Width;
            boneLine.Y1 = (double)j1p.Y / kinect.ColorStream.FrameHeight * canvas.Height;
            ColorImagePoint j2p = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution1280x960Fps12);
            boneLine.X2 = (double)j2p.X / kinect.ColorStream.FrameWidth * canvas.Width;
            boneLine.Y2 = (double)j2p.Y / kinect.ColorStream.FrameHeight * canvas.Height;
            canvas.Children.Add(boneLine);
        }
        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = this.kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point((double)depthPoint.X / kinect.ColorStream.FrameWidth * myCanvas.Width, depthPoint.Y/kinect.DepthStream.FrameHeight * myCanvas.Height);
        }

       

        public void drawSkeleton(Skeleton skeleton, Canvas target, Color color)
        {
            //Spine
            addLine(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter], color, kinect, target);
            addLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine], color, kinect, target);
            //Left leg
            addLine(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter], color, kinect, target);
            addLine(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft], color, kinect, target);
            addLine(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft], color, kinect, target);
            addLine(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft], color, kinect, target);
            addLine(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft], color, kinect, target);
            //Right leg 
            addLine(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter], color, kinect, target);
            addLine(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight], color, kinect, target);
            addLine(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight], color, kinect, target);
            addLine(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight], color, kinect, target);
            addLine(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight], color, kinect, target);
            //Left arm 
            addLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft], color, kinect, target);
            addLine(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft], color, kinect, target);
            addLine(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft], color, kinect, target);
            addLine(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft], color, kinect, target);
            //Right arm 
            addLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight], color, kinect, target);
            addLine(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight], color, kinect, target);
            addLine(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight], color, kinect, target);
            addLine(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight], color, kinect, target);
        }
       
        }




          
       
        }
        
        





    

