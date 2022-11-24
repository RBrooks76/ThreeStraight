using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System;

public class UI_Shop : MonoBehaviour, IStoreListener
{
    static public UI_Shop UIShop;
    public Transform contentView;
    int playerLevel = 0;

    public Button BuyBtn50;
    public Button BuyBtn100;
    public Button BuyBtn200;
    public Button BuyBtn500;

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased:
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.

    //public static string kProductIDConsumable = "consumable";
    //public static string kProductIDNonConsumable = "nonconsumable";
    //public static string kProductIDSubscription = "subscription";

    //// Apple App Store-specific product identifier for the subscription product.
    //private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

    //// Google Play Store-specific product identifier subscription product.
    //private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

    public static string kProductID_50_Tier1 = "bs.threestraight.tier1";
    public static string kProductID_100_Tier2 = "bs.threestraight.tier2";
    public static string kProductID_200_Addon1 = "bs.threestraight.buycoin10";
    public static string kProductID_500_Addon2 = "bs.threestraight.buycoin50";

    void Start()
    {
        if (UIShop == null)
            UIShop = this;
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            Debug.Log("Already initialized the Products");
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(kProductID_50_Tier1, ProductType.Subscription);
        builder.AddProduct(kProductID_100_Tier2, ProductType.Subscription);
        builder.AddProduct(kProductID_200_Addon1, ProductType.Consumable);
        builder.AddProduct(kProductID_500_Addon2, ProductType.Consumable);

        // Continue adding the non-consumable product.
        //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
        //// And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        //// if the Product ID was configured differently between Apple and Google stores. Also note that
        //// one uses the general kProductIDSubscription handle inside the game - the store-specific IDs
        //// must only be referenced here.
        //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
        //        { kProductNameAppleSubscription, AppleAppStore.Name },
        //        { kProductNameGooglePlaySubscription, GooglePlay.Name },
        //});

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
        Debug.Log("Initialize succeeded");
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyConsumable(string productId)
    {
        // Buy the consumable product using its general identifier. Expect a response either
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        Debug.Log("onClickBuy");

        //BuyProductID(kProductIDConsumable);
        //BuyProductID(productId);
    }


    public void BuyNonConsumable()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either
        // through ProcessPurchase or OnPurchaseFailed asynchronously.

        //BuyProductID(kProductIDNonConsumable);
    }


    public void BuySubscription(string productId)
    {
        // Buy the subscription product using its the general identifier. Expect a response either
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        // Notice how we use the general product identifier in spite of this ID being mapped to
        // custom store-specific identifiers above.

        //BuyProductID(kProductIDSubscription);
        BuyProductID(productId);
    }


    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ...
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));

                //transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = productId;
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


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google.
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
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
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then
                // no purchases are available to be restored.
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


    //
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        //Debug.Log("OnInitialized: PASS");

        //// Overall Purchasing system, configured with products for this application.
        //m_StoreController = controller;
        //// Store specific subsystem, for accessing device-specific store features.
        //m_StoreExtensionProvider = extensions;

        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        // Adjust Local Prices.

        // Black Jokers.

        Product tier1 = m_StoreController.products.WithID(kProductID_50_Tier1);
        //contentView.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = tier1.metadata.localizedPriceString;
        //contentView.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = string.IsNullOrEmpty(tier1.metadata.localizedPriceString).ToString();
        //Debug.Log("Tier1 =====>" + tier1.metadata.localizedPriceString);
        if (tier1 != null && tier1.metadata != null)
        {
            BuyBtn50.GetComponentInChildren<TextMeshProUGUI>().text = tier1.metadata.localizedPriceString;
        }
        Product tier2 = m_StoreController.products.WithID(kProductID_100_Tier2);
        if (tier2 != null && tier2.metadata != null)
        {
            BuyBtn100.GetComponentInChildren<TextMeshProUGUI>().text = tier2.metadata.localizedPriceString;
        }
        Product addon1 = m_StoreController.products.WithID(kProductID_200_Addon1);
        if (addon1 != null && addon1.metadata != null)
        {
            BuyBtn200.GetComponentInChildren<TextMeshProUGUI>().text = addon1.metadata.localizedPriceString;
        }
        Product addon2 = m_StoreController.products.WithID(kProductID_500_Addon2);
        if (addon2 != null && addon2.metadata != null)
        {
            BuyBtn500.GetComponentInChildren<TextMeshProUGUI>().text = addon2.metadata.localizedPriceString;
        }
        //Debug.Log("Tier1 =====>" + tier1.metadata.localizedPriceString);
        //Debug.Log("Tier2 =====>" + tier2.metadata.localizedPriceString);
        //Debug.Log("Addon1 =====>" + addon1.metadata.localizedPriceString);
        //Debug.Log("Addon2 =====>" + addon2.metadata.localizedPriceString);
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, kProductID_50_Tier1, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            //ScoreManager.score += 100;
            gameApi.request.CoinPurchase(1);
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, kProductID_100_Tier2, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
            gameApi.request.CoinPurchase(2);
        }
        // Or ... a subscription product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, kProductID_200_Addon1, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The subscription item has been successfully purchased, grant this to the player.
            gameApi.request.CoinPurchase(3);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, kProductID_500_Addon2, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The subscription item has been successfully purchased, grant this to the player.
            gameApi.request.CoinPurchase(4);
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still
        // saving purchased products to the cloud, and when that save is delayed.
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }


    // Update is called once per frame
    void Update()
    {
        if (playerLevel != UI_Main.UIM.playerLevel)
        {
            playerLevel = UI_Main.UIM.playerLevel;
            DispPlayerLevel(playerLevel);
        }
    }


    public void OnCoinParchase(int level)
    {
        if (level == gamePropertySettings.SHOP_TIER_1)
        {
            if (level == playerLevel)
            {
                // error alert
                UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.ALREADY_OWNED);
                UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
                return;
            }
            BuyProductID(kProductID_50_Tier1);
        }
        else if (level == gamePropertySettings.SHOP_TIER_2)
        {
            if (level == playerLevel)
            {
                // error alert
                UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.ALREADY_OWNED);
                UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
                return;
            }
            BuyProductID(kProductID_100_Tier2);
        }
        else if (level == gamePropertySettings.SHOP_ADDON_1)
        {
            BuyProductID(kProductID_200_Addon1);
        }
        else if (level == gamePropertySettings.SHOP_ADDON_2)
        {
            BuyProductID(kProductID_500_Addon2);
        }
    }


    public void OnBack()
    {
        transform.gameObject.SetActive(false);
        transform.GetComponent<Animation>().Play();
    }


    public void DispPlayerLevel(int level)
    {
        if (level == gamePropertySettings.SHOP_TIER_1)
        {
            contentView.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/shop_level_bg");
            contentView.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/shop_alert_bg");
        }
        else if (level == gamePropertySettings.SHOP_TIER_2)
        {
            contentView.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/shop_alert_bg");
            contentView.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/shop_level_bg");
        }
        else
        {
            contentView.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/shop_alert_bg");
            contentView.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/shop_alert_bg");
        }
    }
}
