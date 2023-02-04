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

    private bool skillIsReady = false, eyeIsReady = true;
    private int eyeCooldown = 30;

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

    public static bool get;

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
    void Update()
    {
        if (!IsOwner) return;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.Space) && get)
        {
            Debug.Log("use");
            get = false;
        }

        if (Input.GetKey(KeyCode.F) && eyeIsReady)
        {
            eyeIsReady = false;
            StartCoroutine(startDelayEyeCooldown());
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players)
            {
                p.GetComponent<PlayerController>().ShowMe(OwnerClientId);
            }
            Debug.Log("Total Player: " + players.Length);
        }
        if (Input.GetKey(KeyCode.X))
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            // TOOD: check player in range
            var target = players[0].GetComponent<NetworkObject>().NetworkObjectId;
            Debug.Log("Player Id: " + target.ToString());
            UpdateTargetSpeedClientRpc(target);
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
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        currentSpeed = (defaultSpeed + buffSpeed) * multipySpeed;
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DeathZone")
        {
            Destroy(gameObject);
        }
    }
    private void AddSpeed() {
        multipySpeed += 0.2f;
    }

    [ClientRpc]
    private void UpdateTargetSpeedClientRpc(ulong clientId)
    {
        //Debug.Log("UpdateTargetSpeed Target ID: " + clientId.ToString());
        if (clientId == this.gameObject.GetComponent<NetworkObject>().NetworkObjectId)
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
}
