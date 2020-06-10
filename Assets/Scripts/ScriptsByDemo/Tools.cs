using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ControllerManager {

    /*private enum Tool { TELEPORTER = 0, PROPULSER = 1, HAND = 2, TRASH = 3, CATALOG = 4 };
    private List<GameObject> toolGameObjects;

    private void initToolAssets()
    {
        for (int i = 0; i < numberOfTool(); i++)
        {
            setToolAsset((Tool)i,i);
        }

    }

    private void hideToolAsset(int tool)
    {
        GameObject toolAsset = toolGameObjects[tool];
        toolAsset.SetActive(false);
    }

    private void showToolAsset(int tool)
    {

        GameObject toolAsset = toolGameObjects[tool];
        
        toolAsset.SetActive(true);
    }

    private void moveToolAsset(int tool, float xTransform, float yTransform, float zTransform)
    {
        GameObject toolAsset = toolGameObjects[tool];
        toolAsset.transform.position = menu.transform.position+ new Vector3(xTransform, yTransform, zTransform);

    }


    private int numberOfTool()
    {
        return Enum.GetNames(typeof(Tool)).Length;
    }

    String getToolAssetName(Tool tool)
    {
        return getCamelCase(Enum.GetName(typeof(Tool), tool))+"Icon";
    }

    private String getCamelCase(String toCamelCase)
    {
        String result = toCamelCase.Substring(0, 1).ToUpper();
        result += toCamelCase.Substring(1, toCamelCase.Length - 1).ToLower();
        return result;

    }

    private int getToolAssetIndice(string ToolAssetName)
    {
        for(int i = 0; i < toolGameObjects.Count; i++)
        {
            if (toolGameObjects[i].name == ToolAssetName)
            {
                return i;
            }
        }
        return -1;
    }

    private String getToolName(Tool tool)
    {
        return (Enum.GetName(typeof(Tool), tool));
    }

   

    public void updateCurrentToolPosition()
    {
        if (currentTool != null) {
            if (vrMode)
            {
                
            }
            else
            {
                
            }
        }
    }

    private GameObject getToolAsset(String tool)
    {
        return toolGameObjects[getToolAssetIndice(getToolAssetName((Tool)Enum.Parse(typeof(Tool), tool)))];
    }*/
}
