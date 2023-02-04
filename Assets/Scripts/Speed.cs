using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    public float delayTime;
    public float setSpeed;
    public static float delay;
    public static float speed;
    // Start is called before the first frame update
    void Start()
    {
        delay = delayTime;
        speed = setSpeed;   
    }
}
