using UnityEngine;
using System.Collections;
using Soomla.Store;

public class ExampleAssets : IStoreAssets{		
	public int GetVersion() {
		return 0;
	}
	
	// NOTE: Even if you have no use in one of these functions, you still need to
	// implement them all and just return an empty array.
	
	public VirtualCurrency[] GetCurrencies() {
		return new VirtualCurrency[]{};
	}
	
	public VirtualGood[] GetGoods() {
		return new VirtualGood[] {};
	}
	
	public VirtualCurrencyPack[] GetCurrencyPacks() {
		return new VirtualCurrencyPack[] {};
	}
	
	public VirtualCategory[] GetCategories() {
		return new VirtualCategory[]{};
	}
}
