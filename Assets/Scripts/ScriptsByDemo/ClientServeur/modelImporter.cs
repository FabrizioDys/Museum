using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Net;
//using UnityExtension;

public class ModelImporter{
    public string FTPID = "projetvr";
    public string FTPPassword = "9tUv55WX.";
	public string serverIP = "iutbg-lacielp.univ-lyon1.fr";
    public string scriptPath = "C:\\Users\\God builder\\Desktop\\Client Serveur\\CLIENTSERVEUR\\script.txt"; //path of the "script.txt" file, which is used to run the ftp script - Maybe Deprecated
	public string whereToRegister = "C:\\Users\\God builder\\Desktop\\Client Serveur\\CLIENTSERVEUR\\ClientModels";
    public string COREFTPPATH = Application.dataPath + "\\Scripts\\ClientServeur\\coreftp.exe";

    /** Imports a .obj file from the server defined in attributes, and inserts it in game
	 * @params filePathInServerDirectory : The path of the file you want to import (from the server directory).
	 * @return the object created
	 */
    public GameObject ImportModel(string filePathInServerDirectory)
	{
        /*ImportViaCoreFTP ("\\projetvr\\" + filePathInServerDirectory);
        float timeAtLaunch = Time.realtimeSinceStartup;
        while (!File.Exists (whereToRegister + "\\" + filePathInServerDirectory ) && Time.realtimeSinceStartup - timeAtLaunch < 10) {}
        return ImportObj (filePathInServerDirectory);*/

        string fileContent = ImportViaNETFTP(filePathInServerDirectory);
        float timeAtLaunch = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - timeAtLaunch < 3) { }
        GameObject toReturn = ImportObjByContent(fileContent, filePathInServerDirectory);
        return toReturn;
	}


	private GameObject ImportObj(string fileName)
	{
        System.IO.StreamReader streamReader = new System.IO.StreamReader("C:\\Users\\God builder\\Desktop\\Client Serveur\\CLIENTSERVEUR\\ClientModels\\" + fileName);
        string c = streamReader.ReadToEnd();
        GameObject toReturn = ObjImporter.Import(c);
        toReturn.name = fileName;

        return toReturn;
	}

    private GameObject ImportObjByContent(string fileContent, string fileName)
    {
        GameObject toReturn = ObjImporter.Import(fileContent);
        toReturn.name = fileName;

        return toReturn;
    }

    private void setObjectNameInScriptFile(string objectName)
    {
        string[] toWrite = {
            "@echo off",
            "open",
            serverIP,
            FTPID,
            FTPPassword,
            "get",
            objectName,
            "\""+whereToRegister/*+"\\"*/ + objectName+"\"",
            "quit"};
        System.IO.File.WriteAllLines(scriptPath, toWrite);
    }

    private string ImportViaNETFTP(string filename)
    {
        FtpWebRequest request = 
        (FtpWebRequest)WebRequest.Create("ftp://" + serverIP 
                                    + "/projetvr/" + filename);
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        request.Credentials = new NetworkCredential(FTPID, FTPPassword);

        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);
        string toReturn = reader.ReadToEnd();

        UnityEngine.Debug.Log(toReturn);

        return toReturn;
    }

    private void ImportViaCoreFTP(string filename)
    {
        Process fileTransferProcess = new Process();
        fileTransferProcess.StartInfo.FileName = COREFTPPATH;
        fileTransferProcess.StartInfo.Arguments = "-s -O -site " + serverIP + "-d /" + filename + "-p " + whereToRegister;
        Process.Start(fileTransferProcess.StartInfo);
    }
		
	private void ImportViaFTP (string fileName) {
        setObjectNameInScriptFile(fileName);

		Process fileTransferProcess = new Process();
		fileTransferProcess.StartInfo.FileName = "ftp";
		fileTransferProcess.StartInfo.Arguments = "-s:\"" + scriptPath + "\"";
        Process.Start (fileTransferProcess.StartInfo);

    }

}

