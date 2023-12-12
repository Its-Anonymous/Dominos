using System.Collections.Generic;
using System.Runtime.InteropServices;
using AssetBuilder;
using AvatarBuilder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Linq;
using System;

public enum DispatchEvents
{
    MINT,
    GET_CHARACTERS,
    GET_ASSETS,
    EXIT_GAME,
    GET_MATCHTYPE,
    GET_OWN_NFTS,
    TOURNAMENT_MATCH_ENDED,
    GO_HOME,
    GO_TO_INVENTORY,
    UPDATE_COINS
}

public class MintHandler
{
    // Importing "PassTextParam"
    [DllImport("__Internal")]
    public static extern void PassTextParam(string text);

    public static string currentSelectedAvatarPrice = "0";
    public static string currentSelectedAvatarCode = "";

    #region Upload Picture to S3
    public static void UploadScreenShot_S3(byte[] imageByte)
    {
        //StartCoroutine(_UploadScreenShot_S3(imageByte));
        FileUplaod fileUplaod = new FileUplaod();
        fileUplaod.key = "file";
        fileUplaod.data = imageByte;

        Debug.Log("fileUplaod.data.Length: " + fileUplaod.data.Length);
        WebServiceManager.instance.APIRequest(WebServiceManager.instance.uploadImageToS3Server, Method.POST, null, null, OnSuccessfullyImageUpload, OnFailS3Upload, CACHEABLE.NULL, true, fileUplaod);
    }

    private static void OnFailS3Upload(string error)
    {
        Debug.LogError("error: " + error);
    }

    #endregion Upload Picture to S3 Ended

    #region Upload Json to S3

    private static string FindPrice()
    {
        string price = "";

        if (PlayerPersonalData.discount > 0)
        {
            double originalPrice = double.Parse(currentSelectedAvatarPrice);
            double afterDiscount = originalPrice - (originalPrice * PlayerPersonalData.discount / 100);
            price = afterDiscount.ToString();
        }
        else
        {
            price = currentSelectedAvatarPrice;
        }
        return price;
    }

    private static void OnSuccessfullyImageUpload(JObject J_Object, long code)
    {
        Debug.Log("Response Code: " + code);
        Debug.Log("Response: " + J_Object.ToString());
        UploadImageToS3 uploadPicture = UploadImageToS3.FromJson(J_Object.ToString());
        //PlayerPersonalData.playerStates.blockChainData.avatarScreenShot_S3URL = uploadPicture.Data.FileUrl;

        Mint mint = new Mint();
        MintData mintData = new MintData();

        mintData.name = AvatarParent_FbxHolder.instance.cachedSelecteditem.avatarName;


        mintData.price = FindPrice();
        mintData.itemclass = currentSelectedAvatarCode;
        mintData.type = "Avatar";
        mintData.itemCategory = "Avatar";
        mintData.shortCode = currentSelectedAvatarCode;
        mintData.description = "By Dominoes Earning World. DEW Assets are on-chain customized NFTs. This item is a " +AvatarParent_FbxHolder.instance.cachedSelecteditem.gender+ " character.";
        mintData.imageS3URL = uploadPicture.Data.FileUrl;
        mintData.ExternalUrl = "https://dewskill2earn.com/";

        MintAttribute mintAttribute = new MintAttribute
        {
            Message = "minted character",
            gender = AvatarParent_FbxHolder.instance.cachedSelecteditem.gender,
            DataList = AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList,
        };

        mintData.Attributes = mintAttribute;

        mint.tokens = new List<MintData> { mintData };

        var mintDataJson = AvatarBuilder.Serialize.ToJson(mint);
        Debug.Log("mintDataJson: " + mintDataJson);

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.uploadObjectToS3Server, Method.POST, mintDataJson, null, OnSuccessfullyJsonUpload, OnFailS3Upload, CACHEABLE.NULL, true, null);
    }

    private static void OnSuccessfullyJsonUpload(JObject J_Object, long code)
    {
        Debug.Log("Response Code: " + code);
        Debug.Log("Response: " + J_Object.ToString());
        UploadObjectToS3 uploadS3 = UploadObjectToS3.FromJson(J_Object.ToString());

        DispatchEvent dispatchEvent = new DispatchEvent();
        DispatchEventPayload dispatchEventPayload = new DispatchEventPayload();
        dispatchEventPayload.tokenUri = uploadS3.Data;
        string price = FindPrice();
        dispatchEventPayload.price = new List<string> { price };
        dispatchEventPayload.shortCode = new List<string> { currentSelectedAvatarCode };
        dispatchEventPayload.categoryType = new List<string> { "avatar" };
        dispatchEventPayload.type = new List<string> { "Avatar" };
        dispatchEventPayload.itemClass = new List<string> { currentSelectedAvatarCode };

        dispatchEvent.eventType = DispatchEvents.MINT.ToString();
        dispatchEvent.payload = dispatchEventPayload.ToJson();

        var dispatchEventJson = dispatchEvent.ToJson();
        Debug.Log("dispatchEventJson: " + dispatchEventPayload.ToJson());
#if !UNITY_EDITOR
                     PassTextParam(dispatchEventJson);
#endif
        AvatarScreenManager.instance.MintAvatarPopUpScreen.ClosePopUpDelegate();
    }
    #endregion Upload Json to S3 Ended

    //Asset Builder

    #region Upload Json to S3 For Assets  

    public static MintAssetsData AssetsMintedObjects(MintAttribute defaultCharacterAsset)
    {
        MintAssetsData mintData = new MintAssetsData();

        mintData.name = defaultCharacterAsset.item[0].itemName;
        mintData.price = defaultCharacterAsset.item[0].itemPrice.usdt.ToString();// AssetsFBXManager.instance.mintingData.Find(x => x.shortCode == defaultCharacterAsset.shortCode).item[0].itemPrice.usdt.ToString();
        mintData.itemclass = defaultCharacterAsset.item[0].itemClass.ToString();
        mintData.type = "Asset";
        mintData.externalUrl = "https://dewskill2earn.com/";       
        mintData.itemCategory = defaultCharacterAsset.shortCode;
        string description = "By Dominoes Earning World. DEW Assets are on-chain customized NFTs. This item belongs to " + defaultCharacterAsset.shortCode + " Category. It's name is " + defaultCharacterAsset.item[0].itemName;
        mintData.description = description;
        mintData.shortCode = defaultCharacterAsset.item[0].itemShortCode;

        if (defaultCharacterAsset.shortCode.ToLower() == "tattoo")
        {
            mintData.imageS3URL = defaultCharacterAsset.item[0].itemIcon;//AssetsFBXManager.instance.mintingData.Find(x => x.shortCode == defaultCharacterAsset.shortCode).item[0].itemIcon;
        }
        else
        {
            mintData.imageS3URL = defaultCharacterAsset.item[0].itemTexUrls[0].iconUrl;
        }

        mintData.attributes = defaultCharacterAsset;
        mintData.attributes.gender = AssetsConfiguratorManager.instance.gender.ToString();

        return mintData;     
    }


    public static void SendDataToS3(Dictionary<string, MintAssetsData> assetNftCache)
    {
        MintAssets mint = new MintAssets();
        AssetsConfiguratorManager.instance.cachePriceList.Clear();
        AssetsConfiguratorManager.instance.cacheAssetCategoryCode.Clear();
        AssetsConfiguratorManager.instance.cacheAssetItemShortCode.Clear();
        AssetsConfiguratorManager.instance.cacheItemClasses.Clear();


        foreach (var item in assetNftCache)
        {
            mint.tokens.Add(item.Value);
            AssetsConfiguratorManager.instance.cacheAssetItemShortCode.Add(item.Value.attributes.item[0].itemShortCode.ToString());
            AssetsConfiguratorManager.instance.cacheAssetCategoryCode.Add(item.Value.attributes.shortCode);
            AssetsConfiguratorManager.instance.cacheItemClasses.Add(item.Value.itemclass.ToString());

        }

        //double totalPrice = double.Parse(AssetsScreenManager.instance.itemPricingPanel.totalTextPrice.text.Split(' ')[0]);
        double totalPrice = (double)AssetsScreenManager.instance.MintAssetsPopUpScreen.priceVariable;

        if (PlayerPersonalData.playerWhiteListed && PlayerPersonalData.discount > 0)
        {
            Debug.Log("Yes discount is available.. Discount = " + PlayerPersonalData.discount);

            double originalPrice = totalPrice;
            double afterDiscount = originalPrice - (originalPrice * PlayerPersonalData.discount / 100);
            totalPrice = afterDiscount;
        }


        AssetsConfiguratorManager.instance.cachePriceList.Add(totalPrice.ToString());

        var mintDataJson = JsonConvert.SerializeObject(mint);
        Debug.Log("mintDataJson: " + mintDataJson);
        WebServiceManager.instance.APIRequest(WebServiceManager.instance.uploadObjectToS3Server, Method.POST, mintDataJson, null, OnSuccessfullyAssetJsonUpload, OnFailS3Upload, CACHEABLE.NULL, true, null);
    }

    private static void OnSuccessfullyAssetJsonUpload(JObject J_Object, long code)
    {   
        WaitingLoader.instance.ShowHide(true);
        //Debug.Log("Response Code: " + code);
        Debug.Log("Response of Upload Json: " + J_Object.ToString());
        UploadObjectToS3 uploadS3 = UploadObjectToS3.FromJson(J_Object.ToString());

        DispatchEvent dispatchEvent = new DispatchEvent();
        DispatchEventPayload dispatchEventPayload = new DispatchEventPayload();
        dispatchEventPayload.tokenUri = uploadS3.Data;
        dispatchEventPayload.price = AssetsConfiguratorManager.instance.cachePriceList;
        dispatchEventPayload.shortCode = AssetsConfiguratorManager.instance.cacheAssetItemShortCode;
        dispatchEventPayload.categoryType = AssetsConfiguratorManager.instance.cacheAssetCategoryCode;
        dispatchEventPayload.type = new List<string> { "Asset" };
        dispatchEventPayload.itemClass = AssetsConfiguratorManager.instance.cacheItemClasses;

        dispatchEvent.eventType = DispatchEvents.MINT.ToString();
        dispatchEvent.payload = dispatchEventPayload.ToJson();

        var dispatchEventJson = dispatchEvent.ToJson();
        Debug.Log("dispatchEventJson: " + dispatchEventJson);
#if !UNITY_EDITOR
                     PassTextParam(dispatchEventJson);
#endif
        AssetsScreenManager.instance.MintAssetsPopUpScreen.ClosePopUpDelegate();
    }
    #endregion
}

[Serializable]
public class MintAttribute
{
    public string shortCode;
    public string name;

    public string _id;
    public int id;

    [JsonProperty("message")]
    public string Message;

    [JsonProperty("avatarCategories")]
    public List<ThumbnailJsonData> DataList = new List<ThumbnailJsonData>();
    public List<Item> item;

    [JsonProperty("gender")]
    public string gender;

    [JsonProperty("avatarGender")]
    public string avatarGender { set { gender = value; } }
}

[Serializable]
public class MintAssetsAttribute
{
    [JsonProperty("message")]
    public string Message;

    [JsonProperty("avatarCategories")]
    public List<MintAttribute> DataList = new List<MintAttribute>();

    [JsonProperty("avatarGender")]
    public string gender = "";
}