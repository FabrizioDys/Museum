using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateObject : MonoBehaviour
{

    private GameObject selectedObject;
    private ModelImporter modelImporter;
    // Start is called before the first frame update
    void Start()
    {
     selectedObject = null;
     modelImporter= new ModelImporter();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void importObjectAndSelectIt(string modelName)
    {
        if (selectedObject != null)
            Destroy(selectedObject);
        selectedObject = modelImporter.ImportModel(modelName);
        while (selectedObject.transform.lossyScale.magnitude > 0.005f)
            selectedObject.transform.localScale *= 0.8f;
        selectedObject.transform.position = this.transform.position + new Vector3(0, 0.05f, 0);
        selectedObject.transform.parent = this.transform;
    }

}
