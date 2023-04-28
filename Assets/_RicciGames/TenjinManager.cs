using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class TenjinManager : MonoBehaviour
{
    /*  public GameObject StartPanel, Button, Image;
      bool isokay;
      float time, TimeAlpha = 1;
      public static int Status;
      string PlayerType;
    */

    void Start()
    {


        TenjinConnect();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            TenjinConnect();
        }
    }
    private void Update()
    {
        /* if (isokay)
         {
             Button.SetActive(true);
             Image.SetActive(true);
             isokay = false;
         }
         if (Status != -105)
         {
             PlayerPrefs.SetInt("FirstEntranceTenjin", 1);
             GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, PlayerType + Status.ToString());
             Status = -105;
         }*/
    }
    public void TenjinConnect()
    {
        BaseTenjin instance = Tenjin.getInstance("TSDUUNZEQPXDHMZ8TYGJBJEPLBV6QJSS");

        // Sends install/open event to Tenjin
        instance.Connect();

#if UNITY_IOS

        // Tenjin wrapper for requestTrackingAuthorization
        /*
        instance.RequestTrackingAuthorizationWithCompletionHandler((status) => {
            Debug.Log("===> App Tracking Transparency Authorization Status: " + status);
            Status = status;

            // Sends install/open event to Tenjin
            instance.Connect();

        });*/

#elif UNITY_ANDROID

      // Sends install/open event to Tenjin
      instance.Connect();

#endif
    }
}
