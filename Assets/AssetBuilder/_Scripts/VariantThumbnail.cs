using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AssetBuilder
{

    public class VariantThumbnail : MonoBehaviour
    {
        public ItemThumbnail itemThumbnail;
        public VariantThumb variantThumb;
        //public ItemTexUrl texUrl;
        public Texture texture;
        public Image iconImage;

        [SerializeField] Image SelectedImage;
        [SerializeField] Renderer ModelMeshReference;

        bool _canPlace;

        //Action SetData; 
        public Image LoadingImage;
        protected Vector3 rotationEuler;


        private void Update()
        {
            if (LoadingImage.gameObject.activeInHierarchy) //To save process if disable
            {
                rotationEuler += Vector3.forward * 30 * Time.deltaTime; //increment 30 degrees every second
                LoadingImage.transform.rotation = Quaternion.Euler(rotationEuler);
            }
        }

        public void SetVariantData(VariantThumb variantThumb, GameObject ItemRef = null)
        {
            this.variantThumb = variantThumb;


            if (ItemRef != null)
                ModelMeshReference = ItemRef.GetComponent<Renderer>();

            if (variantThumb.texture != null)
                texture = variantThumb.texture;


            if (variantThumb.iconImage != null)
                if (iconImage)
                {
                    iconImage.sprite = variantThumb.iconImage;
                    iconImage.enabled = true;
                }


            if (variantThumb.iconImage != null)
            {
                if (iconImage)
                    iconImage.transform.GetChild(0).gameObject.SetActive(false);
                _canPlace = true;


            }
            else
            {
                if (iconImage)
                    iconImage.transform.GetChild(0).gameObject.SetActive(true);
                _canPlace = false;
            }




        }

        public void EnableDisableSelectable(bool doEnable)
        {
            if (doEnable)
                SelectedImage.gameObject.SetActive(true);
            else
                SelectedImage.gameObject.SetActive(false);
        }

        public void TextureDownload()
        {
            WaitingLoader.instance.ShowHideAssetsLoader(true);
            if (variantThumb.texture == null)
            {
                foreach (var item in variantThumb.itemThumbnail.item.itemTexUrls)
                {
                    if (item.texId == this.variantThumb.id)
                    {
                        Debug.Log("Request of Download");
                        AssetsConfiguratorManager.instance.TextureDownload(item, this.variantThumb);
                    }
                }
            }
            else
            {             
                ApplyTexture();
            }

            itemThumbnail.ChangeSelectableVariantThumbnail(this);
        }

        public void ApplyTexture() // using from outside button click
        {
            if (_canPlace)
            {
                Debug.Log("Texture Applied");             
                
                WaitingLoader.instance.ShowHideAssetsLoader(false);
                if (ModelMeshReference)
                    ModelMeshReference.material.mainTexture = variantThumb.texture;//   SetTexture("_MainTex", variantThumb.texture);
            }
        }
    }
}