using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHandler : MonoBehaviour {

    private LineRenderer laserLine;
    private RayCast rayCast;

    private GameObject cylinder;

    

    // Use this for initialization
    void Start () {

        rayCast = this.GetComponent<RayCast>();
        laserLine = GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (rayCast.Hit())
        {
            laserLine.enabled = true;
            
            UpdateLaserAndPointerPos(rayCast.GetHit());
        }
        else
        {
            laserLine.enabled = false;
        }

    }

    private void UpdateLaserAndPointerPos(RaycastHit hit)
    {
        laserLine.SetPosition(1, hit.point);
        laserLine.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));
    }
    
    
    public void setColor(Color color)
    {
        laserLine.GetComponent<Renderer>().material.color = color;
    }
}
