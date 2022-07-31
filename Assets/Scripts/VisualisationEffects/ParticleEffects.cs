using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class ParticleEffects : MonoBehaviour
{

    public GameObject linearPrefab;
    public GameObject circlePrefab;

    //Parents all instantiated bars to 1 parent
    GameObject parentObject;

    public PostProcessProfile postProcessObj;

    [HideInInspector]
    public int lineNum;
    public float lineDist = 0.1f;

    //Circle parameters
    public float maxScale = 1;
    public float circleSize = 5;
    public float scale = 10;

    public float yPosition = -10;

    float screenWidth;

    float[] lastValues;

    List<GameObject> tracks;

    public bool instantiateInMiddle;

    Color dominantColor;
    public bool randomColorBool = false; 

    public enum VisualisationShape
    {
        Linear,
        Circular
    }

    public VisualisationShape visualisationType;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void InstantiateVisualisation()
    {
        DestroyChildren();
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
        if(transform.Find("ParticleObjectParent") != null)
        {

        Destroy(transform.Find("ParticleObjectParent").gameObject);
        }
    }


    public void InstantiateLines()
    {
        this.transform.eulerAngles = Vector3.zero;
        Vector2 screenSize = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        float screenDivided = screenSize.x / lineNum;
        float ratio = screenSize.x * 100 / screenWidth;

        Vector3 objPos = Vector3.zero;

        parentObject = new GameObject();
        parentObject.name = "ParticleObjectParent";
        parentObject.transform.parent = this.transform;

        for (int i = 0; i < lineNum; i++)
        {
            if (instantiateInMiddle)
            {

                int numberSign = 1;
                if (i % 2 == 0)
                {
                    numberSign = -1;
                }
                objPos = new Vector3((screenDivided * i * numberSign), this.transform.localPosition.y - 1, 0);
            }
            else
            {
                objPos = new Vector3(-screenSize.x + 0.2f + i * screenDivided * 2, this.transform.localPosition.y - 1, 0);
            }
            
            var newObj = Instantiate(linearPrefab, objPos, Quaternion.Euler(-90,0,0));
            newObj.name = "Particle" + i;
            newObj.transform.parent = parentObject.transform;
            newObj.transform.localPosition = new Vector3(newObj.transform.localPosition.x, yPosition, newObj.transform.localPosition.z);
            newObj.transform.localScale = new Vector3(1, 1, 1);
            Debug.Log(dominantColor);
            if (dominantColor != Color.clear)
            {
                var particleSys = newObj.GetComponent<ParticleSystem>().main;
                particleSys.startColor = dominantColor;
            }
            tracks.Add(newObj);
        }
    }

    public void InstantiateCircle()
    {
        float rotationOffest = 360.0f / lineNum;
        parentObject = new GameObject();
        parentObject.name = "ParticleObjectParent";
        parentObject.transform.parent = this.transform;

        for (int i = 0; i < lineNum; i++)
        {
            parentObject.transform.eulerAngles = new Vector3(0, 0, rotationOffest * i);
            GameObject instanceParticleEffect = (GameObject)Instantiate(circlePrefab);
            instanceParticleEffect.transform.position = this.transform.position;
            instanceParticleEffect.transform.parent = parentObject.transform;
            instanceParticleEffect.name = "SpectrumCube" + i;
            instanceParticleEffect.transform.position = Vector3.up * circleSize;
            if (dominantColor != Color.clear)
            {
                var particleSys = instanceParticleEffect.GetComponent<ParticleSystem>().main;
                particleSys.startColor = dominantColor;
            }
            tracks.Add(instanceParticleEffect);
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

            ParticleSystem particleSystem = parentObject.transform.GetChild(i).GetComponent<ParticleSystem>();
            //float scale = Mathf.Pow(resData[i] * i + 1 , 2);
            float scale = resData[i] * 100;
            float size = resData[i] / 10;
            if (size > 0.5f)
                size = 0.5f;
            //particleSystem.playbackSpeed = 100;

            //Particle size
            var particleMainProperties = particleSystem.main;
            particleMainProperties.startSize = size;

            //Emission rate
            float emissionRate = scale * 100;
            if (emissionRate > 250)
                emissionRate = 250;
            var emission = particleSystem.emission;
            emission.rateOverTime = emissionRate;

            scale += averageValues;
            scale /= 2500;
            if (scale < 0.01f )
                scale = 0.01f ;

            //Smooth speeding
            scale = (scale + lastValues[i]) / 1.2f;

            if (i == 3)
            {

                postProcessObj.TryGetSettings<Bloom>(out var bloom);
                bloom.intensity.overrideState = true;
                float bloomScale = Mathf.Pow(scale+1, 4);
                bloom.intensity.value =  bloomScale;
            }


            particleSystem.playbackSpeed = scale;

            //Randomize colors
            if (randomColorBool)
            {
                particleSystem.startColor = new Color(dominantColor.r + Random.Range(0.1f, 1), dominantColor.g + Random.Range(0.1f, 1), dominantColor.b + Random.Range(0.1f, 1));
            }
            //var noise = systemParticles.noise;

            lastValues[i] = scale;
            
              //  noise.strengthMultiplier = resData[i];
        }

        if (visualisationType == VisualisationShape.Circular)
        {
            parentObject.transform.Rotate(Vector3.forward * averageValues / 5f, Space.Self);
        }



    }

    public void ChangeColor(string hexColor, bool randomCol)
    {
        Color newCol;

        if (ColorUtility.TryParseHtmlString(hexColor, out newCol))
        {

            for (int i = 0; i < lineNum; i++)
            {
                tracks[i].GetComponent<ParticleSystem>().startColor = newCol;
            }
        }

        dominantColor = newCol;
        randomColorBool = randomCol;
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

    public void ChangeOrientation()
    {
        yPosition *= -1;
        for (int i = 0; i < lineNum; i++)
        {
            tracks[i].transform.localPosition = new Vector3(tracks[i].transform.localPosition.x, yPosition, tracks[i].transform.localPosition.z);
            tracks[i].transform.rotation = Quaternion.Euler(-tracks[i].transform.rotation.eulerAngles.x , tracks[i].transform.rotation.y, tracks[i].transform.rotation.z);
        }
    }
}
