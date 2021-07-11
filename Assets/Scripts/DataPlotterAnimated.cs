using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DataPlotterAnimated : MonoBehaviour
{
    public string inputFile;
    public GameObject pointPrefab;
    public GameObject pointContainer;
    public GameObject particleTail;

    public float magnetScale = 10;
    public float accelScale = 1;

    private List<Dictionary<string,object>> pointList;

    private Vector3[] magnetPoints;
    private Vector3[] accelerationPoints;
    private GameObject magnetObject;
    private GameObject accelObject;
    public float animationSpeed = 2.0f;

    public CinemachineVirtualCamera virtualCamera;

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

        magnetPoints = new Vector3[pointList.Count];
        accelerationPoints = new Vector3[pointList.Count];

        // make a copy of the particle tail so we can use it for the 2nd object
        var accelParticleTail = Instantiate(particleTail, new Vector3(0, 0, 0), Quaternion.identity);


        // create new data point object and parent it to point container
        // this will hold magnet animation
        magnetObject = Instantiate(pointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        magnetObject.transform.parent = pointContainer.transform;
        magnetObject.transform.name = "MagneticFieldStrength";
        // magnetObject.GetComponent<Renderer>().material.color =  Color.green;

        // add 1st particle tail as child of magnet obj
        particleTail.transform.parent = magnetObject.transform;

        // this will hold accel animation
        accelObject = Instantiate(pointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        accelObject.transform.parent = pointContainer.transform;
        accelObject.transform.name = "Accel";        
        // accelObject.GetComponent<Renderer>().material.color =  Color.blue;

        // add particle tail copy as child of accel obj
        accelParticleTail.transform.parent = accelObject.transform;

        virtualCamera.LookAt = magnetObject.transform;
        virtualCamera.Follow = magnetObject.transform;

        for (var i = 0; i < pointList.Count; i++)
        {
            // Get value in pointList at ith "row", in "column" Name
            // note that scale is only applied to the magnets for now
            float magnetX = System.Convert.ToSingle(pointList[i]["MagnetX"]) * magnetScale;
            float magnetY = System.Convert.ToSingle(pointList[i]["MagnetY"]) * magnetScale;
            float magnetZ = System.Convert.ToSingle(pointList[i]["MagnetZ"]) * magnetScale;
            float accelerationX = System.Convert.ToSingle(pointList[i]["AccelX"]) * accelScale;
            float accelerationY = System.Convert.ToSingle(pointList[i]["AccelY"]) * accelScale;
            float accelerationZ = System.Convert.ToSingle(pointList[i]["AccelZ"]) * accelScale;

            magnetPoints[i] = new Vector3(magnetX, magnetY, magnetZ);
            accelerationPoints[i] = new Vector3(accelerationX, accelerationY, accelerationZ);
        }
    }

    void Update()
    {
        // increment animationIndex based on Time.fixedTime, looping at end of array
        int animationIndex = Mathf.RoundToInt(animationSpeed * Time.fixedTime) % (pointList.Count - 1);
       
        Vector3 desiredMagnetPosition = magnetPoints[animationIndex];
        Vector3 desiredAccelPosition = accelerationPoints[animationIndex];

        Vector3 interpolatedMagnetPosition = Vector3.Lerp(magnetObject.transform.position, desiredMagnetPosition, Time.deltaTime);
        Vector3 interpolatedAccelPosition = Vector3.Lerp(accelObject.transform.position, desiredAccelPosition, Time.deltaTime);

        Debug.Log($"Desiredmag: {desiredMagnetPosition}, interpolatedmag: {interpolatedMagnetPosition}");
        Debug.Log($"Desiredaccel: {desiredAccelPosition}, interpolatedaccel: {interpolatedAccelPosition}");

        // lerp towards desired positions
        magnetObject.transform.localPosition = interpolatedMagnetPosition;
        accelObject.transform.localPosition = interpolatedAccelPosition;
    }
}
