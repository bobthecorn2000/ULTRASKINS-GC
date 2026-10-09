using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UltraSkins.UI;
using UnityEngine;

namespace UltraSkins.BatonTouchNotifier
{
    public class BTNotifBus : MonoBehaviour
    {

        public GameObject notifButtonPrefab;
        public GameObject NotificationStack;
        public Dictionary<Guid,BTNotifButton> TrackedNotifications = new Dictionary<Guid, BTNotifButton>();

        void Awake()
        {
            SyncAllNotifications();
            //if a notification is made off main thread in between when the function above and these subscribing
            //it will probably get missed, it shouldnt happen but probably dont make notifications off main thread
            BatonTouch.OnNewNotification += MakeNewNotification;
            BatonTouch.OnDismissNotification += PopNotification;
        }
        private void SyncAllNotifications()
        {
            BatonTouchNotification[] BTNS = BatonTouch.GetAllCurrentNotifications();
            
            foreach (BatonTouchNotification BTN in BTNS )
            {
                if (TrackedNotifications.ContainsKey(BTN.guid))
                {
                    continue;
                }
                makeSingleButton(BTN);

            }
        }

        private void MakeNewNotification(Guid guid)
        {
            if (TrackedNotifications.ContainsKey(guid))
            {
                return;
            }
            BatonTouchNotification notifInfo = BatonTouch.GetNotificationObjectByGUID(guid);
            makeSingleButton(notifInfo);
        }
        private void PopNotification(Guid guid)
        {
            if (TrackedNotifications.TryGetValue(guid, out BTNotifButton notifButton))
            {
                Destroy(notifButton.gameObject);
                TrackedNotifications.Remove(guid);
            }
        }
        private void makeSingleButton(BatonTouchNotification btn)
        {
            GameObject notif = Instantiate(notifButtonPrefab,NotificationStack.transform);
            //There is intentionally no saftey check here. make sure it has a BTNotifButton at root or it will throw
            BTNotifButton notifObject = notif.GetComponent<BTNotifButton>();
            notifObject.SetNotifInfo(btn);
            notifObject.MakeButton();
            TrackedNotifications.TryAdd(notifObject.notificationGUID,notifObject);
        }

        void OnDestroy()
        {
            BatonTouch.OnNewNotification -= MakeNewNotification;
            BatonTouch.OnDismissNotification -= PopNotification;
        }
    }
}
