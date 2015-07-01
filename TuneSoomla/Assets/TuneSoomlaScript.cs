using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MATSDK;
using Soomla.Levelup;
using Soomla.Profile;
using Soomla.Store;

public class TuneSoomlaScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Initialize TUNE SDK
		MATBinding.Init("tune_advertiser_id", "tune_conversion_key");
		// Measure initial app open
		MATBinding.MeasureSession();
		
		// Listen for Soomla OnLoginFinished event
		ProfileEvents.OnLoginFinished += onLoginFinished;
		// Listen for Soomla OnMarketPurchase event
		StoreEvents.OnMarketPurchase += onMarketPurchase;
		// Listen for Soomla OnLevelStarted event
		LevelUpEvents.OnLevelStarted += onLevelStarted;
		
		// Initialize Soomla Profile
		SoomlaProfile.Initialize();
		
		// Initialize Soomla Store with your Store Assets
		SoomlaStore.Initialize(new ExampleAssets());

		// Create an example World with Level
		World world = new World("exampleWorld");
		Level lvl1 = new Level("Level 1");
		world.AddInnerWorld(lvl1);
		
		// Initialize Soomla LevelUp with the example world containing a level
		SoomlaLevelUp.Initialize(world);
		
		// Start the level, will trigger OnLevelStarted event
		lvl1.Start();
	}
	
	void OnApplicationPause(bool pauseStatus) {
		if (!pauseStatus) {
			// Measure app resumes from background
			MATBinding.MeasureSession();
		}
	}

	// Set user ID and measure login event upon login finished
	public void onLoginFinished(UserProfile userProfileJson, string payload) {
		Provider provider = userProfileJson.Provider;
		string userId = userProfileJson.ProfileId;
		
		// Set different user IDs in TUNE SDK based on provider
		if (provider == Provider.FACEBOOK) {
			MATBinding.SetFacebookUserId(userId);
		} else if (provider == Provider.GOOGLE) {
			MATBinding.SetGoogleUserId(userId);
		} else if (provider == Provider.TWITTER) {
			MATBinding.SetTwitterUserId(userId);
		} else {
			MATBinding.SetUserId(userId);
		}
		// Measure a login event for this user ID
		MATBinding.MeasureEvent("login");
	}

	// On purchase complete, set purchase info and measure purchase in TUNE
	public void onMarketPurchase(PurchasableVirtualItem pvi, string payload,
	                             Dictionary<string, string> extras) {
		double revenue = 0;
		string currency = "";
		MATItem[] items = new MATItem[] {};
		MarketItem item = ((PurchaseWithMarket)pvi.PurchaseType).MarketItem;
		revenue = item.MarketPriceMicros / 1000000;
		currency = item.MarketCurrencyCode;
		
		// Create event item to store purchase item data
		MATItem matItem = new MATItem();
		matItem.name = item.MarketTitle;
		matItem.attribute1 = item.ProductId;
		
		// Add event item to MATItem array in order to pass to TUNE SDK
		items[items.Length] = matItem;
		
		// Get order ID and receipt data for purchase validation
		string receipt = "";
		string receiptSignature = "";
		string orderId = "";
#if UNITY_ANDROID
		extras.TryGetValue("originalJson", out receipt);
		extras.TryGetValue("signature", out receiptSignature);
		extras.TryGetValue("orderId", out orderId);
#elif UNITY_IOS
		extras.TryGetValue("receiptBase64", out receipt);
		extras.TryGetValue("transactionIdentifier", out orderId);
#endif
		
		// Create a MATEvent with this purchase data
		MATEvent purchaseEvent = new MATEvent("purchase");
		purchaseEvent.revenue = revenue;
		purchaseEvent.currencyCode = currency;
		purchaseEvent.advertiserRefId = orderId;
		purchaseEvent.receipt = receipt;
		purchaseEvent.receiptSignature = receiptSignature;
		
		// Set event item if it exists
		if (items.Length != 0) {
			purchaseEvent.eventItems = items;
		}
		// Measure "purchase" event
		MATBinding.MeasureEvent(purchaseEvent);
	}

	// Set level ID and measure a level achieved event upon level start
	public void onLevelStarted(Level level) {
		// Create a MATEvent for level_achieved with the level ID
		MATEvent levelEvent = new MATEvent("level_achieved");
		levelEvent.contentId = level.ID;
		
		// Measure "level_achieved" event for this level ID
		MATBinding.MeasureEvent (levelEvent);
	}
}
