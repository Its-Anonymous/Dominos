using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

namespace AssetBuilder
{
    public class SaveAssetsPopUpScreen : MonoBehaviour
    {
        [Header("PopUp Items")]
        public Button noBtn;
        public Button closeBtn;
        public Button yesBtn;

        void Start()
        {
            noBtn.onClick.AddListener(() => ClosePopUpDelegate());
            yesBtn.onClick.AddListener(() => SaveChanges());
            closeBtn.onClick.AddListener(() => ClosePopUpDelegate());
        }

        public void SaveChanges()
        {
            AssetsPreviewScreen.instance.SaveUserSelectedAssetDataJson();
            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                AssetsConfiguratorManager.instance.SaveUserSelectedAssetDataJson(AssetsPreviewScreen.instance.saveUserAssets);
                AssetsConfiguratorManager.instance.SetUserAssetCategories(WebServiceManager.instance.setUserAssetsCategories, AssetsConfiguratorManager.instance.saveSelectedUserJson);
            }
            if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
            {
                AssetsConfiguratorManager.instance.MintedSaveUserSelectedAssetDataJson(AssetsPreviewScreen.instance.mintedSaveUserAssets);
                //SaveScreenShot();
                AssetsConfiguratorManager.instance.SetUserAssetCategories(WebServiceManager.instance.setMintedUserAssetsCategories, AssetsConfiguratorManager.instance.saveSelectedUserJson, JS_Hook.instance.GoToHome);

            }
            
            AssetsScreenManager.OpenClosePopUp(gameObject, false);

        }

        private void ClosePopUpDelegate()
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

            AssetsScreenManager.OpenClosePopUp(gameObject, false);
        }

        void SaveScreenShot()
        {
            RenderTexture screenTexture = new RenderTexture(1024, 1024, 32);
            CamerControllerforConfigurator.instance.avatarScreenCamera.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            CamerControllerforConfigurator.instance.avatarScreenCamera.Render();
            Texture2D renderedTexture = new Texture2D(1024, 1024);
            renderedTexture.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
            RenderTexture.active = null;
            byte[] byteArray = renderedTexture.EncodeToPNG();          
            UploadScreenShot_S3(byteArray);
        }

        public void UploadScreenShot_S3(byte[] imageByte)
        {           
            FileUplaod fileUplaod = new FileUplaod();
            fileUplaod.key = "file";
            fileUplaod.data = imageByte;
            Debug.Log("fileUplaod.data.Length: " + fileUplaod.data.Length);           
        }

    }
}