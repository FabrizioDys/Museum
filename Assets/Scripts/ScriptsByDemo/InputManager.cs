using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{


    private string CLICKED_TRIGGER_NAME;
    private string CLICKED_GRIP_NAME;
    private string TOUCHED_TRACKPAD_NAME;
    private string PRESSED_TRACKPAD_NAME;


    private RayCast rayCast;


    private bool canClick;
    private bool canGrip;
    private static readonly float CLICK_COOLDOWN_IN_SECOND = 0.5f;

    void Start()
    {
        rayCast = this.GetComponent<RayCast>();

        getInputNames();

        canClick = true;
        canGrip = true;
    }

    private void getInputNames()
    {
        if (this.name == "LeftController")
        {
            CLICKED_TRIGGER_NAME = "TriggerLeft";
            CLICKED_GRIP_NAME = "GripLeft";
            TOUCHED_TRACKPAD_NAME = "TrackpadPosLeft";
            PRESSED_TRACKPAD_NAME = "TrackpadPressLeft";

        }
        else if (this.name == "RightController")
        {
            CLICKED_TRIGGER_NAME = "TriggerRight";
            CLICKED_GRIP_NAME = "GripRight";
            TOUCHED_TRACKPAD_NAME = "TrackpadPosRight";
            PRESSED_TRACKPAD_NAME = "TrackpadPressRight";

        }
    }

    public bool CanClick
    {
        set
        {
            if (!value)
                Invoke("EnableClick", CLICK_COOLDOWN_IN_SECOND);

            canClick = value;

        }
        get
        {
            return canClick;
        }
    }

    private void EnableClick()
    {
        canClick = true;
    }

    public bool IsTriggerClicked()
    {
        return CanClick && (Input.GetAxis(CLICKED_TRIGGER_NAME) == 1);
    }


    public bool IsGripClicked()
    {
        return canGrip && (Input.GetAxis(CLICKED_GRIP_NAME) == 1);

    }

    internal bool UserGrip()
    {
        if (Input.GetKey(KeyCode.G) || IsGripClicked())
        {
            return true;
        }
        return false;
    }

    public bool UserClick()
    {
        return (IsTriggerClicked()) && rayCast.Hit();
    }

    internal String getSelectedTool(bool vrMode, String controllerName)
    {
        if (vrMode)
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, 0.01f);

            if (colliders.Length > 0)
            {
                if (colliders[0].transform.CompareTag("ToolIcon"))
                {
                    Debug.Log(colliders[0].transform.name);
                    Debug.Log(colliders[0].transform.name.Replace("Icon(Clone)", ""));
                    return colliders[0].transform.name.Replace("Icon(Clone)", "");
                }
            }

            return null;

        }
        else
        {
            return rayCast.GetHit().transform.name.Replace("Icon", "");
        }
    }

    public float getTrackpadTouchPos()
    {
        return Input.GetAxis(TOUCHED_TRACKPAD_NAME);
    }

    public bool isTrackpadTopTouched()
    {
        return getTrackpadTouchPos() < 0;
    }

    public bool isTrackpadBottomTouched() {

        return getTrackpadTouchPos() > 0;
    }

    public float getTrackpadPressPos()
    {
        return Input.GetAxis(PRESSED_TRACKPAD_NAME);
    }

    public bool isTrackpadPressed()
    {
        Debug.Log("Trackpad " + Input.GetButton(PRESSED_TRACKPAD_NAME));
        return Input.GetButton(PRESSED_TRACKPAD_NAME); 
    }

}
