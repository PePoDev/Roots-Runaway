using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    public GameObject winButton;
    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("getwin");
            winButton.SetActive(true);
        }
    }
    

    public void tomenu(){
        SceneManager.LoadScene("Menu");
    }
}
