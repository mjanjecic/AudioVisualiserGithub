using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCircleParticles : MonoBehaviour
{

    public GameObject prefab;
    GameObject[] sampleCubes = new GameObject[64];

    public float circleSize = 5;

    // Start is called before the first frame update
    void Start()
    {
        float rotationOffest = 360 / 64;
        for (int i = 0; i < 64; i++)
        {
            GameObject instanceCube = (GameObject)Instantiate(prefab);
            instanceCube.transform.position = this.transform.position;
            instanceCube.transform.parent = this.transform;
            instanceCube.name = "SpectrumCube" + i;
            this.transform.eulerAngles = new Vector3(0, 0, rotationOffest * 2 * i);
            //instanceCube.transform.eulerAngles = new Vector3(0, 0, -0.703125f * i);
            instanceCube.transform.position = Vector3.up * circleSize;
            sampleCubes[i] = instanceCube;

        }
    }
}


