using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollarShirt : MonoBehaviour
{
    // Start is called before the first frame update
    AlternateItem alterITEM;


    private void OnEnable()
    {
       alterITEM=FindObjectOfType<AlternateItem>();
      if(alterITEM!=null)
            alterITEM.UpdateCHain();
       
    }
    private void OnDisable()
    {
      if(alterITEM!=null)
            alterITEM.UpdateCHain();
       // alterITEM.SetActive(false);
    }
}
