using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosPointer : MonoBehaviour {

    public GameObject mainCamera;

	void Update () {
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.y = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(newRotation);

        transform.localPosition = mainCamera.transform.localPosition = Vector3.zero;
        Vector3 position = transform.localPosition;
        position.y = 3.5f;
        transform.localPosition = position;
    }
}
