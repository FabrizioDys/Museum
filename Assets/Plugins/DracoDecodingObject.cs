// Copyright 2017 The Draco Authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//



/* Note: You need to add MeshFilter (and MeshRenderer) to your GameObject.
 * Or you can do something like the following in script:
 * AddComponent<MeshFilter>();
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using UnityEditor;
using System.IO;
using System.IO.Pipes;
using UnityEngine.Events;

public class DracoDecodingObject : MonoBehaviour
{
    private Texture texture;
    [SerializeField] private string obj;
    [SerializeField] private string httpReq;


    // This function will be used when the GameObject is initialized.
    void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            Iniziamo();
        if (Input.GetKeyDown(KeyCode.I))
            IniziamoInternet();
        if (Input.GetKeyDown(KeyCode.P))
            PipeCommun();
    }


    public void PipeCommun()
    {

       byte[] drcMesh = gameObject.GetComponent<PipeBud>().Rc;
        StartCoroutine(GetLod(drcMesh));

    }

    private IEnumerator GetLod(byte[] drcMesh)
    {
        List<Mesh> mesh = new List<Mesh>();
        DracoMeshLoader dracoLoader = new DracoMeshLoader();
        Stopwatch stW = new Stopwatch();
        stW.Start();

        int numFaces = dracoLoader.LoadMeshFromInternet(ref mesh, drcMesh);
        if (numFaces > 0)
        {
            GetComponent<MeshFilter>().mesh = mesh[0];
            //gameObject.transform.localScale = new Vector3(5f, 5f, 5f);
           // gameObject.transform.localRotation = new Quaternion(-0.7071f, 0, 0, 0.7071f);
           // gameObject.transform.localRotation = new Quaternion(0, -0.7071f, 0, 0.7071f);
            Material matmat = new Material(Shader.Find("Standard"));
            if (obj == "Lion")
            {
                matmat.name = "Lion";
                Texture texture = Resources.Load<Texture>("Lion/AsparnLoewe_C_AsparnLoewe_O_Material_u1_v1");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
            if (obj == "Horse")
            {
                matmat.name = "Horse";
                Texture texture = Resources.Load<Texture>("Lion/AsparnLoewe_C_AsparnLoewe_O_Material_u1_v1");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
            if (obj == "BronzeCat")
            {
                matmat.name = "BronzeCat";
                Texture texture = Resources.Load<Texture>("Cat/model");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
            if (obj == "Putti")
            {
                matmat.name = "Putti";
                Texture texture = Resources.Load<Texture>("Putti/Putti_Burggarten_C_Putti_Burggarten_O_Material_u1_v1");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
            if (obj == "Sphinx")
            {
                matmat.name = "Sphinx";
                Texture texture = Resources.Load<Texture>("Sphinx/Sphinx2_C_Sphinx2_Material_u1_v1");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
        }

        stW.Stop();
        TimeSpan ts = stW.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        ts.Hours, ts.Minutes, ts.Seconds,
        ts.Milliseconds / 10);
        Debug.Log("RunTime Draco Level Of Details " + elapsedTime);
        yield return null;
    }
    /*
     Riferimenti per LoadMeshFromAsset:
         1- BorderlandsCosplay/Borderlands16qp.drc
         2- ClassicSideTable/Classic16qp.drc
         3- Deathstroke/Deathstroke16qp.drc
         4- Frog/Frog16qp.drc
         5- Giraffe/Giraffe16qp.drc
         6- Napoleon/Napoleon16qp.drc
         7- Dragonfly/Dragonfly16qp.drc
         8- TigerFighter/TigerFighter16qp.drc
     Riferimenti per Caricamento Texture:
         1- BorderlandsCosplay/Borderlands_cosplay-obj_0
         2- ClassicSideTable/Classic_side_table_0
         3- Deathstroke/Deathstroke-obj_0
         4- Frog/Frog
         5- Giraffe/Creature
         6- Napoleon/Napoleon_0
         7- Dragonfly/Dragonfly_0
         8- TigerFighter/TigerFighter_0
             */
    void Iniziamo()
    {
        Stopwatch stW = new Stopwatch();
        List<Mesh> mesh = new List<Mesh>();
        DracoMeshLoader dracoLoader = new DracoMeshLoader();
       
        stW.Start();
        int numFaces = dracoLoader.LoadMeshFromAsset("Lion/LionFull.drc", ref mesh);
       
        if (numFaces > 0)
        {
            var verti =  mesh[0].vertices;

            for (var v = 0; v < verti.Length; v++)
            {
                verti[v].z -= 10f;
            }
            GetComponent<MeshFilter>().mesh = mesh[0];
            GetComponent<MeshFilter>().mesh.name = "Lion";

            

 
           

           // gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
           // gameObject.transform.localRotation = new Quaternion(-0.7071f, 0, 0, 0.7071f);
            Material matmat = new Material(Shader.Find("Standard"));
            matmat.name = "Lion";
            Texture texture = Resources.Load<Texture>("Lion/AsparnLoewe_C_AsparnLoewe_O_Material_u1_v1");
            gameObject.GetComponent<Renderer>().material = matmat;
            gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        }

        stW.Stop();
        TimeSpan ts = stW.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        ts.Hours, ts.Minutes, ts.Seconds,
        ts.Milliseconds / 10);
        Debug.Log("RunTime Draco" + elapsedTime);
    }

    /*
    Riferimenti per LoadMeshFromInternet:
        1- https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/Borderlands16qp.drc.bytes
        2- https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/Classic16qp.drc.bytes
        3- https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/Deathstroke16qp.drc.bytes
        4- https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/Frog16qp.drc.bytes
        5- https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/Giraffe16qp.drc.bytes
        6- https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/Napoleon16qp.drc.bytes
        7- https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/Dragonfly16qp.drc.bytes
        8- https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/TigerFighter16qp.drc.bytes
*/
   public void IniziamoInternet()
    {
       // string a = "https://perso.liris.cnrs.fr/guillaume.lavoue/Compressed/Napoleon16qp.drc.bytes";  
        StartCoroutine(GetWWWFile(httpReq));
    }

    private IEnumerator GetWWWFile(string a)
    {
       
        List<Mesh> mesh = new List<Mesh>();
        DracoMeshLoader dracoLoader = new DracoMeshLoader();
        Stopwatch stW = new Stopwatch();
        stW.Start();


        UnityWebRequest www = new UnityWebRequest(a);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SendWebRequest();
        
        while (!www.isDone)
        {
           
            Debug.Log("Loading... " + (www.downloadProgress * 100).ToString("f0") + "%");
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                yield break;
            }
            yield return null;
        }
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Error loading: " + www.error);
               yield break;
        }

        byte[] asset = www.downloadHandler.data;
        // TextAsset ta = ConvertStringToTextAsset(asset);

        if (asset == null)
        {
            Debug.Log("Didn't load file!");
            yield break;
        }
                
        int numFaces = dracoLoader.LoadMeshFromInternet(ref mesh,asset);

        if (numFaces > 0)
        {
            GetComponent<MeshFilter>().mesh = mesh[0];
            Material matmat = new Material(Shader.Find("Standard"));
            if (obj == "Lion")
            {
                matmat.name = "Lion";
                Texture texture = Resources.Load<Texture>("Lion/AsparnLoewe_C_AsparnLoewe_O_Material_u1_v1");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
            if (obj == "Horse")
            {
                matmat.name = "Horse";
                Texture texture = Resources.Load<Texture>("Lion/AsparnLoewe_C_AsparnLoewe_O_Material_u1_v1");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
            if (obj == "BronzeCat")
            {
                matmat.name = "BronzeCat";
                Texture texture = Resources.Load<Texture>("Cat/model");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
            if (obj == "Putti")
            {
                matmat.name = "Putti";
                Texture texture = Resources.Load<Texture>("Putti/Putti_Burggarten_C_Putti_Burggarten_O_Material_u1_v1");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
            if (obj == "Sphinx")
            {
                matmat.name = "Sphinx";
                Texture texture = Resources.Load<Texture>("Sphinx/Sphinx2_C_Sphinx2_Material_u1_v1");
                gameObject.GetComponent<Renderer>().material = matmat;
                gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            }
        }

        stW.Stop();
        TimeSpan ts = stW.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        ts.Hours, ts.Minutes, ts.Seconds,
        ts.Milliseconds / 10);
        Debug.Log("RunTime Draco " + elapsedTime);
    }


    TextAsset ConvertStringToTextAsset(string text)
    {
        string temporaryTextFileName = "Borderlands16qp";
        File.WriteAllText(Application.dataPath + "/Resources/" + temporaryTextFileName + ".drc.bytes", text);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        TextAsset textAsset = Resources.Load(temporaryTextFileName+".drc", typeof(TextAsset)) as TextAsset;
        return textAsset;
    }
}
 