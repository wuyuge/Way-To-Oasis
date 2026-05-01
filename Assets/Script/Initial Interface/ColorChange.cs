using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ColorChange : MonoBehaviour
{
    public List<Sprite> sprites;
    public List<Image> images;
    public List<Sprite> initialSprites;
    public float time;
    public float returnTime;
    public GameObject temp;

    private void Start()
    {
        foreach (var value in images)
        {
            initialSprites.Add(value.sprite);
        }

        StartCoroutine(CheckTime());
    }


    private IEnumerator CheckTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            var temp = Random.Range(0, 2);
            if (temp == 0)
            {
                Change();
                Invoke(nameof(ReturnInitial),returnTime);
                yield return new WaitForSeconds(returnTime);
            }
        }
    }

    public void Change()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = sprites[i];
        }
        temp.SetActive(true);
    }

    public void ReturnInitial()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = initialSprites[i];
        }
        temp.SetActive(true);
    }
    
}
