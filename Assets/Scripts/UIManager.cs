using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text eyeCooldown;

    public void UpdateEyeCoolDown(int n)
    {
        if (n <= 0)
        {
            eyeCooldown.text = "F";
            return;
        }

        eyeCooldown.text = n.ToString();
    }
}
