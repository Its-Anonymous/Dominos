using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetBuilder
{
    public class ItemPricingPanel : MonoBehaviour
    {
        public RectTransform pricesObjectParent;
        public List<UIItemPrice> uIItemPrices = new List<UIItemPrice>();
        public Text totalTextPrice;
        public double totalPrice;
        public static double totalPriceStatic;
        public double gasPrice;

        private void OnDisable()
        {
           //ResetPricingPanel();
        }

        public void SetPriceAsPerCategories(Item item = null)
        {
            if (item == null)
                return;
            foreach (var listObject in uIItemPrices)
            {
                if (listObject.itemCategory.ToString() == item.itemType)
                {
                    if (item.itemShortCode.ToLower().Contains("default"))
                    {
                        item.itemPrice.usdt = 0;
                    }
                    listObject.SetData(item.itemPrice);
                    MultiplierGameObjct(listObject);
                }
                totalPrice += listObject.itemPrice.usdt;
            }
            totalPriceStatic = totalPrice;
            totalTextPrice.text = totalPrice.ToString() + " USDT";
            AssetsScreenManager.instance.assetPreviewScreen.mintBtn.interactable = totalPriceStatic > 0;
            AssetsScreenManager.instance.MintAssetsPopUpScreen.priceVariable = totalPriceStatic;
            AssetsScreenManager.instance.MintAssetsPopUpScreen.gasPriceVariable = totalPriceStatic;
            totalPrice = 0;
        }

        public void ClearPriceItemUI()
        {
            foreach (var item in uIItemPrices)
            {
                item.itemPrice = new ItemPrice();
                item.gameObject.SetActive(false);
                totalTextPrice.text = "0 USDT";
                totalPrice = 0;
            }
            totalPriceStatic = 0;
        }

        public void MultiplierGameObjct(UIItemPrice uIItem)
        {
            RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();

            if (!uIItem.gameObject.activeInHierarchy && uIItem.itemPrice.usdt != 0)
            {
                rectTransform.sizeDelta = new Vector3(rectTransform.rect.width, rectTransform.rect.height + 35);
                pricesObjectParent.sizeDelta = new Vector2(pricesObjectParent.rect.width, pricesObjectParent.rect.height + 25);
                uIItem.GameObjectEnableDisable(true);
                Debug.Log(uIItem.gameObject, gameObject);
                print("-------------");

            }
            else if (uIItem.gameObject.activeInHierarchy && uIItem.itemPrice.usdt == 0)
            {
                rectTransform.sizeDelta = new Vector3(rectTransform.rect.width, rectTransform.rect.height - 35);
                pricesObjectParent.sizeDelta = new Vector2(pricesObjectParent.rect.width, pricesObjectParent.rect.height - 25);
                uIItem.GameObjectEnableDisable(false);
                Debug.Log(uIItem.gameObject, gameObject);
                print("+++++++++++++++");
            }
         

        }

        public void ResetPricingPanel()
        {
            RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector3(rectTransform.rect.width, 300);
            pricesObjectParent.sizeDelta = new Vector2(pricesObjectParent.rect.width, 181);
            ClearPriceItemUI();
        }

    }
}