using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterHandler : ToolsHandler {


    private RayCast rayCast;
    private InputManager inputManager;
    private GameObject player;
    private ControllerManager controllerManager;

    void Start()
    {
        rayCast = this.GetComponent<RayCast>();
        inputManager = this.GetComponent<InputManager>();
        player = GameObject.Find("Player");
        controllerManager = GameObject.Find("LeftController").GetComponent<ControllerManager>();
    }

    void Update()
    {
        if (enabled)
        {
            if (inputManager.IsTriggerClicked() && !rayCast.GetHit().transform.tag.Equals("Maze"))
            {
                Teleport(rayCast.GetHit());
                inputManager.CanClick = false;
            }
            else if (inputManager.IsTriggerClicked() && rayCast.GetHit().transform.tag.Equals("Maze"))
            {
                TeleportToMaze();
                controllerManager.disableAllTools();
                inputManager.CanClick = false;
            }
            else if (inputManager.IsTriggerClicked() && rayCast.GetHit().transform.name.Equals("Maze Spawn"))
            {
                if (rayCast.GetHit().transform.name.Equals("maze")) return;
                TeleportInMaze(rayCast.GetHit() );
                inputManager.CanClick = false;
            }
        }
    }

  

    override
    public void enable()
    {
        enabled = true;
        this.GetComponent<LineRenderer>().enabled = true;
        this.GetComponent<LaserHandler>().enabled = true;
    }

    override
    public void disable()
    {
        enabled = false;
        this.GetComponent<LineRenderer>().enabled = false;
        this.GetComponent<LaserHandler>().enabled = false;
    }

    public void Teleport(RaycastHit hit)
    {
        Vector3 newPlayerPos = hit.point;
        //newPlayerPos.y = player.transform.position.y;

        player.transform.position = newPlayerPos;
    }

    private void TeleportToMaze()
    {
        player.transform.position = new Vector3(9f, 1000.9f, -82.833f);
    }

    private void TeleportInMaze(RaycastHit hit)
    {
        Vector3 newPlayerPos = hit.point;
        newPlayerPos.y = player.transform.position.y + 0.9f;

        player.transform.position = newPlayerPos;
    }
}
