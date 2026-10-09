using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UltraSkins.UI.Layout
{
    public class OrderManager : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;

        [SerializeField] private GameObject orderbox;
        public bool disabled = false;
        private Transform lastParent;
        private int lastSiblingIndex;
        public void Start()
        {
            if (disabled)
            {
                moveUpButton.interactable = false;
                moveDownButton.interactable = false;
            }
            if (enabled)
            {
                UpdateButtons();
            }

        }
        public void MoveUp()
        {
            int index = transform.GetSiblingIndex();
            if (index > 0)
            {
                lastParent = transform.parent;
                lastSiblingIndex = transform.GetSiblingIndex();
                transform.SetSiblingIndex(index - 1);
                UpdateButtons();
            }


        }

        public void MoveDown()
        {
            int index = transform.GetSiblingIndex();
            int siblingCount = transform.parent.childCount;
            if (index < siblingCount - 2)
            {
                transform.SetSiblingIndex(index + 1);
                UpdateButtons();
            }
        }

        private void UpdateButtons()
        {
            int index = transform.GetSiblingIndex();
            int siblingCount = transform.parent.childCount;

            if (moveUpButton != null)
                moveUpButton.interactable = index > 0;

            if (moveDownButton != null)
                moveDownButton.interactable = index < siblingCount - 2;
        }

        public void ToggleHideShow(bool state)
        {
            orderbox.SetActive(state);
        }


        void Update()
        {
            if (transform.parent != lastParent || transform.GetSiblingIndex() != lastSiblingIndex)
            {
               
                    
                    lastParent = transform.parent;
                    lastSiblingIndex = transform.GetSiblingIndex();
                    UpdateButtons();
                
            }
        }
    
    }
}
