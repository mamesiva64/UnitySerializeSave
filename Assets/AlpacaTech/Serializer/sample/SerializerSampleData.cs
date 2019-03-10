using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializerSampleData
{
    public string name = "test";
    public int value = 100;
    public float value2 = 3.141592f;
    public List<int> values;
    public List<List<int>> valueLists;  //  多重配列
};