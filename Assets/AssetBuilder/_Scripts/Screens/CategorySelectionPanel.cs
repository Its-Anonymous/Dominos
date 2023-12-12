using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBuilder
{
    public class CategorySelectionPanel : MonoBehaviour
    {
        [SerializeField]
        public List<CategoryThumbnail> categories;
        public CategoryThumbnail currentSelectedCategoryThumbnail;


        private void OnDisable()
        {
            //ClearData();
        }

        public void ChangeSelectableIcon(CategoryThumbnail currentSelectable = null)
        {
            foreach (var item in categories)
            {
                item.EnableDisableSelectable(false);
            }
            if (currentSelectable != null)
            {
                currentSelectedCategoryThumbnail = currentSelectable;
                currentSelectable.EnableDisableSelectable(true);
            }
        }
        // Start is called before the first frame update
        //void Start()
        //{
        //    StartCoroutine(setResponse());
        //}

        //public IEnumerator setResponse()
        //{

        //    yield return new WaitForSeconds(0.5f);
        //    categories[0].SetCatagoryResponseData(categories);
        //}
    }
}