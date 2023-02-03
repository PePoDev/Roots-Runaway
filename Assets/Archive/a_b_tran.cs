using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class a_b_tran : MonoBehaviour
{
    public GameObject A;
    public GameObject B;
    public void aTob()
    {
        A.SetActive(true);
        B.SetActive(false);
    }
    public void bToa()
    {
        B.SetActive(true);
        A.SetActive(false);

    }
}
