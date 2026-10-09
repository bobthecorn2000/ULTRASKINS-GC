using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UltraSkins.UI.Animation
{
    public class AnimationEventToUltraskinsEvent : MonoBehaviour
    {
        // Start is called before the first frame update
    // Make it public so it appears in the Inspector
    public UnityEvent OnAnimationTriggered;

    // This public method will be called by the Animation Event
    public void TriggerUnityEvent()
    {
        OnAnimationTriggered.Invoke();
    }
    }
}
