using System;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarBuilder
{
    public class MintAvatarPopUpScreen : MonoBehaviour
    {
        [Header("PopUp Items")]
        public Button noBtn;
        public Button closeBtn;
        public Button yesBtn;
        public GameObject discountRow;

        public Camera redererCamera;

        public Text priceTxt;
        public Text dicountTxt;
        public Text totalPriceTxt;

        void Start()
        {
            noBtn.onClick.AddListener(() => ClosePopUpDelegate());
            yesBtn.onClick.AddListener(() => OnConfirmedAvatarMinting());
            closeBtn.onClick.AddListener(() => ClosePopUpDelegate());
        }

        private void OnEnable()
        {
            SetData();
        }

        private void SetData()
        {
            totalPriceTxt.text = priceTxt.text = MintHandler.currentSelectedAvatarPrice;

            if (PlayerPersonalData.playerWhiteListed && PlayerPersonalData.discount > 0)
            {
                Debug.Log("Yes discount is available.. Discount = "  + PlayerPersonalData.discount);

                double originalPrice = double.Parse(MintHandler.currentSelectedAvatarPrice);
                double afterDiscount = originalPrice - (originalPrice * PlayerPersonalData.discount / 100);
                dicountTxt.text = PlayerPersonalData.discount.ToString();
                totalPriceTxt.text = afterDiscount.ToString("N2");
            }
            else
            {
                Debug.Log("Discount is NOT available.. ");
                discountRow.SetActive(false);
                dicountTxt.text = "0";
            }
            totalPriceTxt.text += " + GAS FEES";

        }

        public void OnConfirmedAvatarMinting()
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);
            Debug.Log("Mint Changes");
            SaveScreenShot();
        }

        void SaveScreenShot()
        {
            RenderTexture screenTexture = new RenderTexture(1024, 1024, 32);
            redererCamera.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            redererCamera.Render();
            Texture2D renderedTexture = new Texture2D(1024, 1024);
            renderedTexture.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
            RenderTexture.active = null;
            //50 /*Quality (1,100)*/
            byte[] byteArray = renderedTexture.EncodeToPNG();
            //System.IO.File.WriteAllBytes(Application.dataPath + "/cameracapture.png", byteArray);

            MintHandler.UploadScreenShot_S3(byteArray);
        }

        internal void ClosePopUpDelegate()
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

            AvatarScreenManager.OpenClosePopUp(gameObject, false);
        }
    }
}