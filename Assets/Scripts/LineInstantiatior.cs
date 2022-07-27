using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineInstantiatior : MonoBehaviour
{

    public GameObject obj;

    public int lineNum = 16;
    public float lineDist = 0.1f;

    float screenWidth;

    List<GameObject> tracks;
    // Start is called before the first frame update
    void Start()
    {
        tracks = new List<GameObject>();
        InstantiateLines();
        screenWidth = Screen.width;
    }


    public void MapFrequencies(float[] resData)
    {
        for (int i = 0; i < lineNum; i++)
        {
            if (resData[i] < 0.01f)
                resData[i] = 0.01f;
            else if (resData[i] > 6)
                resData[i] = 6;

            tracks[i].transform.localScale = new Vector2(tracks[i].transform.localScale.x, resData[i]);
        }
    }

    public void InstantiateLines()
    {

        Vector2 screenSize = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        float objRenderWidth = obj.GetComponent<Renderer>().bounds.size.x;
        float screenDivided = screenSize.x / lineNum;
        float ratio = screenSize.x * 100 / screenWidth;
        float finalxScale = screenDivided * objRenderWidth/4;

        for (int i = 0; i < lineNum; i++)
        {
            Vector3 objPos = new Vector3(-screenSize.x + i * screenDivided * 2, this.transform.localPosition.y, 0);
            var x = Instantiate(obj, objPos, Quaternion.identity);
            x.name = "LineTrack"+i;
            x.transform.parent = this.transform;
            x.transform.localScale = new Vector3(finalxScale, 1, 1);
            tracks.Add(x);
        }
    }
}
