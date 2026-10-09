using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace UltraSkins.UI.Widgets
{


    public class ReturnTo : MonoBehaviour
    {
        public Animator animator; // Reference to the animator
        public GameObject mainmenucanvas = null;
        public GameObject Configmenu;
        public GameObject fallnoiseoff = null;

        // This is a non-static method, so you can call it from your Harmony patch.
        public void Closeskineditor()
        {
            fallnoiseoff.SetActive(true);
            animator.Play("closemenu");


            // Start the coroutine to disable the object after the animation
            StartCoroutine(DisableAfterAnimation());
        }

        private IEnumerator DisableAfterAnimation()
        {
            // Wait until the animation finishes
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);

            // Disable the GameObject (for example, the main menu canvas)
            mainmenucanvas.SetActive(true);
            Configmenu.SetActive(false);
        }
    }

}
