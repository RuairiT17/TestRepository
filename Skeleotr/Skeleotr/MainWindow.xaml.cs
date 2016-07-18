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
using GestureRecognizer;

namespace Skeleotr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        Skeleton skeleton;
        Skeleton[] skellys;
        private readonly Brush[] _SkeletonBrushes;
        JointType[] joints;
        WriteableBitmap bitmapy;
        private Int32Rect colorImageBitMapRect;
        private int _ColourImageStride;
        GestureRecognitionEngine recognitionEngine;
        GestureRecognitionEngine recog2;
        GestureRecognitionEngine recog3;
        GestureRecognitionEngine recog4;

        GestureImage gestImage;
        


        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            skellys = new Skeleton[6];
            
            gestImage = new GestureImage(GestureType.HandsClapping);


        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor = KinectSensor.KinectSensors[0];
                SetUpKinect(sensor);
               // gestImage = new GestureImage(GestureType.Toilet);
                KinectSensor.KinectSensors.StatusChanged += KinectStatusChanged;
                if (gestImage.GestTypeImage.Equals(GestureType.Toilet))
                {
                    GestureImageTBox.Text = "Toilet";
                    GestureImageTBox.FontSize = 30;
                }


            }
            else
            {


                MessageBox.Show("NO Kinects");
                if (!gestImage.GestTypeImage.Equals(recog4.GestureType))
                {
                    GestureImageTBox.Text = "Toilet";
                    GestureImageTBox.FontSize = 30;
                }
            }
        }

        private void KinectStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (sensor.Status == KinectStatus.Connected)
            {
                SetUpKinect(sensor);
                if (gestImage.GestTypeImage.Equals(GestureType.Toilet)){
                    GestureImageTBox.Text = "Toilet";
                }
            }else
            {
                MessageBox.Show("Kinect not Working");
            }
        }
        
        private void SetUpKinect(KinectSensor sensor)
        {
            
            if (sensor.Status == KinectStatus.Connected)


            {

                sensor.DepthStream.Enable();
                ColorImageStream colorStream = sensor.ColorStream;
                colorStream.Enable();
                sensor.SkeletonFrameReady += SkeletonFrameReady;
                this.bitmapy = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight,
                    96, 96, PixelFormats.Bgr32, null);
                this.colorImageBitMapRect = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
                this._ColourImageStride = colorStream.FrameWidth * colorStream.FrameBytesPerPixel;
                ColourStream.Source = this.bitmapy;
                sensor.ColorFrameReady += SensorColourReady;
                var SmoothingParams = new TransformSmoothParameters
                {
                   

                };
                sensor.SkeletonStream.Enable(SmoothingParams);

                recognitionEngine = new GestureRecognitionEngine();
                recog2 = new GestureRecognitionEngine();
                recog3 = new GestureRecognitionEngine();
                recog4 = new GestureRecognitionEngine();
                recog3.GestureType = GestureType.Eating;
                recog4.GestureType = GestureType.Toilet;
                recog2.GestureType = GestureType.Sleeping;
                recog3.GestureRecognized += new EventHandler<GestureEventArgs>
                    (recog3_GestureDone);
                recog2.GestureRecognized += new EventHandler<GestureEventArgs>
                    (recog2_GestureDone);
                recog4.GestureRecognized += new EventHandler<GestureEventArgs>(recog4_GestureRecognized);
                recognitionEngine.GestureType = GestureType.HandsClapping;
                recognitionEngine.GestureRecognized += new EventHandler<GestureEventArgs>
                    (recognitionEngine_GestureRecognized);
                
                sensor.Start();



            }
        }

        public void recog4_GestureRecognized (object sender , GestureEventArgs e)
        {
            MessageBox.Show("You Need to Pee");
        }

        public void recognitionEngine_GestureRecognized(object sender, GestureEventArgs e)
        {
            MessageBox.Show("You  Clapped Your Hnads");
        }

        public void recog2_GestureDone (object sender, GestureEventArgs e)
        {
            MessageBox.Show("You are Sleeping");
        }

       public void  recog3_GestureDone (object sender, GestureEventArgs e)
        {
            MessageBox.Show("You are Eating");
        }


        private void SensorColourReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame!= null)
                {
                    byte[] pixeldata = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixeldata);

                    this.bitmapy.WritePixels(this.colorImageBitMapRect, pixeldata, this._ColourImageStride, 0);
                }
            }
        }

        private void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            Canvy.Children.Clear();
            Brush brush = new SolidColorBrush(Colors.Red);
            Skeleton[] skeletons = null;
            SkeletonFrame frame;
            using (frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }

                if (skeletons == null) return;

                skeleton = (from trackSkeleton in skeletons
                            where trackSkeleton.TrackingState == SkeletonTrackingState.Tracked
                            select trackSkeleton).FirstOrDefault();

                if (skeleton == null)
                    return;

                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    DrawSkeleton();
                    recognitionEngine.skeleton = skeleton;
                   // recognitionEngine.StartRecognize();

                    if (gestImage.GestTypeImage.Equals(GestureType.Toilet))
                    {
                        recog4.skeleton = skeleton;
                        recog4.StartRecognize();
                    } else if (gestImage.GestTypeImage.Equals(GestureType.HandsClapping))
                    {
                        recognitionEngine.skeleton = skeleton;
                        recognitionEngine.StartRecognize();
                    }
            
            
                       // recog2.skeleton = skeleton;
                       // recog2.StartRecognize();
                        //recog3.skeleton = skeleton;
                      //  recog3.StartRecognize();
                    

                        // MessageBox.Show("NO skeleton");
                    }


                }
            }
        

        private Polyline CreateFigure(Skeleton skeletony, Brush brushy, JointType[] jointer)
        {
            Polyline figure = new Polyline();
            figure.StrokeThickness = 15;
            figure.Stroke = brushy;
            for (int i = 0; i < jointer.Length; i++)
            {



                figure.Points.Add(GetJointPoint(skeletony.Joints[jointer[i]]));
                
                
            }

            return figure;
        }

        private Point GetJointPoint( Joint jointy)
        {
            DepthImagePoint point = this.sensor.MapSkeletonPointToDepth(jointy.Position,
                sensor.DepthStream.Format);
            point.X *= (int)this.LayoutRoot.ActualWidth / this.sensor.DepthStream.FrameWidth;
            point.Y *= (int)this.LayoutRoot.ActualHeight / this.sensor.DepthStream.FrameHeight;

            return new Point(point.X, point.Y);
        }

        private void DrawSkeleton()
        {
            
   
            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);

            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
            drawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
            drawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
            drawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
            drawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
            drawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
            drawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);

            drawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
            //  drawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
            // drawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
            // drawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);

            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
            // drawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
            // drawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
            // drawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
        }

        /// <summary>
        /// Draws the bone.
        /// </summary>
        /// <param name="trackedJoint1">The tracked joint1.</param>
        /// <param name="trackedJoint2">The tracked joint2.</param>
        void drawBone(Joint trackedJoint1, Joint trackedJoint2)
        {
            Line bone = new Line();
            bone.Stroke = Brushes.Red;
            bone.StrokeThickness = 3;
            Point joint1 = this.ScalePosition(trackedJoint1.Position);
            bone.X1 = joint1.X;
            bone.Y1 = joint1.Y;






            Point mappedPoint1 = this.ScalePosition(trackedJoint1.Position);
            Rectangle r = new Rectangle(); r.Height = 10; r.Width = 10;
            r.Fill = Brushes.Red;
            Canvas.SetLeft(r, mappedPoint1.X - 2);
            Canvas.SetTop(r, mappedPoint1.Y - 2);
            Canvy.Children.Add(r);


            Point joint2 = this.ScalePosition(trackedJoint2.Position);
            bone.X2 = joint2.X;
            bone.Y2 = joint2.Y;

            Point mappedPoint2 = this.ScalePosition(trackedJoint2.Position);

            if (LeafJoint(trackedJoint2))
            {
                Rectangle r1 = new Rectangle(); r1.Height = 10; r1.Width = 10;
                r1.Fill = Brushes.Red;
                Canvas.SetLeft(r1, mappedPoint2.X - 2);
                Canvas.SetTop(r1, mappedPoint2.Y - 2);
                Canvy.Children.Add(r1);
            }

            if (LeafJoint(trackedJoint2))
            {
                Point mappedPoint = this.ScalePosition(trackedJoint2.Position);
                TextBlock textBlock = new TextBlock();
                textBlock.Text = trackedJoint2.JointType.ToString();
                textBlock.Foreground = Brushes.Black;
                Canvas.SetLeft(textBlock, mappedPoint.X + 5);
                Canvas.SetTop(textBlock, mappedPoint.Y + 5);
                Canvy.Children.Add(textBlock);
            }

            Canvy.Children.Add(bone);
        }

        /// <summary>
        /// Leafs the joint.
        /// </summary>
        /// <param name="j2">The j2.</param>
        /// <returns></returns>
        private bool LeafJoint(Joint j2)
        {
            if (j2.JointType == JointType.HandRight || j2.JointType == JointType.HandLeft || j2.JointType == JointType.FootLeft || j2.JointType == JointType.FootRight)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Scales the position.
        /// </summary>
        /// <param name="skeletonPoint">The skeleton point.</param>
        /// <returns></returns>
        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            ColorImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.InfraredResolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
    }
}
