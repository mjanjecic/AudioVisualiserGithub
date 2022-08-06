using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ParticleEffects : MonoBehaviour
{

    public GameObject linearPrefab;
    public GameObject circlePrefab;

    //Parents all instantiated bars to 1 parent
    GameObject parentObject;

    

    [HideInInspector]
    public int lineNum;

    //Circle parameters
    public float maxScale = 1;
    public float circleSize = 5;
    public float scale = 10;

    public float yPosition = -10;

    float screenWidth;

    List<GameObject> tracks;

    Color particleColor;

    public enum VisualisationShape
    {
        Linear,
        Circular
    }

    public VisualisationShape visualisationType;

    public void InstantiateVisualisation()
    {
        DestroyChildren();
        tracks = new List<GameObject>();
        if (visualisationType == VisualisationShape.Linear)
            InstantiateLines();
        else
            InstantiateCircle();
        screenWidth = Screen.width;
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
        Vector3 objPos;

        parentObject = new GameObject();
        parentObject.name = "ParticleObjectParent";
        parentObject.transform.parent = this.transform;

        for (int i = 0; i < lineNum; i++)
        {
            objPos = new Vector3(-screenSize.x + 0.2f + i * screenDivided * 2, this.transform.localPosition.y - 1, 0);

            var newObj = Instantiate(linearPrefab, objPos, Quaternion.Euler(-90, 0, 0));
            newObj.name = "Particle" + i;
            newObj.transform.parent = parentObject.transform;
            newObj.transform.localPosition = new Vector3(newObj.transform.localPosition.x, yPosition, newObj.transform.localPosition.z);
            newObj.transform.localScale = new Vector3(1, 1, 1);
            if (particleColor != Color.clear)
            {
                var particleSys = newObj.GetComponent<ParticleSystem>().main;
                particleSys.startColor = particleColor;
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
            GameObject instanceParticleEffect = Instantiate(circlePrefab);
            instanceParticleEffect.transform.position = this.transform.position;
            instanceParticleEffect.transform.parent = parentObject.transform;
            instanceParticleEffect.name = "SpectrumCube" + i;
            instanceParticleEffect.transform.position = Vector3.up * circleSize;
            if (particleColor != Color.clear)
            {
                var particleSys = instanceParticleEffect.GetComponent<ParticleSystem>().main;
                particleSys.startColor = particleColor;
            }
            tracks.Add(instanceParticleEffect);
        }
    }

    public void MapFrequencies(float[] resData)
    {
        float averageValues = resData.Sum() / resData.Length;
                Debug.Log(averageValues);
        if (lineNum == resData.Length)
        {
            for (int i = 0; i < lineNum; i++)
            {
                if (resData[i] < 0.01f)
                    resData[i] = 0.01f;
                else if (resData[i] > 6)
                    resData[i] = 6;

                ParticleSystem particleSystem = parentObject.transform.GetChild(i).GetComponent<ParticleSystem>();
                float scale = resData[i] * 100;
                float size = resData[i] / 10;
                if (size > 0.5f)
                    size = 0.5f;

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
                if (scale < 0.01f)
                    scale = 0.01f;

                particleMainProperties.simulationSpeed = scale;
            }

            if (visualisationType == VisualisationShape.Circular)
            {
                if (averageValues < 0.5f)
                    averageValues = 0.5f;
                parentObject.transform.Rotate(Vector3.forward * averageValues / 5f, Space.Self);
            }
        }
    }

    public void ChangeColor(Color newCol)
    {
        for (int i = 0; i < lineNum; i++)
        {
            var particleSys = tracks[i].GetComponent<ParticleSystem>().main;
            particleSys.startColor = newCol;
        }

        particleColor = newCol;
    }

    public void ChangeVisualisationMode(string visualisationTypeString)
    {
        visualisationType = (VisualisationShape)System.Enum.Parse(typeof(VisualisationShape), visualisationTypeString);
        InstantiateVisualisation();
    }
}
