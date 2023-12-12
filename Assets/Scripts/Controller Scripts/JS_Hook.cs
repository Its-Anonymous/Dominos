using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using AssetBuilder;
using AvatarBuilder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static System.Net.WebRequestMethods;

public class JS_Hook : MonoBehaviour
{
    public int maxPlayer = 3;
    [DllImport("__Internal")]
    public static extern void PassTextParam(string text);
    public static JS_Hook instance;

    public bool haveTokensInWallet;
    public bool characterBlendDownloaded;

    public Dictionary<string, NFT> characterMintData = new Dictionary<string, NFT>();
    //public Dictionary<string, MintAssetsData> assetsMintDataDict = new Dictionary<string, MintAssetsData>();

    public Dictionary<string, NFT> assetsMintDataDict = new Dictionary<string, NFT>();

    [SerializeField]
    public List<MintData> defaultCharactersData = new List<MintData>();

    public const string defaultMaleTokenURL     = "api/avatar-builder/default-male-avatar";
    public const string defaultFemaleTokenURL   = "api/avatar-builder/default-female-avatar";


    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

#if !UNITY_EDITOR
        if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
        {
            GetWalletTokenForCharacters();
        }
        if (SceneManager.GetActiveScene().name.ToLower().Equals(Global.CustomizeAvatarScene.ToLower())  || SceneManager.GetActiveScene().name.ToLower().Equals(Global.SplashScene.ToLower()))
        {
            //GetWalletTokenForCharacters();
            GetWalletTokenForNfs();
        }
#else
        List<NFTUriAndToken> nfts = new List<NFTUriAndToken>();
        if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
        {
            var temp = Resources.Load(AssetsConfiguratorManager.jsonPath + "/UriAssetBuilder") as TextAsset;
            print(temp.ToString());
            GetCharactersPayLoad charactersPayLoad = new GetCharactersPayLoad();
            charactersPayLoad = JsonConvert.DeserializeObject<GetCharactersPayLoad>(temp.ToString());
            LoadCharacters(charactersPayLoad);
        }

        if (SceneManager.GetActiveScene().name.ToLower().Equals(Global.CustomizeAvatarScene.ToLower()) || SceneManager.GetActiveScene().name.ToLower().Equals(Global.SplashScene.ToLower()))
        {
            var temp = Resources.Load(AssetsConfiguratorManager.jsonPath + "/URIandToken") as TextAsset;
            print(temp.ToString());

            CharactersOwnNftPayload charactersPayLoad = new CharactersOwnNftPayload();
            charactersPayLoad = JsonConvert.DeserializeObject<CharactersOwnNftPayload>(temp.ToString());
            LoadAssets(charactersPayLoad);
        }
#endif
    }

    public void GetWalletTokenForCharacters()
    {
        Debug.Log("GetWalletTokenForCharacters");
        DispatchEvent dispatchEvent = new DispatchEvent();
        dispatchEvent.eventType = DispatchEvents.GET_CHARACTERS.ToString();
        dispatchEvent.payload = null;

        var dispatchEventJson = Serialize.ToJson(dispatchEvent);
        Debug.Log("dispatchEventJson: " + dispatchEventJson);
        #if !UNITY_EDITOR
            PassTextParam(dispatchEventJson);
        #endif
    }


    public void UpdateCoinsEvent()
    {
        Debug.Log("UpdateCoinsEvent");
        DispatchEvent dispatchEvent = new DispatchEvent();
        dispatchEvent.eventType = DispatchEvents.UPDATE_COINS.ToString();
        dispatchEvent.payload = null;

        var dispatchEventJson = Serialize.ToJson(dispatchEvent);
        Debug.Log("dispatchEventJson: " + dispatchEventJson);
        #if !UNITY_EDITOR
             PassTextParam(dispatchEventJson);
        #endif
    }

    public void GetWalletTokenForNfs()
    {
        Debug.Log("GetWalletTokenForNfs");
        DispatchEvent dispatchEvent = new DispatchEvent();
        dispatchEvent.eventType = DispatchEvents.GET_OWN_NFTS.ToString();
        dispatchEvent.payload = null;

        var dispatchEventJson = Serialize.ToJson(dispatchEvent);
        Debug.Log("dispatchEventJson: " + dispatchEventJson);
        #if !UNITY_EDITOR
                     PassTextParam(dispatchEventJson);
        #endif
    }

    public void GoToHome()
    {
        Debug.Log("GoToHome");
        DispatchEvent dispatchEvent = new DispatchEvent();
        dispatchEvent.eventType = DispatchEvents.GO_HOME.ToString();
        dispatchEvent.payload = null;

        var dispatchEventJson = Serialize.ToJson(dispatchEvent);
        Debug.Log("dispatchEventJson: " + dispatchEventJson);
#if !UNITY_EDITOR
                     PassTextParam(dispatchEventJson);
#endif
    }


    public void GoToInventory()
    {
        Debug.Log("GoToInventory");
        DispatchEvent dispatchEvent = new DispatchEvent();
        dispatchEvent.eventType = DispatchEvents.GO_TO_INVENTORY.ToString();
        dispatchEvent.payload = null;

        var dispatchEventJson = Serialize.ToJson(dispatchEvent);
        Debug.Log("dispatchEventJson: " + dispatchEventJson);
#if !UNITY_EDITOR
                     PassTextParam(dispatchEventJson);
#endif
    }



    /// <summary>
    /// Calling this function from Web
    /// </summary>
    /// <param name="jsonRespose"></param>
    public void MessageForUnity(string jsonRespose)
    {
        Debug.Log("MessageForUnity: " + jsonRespose);

        DispatchEventObject dispatchEvent = DispatchEventObject.FromJson(jsonRespose);
        string eventType = dispatchEvent.EventType;
        Debug.Log("Event Type " + eventType);
        Debug.Log("dispatchEvent.Payload" + dispatchEvent.Payload);
        switch (eventType)
        {
            //case "MINT":
            //    Debug.Log(payload.message);
            //    break;


            case "GET_CHARACTERS":
                GetCharactersPayLoad charactersNftPayload = JsonConvert.DeserializeObject<GetCharactersPayLoad>(dispatchEvent.Payload);
                LoadCharacters(charactersNftPayload);
                              
                break;

            case "GET_OWN_NFTS":
                CharactersOwnNftPayload charactersOwnNftAssetsPayload = JsonConvert.DeserializeObject<CharactersOwnNftPayload>(dispatchEvent.Payload);
                LoadAssets(charactersOwnNftAssetsPayload);
                break;

            //case "GET_MATCHTYPE":
            //    GetMatchType matchDetails = GetMatchType.FromJson(dispatchEvent.Payload);
            //    SplashScreen.GetMatchTypeForGameCallBack(matchDetails);
            //    break;

            default:
                Debug.LogError("EventType Not Found");
                break;
        }
    }

    public void OnFail(string obj)
    {
        Debug.LogError("On Fail+++++++: "+obj);
    }

    public void OnSuccessfullyTokenDownloaded(JObject arg1, long arg2, NFTUriAndToken nftUriAndToken)
    {
        if (ResponseStatus.Check(arg2))
        {
            Debug.Log("OnSuccessfullyTokenDownloaded"+ arg1.ToString());

            MintData mintData = MintData.FromJson(arg1.ToString());

            NFT nft = new NFT();
            nft.MintData = mintData;
            nft.Uri = nftUriAndToken.Uri;
            nft.TokenID = nftUriAndToken.TokenID;
            if (mintData.type.ToLower() == "avatar")
            {
                characterMintData.Add(nftUriAndToken.Uri, nft);
            }
        }
        else
        {
            Debug.Log("OnSuccessfullyTokenDownloaded Fail: "+ arg1.ToString());
        }
    }

    public void OnSuccessfullyDefaultTokenDownloaded(JObject arg1, long arg2)
    {
        if (ResponseStatus.Check(arg2))
        {
            Debug.Log("OnSuccessfullyTokenDownloaded"+ arg1.ToString());

            MintData mintData = MintData.FromJson(arg1.ToString());
            defaultCharactersData.Add(mintData);
        }
        else
        {
            Debug.Log("OnSuccessfullyTokenDownloaded Fail: "+ arg1.ToString());

        }
    }

    void LoadCharacters(GetCharactersPayLoad payload)
    {        
//        Debug.Log("payload: " + payload.Message);
//        Debug.Log("payload: " + payload.data.Count);
        WaitingLoader.instance.ShowHide(true);
//        Debug.Log(payload.Message);
        characterMintData.Clear();
        defaultCharactersData.Clear();
        if (payload != null && payload.TokenUri.Count > 0)
        {
            haveTokensInWallet = true;
            foreach (var uri in payload.TokenUri)
            {
                NFTUriAndToken nft = new NFTUriAndToken();
                Debug.Log("tokenURi: " +uri.ToString());
                nft.Uri = uri.ToString();                
                GetRequest(nft, OnSuccessfullyTokenDownloaded, OnFail);
            }
        }
        else
        {
            haveTokensInWallet = false;
        }
        WebServiceManager.instance.APIRequest(defaultMaleTokenURL, Method.GET, null, null, OnSuccessfullyDefaultTokenDownloaded, OnFail, CACHEABLE.NULL, true, null);
        WebServiceManager.instance.APIRequest(defaultFemaleTokenURL, Method.GET, null, null, OnSuccessfullyDefaultTokenDownloaded, OnFail, CACHEABLE.NULL, true, null);
        WaitingLoader.instance.ShowHide(true);
        Invoke("Delay",2f);
    }

    void LoadAssets(CharactersOwnNftPayload payload)
    {
        WaitingLoader.instance.ShowHide(true);      
        assetsMintDataDict.Clear();
        characterMintData.Clear();

        haveTokensInWallet = false;

        if (payload != null && payload.data != null && payload.data.Count > 0)
        {

            foreach (NFTUriAndToken nft in payload.data)
            {
                //Debug.Log("AssetstokenURi: " + nft.Uri.ToString());
                GetRequest(nft, OnSuccessfullyAssetsTokenDownloaded, OnFail);
            }
        }
        Invoke("Delay", 2f);
    }

    public void OnTournamentMatchEnded()
    {
        Debug.Log("OnTournamentMatchEnded");
        DispatchEvent dispatchEvent = new DispatchEvent();
        dispatchEvent.eventType = DispatchEvents.TOURNAMENT_MATCH_ENDED.ToString();
        //dispatchEvent.payload = Global.tournamentID;

        TournamentDetails tournamentDetails = new TournamentDetails
        {
            tournamentID = Global.tournamentID
        };
        var payloadJson = Serialize.ToJson(tournamentDetails);
        dispatchEvent.payload = payloadJson;

        var dispatchEventJson = Serialize.ToJson(dispatchEvent);
        Debug.Log("dispatchEventJson: " + dispatchEventJson);
#if !UNITY_EDITOR
                     PassTextParam(dispatchEventJson);
#endif
    }


    public void OnSuccessfullyAssetsTokenDownloaded(JObject arg1, long arg2, NFTUriAndToken nftUriAndToken)
    {
        if (ResponseStatus.Check(arg2))
        {
            Debug.Log("OnSuccessfullyAssetsTokenDownloaded" + arg1.ToString());

            MintData mintData = MintData.FromJson(arg1.ToString());            

            NFT nft = new NFT();
            nft.MintData = mintData;
            
            nft.TokenID = nftUriAndToken.TokenID;
            nft.Uri = nftUriAndToken.Uri;

            if (mintData.type.ToLower() == "asset")
            {
                if (string.IsNullOrEmpty(mintData.Attributes.gender))
                {
                    Debug.LogError("Asset Found, but gender not exist.....");
                    return;
                }
                assetsMintDataDict.Add(nftUriAndToken.TokenID, nft);
            }
            else
            {
                Debug.Log("Token Avatar Found.");
                haveTokensInWallet = true; //Hunain 17 Mar 2023
                characterMintData.Add(nftUriAndToken.TokenID, nft);
            }
            
        }
        else
        {
            Debug.Log("OnSuccessfullyAssetsTokenDownloaded Fail: " + arg1.ToString());

        }        
    }

    private void Delay()
    {
        characterBlendDownloaded = true;
        WaitingLoader.instance.ShowHide(false);
        Debug.Log("Delay characterMintData.Count: " + characterMintData.Count);

    }

    public void GetRequest(NFTUriAndToken nft, Action<JObject, long, NFTUriAndToken> OnSuccess = null, Action<string> OnFail = null)
    {
        StartCoroutine(_GetRequest(nft, OnSuccess, OnFail));
    }

    public IEnumerator _GetRequest(NFTUriAndToken nft, Action< JObject, long, NFTUriAndToken> OnSuccess = null, Action<string> OnFail = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(nft.Uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = nft.Uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    OnFail?.Invoke("notDone");
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;

                case UnityWebRequest.Result.Success:
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    JObject data = new JObject();
                    data = JObject.Parse(webRequest.downloadHandler.text, Global.jsonLoadSettings);
                    OnSuccess?.Invoke(data, webRequest.responseCode, nft);
                    break;
            }
        }
    }

}




//string GameRule, double coinsToPlay = 0, int maxPlayers = 2 , string matchID = ""