using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using UnityEngine.UI;
using GameAnalyticsSDK;
using UnityEngine.Purchasing.Security;
using _YabuGames.Scripts.Signals;
using _YabuGames.Scripts.Spawners;
using _YabuGames.Scripts.Managers;
using TMPro;

public class IAPManager : MonoBehaviour,IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public Image noAdsImage;
    public Text ShopCoinText1, ShopCoinText2, ShopCoinText3;
    
    public TextMeshProUGUI ShopCoinText4, ShopCoinText5, ShopCoinText6, ShopCoinText7, buyUnlmtFLYText;


    public static string cops_100 = "cops_100", cops_1000 = "cops_1000", cops_5000 = "cops_5000", cops_noads = "cops_noads";
    float counter;
    DateTime olddateForWeek;

    public static string unlimited_fly = "WorldPlanet_UnlimitedFLY";

    private void Start()
    {
 
        unlimited_fly = "WorldPlanet_UnlimitedFLY";
        if (PlayerPrefs.GetInt("IsNoAds") == 1)
        {
            noAdsImage.gameObject.SetActive(false);
        }

        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

        checkProd(cops_noads);
       
        ReceiptCheckNoADS(cops_noads);
     

    }

    public static bool InitializedPlay;
    private void Update()
    {
        if (InitializedPlay)
        {
         

            InitializedPlay = false;
        }
        if (PlayerPrefs.GetInt("IsNoAds", 0)==0)
        {
            if (counter!=-1)
            {
               counter += Time.deltaTime;

                 if (counter>=3)
                 {
                     noAdsImage.gameObject.SetActive(true);
                     counter = -1;
                 }

            }
        }
    }
    public void checkProd(string noAds)
    {
#if UNITY_EDITOR
#elif UNITY_IOS
        StartCoroutine("checkProduct",noAds);
       
#endif
    }
    IEnumerator checkProduct(string prodId)
    {
        yield return new WaitForSeconds(3);
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(prodId);
            if (product != null && product.hasReceipt)
            {
                RemoveAds();
            }
            else
            {
                EnableAds();
            }

        }

    }
    public void ReceiptCheckNoADS(string noAds)
    {

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        IAppleConfiguration appleConfig = builder.Configure<IAppleConfiguration>();

        if (!string.IsNullOrEmpty(appleConfig.appReceipt))
        {
            Debug.Log(appleConfig.appReceipt);
            try
            {
                var receiptData = System.Convert.FromBase64String(appleConfig.appReceipt);
                AppleReceipt receipt = new AppleValidator(AppleTangle.Data()).Validate(receiptData);
                Debug.Log("Apple receipt " + receipt);
                foreach (AppleInAppPurchaseReceipt productReceipt in receipt.inAppPurchaseReceipts)
                {
                    if (productReceipt.productID == noAds)
                    {
                        RemoveAds();
                    }
                    else
                    {
                        EnableAds();
                    }
                    Debug.Log("Product ID: " + productReceipt.productID);
                    Debug.Log("Purchase Date: " + productReceipt.purchaseDate);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception on converting receipt:  " + e.ToString());
            }

        }
        else
        {
            Debug.Log("Apple receipt is null");
        }
    }
    public void ReceiptCheckWEEK(string WEEK)
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        IAppleConfiguration appleConfig = builder.Configure<IAppleConfiguration>();

        if (!string.IsNullOrEmpty(appleConfig.appReceipt))
        {
            Debug.Log(appleConfig.appReceipt);
            try
            {
                var receiptData = System.Convert.FromBase64String(appleConfig.appReceipt);
                AppleReceipt receipt = new AppleValidator(AppleTangle.Data()).Validate(receiptData);
                Debug.Log("Apple receipt " + receipt);
                foreach (AppleInAppPurchaseReceipt productReceipt in receipt.inAppPurchaseReceipts)
                {
                    if (productReceipt.productID == WEEK)
                    {
                        BoughtWeekfunc();
                    }
                    else
                    {
                        NoBoughtWeekfunc();
                    }
                    Debug.Log("Product ID: " + productReceipt.productID);
                    Debug.Log("Purchase Date: " + productReceipt.purchaseDate);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception on converting receipt:  " + e.ToString());
            }

        }
        else
        {
            Debug.Log("Apple receipt is null");
        }
    }
    public void ReceiptCheckMONTH(string MONTH)
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        IAppleConfiguration appleConfig = builder.Configure<IAppleConfiguration>();

        if (!string.IsNullOrEmpty(appleConfig.appReceipt))
        {
            Debug.Log(appleConfig.appReceipt);
            try
            {
                var receiptData = System.Convert.FromBase64String(appleConfig.appReceipt);
                AppleReceipt receipt = new AppleValidator(AppleTangle.Data()).Validate(receiptData);
                Debug.Log("Apple receipt " + receipt);
                foreach (AppleInAppPurchaseReceipt productReceipt in receipt.inAppPurchaseReceipts)
                {
                    if (productReceipt.productID == MONTH)
                    {
                        BoughtMonthfunc();
                    }
                    else
                    {
                        NoBoughtMonthfunc();
                    }
                    Debug.Log("Product ID: " + productReceipt.productID);
                    Debug.Log("Purchase Date: " + productReceipt.purchaseDate);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception on converting receipt:  " + e.ToString());
            }

        }
        else
        {
            Debug.Log("Apple receipt is null");
        }
    }
    public void ReceiptCheckYEAR(string YEAR)
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        IAppleConfiguration appleConfig = builder.Configure<IAppleConfiguration>();

        if (!string.IsNullOrEmpty(appleConfig.appReceipt))
        {
            Debug.Log(appleConfig.appReceipt);
            try
            {
                var receiptData = System.Convert.FromBase64String(appleConfig.appReceipt);
                AppleReceipt receipt = new AppleValidator(AppleTangle.Data()).Validate(receiptData);
                Debug.Log("Apple receipt " + receipt);
                foreach (AppleInAppPurchaseReceipt productReceipt in receipt.inAppPurchaseReceipts)
                {
                    if (productReceipt.productID == YEAR)
                    {
                        BoughtYearfunc();
                    }
                    else
                    {
                        NoBoughtYearfunc();
                    }
                    Debug.Log("Product ID: " + productReceipt.productID);
                    Debug.Log("Purchase Date: " + productReceipt.purchaseDate);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception on converting receipt:  " + e.ToString());
            }

        }
        else
        {
            Debug.Log("Apple receipt is null");
        }
    }
    public static bool RemoveAd;
    public static bool BoughtWeek;
    public static bool BoughtMonth;
    public static bool BoughtYear;
    private void EnableAds()
    {
        RemoveAd = false;
    }

    private void RemoveAds()
    {
        RemoveAd = true;

    }

    public void checkProdWeek(string week)
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
           StartCoroutine("checkProductweek",week);

#endif
    }
    IEnumerator checkProductweek(string prodweekId)
    {
        yield return new WaitForSeconds(3);
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(prodweekId);
            if (product != null && product.hasReceipt)
            {
                BoughtWeekfunc();
            }
            else
            {
                NoBoughtWeekfunc();
            }
        }

    }
    public void Query()
    {
      
    }
    private void NoBoughtWeekfunc()
    {
        BoughtWeek = false;
    }

    private void BoughtWeekfunc()
    {
        BoughtWeek = true;

    }
    public void checkProdMonth(string month)
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
           StartCoroutine("checkProductmonth",month);

#endif
    }
    IEnumerator checkProductmonth(string prodmonthId)
    {
        yield return new WaitForSeconds(3);
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(prodmonthId);
            if (product != null && product.hasReceipt)
            {
                BoughtMonthfunc();
            }
            else
            {
                NoBoughtMonthfunc();
            }
        }

    }
    private void NoBoughtMonthfunc()
    {
        BoughtMonth = false;
    }

    private void BoughtMonthfunc()
    {
        BoughtMonth = true;

    }

    public void checkProdYear(string year)
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
           StartCoroutine("checkProductyear",year);

#endif
    }
    IEnumerator checkProductyear(string prodyearId)
    {
        yield return new WaitForSeconds(3);
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(prodyearId);
            if (product != null && product.hasReceipt)
            {
                BoughtYearfunc();
            }
            else
            {
                NoBoughtYearfunc();
            }
        }

    }
    private void NoBoughtYearfunc()
    {
        BoughtYear = false;
    }

    private void BoughtYearfunc()
    {
        BoughtYear = true;

    }


    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            Debug.Log("IsInit: true");
            // ... we are done here.
            return;
        }
        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        bool canMakePayments = builder.Configure<IAppleConfiguration>().canMakePayments;
        if (canMakePayments)
        {
         
            builder.AddProduct(cops_100, ProductType.Consumable);
            builder.AddProduct(cops_1000, ProductType.Consumable);
            builder.AddProduct(cops_5000, ProductType.Consumable);
            builder.AddProduct(cops_noads, ProductType.NonConsumable); 
   
            UnityPurchasing.Initialize(this, builder);
        }
        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.

        // Continue adding the non-consumable product.
        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs
        // must only be referenced here.
        //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
        //       {kProductNameAppleSubscription, AppleAppStore.Name },
        //        {kProductNameGooglePlaySubscription, GooglePlay.Name },
        //    });
        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration
        // and this class’ instance. Expect a response either in OnInitialized or OnInitializeFailed.


        //UnityPurchasing.Initialize(this, builder);


        // If we have already connected to Purchasing ...
    }

   

    public void buyCoin(int i)
    {
        BuyProductID("cops_" + i.ToString());
    }
    public void BuyNoAds()
    {
        BuyProductID(cops_noads);
    }
    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }

    }

    public void BuyConsumable()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        //BuyProductID(kProductIDConsumable);
    }

    public void BuyNonConsumable()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        //BuyProductID(noAds);
    }

    public void BuySubscription()
    {
        // Buy the subscription product using its the general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        // Notice how we use the general product identifier in spite of this ID being mapped to
        // custom store-specific identifiers above.
        //BuyProductID(kProductIDSubscription);
    }
  

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {

        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "mainmenu-coinshop-OnInitializeFailed-IAP");

        // panel false olacak
    }

    public string GetPrice(string productID)
    {
        if (m_StoreController == null) InitializePurchasing();
        return m_StoreController.products.WithID(productID).metadata.localizedPriceString;
    }
    public void SetPriceText()
    {
       

    }

    public void coinSetPriceText()
    {
        if(!ShopCoinText4)
            return;
        ShopCoinText4.text = GetPrice(cops_100);
        ShopCoinText5.text = GetPrice(cops_1000);
        ShopCoinText6.text = GetPrice(cops_5000);
        ShopCoinText7.text = GetPrice(cops_noads);

    }



    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
       
        //LoadingPanel.SetActive(false);
        Debug.LogError("OnPurchasedFailed : product " + i + " and reason is " + p);
    }
    public static bool isRestoredOkay;
    public static bool isRestoredNoOkay;
    /*public string GetPrice(string productID)
    {
        if (m_StoreController == null) InitializePurchasing();
        return m_StoreController.products.WithID(productID).metadata.localizedPriceString;
    }
    */
    public static bool weekOkay, monthOkay, YearOkay, NoAdsOkay;

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, cops_100, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "100coin-Bought");
           
                var coinCount = 100 / 100;
                CoreGameSignals.Instance.OnSpawnCoins?.Invoke(coinCount, 0, 0, true);
                GameManager.Instance.money += 100;
                CoreGameSignals.Instance.OnSave?.Invoke();
           
        }

        else if (String.Equals(args.purchasedProduct.definition.id, cops_1000, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "1000coin-Bought");
         
            var coinCount = 1000 / 100;
            CoreGameSignals.Instance.OnSpawnCoins?.Invoke(coinCount, 0, 0, true);
            GameManager.Instance.money += 1000;
            CoreGameSignals.Instance.OnSave?.Invoke();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, cops_5000, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "5000coin-Bought");
            var coinCount = 5000 / 100;
            CoreGameSignals.Instance.OnSpawnCoins?.Invoke(coinCount, 0, 0, true);
            GameManager.Instance.money += 5000;
            CoreGameSignals.Instance.OnSave?.Invoke();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, cops_noads, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            PlayerPrefs.SetInt("isNoAds", 1);
            Debug.Log("isNoAds setlendi !!!");
            if (noAdsImage.gameObject != null)
            {
                Debug.Log("NoadsImage :null değil");
                noAdsImage.gameObject.SetActive(false);

            }
            NoAdsOkay = true;
            PlayerPrefs.SetInt("IsNoAds", 1);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Noads-Bought");
            RemoveAds();
        }

        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                if (result)
                {
                    isRestoredOkay = true;
                    PlayerPrefs.SetInt("IsNoAds", 1);
                    RemoveAds();
                }
                else
                {
                    isRestoredNoOkay = true;
                    EnableAds();
                }
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }





}
