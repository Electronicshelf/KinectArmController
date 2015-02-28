using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


using System.Diagnostics;
namespace kinectEllipse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinect;
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        private Ellipse rightEllipse, leftEllipse;
        private CoordinateMapper myMapper;
        public MainWindow()
        {
           InitializeComponent();

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
                System.Windows.MessageBox.Show("Could not find Kinect camera: " + ex.Message);
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
            kinect.AllFramesReady += camera_AllFramesReady;

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            kinect.Stop();
        }
        private void CamDownButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (kinect.ElevationAngle <= kinect.MaxElevationAngle)
                    kinect.ElevationAngle -= 3;
            }
            catch
            {
                System.Windows.MessageBox.Show("Elevation angle change not succesfull");
            }
        }
        private void createEllipses()
        {
            rightEllipse = new Ellipse();
            canvas.Children.Add(rightEllipse);
            rightEllipse.Height = 30;
            rightEllipse.Width = 30;
            rightEllipse.Fill = Brushes.Aqua;
            
            leftEllipse = new Ellipse();
            canvas.Children.Add(leftEllipse);
            leftEllipse.Height = 30;
            leftEllipse.Width = 30;
            leftEllipse.Fill = Brushes.PaleVioletRed;
        }
        private void camera_AllFramesReady(object source, AllFramesReadyEventArgs e)
        {
            ColorImageFrame colorImageFrame = null;
            DepthImageFrame depthImageFrame = null;
            SkeletonFrame skeletonFrame = null;
            try
            {
                colorImageFrame = e.OpenColorImageFrame();
                depthImageFrame = e.OpenDepthImageFrame();
                skeletonFrame = e.OpenSkeletonFrame();

                if (DepthRadioButton.IsChecked.Value)
                    image.Source = depthImageFrame.ToBitmapSource();
                else
                    if (ColorRadioButton.IsChecked.Value)
                        image.Source = colorImageFrame.ToBitmapSource();
                    else
                        image.Source = null;

                if (skeletonFrame != null)
                {
                    if ((this.allSkeletons == null) || (this.allSkeletons.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.allSkeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    skeletonFrame.CopySkeletonDataTo(this.allSkeletons);
                }
                               
               foreach (Skeleton sd in allSkeletons)
               {
               
                    if (sd.TrackingState == SkeletonTrackingState.Tracked)
                    {
                drawSkeleton(sd,canvas,Colors.Green);
                   //     Joint LeftHand = sd.Joints[JointType.HandLeft];
                   //     Joint RightHand = sd.Joints[JointType.HandRight];
                   //
                   //     if (RightHand.TrackingState == JointTrackingState.Tracked && LeftHand.TrackingState == JointTrackingState.Tracked)
                   //     {
                   //         if (rightEllipse == null || leftEllipse == null)
                   //             createEllipses();
                   //         if (DepthRadioButton.IsChecked.Value)
                   //         {
                   //             plotOnDepthImage(RightHand, DepthImageFormat.Resolution640x480Fps30, rightEllipse, canvas);
                   //             plotOnDepthImage(LeftHand, DepthImageFormat.Resolution640x480Fps30, leftEllipse, canvas);
                   //         }
                   //         else
                   //         {
                   //             plotOnColorImage(RightHand, ColorImageFormat.RgbResolution1280x960Fps12, rightEllipse, canvas);
                   //             plotOnColorImage(LeftHand, ColorImageFormat.RgbResolution1280x960Fps12, leftEllipse, canvas);
                   //             
                   //         }
                   //     }
                  }
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
      
       
        private void CamUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (kinect.ElevationAngle >= kinect.MinElevationAngle +5)
                    kinect.ElevationAngle += 3;
            }
            catch
            {
                System.Windows.MessageBox.Show("Elevation angle change not succesful");
            }

        }
      
        public void addLine(Joint j1, Joint j2, Color color, KinectSensor sensor ,Canvas canvas) {
            Line boneLine = new Line();
            boneLine.Stroke = new SolidColorBrush(color);
            boneLine.StrokeThickness = 5;
            ColorImagePoint j1p = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution1280x960Fps12);
            //Rescale points to canvas size
            boneLine.X1 = (double)j1p.X/kinect.ColorStream.FrameWidth * canvas.Width;
            boneLine.Y1 = (double)j1p.Y/kinect.ColorStream.FrameHeight * canvas.Height;
            ColorImagePoint j2p = sensor.CoordinateMapper. MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution1280x960Fps12);
            boneLine.X2 = (double)j2p.X/kinect.ColorStream.FrameWidth * canvas.Width;
            boneLine.Y2 = (double)j2p.Y/kinect.ColorStream.FrameHeight* canvas.Height;
            canvas.Children.Add(boneLine); 
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
        private void ColorRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DepthRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void NoneRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

     
    }
}
