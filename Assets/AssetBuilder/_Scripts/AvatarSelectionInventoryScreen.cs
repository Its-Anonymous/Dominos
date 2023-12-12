using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AvatarBuilder;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssetBuilder
{
    public class AvatarSelectionInventoryScreen : MonoBehaviour
    {
        public static string characterID = "Default";
        public static UserAvatarData selectedUserAvatarData;
        public ScrollRect scrollRect;
        [SerializeField] GameObject avatarThumbnail;
        [SerializeField] List<UserAvatarData> userAvatarDatathumbnails = new List<UserAvatarData>();
        [SerializeField] Button nextScreenBtn;
        [SerializeField] Button backBtn;
        int selectedAvatarModelIndex = 0;

        private void Start()
        {
            nextScreenBtn.onClick.AddListener(() => NextScreen());
            backBtn.onClick.AddListener(() => BackScreen());
        }

        private void BackScreen()
        {
            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.genderSelectionScreen.gameObject, gameObject);
            }
        }

        private void OnEnable()
        {
            AssetsPreviewScreen.isAssetPreviewScreenEnabled = false;
            EnableDisableSelectable(userAvatarDatathumbnails[0]);

        }
        private void OnDisable()
        {
            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                AssetsConfiguratorManager.instance.ClearContent(scrollRect.content.gameObject);
                userAvatarDatathumbnails.Clear();
            }
        }

        public void SetAvatarData(string key, MintData mintData , string imageURL, NFTUriAndToken avtarNFTUriAndToken)
        {            
            var temp = Instantiate(avatarThumbnail,scrollRect.content.transform).GetComponent<UserAvatarData>();
            temp.SetAvatarData(key,imageURL,mintData.name,mintData.itemclass, mintData.Attributes, (Gender)Enum.Parse(typeof(Gender), mintData.Attributes.gender), avtarNFTUriAndToken);
            userAvatarDatathumbnails.Add(temp);
        }

        public void EnableDisableSelectable(UserAvatarData userAvatarData)
        {
            foreach (var item in userAvatarDatathumbnails)
            {
               item.selectedImage.gameObject.SetActive(false);
            }

            selectedUserAvatarData = userAvatarData;
            characterID = userAvatarData.tokenKey;
            userAvatarData.selectedImage.gameObject.SetActive(true);
            Debug.Log("selectedUserAvatarData : "  + selectedUserAvatarData.imageS3URL,selectedUserAvatarData.gameObject);
            AssetsConfiguratorManager.instance.gender = userAvatarData.gender;
            AssetsConfiguratorManager.instance.selectedAvatar = userAvatarData.avtarNFTUriAndToken;
            AssetsConfiguratorManager.instance.selectedAvatarName = userAvatarData.characterName;
            AssetsConfiguratorManager.instance.selectedAvatarClass = userAvatarData.characterClass;
            AssetsConfiguratorManager.instance.selectedAvatarImage = userAvatarData.imageS3URL;

            foreach (var item in AvatarParent_FbxHolder.instance.avatars)   
            {
                if(userAvatarData.gender.ToString().ToLower() == item.tag)
                {
                   
                    AvatarParent_FbxHolder.instance.currentSelectedAvatar = item;
                    if(item.tag.ToLower() == "female")
                    {
                        selectedAvatarModelIndex = 0;
                        //AvatarParent_FbxHolder.instance.avatars[1].currentAvatarData.activeHairMesh =  AvatarParent_FbxHolder.instance.avatars[1].currentAvatarData.hairMesheParent.childObjects.Find(x => x.gameObject.name.ToLower().Contains("Default"));
                    }
                    else if (item.tag.ToLower() == "male")
                    {
                        selectedAvatarModelIndex = 1;
                    }
                }
            }
        }

        public void NextScreen()
        {            
            AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.assetCustomizeScreen.gameObject, AssetsScreenManager.instance.avatarSelectionInventory.gameObject);
            AssetsConfiguratorManager.instance.EnableDisableModel(selectedAvatarModelIndex);//(int)userAvatarDatathumbnails[selectedAvatarModelIndex].gender
            AssetsConfiguratorManager.instance.GetAssetBuilderCategories(WebServiceManager.instance.getAssetBuilderCategories);
            AssetsConfiguratorManager.instance.LoadCharacterBlends(selectedUserAvatarData.gender, selectedUserAvatarData.mintAttribute.DataList);
            if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
            {
                AssetsConfiguratorManager.instance.SetMintedDataOnGenderSelection(AssetsConfiguratorManager.instance.gender);
            }
        }
    }
}