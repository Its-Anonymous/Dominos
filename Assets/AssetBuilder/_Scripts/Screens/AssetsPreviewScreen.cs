using System.Collections.Generic;
using System.Linq;
using AvatarBuilder;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssetBuilder
{
    public class AssetsPreviewScreen : MonoBehaviour
    {
        [SerializeField] private Button createNewBtn;
        [SerializeField] private Button editShirtBtn;
        [SerializeField] private Button editPantBtn;
        [SerializeField] private Button editSocksBtn;
        [SerializeField] private Button editShoesBtn;
        [SerializeField] private Button editCapBtn;
        [SerializeField] private Button editGlassesBtn;
        [SerializeField] private Button editJewelleryBtn;
        [SerializeField] private Button editTattooBtn;
        [SerializeField] public Button mintBtn;
        [SerializeField] private Button SaveBtn;

        public SaveUserAssetsData saveUserAssets;
        public MintedSaveUserAssetsData mintedSaveUserAssets;

        internal static bool isAssetPreviewScreenEnabled = false;

        public static AssetsPreviewScreen instance;

        private void Awake()
        {
            instance = this;
        }
        private void OnEnable()
        {
            isAssetPreviewScreenEnabled = true;
            CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.screenCenterView);

            AssetsScreenManager.instance.assetPreviewScreen.mintBtn.interactable = ItemPricingPanel.totalPriceStatic > 0;
            AssetsScreenManager.instance.MintAssetsPopUpScreen.priceVariable =     ItemPricingPanel.totalPriceStatic;
            AssetsScreenManager.instance.MintAssetsPopUpScreen.gasPriceVariable = ItemPricingPanel.totalPriceStatic ;

        }

        // Start is called before the first frame update
        void Start()
        {
            editShirtBtn.onClick.AddListener(() => EditAvatarEvent(AvatarAssetsCateogeries.upper));
            editPantBtn.onClick.AddListener(() => EditAvatarEvent(AvatarAssetsCateogeries.lower));
            editSocksBtn.onClick.AddListener(() => EditAvatarEvent(AvatarAssetsCateogeries.socks));
            editShoesBtn.onClick.AddListener(() => EditAvatarEvent(AvatarAssetsCateogeries.shoes));
            editCapBtn.onClick.AddListener(() => EditAvatarEvent(AvatarAssetsCateogeries.cap));
            editGlassesBtn.onClick.AddListener(() => EditAvatarEvent(AvatarAssetsCateogeries.glasses));
            editJewelleryBtn.onClick.AddListener(() => EditAvatarEvent(AvatarAssetsCateogeries.jewellery));
            editTattooBtn.onClick.AddListener(() => EditAvatarEvent(AvatarAssetsCateogeries.tattoo));

            createNewBtn.onClick.AddListener(() => DiscardAvatarEvent());
            mintBtn.onClick.AddListener(() => MintAvatarEvent());
            SaveBtn.onClick.AddListener(() => SaveAvatarEvent());
        }

        private void DiscardAvatarEvent()
        {
            ObjectRotator.instance.ResetRotation();
            AssetsScreenManager.OpenClosePopUp(AssetsScreenManager.instance.discardAssetsChangesPopupScreen.gameObject, true);
        }

        private void SaveAvatarEvent()
        {
            AssetsScreenManager.OpenClosePopUp(AssetsScreenManager.instance.SaveAssetsPopUpScreen.gameObject, true);
        }

        private void MintAvatarEvent()
        {
            ObjectRotator.instance.ResetRotation();
            AssetsScreenManager.OpenClosePopUp(AssetsScreenManager.instance.MintAssetsPopUpScreen.gameObject, true);
        }

        private void EditAvatarEvent(AvatarAssetsCateogeries categoryType)
        {
            Debug.Log("EditAvatarEvent: " + categoryType);
            AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.assetCustomizeScreen.gameObject, this.gameObject);
            AssetsScreenManager.instance.categorySelectionPanel.categories.Find(x=> x.categoryType == categoryType).LoadThumbnailsDataByCategoryID();
        }

        public void SaveUserSelectedAssetDataJson()
        {
            //saveUserAssets.assetCategories.Clear();

            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                saveUserAssets.characterId = (AssetsConfiguratorManager.instance.gettingUserSaveAssetsData.data.characterId != "")
                    ? AssetsConfiguratorManager.instance.gettingUserSaveAssetsData.data.characterId : AvatarSelectionInventoryScreen.characterID; //Add Wallet Character ID

                Debug.Log("saveUserAssets.characterId: " + saveUserAssets.characterId);
                saveUserAssets.assetGender = AssetsConfiguratorManager.instance.gender.ToString();

                foreach (var item in AssetsScreenManager.instance.categorySelectionPanel.categories)
                {
                    MintAttribute assetCategory = new MintAttribute();

                    assetCategory._id = item.categoryResponseData._id;
                    assetCategory.shortCode = item.categoryResponseData.shortCode.ToLower();

                    Item itemthumbnail = new Item();
                    if (item.itemSelectionPanel.currentSelectedItem != null)
                    {
                        itemthumbnail = item.itemSelectionPanel.currentSelectedItem.item;
                        itemthumbnail.itemShortCode = item.itemSelectionPanel.currentSelectedItem.item.itemShortCode;

                        assetCategory.item = new List<Item>() { itemthumbnail };

                        if (item.categoryType == AvatarAssetsCateogeries.tattoo && ItemThumbnail.tattoo != null)
                        {
                            assetCategory.item[0].itemName = ItemThumbnail.tattoo.name.ToLower();
                            assetCategory.item[0].itemShortCode = ItemThumbnail.tattoo.name.ToLower();
                        }

                        var abc = item.itemSelectionPanel.currentSelectedItem.item.itemTexUrls.Find(obj => obj.texId.Equals(item.itemSelectionPanel.currentSelectedItem.currentSelectedVariantId));

                        assetCategory.item[0].itemTexUrls = abc == null ? new List<ItemTexUrl>() : new List<ItemTexUrl>() { abc };

                        //assetCategory.item[0].itemTexUrls  =  assetCategory.item[0].itemTexUrls != null ? assetCategory.item[0].itemTexUrls : new List<ItemTexUrl>();

                        if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
                        {
                            bool _dataAdded = false;
                            for (int i = 0; i < saveUserAssets.assetCategories.Count; i++)
                            {
                                if (item.categoryResponseData.shortCode == saveUserAssets.assetCategories[i].shortCode)
                                {
                                    _dataAdded = true;
                                    Debug.Log("Saved: " + item.categoryResponseData.shortCode);
                                    saveUserAssets.assetCategories[i] = assetCategory;
                                    break;
                                }
                            }

                            if (!_dataAdded)
                                saveUserAssets.assetCategories.Add(assetCategory);
                        }
                    }
                }
            }

            if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
            {
                mintedSaveUserAssets.mintedCharacter        = AssetsConfiguratorManager.instance.selectedAvatar;
                mintedSaveUserAssets.mintedAvatarName    = AssetsConfiguratorManager.instance.selectedAvatarName;
                mintedSaveUserAssets.mintedAvatarClass   = AssetsConfiguratorManager.instance.selectedAvatarClass;
                mintedSaveUserAssets.mintedAvatarImageUrl   = AssetsConfiguratorManager.instance.selectedAvatarImage;

                Debug.Log("saveUserAssets.character: " + mintedSaveUserAssets.mintedCharacter);
                mintedSaveUserAssets.mintedGender = AssetsConfiguratorManager.instance.gender.ToString();

                List<NFTUriAndToken> nftUriAndTokens = new List<NFTUriAndToken>();
                foreach (var item in AssetsScreenManager.instance.SelectionPanelList)
                {
                    foreach (var dict in JS_Hook.instance.assetsMintDataDict)
                    {
                        NFT nft = dict.Value;

                            Debug.Log("currentSelectedItem");
                        if (item.currentSelectedItem == null)
                        {
                            if (item.name.ToLower().Contains(nft.MintData.itemCategory))
                            {
                                Debug.Log("nft.MintData.itemCategory" + nft.MintData.itemCategory);

                                foreach (var mintItem in AssetsConfiguratorManager.instance.gettingMintedSaveUserData.data.mintedInventory) 
                                {
                                    Debug.Log("mintItem" + mintItem.TokenID + "nft.TokenID" + nft.TokenID);
                                    if (mintItem.TokenID == nft.TokenID)
                                    {
                                        NFTUriAndToken nftUriAndToken = new NFTUriAndToken();
                                        nftUriAndToken.TokenID = nft.TokenID;
                                        nftUriAndToken.Uri = nft.Uri;
                                        nftUriAndTokens.Add(nftUriAndToken);
                                    }
                                }
                            }
                            continue;
                        }
                        if (item.currentSelectedItem.item.itemIcon == nft.MintData.imageS3URL)
                        {
                            NFTUriAndToken nftUriAndToken = new NFTUriAndToken();
                            nftUriAndToken.TokenID = nft.TokenID;
                            nftUriAndToken.Uri = nft.Uri;

                            nftUriAndTokens.Add(nftUriAndToken);
                            break;
                        }
                    }
                }
                mintedSaveUserAssets.mintedInventory = nftUriAndTokens;
                string rawData = JsonUtility.ToJson(nftUriAndTokens);// string.Join(",", nftUriAndTokens);
                AssetsConfiguratorManager.instance.saveSelectedUserJson = rawData;
            }
        }
    }
}