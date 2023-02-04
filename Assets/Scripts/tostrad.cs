using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tostrad : MonoBehaviour
{
    public float delay;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("storyCoroutine");
    }
    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            SceneManager.LoadScene("Menu");
        }
    }

    private IEnumerator storyCoroutine()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }
}
