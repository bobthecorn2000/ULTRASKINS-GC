using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UltraSkins.Configuration;

namespace UltraSkins.UI.Widgets
{
    public class GotoSettingsPage : MonoBehaviour
    {
        [SerializeField] public int targetPageIndex;
        [SerializeField] public GameObject rootpanel;
        [SerializeField] public bool UseStaticPage;

    private int previousPageIndex = -1;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        var manager = SettingsManager.Instance;
            if (rootpanel != null)
            {
            rootpanel.SetActive(false);
        }
        
        previousPageIndex = manager.GetCurrentPageIndex();

            // Hide current page
            if (previousPageIndex != -1)
                manager.HidePage(previousPageIndex);
            else
            {
                manager.ShowPagesContainer();
            }
            // Show target page
            if (!UseStaticPage)
            {
                manager.ShowPage(targetPageIndex);
            }
            else
            {
                manager.ShowPagePause(targetPageIndex);
            }
        

        // Get target page back button
            Button backButton = manager.GetBackButton(targetPageIndex);
        if (backButton == null)
        {
            Debug.LogWarning("Back button not assigned for target page");
            return;
        }

        // Clear previous listeners and assign new one
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            // Hide target page
            manager.HidePage(targetPageIndex);
                        if (rootpanel != null)
            {
            rootpanel.SetActive(true);
        }
            // Show previous page if valid
            if (previousPageIndex != -1)
            {
                if (!UseStaticPage)
                {
                    manager.ShowPage(previousPageIndex);
                }
                else
                {
                    manager.ShowPagePause(previousPageIndex);
                }
                
            }
            else
            {
                manager.HidePagesContainer();
            }
        });
        }

    }
}
