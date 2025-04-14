using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace BatonPassLogger.GUI
{
    [ConfigureSingleton(SingletonFlags.PersistAutoInstance)]
    internal class BPGUIManager : MonoSingleton<BPGUIManager>
    {
        public static BPGUIManager BPGUIinstance { get; set; }
        public static GameObject batonpassGUIInst { get; set; }
        GameObject messagebox = null;
        GameObject ProgressBar = null;
        GameObject messageboxOutline = null;
        GameObject Slider = null;
        Animator BoxAnimator = null;
        Animator BarAnimator = null;
        GameObject messageOBJ = null;
        GameObject TerminalOBJ = null;
        TextMeshProUGUI message = null;
        TextMeshProUGUI terminal = null;
        Image bordercolor = null;
        
        Slider PercentComplete = null;
        public int maxLines = 10;
        private string currentTermText = "";
        private void Awake()
        {
            if (BPGUIinstance == null)
            {
                BPGUIinstance = this;
                LoadBatonPassPrefab();
                DontDestroyOnLoad(BPGUIinstance);
            }
            else
            {
                Destroy(gameObject); // Ensures only one instance exists
            }
        }
        public static BPGUIManager BPGUI => BPGUIinstance;
        private void LoadBatonPassPrefab()
        {
            Addressables.LoadAssetAsync<GameObject>("Assets/BatonpassGUI.prefab").Completed += handle =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    GameObject prefab = handle.Result;
                    batonpassGUIInst = Instantiate(prefab);

                    if (batonpassGUIInst == null)
                    {
                        Debug.LogError("Prefab instantiation failed.");
                        return;
                    }

                    messageboxOutline = batonpassGUIInst.transform.Find("Container/MessageBox/MessageOutline").gameObject;
                    messagebox = batonpassGUIInst.transform.Find("Container/MessageBox").gameObject;
                    ProgressBar = batonpassGUIInst.transform.Find("Container/ProgressBar").gameObject;
                    PercentComplete = ProgressBar.GetComponentInChildren<Slider>();
                    bordercolor = messageboxOutline.GetComponentInChildren<Image>();
                    messageOBJ = messagebox.transform.Find("Message").gameObject;
                    TerminalOBJ = messagebox.transform.Find("Terminal").gameObject;
                    message = messageOBJ.GetComponent<TextMeshProUGUI>();
                    terminal = TerminalOBJ.GetComponent<TextMeshProUGUI>();
                    BoxAnimator = batonpassGUIInst.GetComponent<Animator>();
                    BarAnimator = ProgressBar.GetComponent<Animator>();
                    ProgressBar.SetActive(false);
                    TerminalOBJ.SetActive(false);
                    DontDestroyOnLoad(batonpassGUIInst); // Prevent destruction on scene load
                    batonpassGUIInst.SetActive(false); // Initially inactive
                    batonpassGUIInst.transform.localPosition = Vector3.zero;

                    Debug.Log("Baton Pass GUI instantiated and set to DontDestroyOnLoad.");

                    // You can assign any necessary references here
                    
                }
                else
                {
                    Debug.LogError("Failed to load Baton Pass GUI: " + handle.OperationException);
                }
            };
        }
        public void ShowGUI(string startingtext)
        {
            message.text = startingtext;
            bordercolor.color = Color.white;
            batonpassGUIInst.SetActive(true);
            BoxAnimator.Play("BatonPassAppear");
        }

        public void ShowProgressBar()
        {
            ProgressBar.SetActive(true);
            BarAnimator.Play("ProgressBarEnable");
        }
        public void HideProgressBar()
        {
            StartCoroutine(AwaitbeforeBarClosing());

        }
        private IEnumerator AwaitbeforeBarClosing()
        {
            // Wait until the animation finishes

            

            // Disable the GameObject (for example, the main menu canvas)
            BarAnimator.Play("ProgressBarDisable");
            float animationLength = BarAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);
            ProgressBar.SetActive(false);
        }
        public void HideGUI(float time)
        {
            StartCoroutine(AwaitbeforeMainClosing(time));
        }
        private IEnumerator AwaitbeforeMainClosing(float Time)
        {
            // Wait until the animation finishes
            
            yield return new WaitForSecondsRealtime(Time);

            // Disable the GameObject (for example, the main menu canvas)
            BoxAnimator.Play("BatonPassHide");
            float animationLength = BoxAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);
            batonpassGUIInst.SetActive(false);
        }
        public void EnableTerminal(int newmaxlines)
        {
            maxLines = newmaxlines;
            message.text = "";
            messageOBJ.SetActive(false);
            TerminalOBJ.SetActive(true);
        }
        public void DisableTerminal()
        {
            terminal.text = "";
            messageOBJ.SetActive(true);
            TerminalOBJ.SetActive(false);
        }

        public void BatonPassAnnoucement(Color newbordercolor, string newmessage)
        {

            bordercolor.color = newbordercolor;
            message.text = newmessage;
        }
        public void AddTermLine(string newLine)
        {
            // Append the new line to the existing text
            currentTermText += newLine + "\n";

            // Ensure the text box doesn't exceed the max number of lines
            string[] lines = currentTermText.Split('\n');
            if (lines.Length > maxLines)
            {
                // Remove the first line (oldest) by taking the last (maxLines) lines
                currentTermText = string.Join("\n", lines, lines.Length - maxLines, maxLines);
            }
            terminal.text = currentTermText;
            // Update the terminal display
            
        }
        public void updatebar(float percent)
        {
            PercentComplete.value = percent;
        }

    }
}

