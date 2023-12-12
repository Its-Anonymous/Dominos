using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using AvatarBuilder;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace AssetBuilder
{ 

    public class AssetsConfiguratorManager : MonoBehaviour
    {      
        public static AssetsConfiguratorManager instance;

        [Header("Selected Avatar")]
        public Gender gender;
        
        public List<AvatarScript> fbxModels;
        public static string jsonPath = "AvatarBuilder/Json";

        public AllAssetsCategoriesData allAssetsCategoriesData;
        public ResponseItemData responseItemData;
        public CategorySelectionPanel categorySelectionPanel;

        public GetUserSaveAssetsDataRoot gettingUserSaveAssetsData;
        public GetMintedSaveUserAssetsDataRoot gettingMintedSaveUserData;

        public SaveUserAssetsData savingUserAssetsData;
        public MintedSaveUserAssetsData mintedSavingUserAssetsData;
        [Space]
        public string saveSelectedUserJson;

        [HideInInspector] public string data;
        [HideInInspector] public static string checkUserAssetPlacing;

        public Dictionary<string, MintAssetsData> assetNftCache = new Dictionary<string, MintAssetsData>();

        public List<string> mintedAssetUri = new List<string>();

        public List<string> cachePriceList = new List<string>();
        public List<string> cacheAssetCategoryCode = new List<string>();
        public List<string> cacheAssetItemShortCode = new List<string>();
        public List<string> cacheItemClasses = new List<string>();

        public NFTUriAndToken selectedAvatar;
        public string selectedAvatarName;
        public string selectedAvatarClass;
        public string selectedAvatarImage;
        private Action redirectToHome = null;

        static int count = 0;
        private void Awake()
        {
           instance = this;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => !string.IsNullOrEmpty(Global.GetBearerToken) && !string.IsNullOrEmpty(WebServiceManager.baseURL) && JS_Hook.instance.characterBlendDownloaded);
        
            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                GetUserSaveDataAssets(WebServiceManager.instance.getUserAssetsCategories);
            }
            else if(SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
            {
               // GetUserSaveDataAssets(WebServiceManager.instance.getUserAssetsCategories);
                GetUserSaveDataAssets(WebServiceManager.instance.getMintedUserAssetsCategories);             
            }

            //WebServiceManager.instance.APIRequest(WebServiceManager.instance.getSetUserAvatarSpecs, Method.GET, null, null, OnSuccessGetAvatarSpecs, OnFail, CACHEABLE.NULL, true, null);
        }

        public void Fill_List(List<AvatarScript> fbxModels)
        {
            this.fbxModels = fbxModels;

        }

        public AvatarScript EnableDisableModel(int? selectionModel)
        {
            AvatarScript av = null;
            foreach (var item in fbxModels)
            {
                item.gameObject.SetActive(false);

            }
            if (selectionModel != null)
            {
                av = fbxModels.Find(x => x.tag.Equals(selectionModel == 1 ? "male" : "female"));
                var avtarparent = av.GetComponentInParent<AvatarParent_FbxHolder>().currentSelectedAvatar = av.GetComponent<AvatarFBXController>();
                av.gameObject.SetActive(true);
            }
            return av;
        }
        
        public void GetAssetBuilderCategories(string url = null)
        {
            //Load Dummy Data From Json
            if (url == null)
            {
                var loaded_text_file = Resources.Load(jsonPath + "/GetAssetBuilderCategories") as TextAsset;
                print(loaded_text_file);
                allAssetsCategoriesData = JsonConvert.DeserializeObject<AllAssetsCategoriesData>(loaded_text_file.ToString());
            }
            else
            {
                allAssetsCategoriesData = null;
                Dictionary<string, object> paramsData = new Dictionary<string, object>(); //Add After Api will Created
                paramsData.Add("gender", (int)gender); 
                WebServiceManager.instance.APIRequest(url, Method.GET, null, paramsData, OnSuccessGetAllCategories, OnFail);
                paramsData.Clear();
            }            
        }

        private void OnSuccessGetAllCategories(JObject data, long code)
        {
            if (ResponseStatus.Check(code))
            {
                allAssetsCategoriesData =  JsonConvert.DeserializeObject<AllAssetsCategoriesData>(data.ToString());
                categorySelectionPanel.categories[0].SetCatagoryResponseData(categorySelectionPanel.categories);                
            }
            else
            {
                Debug.Log(code);
            }
        }

        public void GetAssetBuilderItemsByCategoryID(string url = null,CategoryThumbnail categoryThumbnail = null)
        {
            if (url == null && categoryThumbnail == null)
            {
                var loaded_text_file = Resources.Load(jsonPath + "/GetAssetBuilderItemsByCategoryID") as TextAsset;
                print(loaded_text_file);
                responseItemData = JsonConvert.DeserializeObject<ResponseItemData>(loaded_text_file.ToString());
            }
            else if (url == null && categoryThumbnail != null)
            {
                var loaded_text_file = Resources.Load(jsonPath + "/" + categoryThumbnail.categoryResponseData._id) as TextAsset;
                categoryThumbnail.itemsData = JsonConvert.DeserializeObject<ResponseItemData>(loaded_text_file.ToString());
            }
            else
            {
                responseItemData = null;
                Dictionary<string, object> paramsData = new Dictionary<string, object>(); //Add After Api will Created               
                //paramsData.Add("id", categoryThumbnail.categoryResponseData._id);
                WebServiceManager.instance.APIRequest(url, Method.GET, null, paramsData, categoryThumbnail.OnSuccessItemsByCategoryID, OnFail);
                
                //categoryThumbnail.itemsData = responseItemData;
                paramsData = null;
            }

        }

        public void SetUserAssetCategories(string url = null,string jsonAssetsData = null,Action redirectToHome = null)
        {
            this.redirectToHome = redirectToHome;
            WebServiceManager.instance.APIRequest(url, Method.PUT, jsonAssetsData, null, OnSuccessOnSavingAssets, OnFail);
        }

        private void OnSuccessOnSavingAssets(JObject data, long code)
        {
            if (ResponseStatus.Check(code))
            {
                if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
                {
                    if (AssetsScreenManager.instance.assetPreviewScreen.saveUserAssets.assetCategories.Count == 0)
                    {                        
                        AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.genderSelectionScreen.gameObject, AssetsScreenManager.instance.assetPreviewScreen.gameObject);
                    }
                    else
                    {
                        print(data);
                    }
                }

                if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
                {
                    if (AssetsScreenManager.instance.assetPreviewScreen.mintedSaveUserAssets.mintedGender == "" || AssetsScreenManager.instance.assetPreviewScreen.mintedSaveUserAssets.mintedGender == null)
                    {
                        AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.avatarSelectionInventory.gameObject, AssetsScreenManager.instance.assetPreviewScreen.gameObject);
                    }
                    else
                    {
                        print(data);
                        if(this.redirectToHome!=null)
                            this.redirectToHome.Invoke();
                    }                    
                }
            }
            else
            {
                Debug.Log(data);
            }
        }

        public void GetUserSaveDataAssets(string url = null)
        {     
            WebServiceManager.instance.APIRequest(url, Method.GET, null, null, OnSuccessGetUserData, OnFail);
        }

        public void OnSuccessGetUserData(JObject data, long code)
        {
            if (ResponseStatus.Check(code))
            {
                Debug.Log("OnSuccessGetUserData");
                WaitingLoader.instance.ShowHide(true);

                GetUserSaveAssetsDataRoot getUserSaveAssetsDataRoot = JsonUtility.FromJson<GetUserSaveAssetsDataRoot>(data.ToString());
                if (getUserSaveAssetsDataRoot.data.femaleDefaultAsset.Count != 0)
                {
                    gettingUserSaveAssetsData = JsonUtility.FromJson<GetUserSaveAssetsDataRoot>(data.ToString());
                    print("On Success " + data);
                    getUserSaveAssetsDataRoot = null;
                }
                if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
                {
                    print(data);                    
                    this.data = data.ToString();
                    if (gettingUserSaveAssetsData.data.characterId == null || gettingUserSaveAssetsData.data.characterId == "")
                    {
                        AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.genderSelectionScreen.gameObject, AssetsScreenManager.instance.assetPreviewScreen.gameObject);

                        foreach (var avatar in fbxModels)
                        {
                            AvatarScript avatarScript;

                            if (avatar.gameObject.name.ToLower().Contains("female"))
                            {
                                avatarScript = avatar;
                                foreach (var item in gettingUserSaveAssetsData.data.femaleDefaultAsset)
                                {
                                    DefaultTextureDownload(avatarScript, item);
                                }
                            }
                            else if (avatar.gameObject.name.ToLower().Contains("male"))
                            {
                                avatarScript = avatar;
                                foreach (var item in gettingUserSaveAssetsData.data.maleDefaultAsset)
                                {
                                    DefaultTextureDownload(avatarScript, item);
                                }
                            }
                        }
                    }

                    else
                    {
                        Debug.Log("Is else working?");
                        AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.assetPreviewScreen.gameObject, AssetsScreenManager.instance.genderSelectionScreen.gameObject);
                        gender = (gettingUserSaveAssetsData.data.assetGender == Gender.female.ToString()) ? Gender.female : Gender.male;

                        AvatarScript avatarScript;

                        avatarScript = EnableDisableModel((int)gender);

                        //avatarScript = avatar;
                        foreach (var item in gettingUserSaveAssetsData.data.assetCategories)
                        {
                            DefaultTextureDownload(avatarScript, item);
                        }

                        GetAssetBuilderCategories(WebServiceManager.instance.getAssetBuilderCategories);

                        SetDataOnPreviewScreen(gettingUserSaveAssetsData.data.assetCategories);

                        foreach (var item in gettingUserSaveAssetsData.data.assetCategories)
                        {
                            AssetsScreenManager.instance.SetPriceAsPerCategoriesOnUI(item.item[0]);
                        }

                        MintData characterBlends = null;
                        try
                        {
                            characterBlends = (gettingUserSaveAssetsData.data.characterId != "Default" && gettingUserSaveAssetsData.data.characterId != "") ? JS_Hook.instance.characterMintData[gettingUserSaveAssetsData.data.characterId].MintData : null;//.Find(x => x.imageS3URL.Equals(gettingUserSaveAssetsData.data.characterId));
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("ex: " + ex);
                        }

                        if (characterBlends == null)
                        {
                            LoadCharacterBlends(gender, JS_Hook.instance.defaultCharactersData.Find(x => x.Attributes.gender.ToLower().Equals(gender.ToString().ToLower())).Attributes.DataList);
                        }
                        else
                        {
                            LoadCharacterBlends(gender, characterBlends.Attributes.DataList);
                        }

                    }
                }
                
                if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
                {
                    print("Minted Data Printed " + data);
                    gettingMintedSaveUserData = JsonConvert.DeserializeObject<GetMintedSaveUserAssetsDataRoot>(data.ToString());
                    this.data = data.ToString();

                    if (gettingMintedSaveUserData == null)
                    {
                        Debug.Log("Errooorrrrr................");
                    }

                    if (JS_Hook.instance.characterMintData.Count == 0)
                    {
                        Debug.Log("you have to buy the character.");

                        WaitingLoader.instance.ShowHide();
                        ErrorPopup.instance.ShowMessage("Please buy an AVATAR first.", false, JS_Hook.instance.GoToHome);
                        return;
                    }

                    if (gettingMintedSaveUserData.data.mintedGender == "" || gettingMintedSaveUserData.data.mintedGender == null)//gettingMintedSaveUserData.data.mintedCharacterId == null ||
                    {
                        AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.avatarSelectionInventory.gameObject, AssetsScreenManager.instance.assetPreviewScreen.gameObject);
                        List<string> selectedGendersKey = new List<string>();

                        foreach (var nft in JS_Hook.instance.characterMintData)
                        {
                            selectedGendersKey.Add(nft.Key);                             
                        }

                        if (selectedGendersKey.Count > 0)
                        {
                                AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.avatarSelectionInventory.gameObject, AssetsScreenManager.instance.assetPreviewScreen.gameObject);

                            selectedGendersKey.ForEach((string key) =>
                            {
                                NFT nft = JS_Hook.instance.characterMintData[key];

                                NFTUriAndToken avtarNFTUriAndToken = new NFTUriAndToken();
                                avtarNFTUriAndToken.Uri = nft.Uri;
                                avtarNFTUriAndToken.TokenID = nft.TokenID;

                                AssetsScreenManager.instance.avatarSelectionInventory.SetAvatarData(key, nft.MintData, nft.MintData.imageS3URL, avtarNFTUriAndToken);
                            });
                        }

                        GetAssetBuilderCategories(WebServiceManager.instance.getAssetBuilderCategories);

                        foreach (var avatar in fbxModels)
                        {
                            avatar.SetOnDefaultAssets(true);
                        }
                    }
                    else
                    {
                        Debug.Log("Is else working?");
                        AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.assetPreviewScreen.gameObject, AssetsScreenManager.instance.genderSelectionScreen.gameObject);
                        gender = (gettingMintedSaveUserData.data.mintedGender == Gender.female.ToString()) ? Gender.female : Gender.male;


                        //Hunain 17 Mar 2023
                        if (!JS_Hook.instance.characterMintData.ContainsKey(gettingMintedSaveUserData.data.mintedCharacter.TokenID))
                        {
                            MakeDefualtCharacter();
                            ErrorPopup.instance.ShowMessage("The system cannot find the saved AVATAR in your wallet.", false);
                            WaitingLoader.instance.ShowHide();
                            return;
                        }
                        else
                        {
                            JS_Hook.instance.GetRequest(gettingMintedSaveUserData.data.mintedCharacter, OnSuccessOfURI, OnFail);
                        }

                        foreach (var item in gettingMintedSaveUserData.data.mintedInventory)
                        {
                            //Hunain 22 Mar 2023
                            if (JS_Hook.instance.assetsMintDataDict.ContainsKey(item.TokenID))
                            {
                                JS_Hook.instance.GetRequest(item, OnSuccessOfURI, OnFail);
                            }
                            else
                            {
                                Debug.Log("Some saved Assets could not be found in your wallet.");
                                MakeDefualtCharacter();
                                ErrorPopup.instance.ShowMessage("The system cannot find some ASSETS in your wallet.", false);
                                WaitingLoader.instance.ShowHide();
                                return;
                            }
                        }

                        if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
                        {
                            List<string> selectedGendersKey = new List<string>();
                            foreach (var nft in JS_Hook.instance.characterMintData)
                            {
                                selectedGendersKey.Add(nft.Key);
                            }

                            if (selectedGendersKey.Count > 0)
                            {                                
                                selectedGendersKey.ForEach((string key) =>
                                {
                                    NFT nft = JS_Hook.instance.characterMintData[key];

                                    NFTUriAndToken avtarNFTUriAndToken = new NFTUriAndToken();
                                    avtarNFTUriAndToken.Uri = nft.Uri;
                                    avtarNFTUriAndToken.TokenID = nft.TokenID;

                                    AssetsScreenManager.instance.avatarSelectionInventory.SetAvatarData(key, nft.MintData, nft.MintData.imageS3URL, avtarNFTUriAndToken);
                                });
                            }
                        }
                        GetAssetBuilderCategories(WebServiceManager.instance.getAssetBuilderCategories);
                        SetMintedDataOnGenderSelection(gender);
                        SetDataOnPreviewScreenInventory(gettingMintedSaveUserData.data.mintedInventory);
                        selectedAvatar      = gettingMintedSaveUserData.data.mintedCharacter;
                        selectedAvatarName  = gettingMintedSaveUserData.data.mintedAvatarName;
                        selectedAvatarImage = gettingMintedSaveUserData.data.mintedAvatarImageUrl;
                        selectedAvatarClass = gettingMintedSaveUserData.data.mintedAvatarClass;



                    }
                }


                Invoke("DisableLoader", 1f);
            }
        }

        private void OnSuccessOfURI(JObject data, long code, NFTUriAndToken arg3)
        {
            if (ResponseStatus.Check(code))
            {
                AvatarScript avatarScript;
                print("URI Data" +  data + " " + arg3.TokenID);
                avatarScript = EnableDisableModel((int)gender);

                MintData mintAssetsData = JsonConvert.DeserializeObject<MintData>(data.ToString());

                if (mintAssetsData.type.ToLower() == "asset")
                {
                    DefaultTextureDownload(avatarScript, mintAssetsData.Attributes);
                }                

                //MintData characterBlends = null;
                if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
                {
                    try
                    {
                        mintAssetsData = (gettingUserSaveAssetsData.data.characterId != "Default" && gettingUserSaveAssetsData.data.characterId != "") ? JS_Hook.instance.characterMintData[gettingUserSaveAssetsData.data.characterId].MintData : null;//.Find(x => x.imageS3URL.Equals(gettingUserSaveAssetsData.data.characterId));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("ex: " + ex);
                    }
                }
                //if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
                //{
                //    try
                //    {
                //        characterBlends = mintAssetsData.Attributes.DataList;// (mintedSavingUserAssetsData.mintedCharacter.TokenID != "Default" && mintedSavingUserAssetsData.mintedCharacter.TokenID != "") ? JS_Hook.instance.characterMintData[mintedSavingUserAssetsData.mintedCharacter.TokenID].MintData : null;//.Find(x => x.imageS3URL.Equals(gettingUserSaveAssetsData.data.characterId));                                               
                //        print("Save Id Blend "  + JsonConvert.SerializeObject(characterBlends));    
                //    }
                //    catch (Exception ex)
                //    {
                //        Debug.Log("ex: " + ex);
                //    }
                //}


                if (mintAssetsData == null)
                {
                    LoadCharacterBlends(gender, JS_Hook.instance.defaultCharactersData.Find(x => x.Attributes.gender.ToLower().Equals(gender.ToString().ToLower())).Attributes.DataList);
                }
                else
                {
                    LoadCharacterBlends(gender, mintAssetsData.Attributes.DataList);
                }

            }

        }

        //Invoke Method
        public void DisableLoader()
        {
            WaitingLoader.instance.ShowHide(false);
        }

        private void OnFail(string errorMsg)
        {
            ErrorPopup.instance.ShowMessage(errorMsg);
        }

        internal void OnSuccessGetAvatarSpecs(JObject response, long onSuccess)
        {
            if (onSuccess == 200)
            {
                Debug.Log("Response: " + response.ToString());                

                var avatarData = AvatarParent_FbxHolder.instance.GetAvatarSpecs(response.ToString());

                if (avatarData.DataList.Count > 0)
                {
                    //Setting Texture of Facial Hair and Eye Brows
                    //Setting Remaining Categories Data
                    AvatarParent_FbxHolder.instance.UpdateDataOnModel();
                }
            }
            else
            {
                Debug.Log(response.ToString());
            }
        }

        public void SetDataOnPreviewScreen(List<MintAttribute> defaultCharacterAssetData)
        {
            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                AssetsScreenManager.instance.assetPreviewScreen.saveUserAssets.assetCategories = defaultCharacterAssetData;
            }         
        }
        public void SetDataOnPreviewScreenInventory(List<NFTUriAndToken> stringsURI)
        {
              AssetsScreenManager.instance.assetPreviewScreen.mintedSaveUserAssets.mintedInventory = stringsURI;
        }

        public void DefaultTextureDownload(AvatarScript avatarScript, MintAttribute item, bool loader = true)
        {
            StartCoroutine(GetDefaultTexture(avatarScript, item, loader));
        }

        public static IEnumerator GetDefaultTexture(AvatarScript avatarScript, MintAttribute item, bool loader = true)
        {
            if (loader)
            {
                WaitingLoader.instance.ShowHideTillTextureUpdate(true);
            }
            if (item.item != null && item.item.Count > 0 && item.item[0].itemTexUrls.Count == 0)
            {
                AssetsHolder assetsHolder = null;

                if (item.shortCode == "tattoo")
                {
                    assetsHolder = avatarScript.tattos.GetComponent<AssetsHolder>();
                    if (!assetsHolder)
                    {
                        Debug.Log("Error");
                    }
                    try { assetsHolder.childObjects.Find(x => x.name.ToLower() == item.item[0].itemShortCode.ToLower()).gameObject.SetActive(true); }
                    catch (Exception ex) { Debug.Log("ex: " + ex); }
                }

                Debug.Log("break");
                yield break;
            }
            
            if (item.item != null)
            {

                UnityWebRequest www = UnityWebRequestTexture.GetTexture(item.item[0].itemTexUrls[0].texUrl);
                yield return www.SendWebRequest();
                
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture tempTexture;

                    tempTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                 
                    Debug.Log("Defualt Texture Downloaded. . . ");

                    Debug.Log("Item Download: " + item.shortCode);

                    AssetsHolder assetsHolder = null;
                    if (item.shortCode == "upper")
                    {
                        assetsHolder = avatarScript.shirt.GetComponent<AssetsHolder>();
                    }
                    else if (item.shortCode == "lower")
                    {
                        Debug.Log("applied tex on lower");
                        assetsHolder = avatarScript.pant.GetComponent<AssetsHolder>();
                    }
                    else if (item.shortCode == "socks")
                    {
                        assetsHolder = avatarScript.socks.GetComponent<AssetsHolder>();
                    }
                    else if (item.shortCode == "shoes")
                    {
                        assetsHolder = avatarScript.shoes.GetComponent<AssetsHolder>();
                    }
                    else if (item.shortCode == "cap")
                    {
                        assetsHolder = avatarScript.caps.GetComponent<AssetsHolder>();
                    }
                    else if (item.shortCode == "glasses")
                    {
                        assetsHolder = avatarScript.glasses.GetComponent<AssetsHolder>();
                    }
                    else if (item.shortCode == "jewellery")
                    {
                        assetsHolder = avatarScript.jewelleries.GetComponent<AssetsHolder>();
                    }
                    
                   ApplyTextureOnMaterial(assetsHolder, tempTexture, item);                   
                }              
            }
            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                count += 1;
                  if (count == 7)
                  {
                     WaitingLoader.instance.ShowHideTillTextureUpdate(false);
                     count = 0;
                  }
            }
            if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
            {
                WaitingLoader.instance.ShowHideTillTextureUpdate(false);
                count = 0;
            }
        }

        public static void ApplyTextureOnMaterial(AssetsHolder assetsHolder, Texture tempTexture, MintAttribute item)
        {           
            if (assetsHolder != null)
            {
                foreach (var childItems in assetsHolder.childObjects)
                {

                    if (childItems.name.ToLower() == item.item[0].itemShortCode.ToLower())
                    {
                        childItems.gameObject.SetActive(true);
                        AvatarScript.SetOnUserAssets(true, assetsHolder , item.item[0]);                                                    


                        MeshRenderer meshRenderer = childItems.GetComponent<MeshRenderer>();
                        if (meshRenderer != null)
                            meshRenderer.material.SetTexture("_MainTex", tempTexture);
                        else
                        {
                            SkinnedMeshRenderer skinnedMesh = childItems.GetComponent<SkinnedMeshRenderer>();
                            if (skinnedMesh != null)
                                skinnedMesh.material.SetTexture("_MainTex", tempTexture);
                        }
                    }
                }              
            }
        }

        public void TextureDownload(ItemTexUrl item, VariantThumb variantThumbnail)
        {
             StartCoroutine(GetTexture(item, variantThumbnail));
        }

        public static IEnumerator GetTexture(ItemTexUrl item, VariantThumb variantThumbnail)
        {
            Debug.Log("net  issue 1");
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(item.texUrl);
            www.timeout = 20;
            yield return www.SendWebRequest();

            Debug.Log("net  issue 2");


            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("net  issue 3" + www.error);
                ErrorPopup.instance.ShowMessage("Unable to load texture due to connection unstability", true);
                WaitingLoader.instance.ShowHide();
            }
            else
            {
                Texture tempTexture;

                tempTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                //Sprite sprite = Sprite.Create((Texture2D)tempTexture, new Rect(0.0f, 0.0f, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

                variantThumbnail.texture = tempTexture;

                for (int i = 0; i < variantThumbnail.itemThumbnail.variants.Count; i++)
                {   
                    if (variantThumbnail.id == variantThumbnail.itemThumbnail.variants[i].variantThumb.id)
                    {                       
                        variantThumbnail.itemThumbnail.variants[i].ApplyTexture();
                        Debug.Log("Texture Downloaded. . . ");
                    }
                }

            }
       
        }

        public void IconDownload(ItemTexUrl item, VariantThumb variantThumbnail)
        {
             StartCoroutine(GetIcon(item, variantThumbnail));
        }

        public static IEnumerator GetIcon(ItemTexUrl item, VariantThumb variantThumbnail)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(item.iconUrl);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture tempTexture;

                tempTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create((Texture2D)tempTexture, new Rect(0.0f, 0.0f, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

                variantThumbnail.iconImage = sprite;               
                Debug.Log("Variant Icon Downloaded. . . ");

                for (int i = 0; i < variantThumbnail.itemThumbnail.variants.Count; i++)
                {
                    if (variantThumbnail.id == variantThumbnail.itemThumbnail.variants[i].variantThumb.id)
                    {
                        variantThumbnail.itemThumbnail.variants[i].SetVariantData(variantThumbnail, null);
                    }
                }
                
            }
        }

        public void ItemIconDownload(string itemIconUrl, Image iconImage)
        {
             StartCoroutine(ItemGetIcon(itemIconUrl, iconImage));
        }

        public static IEnumerator ItemGetIcon(string itemIconUrl, Image iconImage)
        {
            iconImage.transform.GetChild(0).gameObject.SetActive(true);


            UnityWebRequest www = UnityWebRequestTexture.GetTexture(itemIconUrl);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (iconImage == null)
                {
                    yield break;
                }
                Texture tempTexture;

                tempTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create((Texture2D)tempTexture, new Rect(0.0f, 0.0f, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

                iconImage.sprite = sprite;
                iconImage.enabled = true;
                iconImage.transform.GetChild(0).gameObject.SetActive(false);

                Debug.Log("Item Icon Downloaded. . . ");
            }
            
        }

        public void ClearContent(GameObject go)
        {
            foreach (Transform item in go.transform)
            {
                Destroy(item.gameObject);
            }
        }

        public void SaveUserSelectedAssetDataJson(SaveUserAssetsData userAssetsData)
        {
            savingUserAssetsData = null;
            saveSelectedUserJson = null;
            savingUserAssetsData = userAssetsData;
            saveSelectedUserJson = JsonConvert.SerializeObject(savingUserAssetsData);
        }

        public void MintedSaveUserSelectedAssetDataJson(MintedSaveUserAssetsData userAssetsData)
        {
            mintedSavingUserAssetsData = null;
            saveSelectedUserJson = null;
            mintedSavingUserAssetsData = userAssetsData;
            saveSelectedUserJson = JsonConvert.SerializeObject(mintedSavingUserAssetsData);
            Debug.Log(saveSelectedUserJson);
        }

        public void MakeDefualtCharacter()
        {
            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                gettingUserSaveAssetsData.data = new GetUserSaveAssetsData();
                savingUserAssetsData.assetCategories = new List<MintAttribute>();
                savingUserAssetsData.assetGender = "";
                savingUserAssetsData.characterId = "";
                saveSelectedUserJson = JsonConvert.SerializeObject(savingUserAssetsData);
                SetUserAssetCategories(WebServiceManager.instance.setUserAssetsCategories, saveSelectedUserJson);

            }
            if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
            {
                gettingMintedSaveUserData.data = new MintedSaveUserAssetsData();
                mintedSavingUserAssetsData.mintedInventory = new List<NFTUriAndToken>();
                mintedSavingUserAssetsData.mintedCharacter = new NFTUriAndToken();
                mintedSavingUserAssetsData.mintedGender = "";
                mintedSavingUserAssetsData.mintedAvatarName = "";
                mintedSavingUserAssetsData.mintedAvatarClass = "";
                mintedSavingUserAssetsData.mintedAvatarImageUrl = "";
                saveSelectedUserJson = JsonConvert.SerializeObject(mintedSavingUserAssetsData);
                SetUserAssetCategories(WebServiceManager.instance.setMintedUserAssetsCategories, saveSelectedUserJson);

            }

            CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.screenCenterView);
            fbxModels[(int)gender].gameObject.SetActive(false);
            fbxModels[(int)gender].SetOnDefaultAssets(true);            
        }

        public void LoadCharacterBlends(Gender gender, List<ThumbnailJsonData> DataList)
        {
            AvatarParent_FbxHolder.instance.cachedSelecteditem = new CachedItemThumbnailJson();
            AvatarParent_FbxHolder.instance.cachedSelecteditem.gender = gender.ToString();
            AvatarParent_FbxHolder.instance.cachedSelecteditem.DataList = DataList;
            AvatarParent_FbxHolder.instance.UpdateDataOnModel();
        }
        public void SetMintedDataOnGenderSelection(Gender onGenderSelection)
        {
            foreach (var data in JS_Hook.instance.assetsMintDataDict)
            {
                NFT nft = data.Value;
                foreach (var item2 in AssetsScreenManager.instance.categorySelectionPanel.categories)
                {
                    if (nft.MintData.itemCategory.ToLower() == item2.categoryType.ToString().ToLower())
                    {
                        if (nft.MintData.Attributes.gender.ToLower() == onGenderSelection.ToString().ToLower())
                        {
                            nft.MintData.Attributes.item[0].itemIcon = nft.MintData.imageS3URL;
                            if (item2.itemsData.data.item == null)
                            {
                                item2.itemsData.data.item = new List<Item>();
                            }

                            item2.itemsData.data.item.Add(nft.MintData.Attributes.item[0]);
                        }
                    }
                }
            }
        }


    }

   
}

public enum AvatarAssetsCateogeries
{
    upper,
    lower,
    socks,
    shoes,
    cap,
    glasses,
    jewellery,
    tattoo
}