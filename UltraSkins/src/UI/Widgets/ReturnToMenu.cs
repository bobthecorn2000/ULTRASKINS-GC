using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UltraSkins.UI.Widgets
{
    public class ReturnToMenu : MonoBehaviour
    {
        public GameObject pageRoot;           // This return handler belongs to this page
        public Button backButton;             // The page’s back button
        private GameObject returnTarget;      // Where to go when back is pressed

        void Awake()
        {
            if (backButton != null)
                backButton.onClick.AddListener(Return);
        }

        public void SetReturnTarget(GameObject target)
        {
            returnTarget = target;
        }

        public void Return()
        {
            if (pageRoot != null)
                pageRoot.SetActive(false);

            if (returnTarget != null)
                returnTarget.SetActive(true);
        }
    }
}
