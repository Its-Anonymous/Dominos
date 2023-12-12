using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssetBuilder
{
    public class DiscardAssetsChangesPopupScreen : MonoBehaviour
    {
        [Header("PopUp Items")]
        public Button noBtn;
        public Button closeBtn;
        public Button yesBtn;

        void Start()
        {
            noBtn.onClick.AddListener(() => NoButton());
            yesBtn.onClick.AddListener(() => DiscardChanges());
            closeBtn.onClick.AddListener(() => ClosePopUpDelegate());
        }

        public void DiscardChanges()
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);
            Debug.Log("Discard Changes");
            AssetsScreenManager.instance.ResetAllUI();
            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                if (AssetsScreenManager.instance.assetPreviewScreen.gameObject.activeInHierarchy)
                {
                    AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.genderSelectionScreen.gameObject, AssetsScreenManager.instance.assetPreviewScreen.gameObject);
                }
                else
                {
                    AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.genderSelectionScreen.gameObject, AssetsScreenManager.instance.assetCustomizeScreen.gameObject);
                    AssetsScreenManager.instance.DisableAllItemThumbnailSelectionPanels();
                }
            }

            if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
            {
                if (AssetsScreenManager.instance.assetPreviewScreen.gameObject.activeInHierarchy)
                {
                    AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.avatarSelectionInventory.gameObject, AssetsScreenManager.instance.assetPreviewScreen.gameObject);
                }
                else
                {
                    AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.avatarSelectionInventory.gameObject, AssetsScreenManager.instance.assetCustomizeScreen.gameObject);
                    AssetsScreenManager.instance.DisableAllItemThumbnailSelectionPanels();
                }
            }


            ClosePopUpDelegate();
            ObjectRotator.instance.ResetRotation();

        }

        public void NoButton()
        {
            AssetsScreenManager.OpenClosePopUp(this.gameObject, false);
        }

        private void ClosePopUpDelegate()
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);
            AssetsScreenManager.OpenClosePopUp(gameObject, false);          
        }
    }
}