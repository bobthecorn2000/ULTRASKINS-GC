using UnityEngine;
using System.Collections;


namespace UltraSkins.UI.Animation
{


    public class CameraOrthoPerspectiveSwitcher : MonoBehaviour
    {
        public Camera cam;
        public float transitionDuration = 2f;

        private bool isOrtho = true;
        private Coroutine transitionRoutine;

        void Start()
        {
            if (cam == null)
                cam = Camera.main;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (transitionRoutine == null)
                    transitionRoutine = StartCoroutine(SwitchCameraMode());
            }
        }

        IEnumerator SwitchCameraMode()
        {
            float elapsed = 0f;

            // Save start values
            float startOrthoSize = cam.orthographicSize;
            float startFOV = cam.fieldOfView;
            float startZ = cam.transform.position.z;

            // Target values
            float targetOrthoSize = 5f; // Ortho size when going back
            float targetFOV = 60f;      // Perspective FOV
            float camZOrtho = -10f;     // Z pos for ortho
            float camZPersp = -10f;     // Adjust if you want parallax

            if (isOrtho)
            {
                // Blend out of ortho into perspective
                while (elapsed < transitionDuration)
                {
                    float t = elapsed / transitionDuration;

                    cam.orthographicSize = Mathf.Lerp(startOrthoSize, 0.01f, t); // Fake shrink
                    cam.fieldOfView = Mathf.Lerp(1f, targetFOV, t); // Increase FOV
                    cam.transform.position = Vector3.Lerp(new Vector3(0, 0, camZOrtho), new Vector3(0, 0, camZPersp), t);

                    elapsed += Time.deltaTime;
                    yield return null;
                }

                cam.orthographic = false;
            }
            else
            {
                // Blend out of perspective into ortho
                cam.orthographic = true;

                while (elapsed < transitionDuration)
                {
                    float t = elapsed / transitionDuration;

                    cam.orthographicSize = Mathf.Lerp(0.01f, targetOrthoSize, t);
                    cam.fieldOfView = Mathf.Lerp(targetFOV, 1f, t);
                    cam.transform.position = Vector3.Lerp(new Vector3(0, 0, camZPersp), new Vector3(0, 0, camZOrtho), t);

                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }

            isOrtho = !isOrtho;
            transitionRoutine = null;
        }
    }
}