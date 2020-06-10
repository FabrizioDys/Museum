using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelData{

    private string name;
    private string miniature;
    private string filename;
    private int indexInList;

    public ModelData(string name, string miniature, string filename, int indexInList)
    {
        this.name = name;
        this.miniature = miniature;
        this.filename = filename;
        this.indexInList = indexInList;
    }

    public string getName()
    {
        return name;
    }

    public string getMiniature()
    {
        return miniature;
    }

    public string getFilename()
    {
        return filename;
    }

    public int getIndexInList()
    {
        return indexInList;
    }







}
