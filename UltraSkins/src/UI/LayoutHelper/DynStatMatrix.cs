using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace UltraSkins.UI.Layout
{
    public class DynStatMatrix : MonoBehaviour
    {
        

        public List<GameObject> DynamicObjects;
        public List<GameObject> StaticObjects;

        public List<Behaviour> DynamicComp;
        public List<Behaviour> StaticComp;


        public void ShowStaticAssets()
        {
            foreach (GameObject obj in DynamicObjects)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in StaticObjects)
            {
                obj.SetActive(true);
            }
        }
        public void ShowDynamicAssets()
        {
            foreach (GameObject obj in StaticObjects)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in DynamicObjects)
            {
                obj.SetActive(true);
            }
        }
        public void HideAllAssets()
        {
            foreach (GameObject obj in DynamicObjects)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in StaticObjects)
            {
                obj.SetActive(false);
            }
        }        

    }
}
