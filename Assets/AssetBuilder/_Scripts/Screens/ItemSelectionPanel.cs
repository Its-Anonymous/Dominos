using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetBuilder
{

    public class ItemSelectionPanel : MonoBehaviour
    {
        
        public RectTransform contentTransform;
        public RectTransform variantContentTransform;
        public GameObject variantSelectionPanel;
        public List<ItemThumbnail> itemThumbnails;

        public ItemThumbnail currentSelectedItem = null;
        public VariantThumbnail currentSelectedVariant = null;


        Animation anim;
        AnimationClip animClip;

        /// <summary>
        /// Item Classes Work
        /// </summary>
        public enum Classes
        {
           all, asset_class_1, asset_class_2, asset_class_3
        };
        public Image selectionImage;

        [Header("Class Buttons")]
        public Button class_A_Btn;
        public Button class_B_Btn;
        public Button class_C_Btn;
        public Button class_D_Btn;
        public Button class_Reset_Btn;


        public void ClassFilter(Classes sc,Button button)
        {
            if (sc == Classes.all)
            {
                foreach (var item in itemThumbnails)
                {
                    item.gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (var item in itemThumbnails)
                {
                    if (sc.ToString() != item.item.itemClass) item.gameObject.SetActive(false);
                    else item.gameObject.SetActive(true);
                }
            }
            UpdateGreen(button);
        }


        public void UpdateGreen(Button button)
        {
            LeanTween.move(selectionImage.gameObject, button.transform.position, 0.4f).setEaseOutSine();
            RectTransform forImage = selectionImage.GetComponent<RectTransform>();
            forImage.sizeDelta = new Vector2(button.GetComponent<RectTransform>().sizeDelta.x, forImage.sizeDelta.y);

        }

        private void Start()
        {          
            class_A_Btn.onClick.AddListener(()=>ClassFilter( Classes.asset_class_1,class_A_Btn));
            class_B_Btn.onClick.AddListener(()=> ClassFilter(Classes.asset_class_2, class_B_Btn));
            class_C_Btn.onClick.AddListener(()=> ClassFilter(Classes.asset_class_3, class_C_Btn));
            //class_D_Btn.onClick.AddListener(()=> ClassFilter(Classes.four, class_D_Btn));
            class_Reset_Btn.onClick.AddListener(()=> ClassFilter(Classes.all, class_Reset_Btn));
            //itemThumbnails[0].SelectItem();
        }

        /// <summary>
        /// Item Classes Work End
        /// </summary>

        private void Awake()
        {
            anim = GetComponent<Animation>();
            animClip = anim.GetClip("AvatarCustomizeScreenSelectionPanels");
        }


        public void OnEnable()
        {
            anim[animClip.name].speed = 1;
            anim.Play(animClip.name);

        }

        public void OnReset()
        {
            anim[animClip.name].speed = -1;
            anim[animClip.name].time = anim[animClip.name].length;
            anim.Play(animClip.name);
        }


        public void EnableDisablePanel(bool doEnable = false)
        {
            gameObject.SetActive(doEnable);
            if (variantContentTransform != null)
            {
                variantSelectionPanel.SetActive(doEnable);
            }
        }

        public void ChangeSelectableItemThumbnail(ItemThumbnail currentSelectable = null)
        {
            foreach (var item in itemThumbnails)
            {
                item.EnableDisableSelectable(false);
            }
            if (currentSelectable)
            {
                currentSelectable.EnableDisableSelectable(true);
                currentSelectedItem = currentSelectable;
            }
        }

        public void ChangeSelectableVariantThumbnail(VariantThumbnail currentSelectable = null)
        {
            if (variantSelectionPanel != null)
            {
                foreach (var item in variantSelectionPanel.GetComponentsInChildren<VariantThumbnail>())
                {
                    item.EnableDisableSelectable(false);
                }
            }
            if (currentSelectable)
            {
                currentSelectable.EnableDisableSelectable(true);
                currentSelectedVariant = currentSelectable;
            }
        }
    }
}