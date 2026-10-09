using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UltraSkins.UI.Camp;

namespace UltraSkins.UI.DAD
{
    public class DropZoneOrdered : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // Start is called before the first frame update
        public GameObject Bar;
        public Image Border;

        public GameObject DraggedItem;
        public bool DoBarPostitioning;
        RectTransform content;
        RectTransform zoneRect;
        Coroutine barRoutine;
        RectTransform barRect;
        public ViewFinderSearch searchbox;

        public GameObject NoSkinsSetModel;


        public void OnDrop(PointerEventData PE)
        {
            DraggedItem = null;
            DoBarPostitioning = false;
            Border.color = Color.white;
            if (barRoutine != null)
            {
                StopCoroutine(barRoutine);
                barRoutine = null;
            }
            Bar.SetActive(false);

        }

        public void getFinalPosition(Transform Tile)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                zoneRect,
                Input.mousePosition,
                null,
                out localMousePos
            );

            if (!zoneRect.rect.Contains(localMousePos))
                return;



            // 3. If mouse NOT inside content area add to bottom not including system object
            Vector2 localContentMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                content,
                Input.mousePosition,
                null,
                out localContentMousePos
            );


            if (!content.rect.Contains(localContentMousePos))
            {

                Tile.SetParent(content);
                Tile.SetSiblingIndex(content.childCount - 2);
                Debug.Log("Set to list end");
                //lastplaceholderposition = content.childCount;

                return;
            }


            float mouseYWorld = Input.mousePosition.y;
            int newIndex = content.childCount;

            for (int i = 0; i < content.childCount; i++)
            {
                var child = content.GetChild(i) as RectTransform;
                if (child == Tile) continue;


                Vector3 worldPos = child.TransformPoint(child.rect.center);
                float childMidY = worldPos.y;

                if (mouseYWorld > childMidY)
                {
                    //Debug.Log(i + " " + childMidY + " " + mouseYWorld);

                    newIndex = i;
                    break;
                }
            }
            Tile.SetParent(content);
            Tile.SetSiblingIndex(newIndex);
            Debug.Log("Set index to " + newIndex);
            searchbox.RefreshItems();
        }






        public void OnPointerEnter(PointerEventData PE)
        {
            var drag = PE.pointerDrag;
            if (!drag) return;
            Bar.SetActive(true);
            DraggedItem = drag;
            drag.GetComponent<DraggableItem>()?.SetHoverZone(this.gameObject);
            Border.color = Color.blue;
            DoBarPostitioning = true;
            if (barRoutine == null)
                barRoutine = StartCoroutine(MouseToBarPostition());
        }

        public void OnPointerExit(PointerEventData PE)
        {
            var drag = PE.pointerDrag;
            if (!drag) return;
            DraggedItem = null;
            drag.GetComponent<DraggableItem>()?.ClearHoverZone(gameObject);
            Border.color = Color.white;
            Bar.SetActive(false);
            DoBarPostitioning = false;
            if (barRoutine != null)
            {
                StopCoroutine(barRoutine);
                barRoutine = null;
            }
        }
        // Update is called once per frame
        void Awake()
        {
            zoneRect = transform as RectTransform;
            content = (RectTransform)transform.GetChild(0);
            barRect = Bar.GetComponent<RectTransform>();
        }
        IEnumerator MouseToBarPostition()
        {
            //Debug.Log("OH MY GOD THE COROUTINE IS RUNNING");

            while (DoBarPostitioning)
            {

                //Debug.Log("OH MY GOD ITS LOOPING");
                // 1. Check if mouse is inside this drop zone
                Vector2 localMousePos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    zoneRect,
                    Input.mousePosition,
                    null,
                    out localMousePos
                );

                if (!zoneRect.rect.Contains(localMousePos))
                    yield return null;




                // If mouse NOT inside content area add to bottom
                Vector2 localContentMousePos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    content,
                    Input.mousePosition,
                    null,
                    out localContentMousePos
                );


                if (!content.rect.Contains(localContentMousePos) && content.childCount > 0)
                {
                    //this handles list end
/*                     var child = content.GetChild(content.childCount - 1) as RectTransform;
                    
                    Vector3 worldPos = child.TransformPoint(child.rect.min);
                    float childMidY = worldPos.y;
                    float childMidX = worldPos.x; */
                    //Bar.transform.position = content.TransformPoint(new Vector2(content.rect.center.x, content.rect.yMin));
/*                     var child2 = content.GetChild(content.childCount - 1) as RectTransform;
                    Vector3 worldPos2 = child2.TransformPoint(child2.rect.center);
                    float childmidy2 = worldPos2.y;
                    float newvalue = childmidy2 + childMidY; */
                    Debug.Log("bottom code");
                    barRect.position = content.TransformPoint(new Vector2(content.rect.center.x, content.rect.yMin));
                    yield return null;
                }


                float mouseYWorld = Input.mousePosition.y;


                for (int i = 0; i < content.childCount; i++)
                {
                    var child = content.GetChild(i) as RectTransform;
                    if (child == Bar) continue;


                    Vector3 worldPos = child.TransformPoint(child.rect.center);
                    float childMidY = worldPos.y;
                    float childMidX = worldPos.x;

                    if (mouseYWorld > childMidY)
                    {
                        if (child.GetSiblingIndex() != 0)
                        {
                            //This handles placing in between top and bottom
                            var child2 = content.GetChild(child.GetSiblingIndex() - 1) as RectTransform;
                            Vector3 worldPos2 = child2.TransformPoint(child2.rect.center);
                            float childmidy2 = worldPos2.y;
                            float newvalue = childmidy2 + childMidY;
                            barRect.position = new Vector2(childMidX, newvalue / 2);
                            Debug.Log("between code");
                        }
                        else
                        {
                            //this handles placing at the top
                            Debug.Log("top code");
                            barRect.position = content.TransformPoint(new Vector2(content.rect.center.x, content.rect.yMax));
                        }


                        // Match width
                        barRect.SetSizeWithCurrentAnchors(
                            RectTransform.Axis.Horizontal,
                            child.rect.width
                        );

                        break;
                    }
                }

                if (content.childCount == 0)
                {
                    Debug.Log("failsafe code");
                    barRect.position = content.TransformPoint(new Vector2(content.rect.center.x, content.rect.yMax));
                }


                yield return null;

            }
        }





    }
}
