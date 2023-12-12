using System;
using System.Collections;
using System.Collections.Generic;
using AvatarBuilder;
using UnityEngine;
using UnityEngine.UI;

namespace AssetBuilder
{
    public class UserAvatarData : MonoBehaviour
    {
        public Gender gender;
        public string tokenKey;
        public NFTUriAndToken avtarNFTUriAndToken;
        public Image avatarImage;
        [SerializeField]public MintAttribute mintAttribute;
        public string imageS3URL;
        public string characterName;
        public string characterClass;
        public Image selectedImage;

        public Button button;

        public Image LoadingImage;
        protected Vector3 rotationEuler;

        private void Start()
        {
            button.onClick.AddListener(() => OnClickedEvent());
        }

        private void Update()
        {
            if (LoadingImage.gameObject.activeInHierarchy) //To save process if disable
            {
                rotationEuler -= Vector3.forward * 50 * Time.deltaTime; //increment 30 degrees every second
                LoadingImage.transform.rotation = Quaternion.Euler(rotationEuler);
            }
        }

        public void SetAvatarData(string key , string imageURL ,string characterName, string characterClass, MintAttribute mintAttribute , Gender gender,NFTUriAndToken avtarNFTUriAndToken)
        {            
            LoadingImage.gameObject.SetActive(true);
            this.gender = gender;
            this.tokenKey = key;
            this.imageS3URL = imageURL;
            this.characterName = characterName;
            this.characterClass = characterClass;
            this.mintAttribute = mintAttribute;
            this.avtarNFTUriAndToken = avtarNFTUriAndToken;

            if (!string.IsNullOrEmpty(imageURL))
            {
                WebServiceManager.instance.DownloadTexture(imageURL, Method.GET, null, null, OnSuccessfullyImageDownloaded, JS_Hook.instance.OnFail, CACHEABLE.NULL, true, null);
            }
        }

        private void OnSuccessfullyImageDownloaded(Texture texture, long arg2)
        {
            var sprite = Sprite.Create((Texture2D)texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            sprite.name = texture.name;

            if(avatarImage)
                avatarImage.sprite = sprite;

            if(LoadingImage)
                LoadingImage.gameObject.SetActive(false);
        }

        public void OnClickedEvent()
        {           
            AssetsScreenManager.instance.avatarSelectionInventory.EnableDisableSelectable(this);
        }
    }
}