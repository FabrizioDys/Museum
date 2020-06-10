using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class ControllerManager : MonoBehaviour {
    
    private InputManager inputManager;
    private ControllerManager secondController;
    private RayCast rayCast;

    public List<string> toolNames;
    public string defaultToolName;

    private List<ToolsHandler> tools;

    private GameObject menu;

    private ToolsHandler currentTool;
    private GameObject currentToolIcon;

    private string lastSelectedToolIcon;

    bool choosingTool = false;
    bool vrMode;

	// Use this for initialization
	void Start () { 
        inputManager = this.GetComponent<InputManager>();
        setSecondController();
        rayCast = this.GetComponent<RayCast>();

        tools = new List<ToolsHandler>();

        initMenu();

        setDefaultCurrentTool();
        
        hideMenu();
    }
    

    // Update is called once per frame
    void Update () {
        if (Grab() && !choosingTool && !secondController.isChoosingTool() && !inputManager.IsTriggerClicked())
        {
            //makePlayerStatic();
            disableCurrentTool();
            displayMenu();
            choosingTool = true;
        }

        //apply the shader to the selected tool icon
        if(Grab() && choosingTool)
        {
            string selectedToolIcon = inputManager.getSelectedTool(vrMode, this.name);
            if (selectedToolIcon != null)
            {
                Debug.Log(selectedToolIcon);
                lastSelectedToolIcon = selectedToolIcon;
                GameObject.Find(selectedToolIcon + "Icon(Clone)").GetComponent<Renderer>().material.shader = Shader.Find("VR/SpatialMapping/Wireframe");
            }
            else if(lastSelectedToolIcon != null)
            {
                GameObject.Find(lastSelectedToolIcon + "Icon(Clone)").GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                lastSelectedToolIcon = null;
            }
        }
            
        if (!Grab() && choosingTool)
        {
            if (getSelectedTool() != null)
            {
                GameObject.Find(lastSelectedToolIcon + "Icon(Clone)").GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                setCurrentTool(getSelectedTool());
            }

            choosingTool = false;
            hideMenu();
            //allowPlayerMovement();
        }
    }

    private void setSecondController()
    {
        if (name.Equals("RightController"))
            secondController = GameObject.Find("LeftController").GetComponent<ControllerManager>();
        else if (name.Equals("LeftController"))
            secondController = GameObject.Find("RightController").GetComponent<ControllerManager>();

        if (secondController != null)
            vrMode = true;
        else
            vrMode = false;
    }

    private void initMenu()
    {
        for (int i = 0; i < toolNames.Count; i++)
        {
            
            string toolHandlerName = toolNames[i] + "Handler";
            ToolsHandler toolHandler = this.GetComponent(Type.GetType(toolHandlerName)) as ToolsHandler;
            tools.Add(toolHandler);
        }
        
        string menuName = this.name + "Menu";
        menu = new GameObject(menuName);

        for(int i=0 ; i < tools.Count ; i++)
        {
            Debug.Log(tools[3].getToolIcon().transform.name);
            tools[i].getToolIcon().transform.parent = menu.transform;
        }

        initMenuAssetsPosition();
    }

    private void initMenuAssetsPosition()
    {
        for (int i = 0; i < tools.Count; i++)
        {
            double angleRangeToPlaceIconsIn = Math.PI; //in radians
            double angleLeft = ((Math.PI - angleRangeToPlaceIconsIn) / 2);
            float circleRadius = 2f;
            Vector3 controllerOffset = new Vector3(0, 0, 1.5f);

            tools[i].getToolIcon().transform.position = new Vector3(
                circleRadius * (float)(Math.Cos(angleLeft + 
                   ((i * angleRangeToPlaceIconsIn) / 
                   (tools.Count - 1)))),
                tools[i].getToolIcon().transform.position.y,
                circleRadius * (float)(Math.Sin(angleLeft + 
                   ((i * angleRangeToPlaceIconsIn) / 
                   (tools.Count - 1))))
                ) - controllerOffset;
        }
    }

    public void hideMenu()
    {
        menu.SetActive(false);
    }

    private bool Grab()
    {
        return inputManager.UserGrip();
    }

    private void displayMenu()
    {
        initMenuPostition();
        menu.SetActive(true);
    }

    private void initMenuPostition()
    {

        if (vrMode)
        {
            menu.transform.position = this.transform.position;
            menu.transform.position += this.transform.forward * 0.2f;
            menu.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            menu.transform.rotation = new Quaternion(0, 0, 0, 0);
            menu.transform.Rotate(new Vector3(0, this.transform.rotation.eulerAngles.y,0), Space.Self);
        } 
        else     
        {
            menu.transform.position = rayCast.GetHit().point;
            menu.transform.Rotate(new Vector3(0, 0, 0), Space.World);
        }
    } 

    private void setDefaultCurrentTool()
    {
        for(int i=0; i < tools.Count; i++)
        {
            if (defaultToolName == tools[i].getToolName())
            {
                setCurrentTool(tools[i]);
            }
        }
    }

    private void setCurrentTool(ToolsHandler tool)
    {
        disableAllTools();
        if(tool != null)
        {
            destroyCurrentToolIcon();
            currentTool = tool;
            currentTool.enable();
            showCurrentTool();
        }
    }

    public void disableAllTools()
    {
        for(int i = 0; i < tools.Count; i++)
        {
            tools[i].disable();
        }
    }

    private void destroyCurrentToolIcon()
    {
        Destroy(currentToolIcon);
    }

    private void showCurrentTool()
    {
        currentToolIcon = Instantiate(currentTool.getToolIcon());
        
        if (vrMode)
        {
            currentToolIcon.GetComponent<MeshCollider>().enabled = false;
            currentToolIcon.transform.localScale *= 0.05f;
            currentToolIcon.transform.parent = this.transform;
            currentToolIcon.transform.position = this.transform.position + new Vector3(0f, 0.033f, 0f);
        } else
        {
            currentToolIcon.transform.localScale *= 0.2f;
            currentToolIcon.transform.position = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.1f, 0.8f, 2.0f));
            currentToolIcon.transform.parent = GameObject.Find("FirstPersonCharacter").transform;
        }
        
    }

    private ToolsHandler getSelectedTool()
    {
        string selectedTool = inputManager.getSelectedTool(vrMode, this.name);

        for(int i=0; i<tools.Count; i++)
        {
            if (tools[i].getToolName()==selectedTool)
            {
                return tools[i];
            }
        }
        return null;
        
    }

    private void makePlayerStatic()
    {
        GameObject.Find("Player").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void allowPlayerMovement()
    {
        GameObject.Find("Player").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    private void disableCurrentTool()
    {
        if(currentTool!=null)
        {
            currentTool.disable();
            destroyCurrentToolIcon();
        }
    }

    private void enableCurrentTool ()
    {
        if(currentTool != null)
        {
            currentTool.enable();
            showCurrentTool();
        }
    }


    /*
    private void disableCurrentTool()
    {
        disableCurrentToolScripts();
        destroyCurrentToolIcon();
    }

    private void disableCurrentToolScripts()
    {
        if (currentTool == getToolName(Tool.CATALOG))
        {
            this.GetComponent<CatalogHandler>().enabled = false;

            this.GetComponent<LineRenderer>().enabled = false;
            this.GetComponent<LaserHandler>().enabled = false;
        }

        if (currentTool == getToolName(Tool.HAND))
        {
            this.GetComponent<GrabHandler>().enabled = false;
        }

        if (currentTool == getToolName(Tool.PROPULSER))
        {
            this.GetComponent<PropulserHandler>().enabled = false;
            
            GameObject.Find("Player").GetComponent<Rigidbody>().mass = 10000;
        }

        if (currentTool == getToolName(Tool.TELEPORTER))
        {
            this.GetComponent<TeleporterHandler>().enabled = false;

            this.GetComponent<LineRenderer>().enabled = false;
            this.GetComponent<LaserHandler>().enabled = false;
        }

        if (currentTool == getToolName(Tool.TRASH))
        {
            this.GetComponent<TrashHandler>().disable();
            this.GetComponent<TrashHandler>().enabled = false;

            this.GetComponent<LineRenderer>().enabled = false;
            this.GetComponent<LaserHandler>().enabled = false;
        }
    }

    private void enableCurrentToolScript()
    {
        disableAllScripts();

        if (currentTool == getToolName(Tool.CATALOG))
        {
            this.GetComponent<CatalogHandler>().enabled = true;
            this.GetComponent<CatalogHandler>().enable();

            this.GetComponent<LaserHandler>().enabled = true;
            this.GetComponent<LineRenderer>().enabled = true;

            this.GetComponent<LaserHandler>().setColor(Color.magenta);
        }

        if (currentTool == getToolName(Tool.HAND))
        {
            this.GetComponent<GrabHandler>().enabled = true;
            this.GetComponent<GrabHandler>().enable();
        }

        if (currentTool == getToolName(Tool.PROPULSER))
        {
            this.GetComponent<PropulserHandler>().enabled = true;
            GameObject.Find("Player").GetComponent<Rigidbody>().mass = 1;
        }

        if (currentTool == getToolName(Tool.TELEPORTER))
        {
            this.GetComponent<TeleporterHandler>().enabled = true;
            
            this.GetComponent<LineRenderer>().enabled = true;
            this.GetComponent<LaserHandler>().enabled = true;

            this.GetComponent<LaserHandler>().setColor(Color.green);
        }

        if (currentTool == getToolName(Tool.TRASH))
        {
            this.GetComponent<TrashHandler>().enabled = true;

            this.GetComponent<LineRenderer>().enabled = true;
            this.GetComponent<LaserHandler>().enabled = true;
            
            this.GetComponent<LaserHandler>().setColor(Color.red);

        }
    }

    private void disableAllScripts()
    {

        this.GetComponent<CatalogHandler>().enabled = false;
        this.GetComponent<CatalogHandler>().disable();

        this.GetComponent<GrabHandler>().enabled = false;
        this.GetComponent<GrabHandler>().disable();

        this.GetComponent<PropulserHandler>().enabled = false;
        this.GetComponent<TeleporterHandler>().enabled = false;
        this.GetComponent<TrashHandler>().enabled = false;

        this.GetComponent<LaserHandler>().enabled = false;
        this.GetComponent<LineRenderer>().enabled = false;

    }
    */

    public ToolsHandler getCurrentTool()
    {
        return currentTool;
    }

    private bool isChoosingTool()
    {
        return choosingTool;
    }
    
    public ControllerManager getSecondController()
    {
        return secondController;
    }

}
