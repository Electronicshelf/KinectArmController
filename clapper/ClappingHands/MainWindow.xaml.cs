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
        double[] cord3d;
        
        public MainWindow()
        
        {
            
            InitializeComponent();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                kinect = KinectSensor.KinectSensors[0];
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
            kinect.AllFramesReady += Kinect_SkeletonAllFramesReady;

            
            recognitionEngine = new GestureRecognitionEngine();
            armEngine = new ArmControllerEngine();
            //recognitionEngine.GestureType = GestureType.HandClapping;
            recognitionEngine.GestureRecognized += new EventHandler<GestureEventArgs>(recognitionEngine_GestureRecognized);
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
         
                 cord3d = recognitionEngine.HandXYZ(skel, a);
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
                    skeletonFrame.CopySkeletonDataTo(allSkeletons);
                }


                foreach(Skeleton firstskeleton in allSkeletons)
             {
                // wait(200);
               //if (firstskeleton.TrackingState ==  SkeletonTrackingState.Tracked)
               //{
                   
                   if (firstskeleton == null)
                   {
                       return;
                   }
                   
                    recognitionEngine.skeleton = firstskeleton;
                    recognitionEngine.StartRecognise();
                   //determine Joint coordinates
                      GetArmCord();
                      findAngles();
                      
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

        private void elevationAngle(int angleChange) 
        {

            if (angleChange > kinect.MinElevationAngle || angleChange < kinect.MaxElevationAngle) 
            {
            
              this.kinect.ElevationAngle = angleChange;
            
            }
        
        
        }
        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.elevationAngle(Int32.Parse(e.NewValue.ToString()));
        }

        }

          
       
        }
        
        





    

