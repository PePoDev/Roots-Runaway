using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public float delayTime;
    public int timesNum;
    public static float delay;
    public static int times;
    // Start is called before the first frame update
    void Start()
    {
        delay = delayTime;
        times = timesNum;
    }
}
