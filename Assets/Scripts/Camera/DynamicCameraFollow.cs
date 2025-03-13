using UnityEngine;
using Unity.Cinemachine;

namespace CustomCamera
{
    [AddComponentMenu("CustomCamera/Dynamic Camera Follow")]
    public class DynamicCameraFollow : CinemachineFollow
    {
        [Tooltip("Additional offset when the player moves to the right")]
        public Vector3 RightOffset = new Vector3(5f, 0, 0);

        [Tooltip("Additional offset when the player moves to the left")]
        public Vector3 LeftOffset = new Vector3(-5f, 0, 0);

        [Tooltip("Time in seconds to reach the offset")]
        public float OffsetTransitionTime = 0.5f;

        private Vector3 previousPosition;
        private Vector3 currentOffset;
        private float offsetLerpTime;

        void Start()
        {
            previousPosition = FollowTargetPosition;
            currentOffset = Vector3.zero;
            offsetLerpTime = 0f;
        }

        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            base.MutateCameraState(ref curState, deltaTime);

            if (IsValid)
            {
                Vector3 currentPosition = FollowTargetPosition;
                Vector3 direction = currentPosition - previousPosition;

                if (direction.x > 0)
                {
                    currentOffset = RightOffset;
                    offsetLerpTime = 0f;
                }
                else if (direction.x < 0)
                {
                    currentOffset = LeftOffset;
                    offsetLerpTime = 0f;
                }
                else
                {
                    offsetLerpTime += deltaTime;
                    currentOffset = Vector3.Lerp(currentOffset, Vector3.zero, offsetLerpTime / OffsetTransitionTime);
                }

                curState.RawPosition += currentOffset;
                previousPosition = currentPosition;
            }
        }
    }
}
