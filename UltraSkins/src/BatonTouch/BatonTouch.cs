using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace UltraSkins.BatonTouchNotifier
{
    public class BatonTouch
    {
        // Start is called before the first frame update
        
        public static event Action<Guid> OnNewNotification;
        public static event Action<Guid> OnDismissNotification;

        private static Dictionary<Guid,BatonTouchNotification> Notifications = new Dictionary<Guid,BatonTouchNotification>();


        public static BatonTouchNotification CreateNewNotification(string title, string message, BTNotifPriority notifPriority = BTNotifPriority.Generic, Color? borderColor = null, bool packCreateDismiss = false)
        {
            BatonTouchNotification btnotif = new BatonTouchNotification(title, message, notifPriority,borderColor);
            Notifications.TryAdd(btnotif.guid, btnotif);
            OnNewNotification?.Invoke(btnotif.guid);
            return btnotif;
        }

        public static void DismissNotificationByGUID(Guid guid)
        {
            Notifications.Remove(guid);
            OnDismissNotification?.Invoke(guid);

        }

        public static BatonTouchNotification[] GetAllCurrentNotifications()
        {
            return Notifications.Values.ToArray();
        }

        public static BatonTouchNotification GetNotificationObjectByGUID(Guid guid)
        {
            bool exists = Notifications.TryGetValue(guid, out BatonTouchNotification BTN);
            if (exists)
            {
                return BTN;
            }
            else
            {
                return null;
            }
             
        }

        
    }
    public class BatonTouchNotification
    {
        public BTFlags NotifFlags {get; private set;}
        public string Title {get; private set;} = "Untitled Notification";
        public string Message {get; private set;} = "This is a notification. I am normally filled with cool text";

        public Color BorderColor {get; private set;} = Color.white;

        public Guid guid {get;} = new Guid();

        public BTNotifPriority Priority {get; private set;} = BTNotifPriority.Generic;


        public BatonTouchNotification()
        {
            // a basic notification with no extra stuff
        }


        public BatonTouchNotification(string title, string message, BTNotifPriority notifPriority = BTNotifPriority.Generic, Color? borderColor = null, BTFlags flags = new BTFlags())
        {
            Title = title;
            Message = message;
            Priority = notifPriority;
            BorderColor = borderColor ?? GetColorByPriority(notifPriority);
            //DismissOnPackCreation = flags;
            
        }
        //yes i know the break at the end isnt needed. im to lazy to remove it so i wrote this comment that took way longer to write instead of just removing them;
        private Color GetColorByPriority(BTNotifPriority notifPriority)
        {
            switch (notifPriority)
            {
                case BTNotifPriority.Warning:
                    return Color.yellow;
                    break;
                case BTNotifPriority.Error:
                    return Color.red;
                    break;
                case BTNotifPriority.Generic:
                    return Color.white;
                    break;
                case BTNotifPriority.Info:
                    return Color.cyan;
                    break;
                default: 
                return Color.white;
            }
        }

        
    }

    public class BatonTouchBanner
    {

        public Color BannerTextColor {get; private set;} = Color.black;
        public Color BannerBackgroundColor {get; private set;} = Color.white;
    }


    public enum BTNotifPriority
    {
        Generic,
        Info,
        Warning,
        Error,
    }
    [Flags]
    public enum BTFlags
    {
        None = 0,
        AutoDismissOnPageRefresh = 1,
        AutoDismissOnApply = 2,
        AutoGenerateBanner = 4,
        DisplayToast = 8,
        NoUserDismiss = 16
    }
}
