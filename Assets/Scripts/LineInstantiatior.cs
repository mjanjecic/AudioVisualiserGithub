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

    // Start is called before the first frame update
    void Start()
    {
    }

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
        if(lineNum == resData.Length)
        {
        for (int i = 0; i < lineNum; i++)
        {
            if (resData[i] < 0.01f)

                resData[i] = 0.01f;
            else if (resData[i] > 6)
                resData[i] = 6;
            // Scale the data because for some reason bass is always loud and treble is soft
            //tracks[i] = resData[i] + 2 * Mathf.Sqrt(i / (lineNum + 0.0f)) * resData[i];
            //tracks[i].transform.localScale = new Vector2(tracks[i].transform.localScale.x, resData[i] + 2 * Mathf.Sqrt(i / (lineNum + 0.0f)) * resData[i]);
            if (visType == VisualisationType.Linear)
                parentObject.transform.GetChild(i).transform.localScale = new Vector2(tracks[i].transform.localScale.x, resData[i]);
            else
                parentObject.transform.GetChild(i).transform.Rotate(new Vector3(0,0,resData[i]));

        }
        }
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
            tracks.Add(x);
        }
    }

    public void InstantiateCircles()
    {
        parentObject = new GameObject();
        parentObject.name = "LineObjectParent";
        parentObject.transform.parent = this.transform;
        for (int i = lineNum-1; i >= 0; i--)
        {
            var x = Instantiate(circlePrefabs[Random.Range(0, circlePrefabs.Count)], Vector3.zero, Quaternion.identity);
            x.name = "LineCircle" + i;
            x.transform.parent = parentObject.transform;
            LineRenderer LineDrawer = x.GetComponent<LineRenderer>();
            LineDrawer.widthMultiplier = 0.6f;
            float scale = 0.025f * i*2+1;
            x.transform.localScale = new Vector3(scale, scale, scale);
            x.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
            tracks.Add(x);
        }
    }

    public void DrawCircles()
    {
        float ThetaScale = 0.01f;
        float radius = 3f;
        int Size;
        LineRenderer LineDrawer = gameObject.AddComponent<LineRenderer>(); 
        float Theta = 0f;
        Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        LineDrawer.SetVertexCount(Size);
        for (int i = 0; i < Size; i++)
        {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = radius * Mathf.Cos(Theta);
            float y = radius * Mathf.Sin(Theta);
            LineDrawer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public void ChangeColor(string hexColor)
    {
        Color newCol;

        if (ColorUtility.TryParseHtmlString(hexColor, out newCol))
        {

            for (int i = 0; i < lineNum; i++)
            {
                tracks[i].GetComponent<SpriteRenderer>().color = newCol;
            }
        }
    }
}
