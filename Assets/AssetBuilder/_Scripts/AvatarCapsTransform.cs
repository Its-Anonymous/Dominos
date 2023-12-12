using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBuilder
{
    public class AvatarCapsTransform : MonoBehaviour
    {
        public List<HairTransform> malehairTransforms = new List<HairTransform>();
        public List<HairTransform> femalehairTransforms = new List<HairTransform>();

        public static AvatarCapsTransform instance;

        private void Awake()
        {
            instance = this;
        }

        public void SetScalingCaps(string hairShortCode, Transform capTransform)
        {
            float temp = 1f;

            if (AssetsConfiguratorManager.instance.gender == AvatarBuilder.Gender.female)
            {
                temp = femalehairTransforms.Find(x => x.hairShortCode == hairShortCode).scale;

            }
            else
            {
                temp = malehairTransforms.Find(x => x.hairShortCode == hairShortCode).scale;
                if (capTransform.gameObject.name.Equals("C3_Rapper_Hat_01"))
                {
                    temp = 1.1f;
                }
            }

            capTransform.localScale = new Vector3(temp, temp, temp);
        }


    }
    [System.Serializable]
    public class HairTransform
    {
        public string hairShortCode;
        public float scale;
    }
}