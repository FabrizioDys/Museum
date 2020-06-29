using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartFullSimulation : MonoBehaviour
{
    public UnityEvent startF;
    [SerializeField] private string obj;
    bool done = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!done)
        {
            startF.Invoke();
            done = true;
        }
    }
}
