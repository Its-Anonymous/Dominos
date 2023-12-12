using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetBuilder
{
    public class UIItemPrice : MonoBehaviour
    {
        public ItemPrice itemPrice;
        public Text itemText;
        public AvatarAssetsCateogeries itemCategory;

        public void SetData(ItemPrice itemPrice = null)
        {
            if(itemPrice == null)
            {
                itemText.text = "0 USDT";
            }
            this.itemPrice = itemPrice;
            itemText.text = itemPrice.usdt.ToString() + " USDT";            
        }

        public void GameObjectEnableDisable(bool isOpen)
        {
            this.gameObject.SetActive(isOpen);

        }
    }
}