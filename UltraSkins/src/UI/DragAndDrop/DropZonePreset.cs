using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UltraSkins.UI.Camp;
namespace UltraSkins.UI.DAD
{
    public class DropZonePreset : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // Start is called before the first frame update
        public Image Border;
        public ViewFinderSearch searchbox;
        public GameObject PlusIcon;

        public GameObject AllEquipedModel;
        RectTransform content;
        RectTransform zoneRect;
        public void OnDrop(PointerEventData PE)
        {
            PlusIcon.SetActive(false);
            Border.color = Color.white;
        }
        void Awake()
        {
            zoneRect = transform as RectTransform;
            content = (RectTransform)transform.GetChild(0);
        }
        public void OnPointerEnter(PointerEventData PE)
        {
        var drag = PE.pointerDrag;
        if (!drag) return;
         PlusIcon.SetActive(true);
         
         AllEquipedModel.SetActive(false);
         Border.color = Color.blue;
         drag.GetComponent<DraggableItem>()?.SetHoverZone(this.gameObject);
        } 
        public void OnPointerExit(PointerEventData PE)
        {
            
            var drag = PE.pointerDrag;
            if (!drag) return;
            PlusIcon.SetActive(false);
            if (content.childCount == 0)
            {
                AllEquipedModel.SetActive(true);
            }
            drag.GetComponent<DraggableItem>()?.ClearHoverZone(this.gameObject);
            Border.color = Color.white;
        }
        // Update is called once per frame
        void Update()
        {
        
        }
        public void GetFinalPostition(Transform drop)
        {
            drop.SetParent(content);
            searchbox.RefreshItems();
        }
    }
}
