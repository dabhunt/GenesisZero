using UnityEngine;

namespace Cinemachine.Examples
{

    public class SingleInstanceCameraPanning : MonoBehaviour
    {
        CinemachineVirtualCamera vcam;
        public GameObject ObjectToView;
        public float CameraDistanceZ = 0.0f;
        public float Delay = 0.0f;
        public float Duration = 0.0f;

        // Camera Distance Z determines the distance from the object on the Z-axis
        // Delay determines how much time must pass before moving
        // Duration determines how long to show the object

        void Start()
        {

            // Create a virtual camera that looks at object and set some settings
            vcam = new GameObject("VirtualCamera").AddComponent<CinemachineVirtualCamera>();
            vcam.m_Priority = 11; // Must be higher than other cameras to work
            vcam.gameObject.transform.position = new Vector3(ObjectToView.transform.position.x, ObjectToView.transform.position.y, CameraDistanceZ);
            vcam.enabled = !vcam.enabled;
        }

        bool check1 = false;
        bool check2 = false;

        void Update()
        {
            // Turns on camera showing object after delay and turns it off once the duration has passed.
            if (Time.realtimeSinceStartup > Delay && !check1)
            {
                vcam.enabled = !vcam.enabled;
                check1 = true;
            }
            if (Time.realtimeSinceStartup > (Delay + Duration) && !check2)
            {
                vcam.enabled = !vcam.enabled;
                check2 = true;
            }
        }
    }

}
