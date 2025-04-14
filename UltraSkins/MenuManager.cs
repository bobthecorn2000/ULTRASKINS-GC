using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UltraSkins;
using BatonPassLogger;


namespace UltraSkins.UI
{
    internal class MenuManager : MonoBehaviour
    {
        
        ULTRASKINHand handInstance = ULTRASKINHand.HandInstance;
        internal static MenuManager  MMinstance { get; private set; }
        List<GameObject> loadedButtons = new List<GameObject>();
        Dictionary<string, Button> AvailbleSkins = new Dictionary<string, Button>();

        void Awake() {
            MMinstance = this;

        }
        public void Closeskineditor(GameObject mainmenucanvas, GameObject Configmenu, GameObject fallnoiseoff, Animator animator,ObjectActivateInSequence oais,GameObject content)
        {
            MMinstance.orderifier(oais, content);
            fallnoiseoff.SetActive(true);

            animator.Play("menuclose");

            // Start the coroutine
            MMinstance.StartCoroutine(MMinstance.DisableAfterCloseAnimation(mainmenucanvas, Configmenu, animator));

        }

        private IEnumerator DisableAfterCloseAnimation(GameObject mainmenucanvas, GameObject Configmenu, Animator animator)
        {
            // Wait until the animation finishes
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);

            // Disable the GameObject (for example, the main menu canvas)
            Configmenu.SetActive(false);
            mainmenucanvas.SetActive(true);
        }

        public void GenerateButtons(string skinfolderdir, GameObject contentfolder, ObjectActivateInSequence activateanimator)
        {
            string[] subfolders = Directory.GetDirectories(skinfolderdir);
            
            AvailbleSkins.Clear();
            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskinsButton.prefab").Completed += buttonHandle =>
            {
                if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                {

                    int buttonsToLoad = subfolders.Length;
                    int buttonsLoaded = 0;
                   
                    GameObject prefab = buttonHandle.Result;
                    foreach (string subfolder in subfolders)
                    {
                        string folder = Path.GetFileName(subfolder);
                        GameObject instance = Instantiate(prefab, contentfolder.transform);
                        instance.SetActive(true);

                        Button ultraskinsbutton = instance.GetComponentInChildren<Button>();
                        ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                        
                        if (handInstance.filepathArray.Contains(subfolder))
                        {
                            int index = System.Array.IndexOf(handInstance.filepathArray, subfolder);
                            BatonPass.Debug(instance.name +"/"+ subfolder + "/" + index);
                            instance.GetComponent<ButtonEnableManager>().IsEnabled = true;
                            instance.transform.SetSiblingIndex(index);
                        }
                        instance.GetComponent<ButtonEnableManager>().filePath = folder;
                        AvailbleSkins.Add(folder, ultraskinsbutton);
                         // Add button to list

                        BatonPass.Debug("Successfully loaded and instantiated ultraskinsButton.");

                        buttonsLoaded++;
                        if (buttonsLoaded == buttonsToLoad)
                        {
                            orderifier(activateanimator, contentfolder);
                        }
                    }

                }
                else
                {
                    BatonPass.Error("Failed to load ultraskinsButton: " + buttonHandle.OperationException.Message);
                    BatonPass.Error("Ultraskins will still work, but skin changes via the menu will be disabled. CODE -\"MMAN-GENERATEBUTTONS-MAINMENU-ASSET_BUNDLE_FAILED\"");
                }

                // Check if all buttons are loaded

            };
        }

        void orderifier(ObjectActivateInSequence activateanimator,GameObject contentfolder)
        {
            loadedButtons.Clear();
            foreach (Transform child in contentfolder.transform)
            {
                loadedButtons.Add(child.gameObject);
            }

            BatonPass.Debug("All buttons loaded, setting up activation sequence.");

            activateanimator.objectsToActivate = loadedButtons.ToArray();
            activateanimator.delay = 0.05f;
            BatonPass.Debug("Successfully set up ObjectActivateInSequence.");
        }

        public void GenerateButtons(string skinfolderdir, GameObject contentfolder)
        {
            string[] subfolders = Directory.GetDirectories(skinfolderdir);
            
            AvailbleSkins.Clear();
            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskinsButton.prefab").Completed += buttonHandle =>
            {
                if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                {

                    int buttonsToLoad = subfolders.Length;
                    int buttonsLoaded = 0;
                    foreach (var button in loadedButtons)
                    {
                        Destroy(button); // Destroy old button GameObjects
                    }
                    loadedButtons.Clear();
                    GameObject prefab = buttonHandle.Result;
                    foreach (string subfolder in subfolders)
                    {
                        string folder = Path.GetFileName(subfolder);
                        GameObject instance = Instantiate(prefab, contentfolder.transform);
                        instance.SetActive(true);

                        Button ultraskinsbutton = instance.GetComponentInChildren<Button>();
                        ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                        if (handInstance.filepathArray.Contains(subfolder))
                        {
                            int index = System.Array.IndexOf(handInstance.filepathArray, subfolder);
                            instance.GetComponent<ButtonEnableManager>().IsEnabled = true;
                            instance.transform.SetSiblingIndex(index);
                        }
                        instance.GetComponent<ButtonEnableManager>().filePath = folder;
                        string[] folders = new string[] { folder };


                        loadedButtons.Add(instance); // Add button to list
                        AvailbleSkins.Add(folder, ultraskinsbutton);
                        BatonPass.Debug("Successfully loaded and instantiated ultraskinsButton.");

                        buttonsLoaded++;

                    }

                }
                else
                {
                    BatonPass.Error("Failed to load ultraskinsButton: " + buttonHandle.OperationException.Message);
                    BatonPass.Error("Ultraskins will still work, but skin changes via the menu will be disabled. CODE -\"MMAN-GENERATEBUTTONS-PAUSEMENU-ASSET_BUNDLE_FAILED\"");
                }

                // Check if all buttons are loaded

            };
        }

        public void applyskins(GameObject content)
        {
            
            List<string> validButtons = new List<string>();
            foreach (Transform childTransform in content.transform) { 
            BatonPass.Debug(childTransform.name);
            }
            BatonPass.Debug("starting apply process");
            foreach (Transform child in content.transform)
            {
                GameObject USbutton = child.gameObject;
                BatonPass.Debug("working on " + USbutton.name);
                ButtonEnableManager bem = USbutton.GetComponent<ButtonEnableManager>();
                if (bem.IsEnabled)
                {
                    validButtons.Add(bem.filePath);
                }
            }
            string[] paths = validButtons.ToArray();
            handInstance.refreshskins(paths);


        }


        public void DisableButtons()
        {
            foreach (var button in AvailbleSkins.Values)
            {
              button.interactable = false;
            }
        }
        public void EnableButtons()
        {
            foreach (var button in AvailbleSkins.Values)
            {
                button.interactable = true;
            }
        }
    }
}

