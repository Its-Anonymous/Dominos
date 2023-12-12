using System.Runtime.InteropServices;
using AvatarBuilder;
using UnityEngine;
using UnityEngine.UI;

public class QuitAppPopUpScreen : MonoBehaviour
{
    // Importing "PassTextParam"
    [DllImport("__Internal")]
    public static extern void PassTextParam(string text);

    [Header("PopUp Items")]
    public Button noBtn;
    public Button closeBtn;
    public Button yesBtn;

    void Start()
    {
        noBtn.onClick.AddListener(() => ClosePopUpDelegate());
        yesBtn.onClick.AddListener(() => OnConfirmedQuitApp());
        closeBtn.onClick.AddListener(() => ClosePopUpDelegate());
    }

    private void OnConfirmedQuitApp()
    {
        DispatchEvent dispatchEvent = new DispatchEvent();
        dispatchEvent.eventType = DispatchEvents.EXIT_GAME.ToString();
        dispatchEvent.payload = null;

        var dispatchEventJson = Serialize.ToJson(dispatchEvent);
        Debug.Log("dispatchEventJson: " + dispatchEventJson);
#if !UNITY_EDITOR
                     PassTextParam(dispatchEventJson);
#endif
        ClosePopUpDelegate();
    }

    internal void ClosePopUpDelegate()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

        AvatarScreenManager.OpenClosePopUp(gameObject, false);
    }

    /// <summary>
    /// Calling this function from OnClick Event in Inspector
    /// </summary>
    public void OpenPopUp()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

        AvatarScreenManager.OpenClosePopUp(gameObject, true);
    }
}
