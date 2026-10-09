using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UltraSkins.UI.Camp
{
    public class EditMenuInfoBox : MonoBehaviour
    {
        // Start is called before the first frame update
        public Image Border;

        public Image PlusImage;
        public Transform PlusTransform;
        public Animator plusAnimator;
        public TextMeshProUGUI TextSlot1;
        public TextMeshProUGUI TextSlot2;
        public float spinSpeed = 180f; // degrees per second
        public Button OpenFolder;
        public Button ReturnToMainMenu;
        public Button GoBack;
        
        private Coroutine spinRoutine;
        private bool isSpinning = false;


        public void Start()
        {

        }
        public void StartThrobber()
        {
            /*           spinSpeed = 180f;
                      if (spinRoutine != null)
                          StopCoroutine(spinRoutine);

                      isSpinning = true;
                      spinRoutine = StartCoroutine(SpinThrobber()); */
            plusAnimator.SetTrigger("StartLoop");

        }
        /*         private IEnumerator SpinThrobber()
                {
                    while (isSpinning)
                    {
                        PlusTransform.Rotate(Vector3.forward, -spinSpeed * Time.deltaTime);
                        yield return null;
                    }
                } */



        public void StopThrobber(SpinState spinstate)
        {
            StartCoroutine(SpinToFinalPosition(spinstate));
        }


        private IEnumerator SpinToFinalPosition(SpinState spinstate)
        {
            yield return null;
            switch (spinstate)
            {


                case SpinState.Good:
                    plusAnimator.SetTrigger("Good");
                    break;
                case SpinState.Bad:
                    plusAnimator.SetTrigger("Bad");
                    break;
            }
        }
        public enum SpinState
        {
            Good,
            Bad,
        }




        /*         public void StopThrobber(float finalRotation = 0f, Color? fadeTo = null, float duration = 0.5f)
                    {
                        isSpinning = false;

                        if (spinRoutine != null)
                            StopCoroutine(spinRoutine);

                            spinRoutine = StartCoroutine(SpinToFinalPosition(finalRotation, fadeTo ?? Color.white, duration));
                        } */
        /*         private IEnumerator SpinToFinalPosition(float targetAngle, Color fadeTo, float duration)
                {
                    float elapsed = 0f;
                    float startSpeed = spinSpeed;

                    // Normalize angles to [0,360)
                    float currentAngle = PlusTransform.eulerAngles.z % 360f;
                    targetAngle = targetAngle % 360f;

                    // Calculate how many degrees to rotate to reach target after next full revolution
                    float degreesToTarget = 360f - currentAngle + targetAngle;

                    // Track rotation travelled
                    float rotated = 0f;

                    Color startColor = PlusImage.color;

                    while (elapsed < duration)
                    {
                        elapsed += Time.deltaTime;
                        float t = elapsed / duration;

                        // Ease out spinSpeed from startSpeed to 0
                        spinSpeed = Mathf.Lerp(startSpeed, 0f, t);

                        // Rotate based on current spinSpeed
                        float deltaRotation = spinSpeed * Time.deltaTime;
                        PlusTransform.Rotate(Vector3.forward, -deltaRotation);

                        rotated += deltaRotation;

                        // Lerp color
                        PlusImage.color = Color.Lerp(startColor, fadeTo, t);

                        yield return null;
                    }

                    // After easing out speed, complete the remaining rotation to targetAngle exactly
                    float finalAngle = PlusTransform.eulerAngles.z % 360f;
                    float remainingRotation = degreesToTarget - rotated;

                    // Rotate smoothly from current to target angle over 0.25s
                    float smoothDuration = 0.25f;
                    float smoothElapsed = 0f;

                    Quaternion startRot = PlusTransform.rotation;
                    Quaternion endRot = Quaternion.Euler(0, 0, targetAngle);

                    while (smoothElapsed < smoothDuration)
                    {
                        smoothElapsed += Time.deltaTime;
                        float smoothT = smoothElapsed / smoothDuration;
                        PlusTransform.rotation = Quaternion.Slerp(startRot, endRot, smoothT);
                        yield return null;
                    }

                    PlusTransform.rotation = endRot;
                    PlusImage.color = fadeTo;
                } */

        public void ForceSetThrobber(float degree, Color color)
        {
            PlusTransform.rotation = Quaternion.Euler(0, 0, degree);
            PlusImage.color = color;
        }

        public void setText1(string text)
        {
            TextSlot1.text = text;
        }
        public void setText2(string text)
        {
            TextSlot2.text = text;
        }
        // Update is called once per frame

    }
}
