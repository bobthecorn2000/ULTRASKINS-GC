using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltraSkins.BatonTouchNotifier;
using UnityEngine;
using UnityEngine.UI;

namespace UltraSkins.BatonTouchNotifier
{
    public class BTNotifButton : MonoBehaviour
    {
        private BatonTouchNotification btNotifInfo;
        [SerializeField]
        private TextMeshProUGUI TitleButtonText;
        [SerializeField]
        private Image Border;

        public Guid notificationGUID {get; private set;}


        public void MakeButton()
        {
            if (btNotifInfo == null)
            {
                return;
            }
            TitleButtonText.text = btNotifInfo.Title;
            Border.color = btNotifInfo.BorderColor;
            notificationGUID = btNotifInfo.guid;


        }

        public void SetNotifInfo(BatonTouchNotification btn)
        {
            btNotifInfo = btn;
        }
    }
}
