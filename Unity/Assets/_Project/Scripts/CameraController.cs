using _Project.Ray_Tracer.Scripts;
using UnityEngine;

namespace _Project.Scripts
{
    /// <summary>
    /// A camera controller that mimics the controls and behavior of the Unity editor camera. Adapted from
    /// http://wiki.unity3d.com/index.php?title=MouseOrbitZoom.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        public Vector3 RTCamOffset;
        public Transform player;
        public Transform playerCamera;

        void Start()
        {

        }

        private bool flytocam = false;

        public void FlyToRTCamera()
        {
            flytocam = true;
        }

        private void FlyToRTCameraStep()
        {
            Transform RTCamTransform = RTSceneManager.Get().Scene.Camera.transform;
            Quaternion RTCamRotation = Quaternion.Euler(0.0f, RTCamTransform.eulerAngles.y, 0.0f);

            player.position = Vector3.Lerp(player.position + RTCamOffset, RTCamTransform.position, 0.1f);
            playerCamera.rotation = Quaternion.Lerp(playerCamera.rotation, RTCamRotation, 0.1f);
        }

        private void FixedUpdate()
        {
            if (flytocam)
            {
                Transform RTCamTransform = RTSceneManager.Get().Scene.Camera.transform;
                float positionDifference = (player.position - (8.6f * RTCamOffset) - RTCamTransform.position).magnitude;
                float rotationSimilarity = playerCamera.eulerAngles.y - RTCamTransform.eulerAngles.y;
                // Debug.Log($"pos dif: {positionDifference}");
                // Debug.Log($"rot sim: {rotationSimilarity}");
                if (positionDifference < 0.075f && rotationSimilarity < 0.1f)
                    flytocam = false;
                else
                    FlyToRTCameraStep();
            }
        }

        void Update()
        {

        }
    }
}
