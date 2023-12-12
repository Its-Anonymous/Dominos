using System;
using System.Collections.Generic;
using AvatarBuilder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssetBuilder
{
    public class CategoryThumbnail : MonoBehaviour
    {
        public GameObject selectedObj;
        public GameObject itemThumbnail;
        public GameObject varientThumbnail;
        public ItemSelectionPanel itemSelectionPanel;

        public AvatarAssetsCateogeries categoryType;
        [SerializeField] internal AssetCategoryResponseData categoryResponseData;
        [SerializeField] internal ResponseItemData itemsData;


        public Button btn;

        private void Start()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(() => LoadThumbnailsDataByCategoryID());
        }

        public void EnableDisableSelectable(bool value) { selectedObj.SetActive(value); }

        public void LoadThumbnailsDataByCategoryID()
        {

            AssetsScreenManager.instance.categorySelectionPanel.ChangeSelectableIcon(this);

            if (!itemSelectionPanel.gameObject.activeInHierarchy)
            {
                AssetsScreenManager.instance.DisableAllItemThumbnailSelectionPanels();
                itemSelectionPanel.EnableDisablePanel(true);
            }
            else
            {
                AssetsScreenManager.instance.categorySelectionPanel.ChangeSelectableIcon();
                AssetsScreenManager.instance.DisableAllItemThumbnailSelectionPanels();
            }

            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                if (this.itemsData.data._id == "" || this.itemsData.data == null)// && this.categoryResponseData.shortCode != "" )
                {
                    print("Empty Data");
                    AssetsConfiguratorManager.instance.GetAssetBuilderItemsByCategoryID(WebServiceManager.instance.GetAssetBuilderItemsByCategoryID + "/" + this.categoryResponseData._id + "/items", this);
                }
            }
            else
            {
                if(itemSelectionPanel.contentTransform.childCount == 0)
                {
                    SpawnMintedThumbnail();
                }  
            }

            if (this.categoryType == AvatarAssetsCateogeries.jewellery || this.categoryType == AvatarAssetsCateogeries.tattoo)
            {
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.screenCenterView);
                print(this.categoryResponseData.shortCode);
            }
            else
            {
                SetCamera(this.categoryResponseData.shortCode);
                print(this.categoryResponseData.shortCode);
            }
        }

        public void SetCatagoryResponseData(List<CategoryThumbnail> categoryThumbnail)
        {          
            foreach (var item in AssetsConfiguratorManager.instance.allAssetsCategoriesData.data)
            {
                foreach (var item2 in categoryThumbnail)
                {
                    if (item.shortCode == item2.categoryType.ToString())
                    {
                        item2.categoryResponseData = item;
                    }
                }
            }
        }

        public void OnSuccessItemsByCategoryID(JObject data, long code)
        {
            if (ResponseStatus.Check(code))
            {
                Debug.Log("Data by Categroy: " + data);
                this.itemsData = JsonConvert.DeserializeObject<ResponseItemData>(data.ToString());
                if (this.itemsData.data.item != null)
                {
                    foreach (var item in itemsData.data.item)
                    {
                        ItemThumbnail itemthumbss = Instantiate(itemThumbnail, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();

                        itemSelectionPanel.itemThumbnails.Add(itemthumbss);
                        itemthumbss.itemSelectionPanel = itemSelectionPanel;
                        //itemthumbss.SetData(item);
                        itemthumbss.SetData(item, this.categoryResponseData);
                        if (this.itemSelectionPanel.currentSelectedItem == null)
                        {
                            if (AssetsConfiguratorManager.instance.gettingUserSaveAssetsData.data.assetCategories.Count != 0)
                            {
                                foreach (var thumbnailitem in AssetsConfiguratorManager.instance.gettingUserSaveAssetsData.data.assetCategories)
                                {
                                    if (thumbnailitem.shortCode.ToLower().Equals("tattoo"))
                                    {
                                        if (!string.IsNullOrEmpty(thumbnailitem.item[0].itemShortCode))
                                        {
                                            string[] itemNameArray = thumbnailitem.item[0].itemShortCode.Trim().Split('_');
                                            if (itemNameArray[itemNameArray.Length - 1].Contains("l") || itemNameArray[itemNameArray.Length - 1].Contains("r"))
                                            {
                                                thumbnailitem.item[0].itemShortCode = thumbnailitem.item[0].itemShortCode.Remove(thumbnailitem.item[0].itemShortCode.Length - 1, 1);
                                                thumbnailitem.item[0].itemName = thumbnailitem.item[0].itemName.Remove(thumbnailitem.item[0].itemName.Length - 1, 1);

                                            }
                                        }
                                    }
                                    if (thumbnailitem.item[0].itemShortCode.ToLower() == itemthumbss.item.itemShortCode.ToLower())
                                    {
                                        itemSelectionPanel.ChangeSelectableItemThumbnail(itemthumbss);
                                        //if (thumbnailitem.item[0].itemTexUrls.Count == 0)
                                        //    return;
                                        if (thumbnailitem.item[0].itemTexUrls.Count > 0) itemthumbss.currentSelectedVariantId = thumbnailitem.item[0].itemTexUrls[0].texId;
                                    }
                                }
                            }
                            else
                            {
                                itemSelectionPanel.ChangeSelectableItemThumbnail(itemthumbss);
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError(code);
            }
        }

        public void SetCamera(string shortcode)
        {
            if (shortcode == "upper")
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.upperView);
            else if (shortcode == "lower")
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.lowerView);
            else if (shortcode == "socks")
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.socksView);
            else if (shortcode == "shoes")
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.shoesView);
            else if (shortcode == "cap")
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.capView);
            else if (shortcode == "glasses")
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.glassesView);
            else
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.fullBodyView);
            if (itemSelectionPanel.currentSelectedItem == null)
                return;
            else if (itemSelectionPanel.currentSelectedItem.currentSelectedVariant.variantThumb.texture == null)
            {
                itemSelectionPanel.currentSelectedItem.currentSelectedVariant.TextureDownload();
                print("Texture Downloading from Category");
            }
        }

        public void SpawnMintedThumbnail()
        {
            //if (itemsData.data.item == null)
            //    return;
            foreach (var item in itemsData.data.item)
            {
                ItemThumbnail itemthumbss = Instantiate(itemThumbnail, itemSelectionPanel.contentTransform).GetComponent<ItemThumbnail>();
                itemthumbss.itemSelectionPanel = itemSelectionPanel;
                itemSelectionPanel.itemThumbnails.Add(itemthumbss);
                itemthumbss.SetData(item, null);
            }
        }

    }
}