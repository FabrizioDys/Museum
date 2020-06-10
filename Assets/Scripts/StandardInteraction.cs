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

public class StandardInteraction : InteractDesk
{
    public bool interacting = false;
    public bool free = false;
    public bool continueCoroutine = true;
    private bool alreadyDownloaded = false;
    private List<ModelData> modelsData;
    private GameObject selectedObject;
    private ModelImporter modelImporter;
    private Interactable interactable;
    public bool VRreturning = false;
    public bool checki = false;

    //private string objFileName = "ftp://projetvr:9tUv55WX.@iutbg-lacielp.univ-lyon1.fr:21/projetvr/";
    private string objFileName = "https://perso.liris.cnrs.fr/guillaume.lavoue/Textured/TigerFighter.obj/obj/TigerFighter.obj";
    public Material standardMaterial;   // The shader for non-transparent objects is supplied by this material. Also used for objects that have no MTL file.
    ObjReader.ObjData objData;
    string loadingText = "";
    bool loading = false;
    // Start is called before the first frame update
    void Start()
    {
        selectedObject = null;
        interactable = GetComponent<Interactable>();
        modelImporter = new ModelImporter();
        modelsData = new List<ModelData>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        { 
            interacting = true;
            Interact();
        }
     }

    public override void Interact()
    {
        if (Input.GetKeyDown(KeyCode.I) || interacting==true)
        {
            interacting = false;
            if (alreadyDownloaded) { } //maybe already Downloading?
            else
            {
                string modelName = this.transform.GetComponent<pieceOfArt>().artContent;
                objFileName += modelName;
                //Task obTask = Task.Run(() => Load());
                StartCoroutine(Load());
                StartCoroutine(check());
                Debug.Log("{1} " + free);

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
               // Carica();
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

            Debug.Log("UNO");
            stW.Start();
            objData = ObjReader.use.ConvertFileAsync(objFileName, true, standardMaterial);
            Debug.Log("DUE");
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
            Debug.Log("QUATTRO");
            loading = false;
            if (objData == null || objData.gameObjects == null)
            {
                Debug.Log("Errore?");
                loadingText = "Error loading file";
                yield return null;
                yield break;
            }
            Debug.Log("CINQUE");
            loadingText = "Import completed";
            free = true;
            Debug.Log("SEI");
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Debug.Log("RunTime " + elapsedTime);
            FocusOnObjects();
        }
    }


    /*public void Carica()
    {
        if (this.free == true)
        {
            Debug.Log("{1} I accessed the if condition " + free);
            GameObject obg = this.transform.GetChild(0).transform.GetChild(0).gameObject;
            obg.SetActive(true);
            Debug.Log("{1} I'm leaving");
            Debug.Log("{1} I've released the Semaphore");
            Debug.Log(free);
            alreadyDownloaded = true;
        }
    }*/



    void FocusOnObjects()
    {
        //problems: 1-The cylinder is moved and the stand disappeared 2-Need to regroup all parts of imported object and make them child of the stand 3- Maybe it stops while downloading, resee Task instead of Coroutine
        var bounds = new Bounds(objData.gameObjects[0].transform.position, Vector3.zero);
        for (var i = 0; i < objData.gameObjects.Length; i++)
        {
            bounds.Encapsulate(objData.gameObjects[i].GetComponent<Renderer>().bounds);
           // objData.gameObjects[i].transform.position = this.transform.parent.GetChild(1).GetChild(0).transform.position + new Vector3(0, 1f, 0);
            objData.gameObjects[i].transform.parent = this.transform.parent.GetChild(1).GetChild(0).transform;
            objData.gameObjects[i].transform.localPosition = new Vector3(0,0.2f,0);
            objData.gameObjects[i].transform.localRotation = new Quaternion(-0.7071068f, 0, 0, 0.7071068f);
            objData.gameObjects[i].transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        }

        /* var cam = Camera.main;
         var bounds = new Bounds(objData.gameObjects[0].transform.position, Vector3.zero);
         for (var i = 0; i < objData.gameObjects.Length; i++)
         {
             bounds.Encapsulate(objData.gameObjects[i].GetComponent<Renderer>().bounds);
         }

         var maxSize = bounds.size;
         var radius = maxSize.magnitude / 2.0f;
         var horizontalFOV = 2.0f * Mathf.Atan(Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2.0f) * cam.aspect) * Mathf.Rad2Deg;
         var fov = Mathf.Min(cam.fieldOfView, horizontalFOV);
         var distance = radius / Mathf.Sin(fov * Mathf.Deg2Rad / 2.0f);

         cam.transform.position = bounds.center;
         cam.transform.Translate(-Vector3.forward * distance);
         cam.transform.LookAt(bounds.center);*/
    }


    /*     
     
      public async void Load()
    {
        await Task.Delay(500);
       loading = true;

        if (objData != null && objData.gameObjects != null)
        {
            for (var i = 0; i < objData.gameObjects.Length; i++)
            {
                Destroy(objData.gameObjects[i]);
            }
        }
          Debug.Log("Parte1");
        objData = ObjReader.use.ConvertFileAsync(objFileName, true, standardMaterial);
      
        while (!objData.isDone)
        {
            loadingText = "Loading... " + (objData.progress * 100).ToString("f0") + "%";
            Debug.Log(loadingText);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                objData.Cancel();
                loadingText = "Cancelled download";
                loading = false;
                break;
            }
            await Task.Delay(500);
        }
        loading = false;
        if (objData == null || objData.gameObjects == null)
        {
            loadingText = "Error loading file";
            return;
        }

        loadingText = "Import completed";
        free = true;
        FocusOnObjects();
    }*/
}
