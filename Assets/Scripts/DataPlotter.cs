using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPlotter : MonoBehaviour
{
    public string inputFile;
    public GameObject pointPrefab;
    public GameObject pointContainer;

    public float plotScale = 10;

    private List<Dictionary<string,object>> pointList;

    // Start is called before the first frame update
    void Start()
    {
        pointList = CSVReader.Read(inputFile);
        Debug.Log(pointList);

        // Declare list of strings, fill with keys (column names)
        List<string> columnList = new List<string>(pointList[1].Keys);
        
        // Print number of keys (using .count)
        Debug.Log("There are " + columnList.Count + " columns in CSV");
        
        foreach (string key in columnList)
        {
            Debug.Log("Column name is " + key);
        }

        for (var i = 0; i < pointList.Count; i++)
        {
            // Get value in pointList at ith "row", in "column" Name
            float x = System.Convert.ToSingle(pointList[i]["MagnetX"]);
            float y = System.Convert.ToSingle(pointList[i]["MagnetY"]);
            float z = System.Convert.ToSingle(pointList[i]["MagnetZ"]);
            
            // create new data point object and parent it to point container
            GameObject dataPoint = Instantiate(pointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
            dataPoint.transform.parent = pointContainer.transform;
            dataPoint.transform.name = $"{x}, {y}, {z}";

            // Gets material color and sets it to a new RGBA color we define
            dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);
        }
    }
}
