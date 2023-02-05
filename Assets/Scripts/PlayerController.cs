using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

//public struct PlayerState : INetworkSerializable, IEquatable<PlayerState>
//{
//    public ulong ClientId;
//    public ulong SeletedCharacterId;

//    public PlayerState(ulong clientId, ulong seletedCharacterId)
//    {
//        ClientId = clientId;
//        SeletedCharacterId = seletedCharacterId;
//    }

//    public bool Equals(PlayerState other)
//    {
//        return ClientId == other.ClientId;
//    }

//    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//    {
//        serializer.SerializeValue(ref ClientId);
//        serializer.SerializeValue(ref SeletedCharacterId);
//    }
//}

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed;
    public float currentSpeed;
    public static float defaultSpeed;
    public static float buffSpeed;
    public static float multipySpeed = 1;
    public Rigidbody2D rb;
    public Animator animator;
    public Transform template;
    public GameObject effect;

    private bool eyeIsReady = true;
    private int eyeCooldown = 30;
    private string skillName = "";
    private GameObject targetPlayer;

    public void ShowMe(ulong id)
    {
        Debug.Log($"Player {id} and OwnerID {OwnerClientId}");
        if (OwnerClientId == id) return;

        effect.SetActive(true);
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(5f);
            effect.SetActive(false);
        }
    }

    public GameObject[] AllCharacterModel;
    Vector2 movement;

    void Start()
    {
        var clientId = OwnerClientId;

        Debug.Log("PlayerController id: " + clientId.ToString());

        //var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);

        var model = AllCharacterModel[UnityEngine.Random.Range(0, 8)];

        var parent = template.parent;

        var newPlayerCharacter = Instantiate(model, template.transform.position, template.transform.rotation, parent);
        newPlayerCharacter.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
        parent.gameObject.GetComponent<anim>().Anim = animator;
        Debug.Log("Spawned PlayerID:" + clientId.ToString());

        defaultSpeed = moveSpeed;
        currentSpeed = moveSpeed;

        if (!IsOwner)
        {
            GetComponent<Meteor>().enabled = false;
        }

    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (IsOwner == false) return;
        Debug.Log("OnDestroy clientID: " + gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().lose.SetActive(true);
    }

    void Update()
    {
        if (!IsOwner) return;
        
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        if (Input.GetKey(KeyCode.Space) && skillName != "")
        {
            Debug.Log("Use skill: "+ skillName);
            switch (skillName)
            {
                case "ActiveStunItem":
                    if (targetPlayer != null) StunClientRpc(targetPlayer.GetComponent<NetworkObject>().NetworkObjectId);
                    break;
                case "LightningItem":
                    if (targetPlayer != null) LightingEffectClientRpc(targetPlayer.GetComponent<NetworkObject>().NetworkObjectId);
                    break;
                case "SlowItem":
                    if (targetPlayer != null) SlowClientRpc(targetPlayer.GetComponent<NetworkObject>().NetworkObjectId);
                    break;
            }
            var ui = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>();
            ui.skill.overrideSprite = ui.no_skill;
            GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().itemBth.interactable = false;
            skillName = "";
        }

        if (Input.GetKey(KeyCode.F) && eyeIsReady)
        {
            eyeIsReady = false;
            GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().UpdateEyeCoolDown(30);
            GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().skillBth.interactable = false;
            StartCoroutine(startDelayEyeCooldown());
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players)
            {
                p.GetComponent<PlayerController>().ShowMe(OwnerClientId);
            }
            Debug.Log("Total Player: " + players.Length);
        }
    }
    private IEnumerator startDelayEyeCooldown()
    {
        while (eyeCooldown > 0)
        {
            yield return new WaitForSeconds(1f);
            eyeCooldown--;
            GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().UpdateEyeCoolDown(eyeCooldown);
        }

        GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().UpdateEyeCoolDown(eyeCooldown);
        eyeIsReady = true;
        eyeCooldown = 30;
        GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().skillBth.interactable = true;
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        currentSpeed = (defaultSpeed + buffSpeed) * multipySpeed;
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsOwner == false) return;
        var ui = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>();
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            DestoryPlayerObjectServerRpc(gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
        else if (collision.gameObject.CompareTag("StunItem"))
        {
            StunAllClientRpc(gameObject.GetComponent<NetworkObject>().NetworkObjectId);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("SpeedItem"))
        {
            AddSpeed();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("SlowItem"))
        {
            ui.skill.overrideSprite = ui.slow_skill;
            GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().itemBth.interactable = true;
            skillName = collision.tag;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("LightningItem"))
        {
            ui.skill.overrideSprite = ui.lighting_skill;
            GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().itemBth.interactable = true;
            skillName = collision.tag;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ActiveStunItem"))
        {
            ui.skill.overrideSprite = ui.active_stun_skill;
            GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<UIManager>().itemBth.interactable = true;
            skillName = collision.tag;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("PlayerArea"))
        {
            targetPlayer = collision.gameObject.transform.parent;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsOwner == false) return;
        if (collision.gameObject.CompareTag("PlayerArea"))
        {
            targetPlayer = null;
        }
    }

    private void AddSpeed() {
        multipySpeed += 0.2f;
    }

    [ClientRpc]
    private void SlowClientRpc(ulong clientId)
    {
        //Debug.Log("UpdateTargetSpeed Target ID: " + clientId.ToString());
        if (clientId == gameObject.GetComponent<NetworkObject>().NetworkObjectId)
        {
            buffSpeed = (defaultSpeed*0.5f) * -1;
            Debug.Log("UpdateTargetSpeed Target ID: " + clientId.ToString());
            StartCoroutine(delay());

            IEnumerator delay()
            {
                yield return new WaitForSeconds(5f);
                buffSpeed = 0;
            }
        }
    }

    [ClientRpc]
    private void LightingEffectClientRpc(ulong clientId)
    {
        //Debug.Log("UpdateTargetSpeed Target ID: " + clientId.ToString());
        if (clientId == gameObject.GetComponent<NetworkObject>().NetworkObjectId)
        {
            buffSpeed = defaultSpeed * -1;
            Debug.Log("LightingEffectClientRpc Target ID: " + clientId.ToString());
            StartCoroutine(delay());

            IEnumerator delay()
            {
                yield return new WaitForSeconds(0.8f);
                buffSpeed = 0;
                yield return new WaitForSeconds(0.8f);
                buffSpeed = defaultSpeed * -1;
                yield return new WaitForSeconds(0.8f);
                buffSpeed = 0;
                yield return new WaitForSeconds(0.8f);
                buffSpeed = defaultSpeed * -1;
                yield return new WaitForSeconds(0.8f);
                buffSpeed = 0;
            }
        }
    }

    [ClientRpc]
    private void StunClientRpc(ulong clientId)
    {
        //Debug.Log("UpdateTargetSpeed Target ID: " + clientId.ToString());
        if (clientId == gameObject.GetComponent<NetworkObject>().NetworkObjectId)
        {
            buffSpeed = defaultSpeed * -1;
            Debug.Log("UpdateTargetSpeed Target ID: " + clientId.ToString());
            StartCoroutine(delay());

            IEnumerator delay()
            {
                yield return new WaitForSeconds(3f);
                buffSpeed = 0;
            }
        }
    }


    [ClientRpc]
    private void StunAllClientRpc(ulong clientId)
    {
        //Debug.Log("UpdateTargetSpeed Target ID: " + clientId.ToString());
        if (clientId != gameObject.GetComponent<NetworkObject>().NetworkObjectId)
        {
            buffSpeed = defaultSpeed * -1;
            Debug.Log("UpdateTargetSpeed Target ID: " + clientId.ToString());
            StartCoroutine(delay());

            IEnumerator delay()
            {
                yield return new WaitForSeconds(3f);
                buffSpeed = 0;
            }
        }
    }

    [ServerRpc]
    private void DestoryPlayerObjectServerRpc(ulong targetID) {
        if (IsServer)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players) {
                if (player.GetComponent<NetworkObject>().NetworkObjectId == targetID) {
                    Destroy(player);
                    Debug.Log("Server Destoryed PlayerID: "+ targetID.ToString());
                    GameObject.Find("Game Manager").GetComponent<GameNetworkManger>().CurrentPlayerLive.Value -= 1 ;
                }
            }
            
        }
    }
}
