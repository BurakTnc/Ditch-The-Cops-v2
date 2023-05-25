
using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;using Unity.Notifications.iOS;
using UnityEngine;
using UnityEngine.Android;
using Random = UnityEngine.Random;

public class Notify : MonoBehaviour
{

    private string notificationId = "test_notification";
    public List<string> notificationStrings;
    private int id;

    private void OnApplicationQuit()
    {
        id = Random.Range(0, notificationStrings.Count);

        iOSNotificationTimeIntervalTrigger timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(20, 0, 0),
            Repeats = false
        };

        iOSNotificationCalendarTrigger calenderNotification = new iOSNotificationCalendarTrigger()
        {
            Hour = 15,
            Minute = 45,
            Repeats = true
        };

        iOSNotification notification = new iOSNotification()
        {
            Identifier = "test_notification",
            Title = notificationStrings[id],
            Subtitle = "",
            Body = "Play Cop Car Rush 3D!!",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotification notification1 = new iOSNotification()
        {
            Identifier = "test_notification1",
            Title = notificationStrings[id],
            Subtitle = "",
            Body = "Play Cop Car Rush 3D",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = calenderNotification,
        };


        iOSNotificationCenter.ScheduleNotification(notification);
        iOSNotificationCenter.OnRemoteNotificationReceived += recievedNotification => { Debug.Log("Recieved notification " + notification.Identifier + " !"); };


        iOSNotificationCenter.ScheduleNotification(notification1);
        iOSNotificationCenter.OnRemoteNotificationReceived += recievedNotification => { Debug.Log("Recieved notification " + notification1.Identifier + " !"); };


        iOSNotification notificationIntentData = iOSNotificationCenter.GetLastRespondedNotification();
        if (notificationIntentData != null)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"-App open from notification");

        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("PAUSE");
        // iOSNotificationCenter.RemoveScheduledNotification(notificationId);
        // iOSNotificationCenter.RemoveDeliveredNotification(notificationId);
    }

}
