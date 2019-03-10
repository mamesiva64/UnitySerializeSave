using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SerializerSample : MonoBehaviour
{
    public SerializerSampleData data;
    [SerializeField] string xmlPath = "test.xml";
    [SerializeField] string jsonPath = "test.json";
    [SerializeField] Text textJson;
    [SerializeField] Text textXML;

    void Start()
    {
        //  ArrayTest
        data.values = new List<int>() { 2, 3, 5, 7, 11, 13 };
        data.valueLists = new List<List<int>>();
        data.valueLists.Add(new List<int>() { 1, 3, 5, 7 });
        updateData();
    }


    void updateData()
    {
        //  Json
        textJson.text = JsonUtility.ToJson(data);
//      data = JsonUtility.FromJson<SerializerSampleData>(textJson.text);

        //  XML
        using (var sw = new StringWriter())
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SerializerSampleData));
            serializer.Serialize(sw, data);
            textXML.text = sw.ToString();
        }
//        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Type));
//        var data = (SerializerSampleData)serializer.Deserialize(text.text);
    }


    /// <summary>
    /// 
    /// </summary>
    public void OnSave()
    {
        AlpacaTech.Serializer.SaveToFile<SerializerSampleData>(xmlPath, data);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnLoad()
    {
        data = AlpacaTech.Serializer.LoadFromFile<SerializerSampleData>(xmlPath);
        updateData();
    }

    public void OnAdd()
    {
        data.value += 10;
        updateData();

    }


}


