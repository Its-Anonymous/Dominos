using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AssetBuilder;
using AvatarBuilder;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ErrorPopup : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void PassTextParam(string text);

    public static ErrorPopup instance;
    public GameObject panel;
    public Text messageText;
    public Button btnOk;
    private bool reload;

    private System.Action callback;

    // Start is called before the first frame update
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
    }

    void Start()
    {       
        btnOk.onClick.AddListener(() => OkButton());
        panel.SetActive(false);
    }


    public void ShowMessage(string message, bool reload = false, System.Action action = null)
    {
        this.reload = reload;
        this.callback = action;
        AssetsScreenManager.OpenClosePopUp(panel, true);
        messageText.text = message;
    }

    public void OkButton()
    {

        //#if !UNITY_EDITOR
        //        AssetsScreenManager.OpenClosePopUp(panel, false);
        //        DispatchEvent dispatchEvent = new DispatchEvent();
        //        dispatchEvent.EventType = DispatchEvents.EXIT_GAME.ToString();
        //        dispatchEvent.Payload = null;

        //        var dispatchEventJson = Serialize.ToJson(dispatchEvent);
        //        Debug.Log("dispatchEventJson: " + dispatchEventJson);
        //        PassTextParam(dispatchEventJson);
        //#else
        this.panel.SetActive(false);
        if (!this.reload)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (this.callback != null)
        {
            this.callback();
        }

        //AssetsConfiguratorManager.instance.GetUserSaveDataAssets(WebServiceManager.instance.getUserAssetsCategories);
//#endif
    }

}
