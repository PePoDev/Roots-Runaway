using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text eyeCooldown;
    public Image skill;

    public Sprite no_skill;
    public Sprite stun_skill;
    public Sprite active_stun_skill;
    public Sprite speed_skill;
    public Sprite slow_skill;
    public Sprite lighting_skill;

    public GameObject win, lose;

    private void Start()
    {
        skill.overrideSprite = no_skill;
    }

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
