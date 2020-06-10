using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportExample : MonoBehaviour {

	public string toImport = "sword.obj";
	// Use this for initialization
	void Start () {
        ModelImporter mI = new ModelImporter ();
		GameObject importedObject = mI.ImportModel (toImport);
	}

	// Update is called once per frame
	void Update () {

	}
}