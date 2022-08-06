using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineInstantiatior : MonoBehaviour
{

    public GameObject prefabLine;
    public List<GameObject> circlePrefabs;
    GameObject parentObject;

    [HideInInspector]
    public int lineNum = 16;
    //public float lineDist = 0.1f;

    public enum VisualisationType
    {
        Linear,
        Circular
    }

    public VisualisationType visType;

    float screenWidth;
    float screenHeight;

    List<GameObject> tracks;

    Color barColor;


    void DestroyChildren()
    {
        tracks = new List<GameObject>();
        if (transform.Find("LineObjectParent") != null)
        {
        Destroy(transform.Find("LineObjectParent").gameObject);
        }
    }

    public void InstantiateVisualisation()
    {
        DestroyChildren();
        tracks = new List<GameObject>();
        if (visType == VisualisationType.Circular)
            InstantiateCircles();
        else
            InstantiateLines();
    }


    public void MapFrequencies(float[] resData)
    {
        //Avoid NullPointer error
        if (lineNum == resData.Length)
        {
            for (int i = 0; i < lineNum; i++)
            {
                if (resData[i] < 0.01f)
                {
                    resData[i] = 0.01f;
                }
                else if (resData[i] > 6)
                {
                    resData[i] = 6;
                }

                if (visType == VisualisationType.Linear)
                {
                    parentObject.transform.GetChild(i).transform.localScale = new Vector2(tracks[i].transform.localScale.x, resData[i]);
                }
                else
                {
                    parentObject.transform.GetChild(i).transform.Rotate(new Vector3(0, 0, resData[i]));
                }
            }
        }
    }

    public void ChangeVisualisationMode(string visualisationTypeString)
    {
        visType = (VisualisationType)System.Enum.Parse(typeof(VisualisationType), visualisationTypeString);
        InstantiateVisualisation();
    }

    public void InstantiateLines()
    {
        Vector2 screenSize = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        float objRenderWidth = prefabLine.GetComponent<Renderer>().bounds.size.x;
        float screenDivided = screenSize.x / lineNum;
        float finalxScale = screenDivided * objRenderWidth/4;
        parentObject = new GameObject();
        parentObject.name = "LineObjectParent";
        parentObject.transform.parent = this.transform;
        for (int i = 0; i < lineNum; i++)
        {
            Vector3 objPos = new Vector3(-screenSize.x + i * screenDivided * 2, -screenSize.y - screenHeight + 0.1f, 0);
            var x = Instantiate(prefabLine, objPos, Quaternion.identity);
            x.name = "LineTrack"+i;
            x.transform.parent = parentObject.transform;
            x.transform.localScale = new Vector3(finalxScale, 1, 1);
            if (barColor != Color.clear)
            {
                x.GetComponent<SpriteRenderer>().color = barColor;
            }
            tracks.Add(x);
        }
    }

    public void InstantiateCircles()
    {
        parentObject = new GameObject();
        parentObject.name = "LineObjectParent";
        parentObject.transform.parent = this.transform;
        Vector2 screenSize = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
            float widthScale = screenSize.x / lineNum;
        Debug.Log(widthScale);
        for (int i = lineNum-1; i >= 0; i--)
        {
            var x = Instantiate(circlePrefabs[Random.Range(0, circlePrefabs.Count)], Vector3.zero, Quaternion.identity);
            x.name = "LineCircle" + i;
            x.transform.parent = parentObject.transform;
            LineRenderer lineDrawer = x.GetComponent<LineRenderer>();
            lineDrawer.widthMultiplier =  widthScale * 5;
            float scale = 0.35f * (i+1)*  widthScale;
            x.transform.localScale = new Vector3(scale, scale, scale);
            x.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
            if (barColor != Color.clear)
                lineDrawer.colorGradient = ApplyGradient();
            tracks.Add(x);
        }
    }

    public void ChangeColor(Color newCol)
    {
        Debug.Log("Applying color!");
        barColor = newCol;
        if (visType == VisualisationType.Linear)
        {
            for (int i = 0; i < lineNum; i++)
            {
                tracks[i].GetComponent<SpriteRenderer>().color = barColor;
            }
        }
        else
        {
            for (int i = 0; i < lineNum; i++)
            {
                tracks[i].GetComponent<LineRenderer>().colorGradient = ApplyGradient();
            }
        }
    }

    Gradient ApplyGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        colorKey = new GradientColorKey[3];
        colorKey[0].color = Color.white;
        colorKey[0].time = 0.0f;
        colorKey[1].color = barColor;
        colorKey[1].time = 0.5f;
        colorKey[2].color = Color.white;
        colorKey[2].time = 1.0f;

        alphaKey = new GradientAlphaKey[3];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 1.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        alphaKey[2].alpha = 1.0f;
        alphaKey[2].time = 1.0f;
        gradient.SetKeys(colorKey, alphaKey);
        return gradient;
    }
}
