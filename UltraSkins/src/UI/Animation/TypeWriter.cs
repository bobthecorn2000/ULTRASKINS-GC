using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace UltraSkins.UI.Animation
{
    public class TypeWriter : MonoBehaviour
    {
        [Header("Text Settings")]
        [TextArea]
        private string fullText;
        public float timeBetweenLetters = 0.05f;

        [Header("Behavior Settings")]
        public bool runOnlyOnce = false;
        public bool showCursor = true;
        private AudioSource audioSource;
        private TextMeshProUGUI textMesh;
        private Coroutine typingCoroutine;
        private string currentText = "";
        private bool hasRun = false;
        private readonly string cursorChar = "█";

        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            if (textMesh == null)
                Debug.LogError("TypewriterText requires a TextMeshProUGUI component.");
            fullText = textMesh.text;
            audioSource = GetComponent<AudioSource>();

        }
        public void RefreshFullText(string newfulltext)
        {
            fullText = newfulltext;
        }

        private void OnEnable()
        {
            if (runOnlyOnce && hasRun) return;

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText());
        }

        private IEnumerator TypeText()
        {
            hasRun = true;
            currentText = "";
            textMesh.text = "";

            for (int i = 0; i <= fullText.Length; i++)
            {
                currentText = fullText.Substring(0, i);
                textMesh.text = currentText + (showCursor && i < fullText.Length ? cursorChar : "");
                if (audioSource)
                {
                    audioSource.Play();
                }
                yield return new WaitForSecondsRealtime(timeBetweenLetters);
            }

            // Final text without cursor
            textMesh.text = currentText;
            typingCoroutine = null;
        }
    }
}
