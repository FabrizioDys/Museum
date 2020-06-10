using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Utility;
using System;

public class InteractionManager : MonoBehaviour
{

    public Transform CameraT;                                //Calculate Ray Origin
    [SerializeField] private float _interactionDistance;     //Maximum interactable distance
    public Image _target;                                    //Crosshair Image


    private CharacterController fpsController;
    private Vector3 rayOrigin;
    private bool _pointingInteractable;


    public GameObject fpsCamera;
    public GameObject pointed;

    public bool isInteracting;

    public float InteractionDistance
    {
        get { return _interactionDistance; }
        set { _interactionDistance = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        fpsController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        rayOrigin = CameraT.position + fpsController.radius * CameraT.forward; //rayOrigin is the Raycast starting point
        CheckInteraction();  //checks if ray hits any interactable object
        UpdateUITarget();    //Change color of crosshair
    }


    private void CheckInteraction()
    {
        Ray ray = new Ray(rayOrigin, CameraT.forward);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, InteractionDistance))
        {
            //Check if is interactable
            InteractDesk interactable = hit.transform.GetComponent<InteractDesk>();
            _pointingInteractable = interactable != null ? true : false;

            if (_pointingInteractable)
            {
                pointed = hit.transform.gameObject;
                interactable.Interact();
            }
        }
        else
        {
            _pointingInteractable = false;
        }

    }

    private void UpdateUITarget()
    {
        if (_pointingInteractable)
        {
            _target.color = Color.white;
            if (pointed.gameObject)
                try
                {
                    pointed.gameObject.GetComponent<Outline>().enabled = true;
                }
                catch
                {
                    Debug.Log("ERRORE OUTLINE SULL OGGETTO PUNTATO:" + pointed.gameObject.name);
                }
        }
        else
        {
            _target.color = Color.gray;
            if (pointed.gameObject)
                try
                {
                    pointed.gameObject.GetComponent<Outline>().enabled = false;
                }
                catch
                {
                    Debug.Log("ERRORE OUTLINE SULL OGGETTO PUNTATO:" + pointed.gameObject.name);
                }
        }
    }
}