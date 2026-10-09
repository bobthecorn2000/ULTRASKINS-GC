using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace UltraSkins.UI.Creator
{
    public class CreatorStateHand : MonoBehaviour
    {
        // Start is called before the first frame update

        public GameObject PlusIcon;
        public List<GameObject> TextToEnableTemp;
        public List<GameObject> TextToEnableScratch;
        public Animator animator;
        public GameObject blocker;
        public string CreationName;
        public string CreationDescription;

        public string CreationDepend;

        void Start()
        {
        
        }
        public void PlayAnimationFromStart(string animationName)
        {
            animator.Play(animationName, -1, 0f);
            
        }


        public void UpdateName(string newtext)
        {
            CreationName = newtext;
            
        }

        public void UpdateDesc(string newtext)
        {
            CreationDescription = newtext;
            
        }
        
    }
}
