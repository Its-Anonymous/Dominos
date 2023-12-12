using UnityEngine;
using UnityEngine.UI;
using AvatarBuilder;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace AssetBuilder
{
    public class GenderSelectionScreen : MonoBehaviour
    {
        public Button maleBtn;
        public Button femaleBtn;
        public GameObject maleSelectable;
        public GameObject femaleSelectable;
        public Button nextBtn;        
        Animation anim;
        AnimationClip animClip;

        private void Awake()
        {
            anim = GetComponent<Animation>();
            animClip = anim.GetClip("AvatarBuilderGenderSelectionScreen");
        }

        public void OnEnable()
        {
            AssetsPreviewScreen.isAssetPreviewScreenEnabled = false;
            //Debug.LogError("OnEnable");
            anim[animClip.name].speed = 1;
            anim.Play(animClip.name);
            if (!AssetsConfiguratorManager.instance)
                return;
            AssetsConfiguratorManager.instance.EnableDisableModel(null);
            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                AssetsConfiguratorManager.instance.GetUserSaveDataAssets(WebServiceManager.instance.getUserAssetsCategories);
            }
            if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
            {
                AssetsConfiguratorManager.instance.GetUserSaveDataAssets(WebServiceManager.instance.getMintedUserAssetsCategories);
            }
        }

        public void OnReset()
        {
            anim[animClip.name].speed = -1.5f;
            anim[animClip.name].time = anim[animClip.name].length;
            anim.Play(animClip.name);
        }

        // Start is called before the first frame update
        void Start()
        {
            SelectAvatar(0);
            maleBtn.onClick.AddListener(() => SelectAvatar(1));
            femaleBtn.onClick.AddListener(() => SelectAvatar(0));
            nextBtn.onClick.AddListener(() => NextScreen());
        }

        private void NextScreen()
        {
            OnReset();
            //AssetsScreenManager.instance.ResetAssetConfiguratorData();

            if (JS_Hook.instance.haveTokensInWallet == false)
            {
                Debug.LogError("No Tokens in wallet of any gender.......");
                AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.assetCustomizeScreen.gameObject, this.gameObject);
                AssetsConfiguratorManager.instance.EnableDisableModel((int)AssetsConfiguratorManager.instance.gender);
                Debug.Log("AssetsConfiguratorManager.instance.gender: " + AssetsConfiguratorManager.instance.gender);
                AssetsConfiguratorManager.instance.LoadCharacterBlends(AssetsConfiguratorManager.instance.gender, JS_Hook.instance.defaultCharactersData.Find(x => x.Attributes.gender.ToLower().Equals(AssetsConfiguratorManager.instance.gender.ToString().ToLower())).Attributes.DataList);
            }

            else
            {
                List<string> selectedGendersKey = new List<string>();

                foreach (var nft in JS_Hook.instance.characterMintData)
                {
                    if (AssetsConfiguratorManager.instance.gender.ToString() == nft.Value.MintData.Attributes.gender.ToString())
                    {
                        selectedGendersKey.Add(nft.Key);
                    }
                }

                if (selectedGendersKey.Count > 0)
                {
                    Debug.LogError("Have tokens in wallet of this " + AssetsConfiguratorManager.instance.gender.ToString() + " gender.......");
                    AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.avatarSelectionInventory.gameObject, this.gameObject);

                    selectedGendersKey.ForEach((string key) =>
                    {
                        NFT nft = JS_Hook.instance.characterMintData[key];

                        NFTUriAndToken avtarNFTUriAndToken = new NFTUriAndToken();
                        avtarNFTUriAndToken.Uri = nft.Uri;
                        avtarNFTUriAndToken.TokenID = nft.TokenID;

                        AssetsScreenManager.instance.avatarSelectionInventory.SetAvatarData(key, nft.MintData, nft.MintData.imageS3URL, avtarNFTUriAndToken);
                    });                   
                }
                else
                {
                    Debug.LogError("Have tokens in wallet, but not of this "+ AssetsConfiguratorManager.instance.gender.ToString() + " gender.......");
                    AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.assetCustomizeScreen.gameObject, this.gameObject);
                    AssetsConfiguratorManager.instance.EnableDisableModel((int)AssetsConfiguratorManager.instance.gender);
                    AssetsConfiguratorManager.instance.LoadCharacterBlends(AssetsConfiguratorManager.instance.gender, JS_Hook.instance.defaultCharactersData.Find(x => x.Attributes.gender.ToLower().Equals(AssetsConfiguratorManager.instance.gender.ToString().ToLower())).Attributes.DataList);
                }
            }

            AssetsConfiguratorManager.instance.GetAssetBuilderCategories(WebServiceManager.instance.getAssetBuilderCategories);

            if (SceneManager.GetActiveScene().name.Equals(Global.AssetBuilderScene))
            {
                GetUserSaveAssetsDataRoot getUserData = new GetUserSaveAssetsDataRoot();
                getUserData = JsonUtility.FromJson<GetUserSaveAssetsDataRoot>(AssetsConfiguratorManager.instance.data.ToString());
                
                if ((int)AssetsConfiguratorManager.instance.gender == 0)
                {
                    AssetsConfiguratorManager.instance.SetDataOnPreviewScreen(getUserData.data.femaleDefaultAsset);
                } //female
                else
                {
                    AssetsConfiguratorManager.instance.SetDataOnPreviewScreen(getUserData.data.maleDefaultAsset);
                } //male
            }
            
        }

        /// <summary>
        /// 0 = Female
        /// 1 = Male
        /// </summary>
        /// <param name="gender"></param>
        private void SelectAvatar(int gender)
        {
            //AssetsConfigurator.currentSelectedGender = (gender == 0) ? Gender.female : Gender.male;
            
            //Reset Selectable
            GameObject currentSelectable = (gender == 0) ? femaleSelectable : maleSelectable;
            AssetsConfiguratorManager.instance.gender = (Gender)gender;
            UpdateSelectable(currentSelectable);
        }

        /// <summary>
        /// Update Selection
        /// </summary>
        /// <param name="currentSelectable"></param>
        private void UpdateSelectable(GameObject currentSelectable)
        {
            maleSelectable.SetActive(false);
            femaleSelectable.SetActive(false);

            currentSelectable.SetActive(true);
        }


    }

}