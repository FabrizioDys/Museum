using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolsHandler 
                    : MonoBehaviour
{

    protected string toolName;
    protected GameObject toolIcon;

    protected bool enabled;

    public abstract void enable();
    public abstract void disable();

    void Awake()
    {
        toolName = this.GetType().Name.Replace("Handler","");
        fetchToolIcon();
    }

    private void fetchToolIcon()
    {
        toolIcon = Instantiate(GameObject.Find(toolName + "Icon"));
    }

    public GameObject getToolIcon()
    {
        return toolIcon;
    }

    public string getToolName()
    {
        return toolName;
    }
}
