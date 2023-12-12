using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssetBuilder
{
    public class AssetsCustomizeScreen : MonoBehaviour
    {
        public Button backBtn;
        public Button previewBtn;
        Animation anim;
        AnimationClip animClip;
        public Toggle tattooToggle;//using in item thumnail
        public string toggleValue = "L";//using in item thumnail

        private void Awake()
        {
            anim = GetComponent<Animation>();
            animClip = anim.GetClip("AvatarCustomizeScreen");
        }
        
        public void OnEnable()
        {
            AssetsPreviewScreen.isAssetPreviewScreenEnabled = false;
            anim[animClip.name].speed = 1;
            anim.Play(animClip.name);
        }

        private void OnDisable()
        {
            OnAnimationReset();
        }

        public void OnAnimationReset()
        {
            anim[animClip.name].speed = -1;
            anim[animClip.name].time = anim[animClip.name].length;
            anim.Play(animClip.name);
        }

        void Start()
        {
            backBtn.onClick.AddListener(() => PreviousScreen());
            previewBtn.onClick.AddListener(() => PreviewData());
            tattooToggle.onValueChanged.AddListener(delegate {
                TattooToggler();
            });
            if (SceneManager.GetActiveScene().name.Equals(Global.CustomizeAvatarScene))
            {
                tattooToggle.gameObject.SetActive(false);
            }
        }

        public void TattooToggler()
        {
            if (!tattooToggle.isOn)
            {
                tattooToggle.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                toggleValue = "L";
            }
            else
            {
                tattooToggle.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                toggleValue = "R";
            }
            AssetsScreenManager.instance.categorySelectionPanel.currentSelectedCategoryThumbnail.itemSelectionPanel.currentSelectedItem.SelectItem();
        }

        private void PreviewData()
        {
            AssetsScreenManager.instance.DisableAllItemThumbnailSelectionPanels();           
            AssetsScreenManager.ChangeScreen(AssetsScreenManager.instance.assetPreviewScreen.gameObject, this.gameObject);         
        }

        private void PreviousScreen()
        {
            AssetsScreenManager.OpenClosePopUp(AssetsScreenManager.instance.discardAssetsChangesPopupScreen.gameObject, true);
        }
    }
}