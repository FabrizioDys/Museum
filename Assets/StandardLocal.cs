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

public class StandardLocal : InteractDesk
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

    /*
     FilePaths to use in objFileName:
     1- Borderlands Cosplay: /BorderlandsCosplay/Borderlands_cosplay-obj.obj.txt
     2- Classic Side Table: /ClassicSideTable/Classic_side_table.obj.txt
     3- Deathstroke: /Deathstroke/Deathstroke-obj.obj.txt
     4- Frog: /Frog/Frog_obj.txt
     5- Giraffe: /Giraffe/Creature.obj.txt
     6- Napoleon: /Napoleon/Napoleon.obj.txt
     7- DragonFly: /DragonFly/Dragonfly.obj.txt
     7- Tiger Fighter: /TigerFighter/TigerFighter.obj.txt

         
         */

    private string objFileName = "/DragonFly/Dragonfly.obj.txt";
    public Material standardMaterial;   // The shader for non-transparent objects is supplied by this material. Also used for objects that have no MTL file.
    public Material transparentMaterial;
    string loadingText = "";
    bool loading = false;
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
        if (Input.GetKeyDown(KeyCode.I) || interacting == true)
        {
            interacting = false;
            if (alreadyDownloaded) { } //maybe already Downloading?
            else
            {
                
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
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Load()
    {
        Stopwatch stW = new Stopwatch();
        {

            stW.Start();
                loadingText = "Loading...";
                yield return null;
                yield return null;

                objFileName = Application.dataPath + "/Resources/" + objFileName;

                ObjReader.use.ConvertFile(objFileName, true, standardMaterial, transparentMaterial);
           

                loadingText = "";

            stW.Stop();
            TimeSpan ts = stW.Elapsed;
          /*  GameObject a = GameObject.Find("SculptMaterial");
            a.transform.localScale = new Vector3(0.045f, 0.045f, 0.045f);
            a.transform.position = new Vector3(5.8f, 1.5f, 8.5f);*/

            free = true;
            
           
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Debug.Log("RunTime Uncompressed" + elapsedTime);
               }
    }


   


}
