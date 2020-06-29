using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using System;
using UnityEngine.AI;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class StartObjSimulation : MonoBehaviour
{
    public bool free = false;
    public bool continueCoroutine = true;
    private bool alreadyDownloaded = false;
    private List<ModelData> modelsData;
    private GameObject selectedObject;
    private ModelImporter modelImporter;
    public bool VRreturning = false;
    public bool checki = false;
    [SerializeField] public string artContent;


    private string objFileName = "https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/";
    public Material standardMaterial;   // The shader for non-transparent objects is supplied by this material. Also used for objects that have no MTL file.
    ObjReader.ObjData objData;
    string loadingText = "";
    bool loading = false;

    [SerializeField] private string obj;
    bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        selectedObject = null;
        modelImporter = new ModelImporter();
        modelsData = new List<ModelData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Go();
        }
    }

    private void Go()
    {
        if (done)
        {
          
        }
        else
        {
            done = true;

            if (alreadyDownloaded) { } //maybe already Downloading?
            else
            {
                string modelName = artContent;
                objFileName += modelName;
                StartCoroutine(Load());
                StartCoroutine(check());

            }
        }
    }

    IEnumerator check()
    {
        while (continueCoroutine)
        {
            if (free == true)
            {
                continueCoroutine = false;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Load()
    {
        Stopwatch stW = new Stopwatch();
        {

            if (!checki)
            {
                checki = true;
                yield return new WaitForSeconds(5f);
            }
            loading = true;
            if (objData != null && objData.gameObjects != null)
            {
                for (var i = 0; i < objData.gameObjects.Length; i++)
                {
                    Destroy(objData.gameObjects[i]);
                }
            }

            stW.Start();
            objData = ObjReader.use.ConvertFileAsync(objFileName, false);
            while (!objData.isDone)
            {
                loadingText = "Loading... " + (objData.progress * 100).ToString("f0") + "%";
                Debug.Log(loadingText);
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    objData.Cancel();
                    loadingText = "Cancelled download";
                    loading = false;
                    yield break;
                }
                yield return null;
            }
            stW.Stop();
            TimeSpan ts = stW.Elapsed;
            loading = false;
            if (objData == null || objData.gameObjects == null)
            {
                Debug.Log("Errore?");
                loadingText = "Error loading file";
                yield return null;
                yield break;
            }
            loadingText = "Import completed";
            free = true;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Debug.Log("RunTime " + elapsedTime);
            FocusOnObjects();
        }
    }

    void FocusOnObjects()
    {
        var bounds = new Bounds(objData.gameObjects[0].transform.position, Vector3.zero);
        for (var i = 0; i < objData.gameObjects.Length; i++)
        {
            bounds.Encapsulate(objData.gameObjects[i].GetComponent<Renderer>().bounds);
            objData.gameObjects[i].transform.parent = this.transform;
            objData.gameObjects[i].transform.localPosition = new Vector3(-8f, 0.2f, 4f);
            objData.gameObjects[i].transform.localRotation = new Quaternion(-0.7071068f, 0, 0, 0.7071068f);
            objData.gameObjects[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        Material matmat = new Material(Shader.Find("Standard"));
        if (obj == "Lion")
        {
            matmat.name = "Lion";
            Texture texture = Resources.Load<Texture>("Lion/AsparnLoewe_C_AsparnLoewe_O_Material_u1_v1");
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = matmat;
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        }
        if (obj == "Horse")
        {
            matmat.name = "Horse";
            Texture texture = Resources.Load<Texture>("Lion/AsparnLoewe_C_AsparnLoewe_O_Material_u1_v1");
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = matmat;
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        }
        if (obj == "BronzeCat")
        {
            matmat.name = "BronzeCat";
            Texture texture = Resources.Load<Texture>("Cat/model");
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = matmat;
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        }
        if (obj == "Putti")
        {
            matmat.name = "Putti";
            Texture texture = Resources.Load<Texture>("Putti/Putti_Burggarten_C_Putti_Burggarten_O_Material_u1_v1");
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = matmat;
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        }
        if (obj == "Sphinx")
        {
            matmat.name = "Sphinx";
            Texture texture = Resources.Load<Texture>("Sphinx/Sphinx2_C_Sphinx2_Material_u1_v1");
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = matmat;
            this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        }

    }

}
