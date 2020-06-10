using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class IsVR : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        if (UnityEngine.XR.XRDevice.isPresent)
        {
            gameObject.SetActive(false);
        }
        else gameObject.SetActive(true);
    }

}
