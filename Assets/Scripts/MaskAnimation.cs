using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaskAnimation : MonoBehaviour
{
    public List<Sprite> images;
    SpriteMask mask;
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        mask = GetComponent<SpriteMask>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(FadeAnimation());
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(AppearAnimation());
        }
    }

    IEnumerator FadeAnimation()
    {
        for (int i = 0; i < images.Count; i++)
        {
            yield return new WaitForSeconds(1 / speed);
            mask.sprite = images[i];
        }
    }

    IEnumerator AppearAnimation()
    {
        for (int i = images.Count - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(1/speed);
            mask.sprite = images[i];
        }
    }
}
