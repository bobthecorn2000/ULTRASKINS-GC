using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// for UI interaction checks

namespace UltraSkins.UI.Animation
{
    public class RotateObject : MonoBehaviour
    {
        public Vector3 rotationSpeed = new Vector3(0, 30f, 0); // Y-axis by default
        public bool pause = false;

        [Header("Drag Rotation")]
        public bool allowDrag = false;
        public RectTransform renderAreaUI;
        public float dragSpeed = 5f;

        [Header("Pan and Zoom")]
        public bool allowPanZoom = false;
        public float panSpeed = 0.1f; // Speed of pan movement
        public float zoomSpeed = 2f; // Speed of zoom (in or out)

        private bool isDragging = false;
        private Vector3 lastMousePosition;
        [SerializeField] float minX = -10f;
        [SerializeField] float maxX = 10f;
        [SerializeField] float minY = -3f;
        [SerializeField] float maxY = 3f;
        bool isPanning = false;

        public Slider sliderX;

        public Slider sliderY;
        public Slider sliderZoom;

        private Vector3 initialPosition; // Store the initial position for panning
        private float initialScale = 1f; // Store the initial scale for zooming

        void Start()
        {
            initialPosition = transform.position; // Capture the initial position of the object
            initialScale = transform.localScale.x; // Capture the initial scale (assuming uniform scale)
            
        }

        private void OnEnable()
        {
            StartCoroutine(UnscaledUpdateLoop());
        }

        private IEnumerator UnscaledUpdateLoop()
        {
            while (true)
            {
                if (allowDrag)
                {
                    HandleDragRotation();
                    HandlePan();
                }
                if (allowPanZoom)
                {
                    HandleZoom();
                }
                if (!pause && !isDragging)
                {
                    transform.Rotate(rotationSpeed * Time.unscaledDeltaTime);
                }

                yield return null;
            }
        }


        void HandleDragRotation()
        {
            if (Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint(renderAreaUI, Input.mousePosition))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;

                // Corrected for 90° Z-axis camera view
                float rotY = delta.y * dragSpeed * Time.deltaTime;  // vertical drag -> Y-axis rotation
                float rotX = delta.x * dragSpeed * Time.deltaTime;  // horizontal drag -> X-axis rotation

                transform.Rotate(Vector3.down, rotY, Space.World);
                transform.Rotate(Vector3.right, -rotX, Space.World); // optional sign flip for feel

                lastMousePosition = Input.mousePosition;
            }
        }
        void HandlePan()
        {
            if (Input.GetMouseButtonDown(1) && RectTransformUtility.RectangleContainsScreenPoint(renderAreaUI, Input.mousePosition))
            {
                isPanning = true;
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                isPanning = false;
            }

            if (isPanning)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                float panY = -delta.x * panSpeed * Time.deltaTime;
                float panX = delta.y * panSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + new Vector3(panX, panY, 0f);
                // Clamp to screen bounds
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

                transform.position = newPosition;
                lastMousePosition = Input.mousePosition;
                sliderY.value = Mathf.InverseLerp(minX, maxX, newPosition.x);
                sliderX.value = Mathf.InverseLerp(minY, maxY, -newPosition.y);
            }
        }
        void HandleZoom()
        {


            if (Input.mouseScrollDelta.y != 0 && RectTransformUtility.RectangleContainsScreenPoint(renderAreaUI, Input.mousePosition)) // Mouse wheel for zoom
            {
                float zoomDelta = Input.mouseScrollDelta.y * zoomSpeed;
                transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x + zoomDelta, 0.1f, 3f),
                                                   Mathf.Clamp(transform.localScale.y + zoomDelta, 0.1f, 3f),
                                                   Mathf.Clamp(transform.localScale.z + zoomDelta, 0.1f, 3f));
            }

            lastMousePosition = Input.mousePosition;
        }

        public void LerptoRot(Quaternion targetRotation, float lerpDuration)
        {
            StartCoroutine(LerpToRotation(targetRotation, lerpDuration));
        }
        public void LerptoScale(Vector3 targetZoom, float lerpDuration)
        {
            StartCoroutine(LerpToZoom(targetZoom, lerpDuration));
        }
        public void LerptoPos(Vector3 targetPos, float lerpDuration)
        {
            StartCoroutine(LerpToLocation(targetPos, lerpDuration));
        }

        private IEnumerator LerpToRotation(Quaternion targetRotation, float duration)
        {
            Quaternion startRotation = transform.rotation;
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRotation; // ensure exact final value
        }
        private IEnumerator LerpToZoom(Vector3 targetZoom, float duration)
        {
            Vector3 startZoom = transform.localScale;
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                transform.localScale = Vector3.Lerp(startZoom, targetZoom, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = targetZoom; // ensure exact final value
        }
        private IEnumerator LerpToLocation(Vector3 targetLocation, float duration)
        {
            Vector3 startLocation = transform.position;
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                transform.position = Vector3.Lerp(startLocation, targetLocation, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetLocation; // ensure exact final value
        }

        public void ResetToOriginal(float duration)
        {
            LerptoPos(new Vector3(0, 0, 0), duration);
            LerptoRot(Quaternion.Euler(0, 90, 0), duration);
            LerptoScale(new Vector3(1, 1, 1), duration);
        }
    }
}
