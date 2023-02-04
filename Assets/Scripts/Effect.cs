using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    #region Data

    public enum EffectType
    {
        Stun,
        Slow,
        Speed,
        Lighting,
        ActiveStun
    }


    #endregion

    public float delay;
    static float mem;



    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("StunItem"))
        {
            StartEffect(EffectType.Stun);
            //anim.Anim.Play("DefenseHit");
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("SpeedItem"))
        {
            StartEffect(EffectType.Speed);
           
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("SlowItem"))
        {

            StartEffect(EffectType.Slow);


            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("LightningItem"))
        {

            StartEffect(EffectType.Lighting);



            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("ActiveStunItem"))
        {
            StartEffect(EffectType.ActiveStun);



            Destroy(other.gameObject);
        }
    }

    void StartEffect(EffectType effectType)
    {
        float timeEnd = Time.time;

        switch (effectType)
        {
            case EffectType.Stun:
                StartCoroutine("StunCoroutine");
                break;

            case EffectType.Slow:
                StartCoroutine("SlowCoroutine");
                break;

            case EffectType.Speed:
                StartCoroutine("SpeedCoroutine");
                break;

            case EffectType.Lighting:
                StartCoroutine("LightingCoroutine");
                break;

            case EffectType.ActiveStun:
                StartCoroutine("ActiveStunCoroutine");
                break;
        }
    }

    private IEnumerator StunCoroutine()
    {
        PlayerController.get = true;
        Debug.Log("getStun");
        PlayerController.multipySpeed *= 0.00001f;
        yield return new WaitForSeconds(Stun.delay);
        PlayerController.multipySpeed /= 0.00001f;
    }

    private IEnumerator SpeedCoroutine()
    {
        PlayerController.get = true;
        Debug.Log("getSpeed");
        PlayerController.buffSpeed += (PlayerController.defaultSpeed * (Speed.speed - 1));
        yield return new WaitForSeconds(Speed.delay);
        PlayerController.buffSpeed -= (PlayerController.defaultSpeed * (Speed.speed - 1));

    }

    private IEnumerator SlowCoroutine()
    {
        PlayerController.get = true;
        Debug.Log("getSlow");
        PlayerController.buffSpeed -= (PlayerController.defaultSpeed * Slow.speed);
        yield return new WaitForSeconds(Slow.delay);
        PlayerController.buffSpeed += (PlayerController.defaultSpeed * Slow.speed);

    }

    private IEnumerator LightingCoroutine()
    {
        Debug.Log("getLighting");
        PlayerController.get = true;
        for (int i = 0; i < Lightning.times; i++)
        {
            PlayerController.multipySpeed *= 0.00001f;
            yield return new WaitForSeconds(Lightning.delay);
            PlayerController.multipySpeed /= 0.00001f;
            yield return new WaitForSeconds(Lightning.delay);
        }

    }

    private IEnumerator ActiveStunCoroutine()
    {
        PlayerController.get = true;
        Debug.Log("getStun");
        PlayerController.multipySpeed *= 0.00001f;
        yield return new WaitForSeconds(Stun.delay);
        PlayerController.multipySpeed /= 0.00001f;
    }


    void Normal()
    {
        PlayerController.multipySpeed = 1;
    }

    void Stop()
    {
        PlayerController.multipySpeed = 0;
    }
}