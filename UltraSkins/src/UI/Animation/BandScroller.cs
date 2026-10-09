using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraSkins.UI.Animation
{
    public class BandScroller : MonoBehaviour
    {
        // Start is called before the first frame update
        public float bandValue;
        public float scrollSpeed = 1f; // how fast it scrolls
        public bool pause = true;

        void Start () {
            Shader.SetGlobalFloat("_BandPosition", bandValue);
        }
        void Update()
        {
/*             if (!pause) {
            // Ping-pong the value between 0 and 1 over time
            float bandValue = Mathf.PingPong(Time.time * scrollSpeed, 1f);

            // Set it globally for all shaders
            Shader.SetGlobalFloat("_BandPosition", bandValue);
            } */
        }
        public void scrollin(float dur, float amountfill)
        {
            StartCoroutine(scrollincoroutine(dur, amountfill));

        }
        private IEnumerator scrollincoroutine(float duration, float amountfill)
        {
            float startRotation = bandValue;
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                bandValue = Mathf.Lerp(startRotation, amountfill, timeElapsed / duration);
                timeElapsed += Time.unscaledDeltaTime;
                Shader.SetGlobalFloat("_BandPosition", bandValue);
                yield return null;
            }


        }

    }
}
