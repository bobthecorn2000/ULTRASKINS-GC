using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


namespace UltraSkins.UI.DAD
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        private static List<DropZone> allZones = new List<DropZone>();
        public int lastplaceholderposition = -1;
        public GameObject placeholderBlock;
        public bool UnorderedDropTarget;
        private void OnEnable() => allZones.Add(this);
        private void OnDisable() => allZones.Remove(this);

        public static void UpdateAllPlaceholders(Transform placeholder)
        {
            foreach (var zone in allZones)
                zone.UpdatePlaceholder(placeholder);
        }


        public static void QueryFinalLocation(Transform Tile)
        {
            foreach (var zone in allZones)
                zone.getfinallocation(Tile);
        }
        public void getfinallocation(Transform Tile)
        {
            RectTransform zoneRect = transform as RectTransform;
            RectTransform content = (RectTransform)transform.GetChild(0);

            // 1. Check if mouse is inside this drop zone
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                zoneRect,
                Input.mousePosition,
                null,
                out localMousePos
            );

            if (!zoneRect.rect.Contains(localMousePos))
                return;


            // 2. Ensure placeholder parent is correct
            //if (placeholder.parent != content)
            //placeholder.SetParent(content);


            // 3. If mouse NOT inside content area → add to bottom
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
                Tile.SetSiblingIndex(content.childCount);
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
        }


        public void UpdatePlaceholder(Transform placeholder)
        {
            RectTransform zoneRect = transform as RectTransform;
            RectTransform content = (RectTransform)transform.GetChild(0);

            // 1. Check if mouse is inside this drop zone
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                zoneRect,
                Input.mousePosition,
                null,
                out localMousePos
            );

            if (!zoneRect.rect.Contains(localMousePos))
                return;


            // 2. Ensure placeholder parent is correct
            //if (placeholder.parent != content)
            //placeholder.SetParent(content);


            // 3. If mouse NOT inside content area → add to bottom
            Vector2 localContentMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                content,
                Input.mousePosition,
                null,
                out localContentMousePos
            );


            if (!content.rect.Contains(localContentMousePos))
            {
                placeholderBlock.transform.position = content.TransformPoint(new Vector2(content.rect.center.x, content.rect.yMin));
                if (placeholder.GetSiblingIndex() != lastplaceholderposition)
                {
                    //placeholder.SetSiblingIndex(content.childCount);
                    Debug.Log("Placeholder Changed Alt");
                    //lastplaceholderposition = content.childCount;
                }
                return;
            }


            float mouseYWorld = Input.mousePosition.y;
            int newIndex = content.childCount;

            for (int i = 0; i < content.childCount; i++)
            {
                var child = content.GetChild(i) as RectTransform;
                if (child == placeholder) continue;


                Vector3 worldPos = child.TransformPoint(child.rect.center);
                float childMidY = worldPos.y + 25;
                float childMidX = worldPos.x;
                if (mouseYWorld > childMidY)
                {
                    //Debug.Log(i + " " + childMidY + " " + mouseYWorld);
                    placeholderBlock.transform.position = new Vector2(childMidX, childMidY);
                    //newIndex = i;
                    break;
                }
            }

        }



        public void OnDrop(PointerEventData eventData)
        {
            lastplaceholderposition = -1;
            // Nothing needed here. The draggable item handles final logic.
        }
    }
}