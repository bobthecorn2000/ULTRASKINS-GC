using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UltraSkins.UI.Camp;
namespace UltraSkins.UI.Widgets
{
    public class PointerHoverTracker : MonoBehaviour , IPointerEnterHandler
    {

        public ButtonEnableManager targetObject;
        // Start is called before the first frame update
        public void OnPointerEnter(PointerEventData eventData)
        {
            targetObject.callRefresh();
        }

        // Update is called once per frame
    }
}
