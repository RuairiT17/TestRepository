using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Kinect;

namespace GestureRecognizer
{
    public class GestureRecognitionEngine
    {

        public GestureRecognitionEngine()
        {

        }

        public event EventHandler<GestureEventArgs> GestureRecognized;
        public event EventHandler<GestureEventArgs> GestureNotRecognized;

        public GestureType GestureType { get; set; }
        public Skeleton skeleton { get; set; }


        public void StartRecognize()
        {
            switch (this.GestureType)
            {
                case GestureType.HandsClapping:
                    this.MatchHandClappingGesture(this.skeleton);
                    break;
                case GestureType.Sleeping:
                    this.MatchSleepingGesture(this.skeleton);
                    break;
                case GestureType.Eating:
                    this.MatchEatGesture(this.skeleton);
                    break;
                case GestureType.Toilet:
                    this.MatchToiletGesture(this.skeleton);
                    break;
                    
                default:
                    break;
            }

        }
        /// <summary>
        /// Gets the joint distance.
        /// </summary>
        /// <param name="firstJoint">The first joint.</param>
        /// <param name="secondJoint">The second joint.</param>
        /// <returns>retunr the distance</returns>
        private static float GetJointDistance(Joint firstJoint, Joint secondJoint)
        {
            float distanceX = firstJoint.Position.X - secondJoint.Position.X;
            float distanceY = firstJoint.Position.Y - secondJoint.Position.Y;
            float distanceZ = firstJoint.Position.Z - secondJoint.Position.Z;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) + Math.Pow(distanceZ, 2));
        }
        float previousDistance = 0.0f;
        private void MatchHandClappingGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return;
            }
            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                float currentDistance = GetJointDistance(skeleton.Joints[JointType.HandRight],
                    skeleton.Joints[JointType.HandLeft]);
                if (currentDistance < 0.1f && previousDistance > 0.1f)
                {
                    if (this.GestureRecognized != null)
                    {
                        this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success));
                    }
                }

                previousDistance = currentDistance;
            }

        }

        float previousRightHandShoulderDistance = 0.0f;
        float previousLeftHandShoulderDistance = 0.0f;
        float previousHeadShoulderDistance = 0.0f;


        private void MatchSleepingGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return;
            }

            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                float currentRightHandToShoulderDistance = GetJointDistance(skeleton.Joints[JointType.HandRight],
                    skeleton.Joints[JointType.ShoulderLeft]);
                float currentLeftHandToSHoulderDistnace = GetJointDistance(skeleton.Joints[JointType.HandLeft],
                    skeleton.Joints[JointType.ShoulderLeft]);
                float currentHeadToShoulderDistnace = GetJointDistance(skeleton.Joints[JointType.Head],
                    skeleton.Joints[JointType.ShoulderLeft]);
               

                if (currentRightHandToShoulderDistance < 0.1f && 
                    currentLeftHandToSHoulderDistnace <  0.5f
                    
                    )
                {
                    if (this.GestureRecognized != null)
                    {
                        this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success));
                    }
                }

                previousRightHandShoulderDistance = currentRightHandToShoulderDistance;
                previousLeftHandShoulderDistance = currentLeftHandToSHoulderDistnace;
                previousHeadShoulderDistance = currentHeadToShoulderDistnace;

            }

            
        }

        float previousRightHandHead = 0.0f;

        private void MatchEatGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return;
            }

            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                float currentRightHandToHead = GetJointDistance(
                    skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.Head]);

                if (currentRightHandToHead < 0.1f)
                {
                    if (this.GestureRecognized != null)
                    {
                        this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success));
                    }
                }
                previousRightHandHead = currentRightHandToHead;
            }

            

        }

        
        public void MatchToiletGesture (Skeleton skelly)
        {
            if (skelly.TrackingState == SkeletonTrackingState.Tracked)
            {
                SkeletonPoint rightHand = skelly.Joints[JointType.HandRight].Position;
                float currentRightHandX = rightHand.X;
                float cuurentRightHandY = rightHand.Y;

                SkeletonPoint leftShoulder = skelly.Joints[JointType.ShoulderLeft].Position;
                float currentLeftShoulderX = leftShoulder.X;
                float currentLeftShoulderY = leftShoulder.Y;

                if (skelly.Joints[JointType.HandRight].Position.X <
                  skelly.Joints[JointType.Spine].Position.X &&
                 skelly.Joints[JointType.HandRight].Position.Y > 
               skelly.Joints[JointType.HipLeft].Position.Y &&
                skelly.Joints[JointType.HandRight].Position.Y <
                skelly.Joints[JointType.ShoulderCenter].Position.Y
                && skelly.Joints[JointType.ShoulderLeft].Position.Y - 
                skelly.Joints[JointType.HandRight].Position.Y  
                <0.05)
               // skelly.Joints[JointType.HandRight].Position.Y - 
                //skelly.Joints[JointType.ElbowLeft].Position.Y)

               
                
                {
                    this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success));
                }

            
            }
        }
    }
}
