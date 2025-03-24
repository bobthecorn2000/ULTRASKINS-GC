﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;

using HarmonyLib;
using NewBlood;
using Unity.Audio;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Reflection;
using static UltraSkins.ULTRASKINHand;

using BepInEx.Logging;
using static UltraSkins.SkinEventHandler;

using System.Net.NetworkInformation;
using HarmonyLib.Tools;
using static MonoMod.RuntimeDetour.Platforms.DetourNativeMonoPosixPlatform;
using static UnityEngine.ParticleSystem.PlaybackState;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;
using System.Collections;

namespace UltraSkins
{
    internal class MenuManager : MonoBehaviour
    {
        ULTRASKINHand handInstance = ULTRASKINHand.HandInstance;
        internal static MenuManager  MMinstance { get; private set; }
        
        void Awake() {
            MMinstance = this;
        }
        public void Closeskineditor(GameObject mainmenucanvas, GameObject Configmenu, GameObject fallnoiseoff, Animator animator)
        {
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
            string folder = null;
            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskinsButton.prefab").Completed += buttonHandle =>
            {
                if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                {

                    int buttonsToLoad = subfolders.Length;
                    int buttonsLoaded = 0;
                    List<GameObject> loadedButtons = new List<GameObject>();
                    GameObject prefab = buttonHandle.Result;
                    foreach (string subfolder in subfolders)
                    {
                        folder = Path.GetFileName(subfolder);
                        GameObject instance = Instantiate(prefab, contentfolder.transform);
                        instance.SetActive(true);

                        Button ultraskinsbutton = instance.GetComponentInChildren<Button>();
                        ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                        ultraskinsbutton.onClick.AddListener(() => handInstance.refreshskins(folder));

                        loadedButtons.Add(instance); // Add button to list

                        BatonPass("Successfully loaded and instantiated ultraskinsButton.");

                        buttonsLoaded++;
                        if (buttonsLoaded == buttonsToLoad)
                        {
                            BatonPass("All buttons loaded, setting up activation sequence.");

                            activateanimator.objectsToActivate = loadedButtons.ToArray();
                            activateanimator.delay = 0.05f;
                            BatonPass("Successfully set up ObjectActivateInSequence.");
                        }
                    }

                }
                else
                {
                    BatonPass("Failed to load ultraskinsButton: " + buttonHandle.OperationException);
                }

                // Check if all buttons are loaded

            };
        }
        public void GenerateButtons(string skinfolderdir, GameObject contentfolder)
        {
            string[] subfolders = Directory.GetDirectories(skinfolderdir);
            string folder = null;
            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskinsButton.prefab").Completed += buttonHandle =>
            {
                if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                {

                    int buttonsToLoad = subfolders.Length;
                    int buttonsLoaded = 0;
                    List<GameObject> loadedButtons = new List<GameObject>();
                    GameObject prefab = buttonHandle.Result;
                    foreach (string subfolder in subfolders)
                    {
                        folder = Path.GetFileName(subfolder);
                        GameObject instance = Instantiate(prefab, contentfolder.transform);
                        instance.SetActive(true);

                        Button ultraskinsbutton = instance.GetComponentInChildren<Button>();
                        ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                        ultraskinsbutton.onClick.AddListener(() => handInstance.refreshskins(folder));

                        loadedButtons.Add(instance); // Add button to list

                        BatonPass("Successfully loaded and instantiated ultraskinsButton.");

                        buttonsLoaded++;

                    }

                }
                else
                {
                    BatonPass("Failed to load ultraskinsButton: " + buttonHandle.OperationException);
                }

                // Check if all buttons are loaded

            };
        }
    }
}

