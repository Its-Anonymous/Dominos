using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AssetBuilder
{
    public class AssetsScreenManager : MonoBehaviour
    {
        [Header("Assets Screens")]
        public GenderSelectionScreen genderSelectionScreen;
        public AssetsCustomizeScreen assetCustomizeScreen;
        public AvatarSelectionInventoryScreen avatarSelectionInventory;
        public AssetsPreviewScreen assetPreviewScreen;

        [Header("PopUps Screens")]
        public SaveAssetsPopUpScreen SaveAssetsPopUpScreen;
        public DiscardAssetsChangesPopupScreen discardAssetsChangesPopupScreen;
        public MintAssetsPopUpScreen MintAssetsPopUpScreen;

        [Header("UI Entity Prefabs")]
        public ItemThumbnail itemThumbnailPrefab;
        public VariantThumbnail variantThumbnailPrefab;

        [Header("Item Selection Panels")]
        public List<ItemSelectionPanel> SelectionPanelList;

        [Header("Classes Sprite")]
        public List<Sprite> classSprites;

        [Space]
        public CategorySelectionPanel categorySelectionPanel;
        public ItemPricingPanel itemPricingPanel;

        public GetCatagories getCatagories;

        public static AssetsScreenManager instance;

        public void Awake()
        {
            instance = this;
        }

        public void ResetAssetConfiguratorData()
        {
            DisableAllItemThumbnailSelectionPanels();
            ResetSelectedItems();
        }

        public void DisableAllItemThumbnailSelectionPanels()
        {
            foreach (var item in SelectionPanelList)
            {
                item.EnableDisablePanel(false);                
            }
        }

        private void ResetSelectedItems()
        {
            foreach (var item in SelectionPanelList)
            {
                item.ChangeSelectableItemThumbnail();
                item.ChangeSelectableVariantThumbnail();
            }

            categorySelectionPanel.ChangeSelectableIcon();
        }

        public static void ChangeScreen(GameObject nextScreen, GameObject currentScreen)
        {
            instance.StartCoroutine(_ChangeScreenCoroutine(nextScreen, currentScreen));
        }

        public static IEnumerator _ChangeScreenCoroutine(GameObject nextScreen, GameObject currentScreen)
        {
            yield return new WaitForSeconds(0.1f);

            nextScreen.SetActive(true);
            currentScreen.SetActive(false);
        }

        public static void OpenClosePopUp(GameObject panel, bool doOpen)
        {
            if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

            if (doOpen)
            {
                panel.SetActive(true);
                LeanTween.scale(panel, Vector3.one, .5f).setEaseOutBack();
            }
            else
            {
                LeanTween.scale(panel, Vector3.zero, .5f).setEaseInBack().setOnComplete(() => panel.SetActive(false));
            }
        }

        public void SetPriceAsPerCategoriesOnUI(Item item)
        {
            itemPricingPanel.SetPriceAsPerCategories(item);
        }

        public void ResetAllUI()
        {

            AssetsConfiguratorManager.instance.MakeDefualtCharacter();
            itemPricingPanel.ResetPricingPanel();
            ClearItemThumnail();
            foreach (var item in SelectionPanelList)
            {
                item.ClassFilter(ItemSelectionPanel.Classes.all, item.class_Reset_Btn);
            }           
        }

        public void ClearItemThumnail()
        {   
            foreach (var selectionPanel in SelectionPanelList)
            {
                foreach (var itemThumnail in selectionPanel.itemThumbnails)
                {
                    Destroy(itemThumnail.gameObject);
                    if(itemThumnail.currentSelectedVariant != null)
                    {
                        Destroy(itemThumnail.currentSelectedVariant.gameObject);
                    }
                }
                selectionPanel.itemThumbnails.Clear();
            }
            foreach (var item in categorySelectionPanel.categories)
            {
                item.categoryResponseData = null;
                item.itemsData = new ResponseItemData();                
            }
            categorySelectionPanel.ChangeSelectableIcon(null);
        }

        #region CallBack Events
        internal void OnFailGetAvatarSpecs(string obj)
        {
            Debug.LogError(obj.ToString());
        }

        internal void OnSuccessGetAvatarSpecs(JObject response, long onSuccess)
        {
        }
        #endregion

    }
}