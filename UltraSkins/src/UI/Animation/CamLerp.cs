using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UltraSkins.UI.Layout;

namespace UltraSkins.UI.Animation
{


    public class LerpRectTransformFull : MonoBehaviour
    {
        [Header("Transforms")]
        public RectTransform startTransform;
        public RectTransform endTransform;
        public float lerpDuration = 2f;

        // The Mask component to enable/disable

        [Header("External UI Controls")]
        public RotateObject spinnyboi;
        public ChildCycler cyclechild;
        public  BandScroller bandScroller;

        private RectTransform rt;
        private Coroutine lerpCoroutine;

        public Quaternion lerpinresetRot = Quaternion.Euler(0, 90, 0);
        public Vector3 lerpresetLoc = new Vector3(0, 0, 0);
        public Vector3 lerpresetScale = new Vector3(1, 1, 1);
        public Quaternion lerpoutresetRot = Quaternion.Euler(0, 0, 0);
        public Image image_mask;
        public GameObject Stylization;

        void Awake()
        {
            rt = GetComponent<RectTransform>();

        }

        [Obsolete]
        public void MenuModeOn()
        {

            StartLerpRoutine(startTransform, endTransform, true);


        }
        public void MenuModeOff()
        {


            StartLerpRoutine(endTransform, startTransform, false);


        }

        [Obsolete]
        public void StartLerpRoutine(RectTransform from, RectTransform to, bool forward)
        {
            if (lerpCoroutine != null)
                StopCoroutine(lerpCoroutine);

            lerpCoroutine = StartCoroutine(LerpSequence(from, to, forward));
        }

        private IEnumerator LerpSequence(RectTransform from, RectTransform to, bool forward)
        {
            // External state handling
            if (forward)
            {
                spinnyboi.pause = true;
                spinnyboi.allowDrag = true;
                spinnyboi.allowPanZoom = true;
                spinnyboi.LerptoRot(lerpinresetRot, 0.5f);

                bandScroller.scrollin(1f, 0);
                cyclechild.isPaused = true;

            }
            else
            {
                image_mask.enabled = false;
                spinnyboi.pause = false;
                spinnyboi.allowDrag = false;
                spinnyboi.allowPanZoom = false;
                spinnyboi.LerptoRot(lerpoutresetRot, 0.5f);
                spinnyboi.LerptoScale(lerpresetScale, 0.5f);
                spinnyboi.LerptoPos(lerpresetLoc, 0.5f);
                cyclechild.isPaused = false;
                bandScroller.scrollin(1f, 1);
                Stylization.SetActive(false);

            }

            // Lerp the transform
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < lerpDuration)
            {
                float t = (Time.realtimeSinceStartup - startTime) / lerpDuration;
                rt.position = Vector3.Lerp(from.position, to.position, t);
                rt.localScale = Vector3.Lerp(from.localScale, to.localScale, t);
                rt.rotation = Quaternion.Lerp(from.rotation, to.rotation, t);
                rt.sizeDelta = Vector2.Lerp(from.sizeDelta, to.sizeDelta, t);

                yield return null;
            }

            // Final state
            rt.position = to.position;

            rt.localScale = to.localScale;
            rt.rotation = to.rotation;
            rt.sizeDelta = to.sizeDelta;
            if (forward)
            {
                image_mask.enabled = true;
                Stylization.SetActive(true);
            }

            lerpCoroutine = null;
        }

        private IEnumerator AnimateMask(float from, float to)
        {
            float duration = 0.5f;
            float time = 0f;

            while (time < duration)
            {
                float t = time / duration;
                float value = Mathf.Lerp(from, to, t);

                time += Time.deltaTime;
                yield return null;
            }


        }
    }
}