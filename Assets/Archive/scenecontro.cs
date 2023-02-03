using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scenecontro : MonoBehaviour
{
    public void toload()
    {
        SceneManager.LoadScene("hostroom");
    }
}
