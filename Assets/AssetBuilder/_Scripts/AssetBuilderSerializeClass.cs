using System.Collections;
using System.Collections.Generic;
using AvatarBuilder;
using UnityEngine;


namespace AssetBuilder
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [System.Serializable]
    public class Data
    {
        public string _id;
        public int id;
        public string name;
        public string shortCode;
        public string gender = "male";
        public List<Item> item;

        public Data()
        {
            _id = "";
        }
    }

    [System.Serializable]
    public class Item
    {
        public string itemName;
        public string itemShortCode;
        public string itemType;
        public string itemClass;
        public string itemIcon;
        public string itemDesc;
        public ItemPrice itemPrice;
        public List<ItemTexUrl> itemTexUrls = new List<ItemTexUrl>();
    }

    [System.Serializable]
    public class ItemPrice
    {
        public string chain;
        public double chainPrice;
        public double usdt;
    }

    [System.Serializable]
    public class ItemTexUrl
    {
        public string texId;
        public string texUrl;
        public string iconUrl;
    }

    [System.Serializable]
    public class ResponseItemData
    {
        public string message{ get; set; }
        public Data data;

        public ResponseItemData()
        {
            data = new Data();
        }
    }

    [System.Serializable]
    public class AssetCategoryResponseData
    {
        //public long id;
        public string _id;
        //public string name;
        public string shortCode;
    }

    [System.Serializable]
    public class AllAssetsCategoriesData
    {
        public string message{ get; set; }
        public List<AssetCategoryResponseData> data;
    }

    [System.Serializable]
    public class AssetCategory
    {
        //public string gender;
        public string _id;
        public string type;
        public string shortCode;
        public List<Item> item = new List<Item>();
    }

    [System.Serializable]
    public class GetUserSaveAssetsDataRoot
    {
        public string message;
        public GetUserSaveAssetsData data;
    }

    [System.Serializable]
    public class GetUserSaveAssetsData
    {
        public string assetGender;
        public string characterId;
        public List<MintAttribute> assetCategories;
        public List<MintAttribute> maleDefaultAsset;
        public List<MintAttribute> femaleDefaultAsset;
    }

    [System.Serializable]
    public class SaveUserAssetsData
    {
        public string characterId;
        public string assetGender;        
        public List<MintAttribute> assetCategories;
    }

    [System.Serializable]
    public class GetMintedSaveUserAssetsDataRoot
    {
        public string message;
        public MintedSaveUserAssetsData data;
    }

    [System.Serializable]
    public class MintedSaveUserAssetsData
    {
        public List<NFTUriAndToken> mintedInventory;
        public string mintedGender = "";
        public string mintedAvatarName = "";
        public string mintedAvatarClass = "";
        public string mintedAvatarImageUrl = "";
        public NFTUriAndToken mintedCharacter;
    }



    //[System.Serializable]
    //public class DefaultCharacterAsset
    //{
    //    public string shortCode;
    //    public string name;
    //    public string _id;
    //    public int id;
    //    public string gender;
    //    public List<Item> item;
    //}

    //[System.Serializable]
    //public class FemaleDefaultAsset
    //{
    //    public string _id;
    //    public int id;
    //    public string name;
    //    public string shortCode;
    //    public string gender;
    //    public List<Item> item;
    //}



}