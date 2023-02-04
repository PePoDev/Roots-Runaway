using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : MonoBehaviour
{
    public float delayTime;
    public static float delay;

    void Start()
    {
        delay = delayTime;
    }
}
