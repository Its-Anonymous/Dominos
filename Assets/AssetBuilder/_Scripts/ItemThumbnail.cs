using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AssetBuilder.ItemSelectionPanel;

namespace AssetBuilder
{
    public class ItemThumbnail : MonoBehaviour
    {
        [SerializeField] internal AssetCategoryResponseData itemCateogery;
        [SerializeField] internal Item item;

        [SerializeField] VariantThumbnail variant;

        [SerializeField] List<VariantThumb> variantThumbnails = new List<VariantThumb>();

        [SerializeField] Image iconImage;
        [SerializeField] Image SelectedImage;

        [SerializeField] Text priceText;
        [SerializeField] Text usdtPriceText;
        public GameObject pricingParent;
        public Image classSprite;

        public static GameObject tattoo = null;

        [HideInInspector] public List<VariantThumbnail> variants = new List<VariantThumbnail>();
        [HideInInspector] public ItemSelectionPanel itemSelectionPanel;


        public VariantThumbnail currentSelectedVariant = null;
        public string currentSelectedVariantId = null;

        [SerializeField] bool variantDownload;

        public Image LoadingImage;
        protected Vector3 rotationEuler;


        void Start()
        {
            // *** Downloading Icon *** 
            AssetsConfiguratorManager.instance.ItemIconDownload(item.itemIcon, iconImage);

            // *** Downloading Variants' Textures/Icons *** 
           
            if (itemSelectionPanel.currentSelectedItem == this)
            {
                Debug.Log("working" + variantThumbnails.Count);
                SelectItem();                
            }

        }
        private void Update()
        {
            if (LoadingImage.gameObject.activeInHierarchy) //To save process if disable
            {
                rotationEuler += Vector3.forward * 50 * Time.deltaTime; //increment 30 degrees every second
                LoadingImage.transform.rotation = Quaternion.Euler(rotationEuler);
            }
        }

        public void EnableDisableSelectable(bool doEnable)
        {
            if (doEnable)
            {
                SelectedImage.gameObject.SetActive(true);                
            }
            else
            {
                SelectedImage.gameObject.SetActive(false);
                itemSelectionPanel.currentSelectedItem = null;
            }
        }

        public void SetData(Item item, AssetCategoryResponseData categoryResponseData = null)
        {
            this.item = item;
            this.itemCateogery = categoryResponseData;
            priceText.text = item.itemPrice.usdt.ToString();
            usdtPriceText.text = item.itemPrice.chainPrice.ToString();

            Classes itemClassIndex;

            Enum.TryParse<Classes>(this.item.itemClass, true, out itemClassIndex);

            classSprite.sprite = AssetsScreenManager.instance.classSprites[(int)itemClassIndex];

            if (item.itemShortCode.ToLower().Contains("default"))
            {
                classSprite.gameObject.SetActive(false);
                pricingParent.SetActive(false);
            }
        }

        public void LoadVariants(GameObject itemGameObject)
        {      
            ItemSelectionPanel itemSelectionPanel = GetComponentInParent<ItemSelectionPanel>();

            if (itemSelectionPanel.variantContentTransform != null)
            {
                AssetsConfiguratorManager.instance.ClearContent(itemSelectionPanel.variantContentTransform.gameObject);

                for (int i = 0; i < variantThumbnails.Count; i++)
                {
                    VariantThumbnail v = Instantiate(variant, itemSelectionPanel.variantContentTransform).GetComponent<VariantThumbnail>();
                    v.itemThumbnail = this;
                    v.SetVariantData(variantThumbnails[i], itemGameObject);
                    variants.Add(v);



                    if (currentSelectedVariantId != null || currentSelectedVariantId != "")
                    {
                        if (variantThumbnails[i].id == currentSelectedVariantId) // to show selectable image on selected variant
                        {
                            ChangeSelectableVariantThumbnail(v);
                        }
                    }
                    else
                        ChangeSelectableVariantThumbnail(variants[0]);
                }

                //if(currentSelectedVariant == null)
                //    ChangeSelectableVariantThumbnail(variants[0]);

                if (currentSelectedVariantId == null || currentSelectedVariantId == "")
                {
                    ChangeSelectableVariantThumbnail(variants[0]);                   
                    AssetsConfiguratorManager.instance.TextureDownload(this.item.itemTexUrls[0], variantThumbnails[0]);
                    //SelectItem();
                }

                if (this.item.itemPrice.usdt > 0)
                {
                    if (currentSelectedVariant.variantThumb.texture == null)
                    {
                        currentSelectedVariant.TextureDownload();
                    }
                    else
                    {
                        currentSelectedVariant.ApplyTexture();
                    }
                }
            }
        }

        public void SelectItem() //called on click
        {         
            if (variantDownload == false)
            {
                foreach (var item in this.item.itemTexUrls)
                {
                    VariantThumb variantThumb = new VariantThumb();
                    variantThumb.itemThumbnail = this;
                    variantThumb.id = item.texId;

                    
                    AssetsConfiguratorManager.instance.IconDownload(item, variantThumb);
                    variantThumbnails.Add(variantThumb);

                }
                variantDownload = true;
            }

            int selectedGenderAvatar = (int)AssetsConfiguratorManager.instance.gender;
            AvatarScript avatarScript = AssetsFBXManager.instance.fbxModels[selectedGenderAvatar].GetComponent<AvatarScript>();

            GameObject itemGameObject = null;
            AssetsScreenManager.instance.SetPriceAsPerCategoriesOnUI(item);

            if (this.item.itemType == AvatarAssetsCateogeries.upper.ToString())
            {
                foreach (var item in avatarScript.shirt.GetComponent<AssetsHolder>().childObjects)
                {
                    item.SetActive(false);
                    
                    if (this.item.itemShortCode.ToLower() == item.name.ToLower())
                    {
                        item.SetActive(true);
                        itemGameObject = item;
                    }
                }
            }
            else if (this.item.itemType == AvatarAssetsCateogeries.lower.ToString())
            {
                foreach (var item in avatarScript.pant.GetComponent<AssetsHolder>().childObjects)
                {
                    item.SetActive(false);
                    if (this.item.itemShortCode.ToLower() == item.name.ToLower())
                    {
                        item.SetActive(true);
                        itemGameObject = item;
                    }
                }

            }
            else if (this.item.itemType == AvatarAssetsCateogeries.socks.ToString())
            {
                foreach (var item in avatarScript.socks.GetComponent<AssetsHolder>().childObjects)
                {
                    item.SetActive(false);

                    if (this.item.itemShortCode.ToLower() == item.name.ToLower())
                    {
                        item.SetActive(true);
                        itemGameObject = item;
                    }
                }

            }
            else if (this.item.itemType == AvatarAssetsCateogeries.shoes.ToString())
            {
                foreach (var item in avatarScript.shoes.GetComponent<AssetsHolder>().childObjects)
                {
                    item.SetActive(false);
                    if (this.item.itemShortCode.ToLower() == item.name.ToLower())
                    {
                        item.SetActive(true);
                        itemGameObject = item;
                    }
                }

            }
            else if (this.item.itemType == AvatarAssetsCateogeries.cap.ToString())
            {
                foreach (var item in avatarScript.caps.GetComponent<AssetsHolder>().childObjects)
                {
                    item.SetActive(false);
                    if (this.item.itemShortCode.ToLower() == item.name.ToLower())
                    {
                        AvatarCapsTransform.instance.SetScalingCaps(AvatarBuilder.AvatarParent_FbxHolder.instance.currentSelectedAvatar.currentAvatarData.activeHairMesh.name, item.transform);
                        item.SetActive(true);
                        itemGameObject = item;
                    }
                }

            }
            else if (this.item.itemType == AvatarAssetsCateogeries.glasses.ToString())
            {
                foreach (var item in avatarScript.glasses.GetComponent<AssetsHolder>().childObjects)
                {
                    item.SetActive(false);
                    if (this.item.itemShortCode.ToLower() == item.name.ToLower())
                    {
                        item.SetActive(true);
                        itemGameObject = item;
                    }
                }

            }
            else if (this.item.itemType == AvatarAssetsCateogeries.jewellery.ToString())
            {
                foreach (var item in avatarScript.jewelleries.GetComponent<AssetsHolder>().childObjects)
                {
                    item.SetActive(false);
                    if (this.item.itemShortCode.ToLower() == item.name.ToLower())
                    {
                        item.SetActive(true);
                        itemGameObject = item;
                    }
                }               
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.jewelleryView,this.item.itemShortCode.ToLower());
            }
            else if (this.item.itemType == AvatarAssetsCateogeries.tattoo.ToString())
            {

                foreach (var item in avatarScript.tattos.GetComponent<AssetsHolder>().childObjects)
                {
                    item.SetActive(false);
                }

                if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
                {
                    string tattooShortCode = this.item.itemShortCode.ToLower();// + AssetsScreenManager.instance.assetCustomizeScreen.toggleValue.ToLower();
                    if (tattooShortCode[tattooShortCode.Length - 1].ToString().ToLower().Equals("l") || tattooShortCode[tattooShortCode.Length - 1].ToString().ToLower().Equals("r"))
                    {
                        Debug.Log("Already Contains Toggle Value. Removing..");
                        tattooShortCode = tattooShortCode.Remove(tattooShortCode.Length - 1, 1);
                    }

                    Debug.Log("tattooShortCode: " + tattooShortCode);

                    if(tattooShortCode.ToLower().Contains("default"))
                        Debug.Log("default tattoo: " + tattooShortCode);
                    else
                        tattooShortCode  +=  AssetsScreenManager.instance.assetCustomizeScreen.toggleValue;

                    Debug.Log("tattooShortCode: " + tattooShortCode);
                    var tattoo =  avatarScript.tattos.GetComponent<AssetsHolder>().childObjects.Find(x => x.name.ToLower().Equals(tattooShortCode.ToLower()));
                    if (tattoo!=null)
                    {
                        if (tattooShortCode.ToLower() == tattoo.name.ToLower())
                        {
                            tattoo.SetActive(true);
                            ItemThumbnail.tattoo = itemGameObject = tattoo;
                        }
                    }
                    else
                    {
                        Debug.LogError("Error in tattoo......");
                    }

                }
                else if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
                {
                    var tattoo =  avatarScript.tattos.GetComponent<AssetsHolder>().childObjects.Find(tattoo => tattoo.name.ToLower().Equals(this.item.itemShortCode.ToLower()));
                    if (tattoo!=null)
                    {
                        tattoo.SetActive(true);
                        itemGameObject = tattoo;
                    }
                    else
                    {
                        Debug.LogError("Error in tattoo......");
                    }
                }

                //foreach (var item in avatarScript.tattos.GetComponent<AssetsHolder>().childObjects)
                //{
                //    if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
                //    {
                //        string tattooShortCode = this.item.itemShortCode.ToLower();// + AssetsScreenManager.instance.assetCustomizeScreen.toggleValue.ToLower();
                //        item.SetActive(false);
                //        Debug.Log("tattooShortCode[tattooShortCode.Length-1]: " + tattooShortCode[tattooShortCode.Length - 1]);

                //        if (tattooShortCode[tattooShortCode.Length-1].ToString().ToLower().Equals("l") || tattooShortCode[tattooShortCode.Length - 1].ToString().ToLower().Equals("r"))
                //        {
                //            Debug.Log("Already contains l or r : " + tattooShortCode[tattooShortCode.Length - 1]);
                //        }
                //        else
                //        {
                //            tattooShortCode = this.item.itemShortCode + AssetsScreenManager.instance.assetCustomizeScreen.toggleValue;
                //        }
                //        if (tattooShortCode.ToLower() == item.name.ToLower())
                //        {
                //            item.SetActive(true);
                //            tattoo = itemGameObject = item;
                //        }
                //    }
                    
                //    else if(this.item.itemShortCode.ToLower()  == item.name.ToLower())
                //    {
                //        item.SetActive(true);
                //        itemGameObject = item;
                //    }

                //}
                AssetsScreenManager.instance.assetCustomizeScreen.tattooToggle.enabled = true;
                CamerControllerforConfigurator.instance.MoveCamera(CamerControllerforConfigurator.AvatarPosition.tattooView,this.item.itemName.ToLower());
            }

            itemSelectionPanel.ChangeSelectableItemThumbnail(this);

            LoadVariants(itemGameObject);
        }

        public void ChangeSelectableVariantThumbnail(VariantThumbnail currentSelectable = null)
        {
            if (itemSelectionPanel.variantSelectionPanel != null)
            {
                foreach (var item in itemSelectionPanel.variantSelectionPanel.GetComponentsInChildren<VariantThumbnail>())
                {
                    item.EnableDisableSelectable(false);
                }
            }
            if (currentSelectable)
            {
                currentSelectable.EnableDisableSelectable(true);
                currentSelectedVariant = currentSelectable;
                currentSelectedVariantId = currentSelectable.variantThumb.id;
            }
        }

    }



    [Serializable]
    public class VariantThumb
    {
        public string id;
        public Texture texture;
        public Sprite iconImage = null;
        public ItemThumbnail itemThumbnail;
    }
}