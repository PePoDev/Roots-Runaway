using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDown : MonoBehaviour
{
    public float speed;

    private bool afterDelay;

    private void Start()
    {
        StartCoroutine(delay());
    }

    private void Update()
    {
        if (afterDelay) transform.Translate(0, speed * (Time.deltaTime * 2), 0);
    }

    private IEnumerator delay()
    {
        yield return new WaitForSeconds(5f);
        afterDelay = true;
    }
}
