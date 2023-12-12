using System.Collections;
using System.Collections.Generic;
using AvatarBuilder;
using UnityEngine;

namespace AssetBuilder
{
    public class AssetsHolder : MonoBehaviour
    {
        public List<GameObject> childObjects = new List<GameObject>();
        public AvatarAssetsCateogeries assetCateogeries;

        [ContextMenu("Get Childs")]
        public void GetChilds()
        {
            if (!gameObject.GetComponent<AssetsHolder>())
            {
                gameObject.AddComponent<AssetsHolder>();
            }
            childObjects.Clear();
            GetAssetInChildObject();
        }

        private void GetAssetInChildObject()
        {
            foreach (Transform item in this.transform)
            {
                childObjects.Add(item.gameObject);
                item.gameObject.layer = item.gameObject.transform.parent.gameObject.layer;
                if (item.GetComponent<SkinnedMeshRenderer>())
                    item.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
            }
            foreach (var item in childObjects)
            {   
                if (item.name.Contains("Default"))
                {
                    item.gameObject.SetActive(true);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }
}
