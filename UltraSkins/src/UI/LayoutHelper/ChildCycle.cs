using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UltraSkins.UI.Widgets;

namespace UltraSkins.UI.Layout
{
    public class ChildCycler : MonoBehaviour
    {
        public float cycleDuration = 2f; // Time each child stays active
        public bool isPaused = false;    // Can be toggled externally for manual control
        public enum PWeaponModel
        {
            Revolver,
            Slab,
            Shotgun,
            Hammer,
            Nailgun,
            Sawblade,
            Railcannon,
            RocketLauncher,
            MainArm,
            Feedbacker,
            Knuckleblaster,
            Whiplash

        }

        private float timer = 0f;
        private Dictionary<PWeaponModel, PWepTag> pWepMap = new();
        private PWeaponModel currentPwep;

        public UIColorSlider UICS;


        void Start()
        {
            pWepMap.Clear();
            foreach (Transform child in transform)
            {
                var tag = child.GetComponent<PWepTag>();
                if (tag != null && !pWepMap.ContainsKey(tag.pWeaponModel))
                {
                    pWepMap[tag.pWeaponModel] = tag;
                    child.gameObject.SetActive(false); // initially hide all
                }
            }


            ShowChildAt(currentPwep);
        }

        void Update()
        {
            if (transform.childCount == 0 || pWepMap.Count == 0) return;

            if (!isPaused)
            {
                timer += Time.deltaTime;
                if (timer >= cycleDuration)
                {
                    timer = 0f;
                    currentPwep = GetNextPwep(currentPwep);
                    ShowChildAt(currentPwep);
                }
            }
        }

        /// <summary>
        /// Shows the child at the given index and resets the timer.
        /// </summary>
        public void ShowChildAt(PWeaponModel pWeaponModel)
        {
            foreach (var kvp in pWepMap)
            {
                kvp.Value.gameObject.SetActive(kvp.Key == pWeaponModel);
            }
            if (pWepMap.TryGetValue(pWeaponModel, out var pWeapon))
            {
                UICS.SupportID(pWeapon.SupportID);
                if (pWeapon.SupportID == false)
                {
                    UICS.modechange(false);
                    UICS.SupportID(false);
                }
                else
                {
                    UICS.SupportID(true);
                }
                //TODO UICS.SupportsEmission(pWeapon.SupportEmission)   
            }
            currentPwep = pWeaponModel;

            timer = 0f;
        }

        PWeaponModel GetNextPwep(PWeaponModel current)
        {
            var values = (PWeaponModel[])System.Enum.GetValues(typeof(PWeaponModel));
            int index = (System.Array.IndexOf(values, current) + 1) % values.Length;
            Debug.Log($"Showing stage {current} next {index}");
            return values[index];
        }
    }
}
