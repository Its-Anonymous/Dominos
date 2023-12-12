using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AvatarBuilder;
using UnityEngine;

namespace AssetBuilder
{

    public class AvatarScript : MonoBehaviour
    {

        public GameObject shirt;
        public GameObject pant;
        public GameObject socks;
        public GameObject shoes;
        public GameObject caps;
        public GameObject glasses;
        public GameObject jewelleries;
        public GameObject tattos;

        public List<AssetsHolder> assetsHolders = new List<AssetsHolder>();

        [ContextMenu("Find Data")]
        public void FindData()
        {
            assetsHolders.Clear();
            //if (avatarSKMR == null) avatarSKMR = GetComponentInChildren<SkinnedMeshRenderer>(true);
            foreach (Transform item in this.transform)
            {
                if (item.name.ToLower().Contains("shirt"))
                {
                    shirt = item.gameObject;
                    AssetsHolder assetsHolder = shirt.GetComponent<AssetsHolder>();

                    if (!assetsHolder)
                        assetsHolder = shirt.AddComponent<AssetsHolder>();

                    assetsHolder.assetCateogeries = AvatarAssetsCateogeries.upper;
                    assetsHolder.gameObject.layer = LayerMask.NameToLayer("Upper");
                    assetsHolder.GetChilds();
                }
                else if (item.name.ToLower().Contains("pant"))
                {
                    pant = item.gameObject;

                    AssetsHolder assetsHolder = pant.GetComponent<AssetsHolder>();

                    if (!assetsHolder)
                        assetsHolder = pant.AddComponent<AssetsHolder>();
                    assetsHolder.assetCateogeries = AvatarAssetsCateogeries.lower;
                    assetsHolder.gameObject.layer = LayerMask.NameToLayer("Lower");
                    assetsHolder.GetChilds();
                }
                else if (item.name.ToLower().Contains("socks"))
                {
                    socks = item.gameObject;

                    AssetsHolder assetsHolder = socks.GetComponent<AssetsHolder>();

                    if (!assetsHolder)
                        assetsHolder = socks.AddComponent<AssetsHolder>();
                    assetsHolder.assetCateogeries = AvatarAssetsCateogeries.socks;
                    assetsHolder.gameObject.layer = LayerMask.NameToLayer("Socks");
                    assetsHolder.GetChilds();
                }
                else if (item.name.ToLower().Contains("shoes"))
                {
                    shoes = item.gameObject;

                    AssetsHolder assetsHolder = shoes.GetComponent<AssetsHolder>();

                    if (!assetsHolder)
                        assetsHolder = shoes.AddComponent<AssetsHolder>();
                    assetsHolder.assetCateogeries = AvatarAssetsCateogeries.shoes;
                    assetsHolder.gameObject.layer = LayerMask.NameToLayer("Shoes");
                    assetsHolder.GetChilds();
                }
                else if (item.name.ToLower().Contains("glasses"))
                {
                    glasses = item.gameObject;

                    AssetsHolder assetsHolder = glasses.GetComponent<AssetsHolder>();

                    if (!assetsHolder)
                        assetsHolder = glasses.AddComponent<AssetsHolder>();
                    assetsHolder.assetCateogeries = AvatarAssetsCateogeries.glasses;
                    assetsHolder.gameObject.layer = LayerMask.NameToLayer("Glasses");
                    assetsHolder.GetChilds();
                }
                else if (item.name.ToLower().Contains("jewellery"))
                {
                    jewelleries = item.gameObject;

                    AssetsHolder assetsHolder = jewelleries.GetComponent<AssetsHolder>();

                    if (!assetsHolder)
                        assetsHolder = jewelleries.AddComponent<AssetsHolder>();
                    assetsHolder.assetCateogeries = AvatarAssetsCateogeries.jewellery;
                    assetsHolder.gameObject.layer = LayerMask.NameToLayer("Jewellery");
                    assetsHolder.GetChilds();
                }
                else if (item.name.ToLower().Contains("tattoo"))
                {
                    tattos = item.gameObject;

                    AssetsHolder assetsHolder = tattos.GetComponent<AssetsHolder>();

                    if (!assetsHolder)
                        assetsHolder = tattos.AddComponent<AssetsHolder>();
                    assetsHolder.assetCateogeries = AvatarAssetsCateogeries.tattoo;
                    assetsHolder.gameObject.layer = LayerMask.NameToLayer("Tattoo");
                    assetsHolder.GetChilds();
                }
                else if (item.name.ToLower().Contains("group"))
                {
                    Transform[] AllChildTran = GetComponentsInChildren<Transform>().ToArray().Where(item => item.gameObject.transform).ToArray();
                    foreach (var child in AllChildTran)
                    {
                        if(child.gameObject.name.ToLower().Contains("caps"))
                        {
                            caps = child.gameObject;
                            break;
                        }                       
                    }

                    AssetsHolder assetsHolder = caps.GetComponent<AssetsHolder>();

                    if (!assetsHolder)
                        assetsHolder = caps.AddComponent<AssetsHolder>();

                    assetsHolder.assetCateogeries = AvatarAssetsCateogeries.cap;
                    assetsHolder.gameObject.layer = LayerMask.NameToLayer("Cap");
                    assetsHolder.GetChilds();
                }
            }
            assetsHolders.Add(shirt.GetComponent<AssetsHolder>());
            assetsHolders.Add(pant.GetComponent<AssetsHolder>());
            assetsHolders.Add(socks.GetComponent<AssetsHolder>());
            assetsHolders.Add(shoes.GetComponent<AssetsHolder>());
            assetsHolders.Add(caps.GetComponent<AssetsHolder>());
            assetsHolders.Add(glasses.GetComponent<AssetsHolder>());
            assetsHolders.Add(jewelleries.GetComponent<AssetsHolder>());
            assetsHolders.Add(tattos.GetComponent<AssetsHolder>());
        }

        public static void SetOnUserAssets(bool defaultOpen, AssetsHolder assetsHolder = null, Item item = null)
        {
            foreach (var childitem in assetsHolder.childObjects)
            {
                if (childitem.name.ToLower() == item.itemShortCode.ToLower())
                {
                    childitem.gameObject.SetActive(defaultOpen);
                }
                else
                {
                    childitem.gameObject.SetActive(!defaultOpen);
                }
            }
        }

        public void SetOnDefaultAssets(bool defaultOpen)
        {
            foreach (var item in assetsHolders)
            {
                foreach (var assets in item.childObjects)
                {
                    assets.gameObject.SetActive(false);
                    
                    if (assets.gameObject.name.Contains("Default"))
                    {
                        assets.gameObject.SetActive(defaultOpen);
                    }

                }
                
            }
        }



    }
}
