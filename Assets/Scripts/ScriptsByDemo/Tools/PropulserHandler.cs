using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropulserHandler : ToolsHandler {

    private InputManager inputManager;
    private GameObject player;
    public float propulsionForce = 10f;
    public float frixion = 1f;
    public float FOVMultiplyer = 5f;


	// Use this for initialization
	void Start()
    {
        this.inputManager = this.GetComponent<InputManager>();
        this.player = GameObject.Find("Player");
	}

    // Update is called once per frame
    void Update () {
        if (enabled)
        {
            propulsionForce = calculateSpeed();

            //StartCoroutine(changeFOV());
            
            if (inputManager.IsTriggerClicked())
            {
                player.GetComponent<Rigidbody>()
               .AddForce(this.transform.forward * propulsionForce);
            }

            if(this.GetComponent<ControllerManager>().getSecondController().getCurrentTool().getToolName() == "Propulser")
                player.GetComponent<Rigidbody>().AddForce(player.GetComponent<Rigidbody>().velocity * -1 * (frixion/2));
            else
                player.GetComponent<Rigidbody>().AddForce(player.GetComponent<Rigidbody>().velocity * -1 * frixion);
            

            endFrame();
            //changeFOV();
        }
    }

    override
    public void enable()
    {
        enabled = true;
        //GameObject.Find("Player").GetComponent<Rigidbody>().mass = 10000;
    }

    override
    public void disable()
    {
        enabled = false;
        //GameObject.Find("Player").GetComponent<Rigidbody>().mass = 1;
    }

    private float calculateSpeed()
    {
        if (inputManager.getTrackpadTouchPos() == 0)
        {
            return propulsionForce;
        } else
        {
            return 10f*(2 - (inputManager.getTrackpadTouchPos() + 1));
        }
         
    }

    IEnumerator endFrame()
    {
        yield return null;
    }

    IEnumerator changeFOV()
    {
        yield return new WaitForEndOfFrame();
        GameObject.Find("RightCamera").GetComponent<Camera>().fieldOfView += FOVMultiplyer * player.GetComponent<Rigidbody>().velocity.magnitude;
        Debug.Log(GameObject.Find("LeftCamera").GetComponent<Camera>().fieldOfView);
    }


}
