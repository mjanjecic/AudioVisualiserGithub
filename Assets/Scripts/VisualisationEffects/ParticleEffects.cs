using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleEffects : MonoBehaviour
{

    public GameObject linearPrefab;
    public GameObject circlePrefab;

    public int lineNum = 16;
    public float lineDist = 0.1f;

    //Circle parameters
    public float maxScale = 1;
    public float circleSize = 5;
    public float scale = 1;

    float screenWidth;

    float[] lastValues;

    List<GameObject> tracks;

    public enum VisualisationShape
    {
        Linear,
        Circular
    }

    public VisualisationShape visualisationType;
    // Start is called before the first frame update
    void Start()
    {
        tracks = new List<GameObject>();
        if (visualisationType == VisualisationShape.Linear)
            InstantiateLines();
        else
            InstantiateCircle();
        screenWidth = Screen.width;
        lastValues = new float[128];
    }

    void DestroyChildren()
    {
        tracks = new List<GameObject>();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }


    public void InstantiateLines()
    {
        this.transform.eulerAngles = Vector3.zero;
        Vector2 screenSize = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        float screenDivided = screenSize.x / lineNum;
        float ratio = screenSize.x * 100 / screenWidth;

        for (int i = 0; i < lineNum; i++)
        {
            Vector3 objPos = new Vector3(-screenSize.x + i * screenDivided * 2, this.transform.localPosition.y-1, 0);
            /*int predznak = 1;
            if(i %2==0)
            {
                predznak = -1;
            }
            Vector3 objPos = new Vector3((screenSize.x/2 + i * predznak * screenDivided * 2)/2, this.transform.localPosition.y-1, 0);
            */
            var newObj = Instantiate(linearPrefab, objPos, Quaternion.Euler(-90,0,0));
            newObj.name = "Particle" + i;
            newObj.transform.parent = this.transform;
            newObj.transform.localPosition = new Vector3(newObj.transform.localPosition.x, -10, newObj.transform.localPosition.z);
            newObj.transform.localScale = new Vector3(1, 1, 1);
            tracks.Add(newObj);
        }
    }

    public void InstantiateCircle()
    {
        Debug.Log("Hello!");
        float rotationOffest = 360 / lineNum;
        for (int i = 0; i < lineNum; i++)
        {
            GameObject instanceCube = (GameObject)Instantiate(circlePrefab);
            instanceCube.transform.position = this.transform.position;
            instanceCube.transform.parent = this.transform;
            instanceCube.name = "SpectrumCube" + i;
            this.transform.eulerAngles = new Vector3(0, 0, rotationOffest * 2 * i);
            instanceCube.transform.position = Vector3.up * circleSize;
            tracks.Add(instanceCube);
        }
        
    }

    public void MapFrequencies(float[] resData)
    {
        float averageValues = resData.Sum() / resData.Length;
        for (int i = 0; i < lineNum; i++)
        {
            //resData[i] = resData[i] + 2 * Mathf.Sqrt(i / (lineNum + 0.0f)) * resData[i];
            if (resData[i] < 0.01f)
                resData[i] = 0.01f;
            else if (resData[i] > 6)
                resData[i] = 6;

            ParticleSystem systemParticles = tracks[i].GetComponent<ParticleSystem>();
            float scale = Mathf.Pow(resData[i] * i + 1 , 2);
            //float scale = resData[i];
            float size = resData[i] / 10;
            if (size > 0.5f)
                size = 0.5f;
            systemParticles.startSize = size;
            float emissionRate = scale / 10;
            if (emissionRate / 10 > 250)
                emissionRate = 250;
            systemParticles.emissionRate = emissionRate;
            scale /= 10000;
            //scale += averageValues;
            if (scale < 0.01f )
                scale = 0.01f ;

            //Smooth speeding
            scale = (scale + lastValues[i]) / 1.2f;
            
            systemParticles.playbackSpeed = scale;
            //systemParticles.startColor = new Color(Random.Range(0.1f, 1), 1, Random.Range(0.1f, 1));
            //var noise = systemParticles.noise;

            lastValues[i] = scale;
            
              //  noise.strengthMultiplier = resData[i];
        }
    }

    public void ChangeColor(string hexColor)
    {
        Color newCol;

        if (ColorUtility.TryParseHtmlString(hexColor, out newCol))
        {

        for (int i = 0; i < lineNum; i++)
        {
            tracks[i].GetComponent<ParticleSystem>().startColor = newCol;
        }
        }
    }

    public void ChangeVisualisationMode(string visualisationTypeString)
    {
        DestroyChildren();
        visualisationType = (VisualisationShape)System.Enum.Parse(typeof(VisualisationShape), visualisationTypeString);
        if (visualisationType == VisualisationShape.Linear)
            InstantiateLines();
        else
            InstantiateCircle();
    }
}
