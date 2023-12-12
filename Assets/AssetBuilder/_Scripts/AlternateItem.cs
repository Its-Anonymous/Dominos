using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateItem : MonoBehaviour
{
    // Start is called before the first frame update
    Renderer collarITEM;
    Renderer parentITEM;

    private void Awake()
    {
       collarITEM= transform.GetChild(0).GetComponent<Renderer>();
       parentITEM= GetComponent<Renderer>();
       collarITEM.GetComponent<Renderer>().material=parentITEM.material;
        
    }
    private void OnEnable()
    {
        UpdateCHain();
    }
    public void UpdateCHain()
    {
        print("UpdatateChain");
        parentITEM.enabled = false;
        collarITEM.enabled = false;
        if (FindObjectOfType<CollarShirt>())
            collarITEM.enabled = true;
        else 
            parentITEM.enabled = true;
    }

    private void OnDisable()
    {
        //collarITEM.enabled = true;
    }
}
