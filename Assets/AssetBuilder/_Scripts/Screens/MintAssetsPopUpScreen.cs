using System.Collections.Generic;
using System.Linq;
using AvatarBuilder;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using static AssetBuilder.CamerControllerforConfigurator;

namespace AssetBuilder
{
    public class MintAssetsPopUpScreen : MonoBehaviour
    {
        [Header("PopUp Items")]
        public GameObject discountRow;
        public Button noBtn;
        public Button closeBtn;
        public Button yesBtn;
        [SerializeField] List<Camera> redererCamera = new List<Camera>();

        [Header("Item Prices")]
        public Text priceTxt;
        public Text totalPriceTxt;
        public Text dicountTxt;
        public double priceVariable;
        public double discountPriceVariable;
        public double gasPriceVariable = 2;

        private void OnEnable()
        {
            totalPriceTxt.text = priceTxt.text = priceVariable.ToString();
            discountRow.SetActive(true);

            if (PlayerPersonalData.playerWhiteListed && PlayerPersonalData.discount > 0)
            {
                Debug.Log("Yes discount is available.. Discount = " + PlayerPersonalData.discount);

                double originalPrice = priceVariable;

                discountPriceVariable = originalPrice - (originalPrice * PlayerPersonalData.discount / 100);

                dicountTxt.text = PlayerPersonalData.discount.ToString();
                totalPriceTxt.text = discountPriceVariable.ToString("N2");
            }
            else
            {
                Debug.Log("Discount is NOT available.. ");
                discountRow.SetActive(false);
                dicountTxt.text = "0";
            }


            totalPriceTxt.text += " + GAS FEES"; 
        }

        void Start()
        {
            noBtn.onClick.AddListener(() => ClosePopUpDelegate());
            yesBtn.onClick.AddListener(() => MintAvatarChanges());
            closeBtn.onClick.AddListener(() => ClosePopUpDelegate());
        }

        public void MintAvatarChanges()
        {
            AssetsPreviewScreen.instance.SaveUserSelectedAssetDataJson();
            AssetsConfiguratorManager.instance.SaveUserSelectedAssetDataJson(AssetsPreviewScreen.instance.saveUserAssets);

            WaitingLoader.instance.ShowHide(true);
            AssetsConfiguratorManager.instance.assetNftCache.Clear();
            AssetsConfiguratorManager.instance.cacheAssetCategoryCode.Clear();
            AssetsConfiguratorManager.instance.cacheAssetItemShortCode.Clear();
            AssetsConfiguratorManager.instance.cachePriceList.Clear();
            AssetsConfiguratorManager.instance.cacheItemClasses.Clear();


            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);
            foreach (var item in AssetsScreenManager.instance.assetPreviewScreen.saveUserAssets.assetCategories)
            {
                if (item.item[0].itemPrice.usdt > 0)
                {                                                   
                   AssetsConfiguratorManager.instance.assetNftCache.Add(item.shortCode, MintHandler.AssetsMintedObjects(item));
                }
                else
                {
                    Debug.Log($"Item {item.name} is for 0 usdt and this cannot able to mint");
                }
            }

           MintHandler.SendDataToS3(AssetsConfiguratorManager.instance.assetNftCache);
        }

        public void ClosePopUpDelegate()
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

            AssetsScreenManager.OpenClosePopUp(gameObject, false);
            WaitingLoader.instance.ShowHide(false);

        }

    }
}