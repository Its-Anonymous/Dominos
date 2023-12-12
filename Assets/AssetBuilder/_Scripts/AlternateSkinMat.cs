using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarBuilder
{
    public class AlternateSkinMat : MonoBehaviour
    {
        public bool isAlterSkin;

        private void OnEnable()
        {        
            if (isAlterSkin)
            {
                AvatarParent_FbxHolder.instance.currentSelectedAvatar.currentAvatarData.skinMat.mainTexture = AvatarParent_FbxHolder.instance.currentSelectedAvatar.alternateTexture;
            }
            else
                AvatarParent_FbxHolder.instance.currentSelectedAvatar.currentAvatarData.skinMat.mainTexture = AvatarParent_FbxHolder.instance.currentSelectedAvatar.mainSkinTexture;

        }
    }
}