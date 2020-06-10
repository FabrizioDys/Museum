using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class startSimulation : MonoBehaviour
{
    public UnityEvent start;
    bool done = false;
    [SerializeField] private string obj;
    private string httpInfo;
  

    Process process = new Process
    {

        StartInfo = new ProcessStartInfo
        {
            FileName = "C:\\Users\\fabro\\MEPP2\\build\\Testing\\boost\\beast\\http\\client\\async-ssl\\Release\\http-client-async-ssl.exe",
            Arguments = "null", //perso.liris.cnrs.fr 443 /guillaume.lavoue/Compressed/LionLod.drc.bytes Lion
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

   

    // Start is called before the first frame update
    void Start()
    {
        httpInfo = "perso.liris.cnrs.fr 443 /guillaume.lavoue/Compressed/" + obj + "Lod.drc.bytes " + obj;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!done)
        {
            start.Invoke();
            done = true;


            process.StartInfo.Arguments = httpInfo;
            process.Start();
        }
    }

    private void OnApplicationQuit()
    {
        process.Kill();
       
    }
}
