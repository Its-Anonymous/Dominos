using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AvatarBuilder;
using Dominos;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WebglUserSession : MonoBehaviour
{
    public GameObject userSessionFailedPopUp;
    public static WebglUserSession instance;
    public static bool userLoggedIn;

    public static Config config;

    private void Awake()
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


        StartCoroutine(LoadLocalizedTextOnAndroid("config.json"));

        //For UNITY_EDITOR and LocalHost Testing.
        //if (string.IsNullOrEmpty(Global.GetBearerToken))
        //{ Global.GetBearerToken = Global.testToken; }


        //For Production/Live Environment.
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(Global.GetBearerToken))
               Global.GetBearerToken = Global.testToken;
#else
            URLParameters.GetSearchParameters().TryGetValue("accessToken", out Global.testToken);
            URLParameters.GetSearchParameters().TryGetValue("userid", out PlayerPersonalData.playerUserID);
            URLParameters.GetSearchParameters().TryGetValue("tournamentId", out Global.tournamentID);
            URLParameters.GetSearchParameters().TryGetValue("gameType", out Global.gameType);
            URLParameters.GetSearchParameters().TryGetValue("network", out Global.network);
            Global.GetBearerToken = Global.testToken;
#endif
    }


    public static IEnumerator LoadLocalizedTextOnAndroid(string fileName)
    {
        string filePath;
        filePath = Path.Combine(Application.streamingAssetsPath + "/", fileName);
        string dataAsJson;
        if (filePath.Contains("://") || filePath.Contains (":///"))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            dataAsJson = www.downloadHandler.text;
        }
        else
        {
            dataAsJson = File.ReadAllText(filePath);
        }
        Debug.Log("config: " + dataAsJson);
        config = Dominos.Config.FromJson(dataAsJson); //JsonUtility.FromJson<Config>(dataAsJson);
        WebServiceManager.baseURL = config.BaseUrl;
        NakamaConnection.Host = config.NakamaHost;
        NakamaConnection.Scheme = config.NakamaScheme;
        NakamaConnection.Port = config.NakamaPort;
        NakamaConnection.ServerKey = config.NakamaServerKey;

    }
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(Global.GetBearerToken) && !string.IsNullOrEmpty(WebServiceManager.baseURL));

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getPlayerProfile, Method.GET, null, null, PlayerPersonalData.OnSuccessfullyProfileDownload, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
    }
}
