using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using System.Security.Cryptography;
using UnityEngine.Events;
using System.Net.Sockets;

public class PipeBud : MonoBehaviour
{
    public byte[] Rc;
    public UnityEvent readDone;
    [SerializeField] private int request;
    [SerializeField] private string pipeName = "mynamedpipe";
   
    public int nOfReads = 0; //connect event to let the other script know then Rc is updated
    private bool shutdown = true;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    public void startExperiment()
    {
        shutdown = false;
    }

    IEnumerator PipeClient()
    {
        using (var client = new NamedPipeClientStream(pipeName))
        {


            client.Connect(100);



            var writer = new StreamWriter(client);
            //var request = 0;
            writer.WriteLine(request);
            writer.Flush();

            byte[] buffer = new byte[2600000];

            int bytesRead = client.Read(buffer, 0, 2600000); 


            if (bytesRead > 0)
            {
                Rc = new byte[bytesRead]; //It has the data I want
                Buffer.BlockCopy(buffer, 0, Rc, 0, bytesRead); //here to add the missing pieces

                /*using (BinaryWriter binWriter = new BinaryWriter(File.Open("C:/Users/fabro/Desktop/provolettaCS" + s + ".p3d", FileMode.Create)))
                {
                    binWriter.Write(Rc);
                    binWriter.Close();
                }*/
                nOfReads++;
                readDone.Invoke();
                Debug.Log(nOfReads);
                buffer.Initialize();
                // client.Close(); //closing does the operations 2 at a time, doing like this won't fulfill all requests
            }

            /*  if (nOfReads == 4)
              {
                  client.Close();
              }*/
            yield return new WaitForSeconds(.5f);
        }
    }



    // Update is called once per frame
    void Update()
    {
        Debug.Log(pipeName);
        if(!shutdown)
            StartCoroutine("PipeClient");
        if (Input.GetKeyDown(KeyCode.X) || nOfReads == 6)
        {
            StopCoroutine("PipeClient");
            shutdown = true;
        }
    }
}
