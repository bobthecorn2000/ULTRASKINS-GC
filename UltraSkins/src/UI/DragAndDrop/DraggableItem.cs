
using UltraSkins.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UltraSkins.UI.DAD
{


public class DraggableItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    public GameObject hoverZone;
    private RectTransform rectTransform;
    Vector2 grabOffset;

    int originalIndex;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {



        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();

        //placeholder for showing new position


        // DRAG MODE
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false; // needed for drop detection
    }

    public void OnDrag(PointerEventData eventData)
    {



        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint
        );

        // This moves the rect by its CENTER (pivot-aware)
        rectTransform.localPosition = localPoint;
        // Ask active drop zones to update the placeholder position

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (hoverZone != null)
        {
            DropZoneOrdered DZO = hoverZone.GetComponent<DropZoneOrdered>();
            if (DZO != null)
            {
                DZO.getFinalPosition(transform);
                return;
            }
            DropZonePreset DZP = hoverZone.GetComponent<DropZonePreset>();
            if (DZP != null)
            {
                DZP.GetFinalPostition(transform);
                return;
            }
            PanicReturn();
        }
        else
        {
            PanicReturn();
        }
        
        // if no drop zone handled it → return it to original list
        //if (placeholder.transform.parent != originalParent)
        // originalParent = placeholder.transform.parent;

        //transform.SetParent(originalParent);
        //transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());


    }
    private void PanicReturn()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalIndex);
    }
    public void SetHoverZone(GameObject zone)
    {
        hoverZone = zone;
        //zone.Preview(transform);
    }
    public void ClearHoverZone(GameObject zone)
    {
        hoverZone = null;
        //zone.Preview(transform);
    }


}
}