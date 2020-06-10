using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class isDesktop : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        if (UnityEngine.XR.XRDevice.isPresent)
        {
            gameObject.SetActive(true);
        }
        else gameObject.SetActive(false);
    }

}
