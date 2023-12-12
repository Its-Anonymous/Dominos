using System.Collections;
using System.Collections.Generic;
using AvatarBuilder;
using UnityEngine;
using System.Linq;

namespace AssetBuilder
{
    public class AssetsFBXManager : MonoBehaviour
    {
        public List<AvatarScript> fbxModels;
        public List<AvatarCapsTransform> avatarCapsTransforms;

        public static AssetsFBXManager instance;

        private void Awake()
        {
            instance = this;
        }

        [ContextMenu("Get Models")]
        public void FindFBX()
        {
            foreach(Transform item in this.transform)
            {   
                if (!item.gameObject.GetComponent<AvatarScript>())
                {
                    item.gameObject.AddComponent<AvatarScript>();
                }
            }

            fbxModels = GetComponentsInChildren<AvatarScript>(true).ToList();
            FindObjectOfType<AssetsConfiguratorManager>(true).Fill_List(fbxModels);
            foreach (var item in fbxModels)
            {
                item.FindData();
                item.gameObject.SetActive(false);
            }
            avatarCapsTransforms = FindObjectsOfType<AvatarCapsTransform>().ToList();
        }      
    }
}
