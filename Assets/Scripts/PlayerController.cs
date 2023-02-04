using System;
using System.Collections;
using System.Collections.Generic;
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
  

    public static bool get;

    public GameObject[] AllCharacterModel;
    Vector2 movement;



    void Start()
    {
        var clientId = OwnerClientId;

        Debug.Log("PlayerController id: " + clientId.ToString());

        //var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);

        var model = AllCharacterModel[UnityEngine.Random.Range(0,8)];

        var parent = template.parent;

        var newPlayerCharacter = Instantiate(model, template.transform.position,template.transform.rotation, parent);
        newPlayerCharacter.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
        parent.gameObject.GetComponent<anim>().Anim = animator;
        Debug.Log("Spawned PlayerID:" + clientId.ToString());

        defaultSpeed = moveSpeed;
        currentSpeed = moveSpeed;

        if (!IsOwner) {
            GetComponent<Meteor>().enabled = false;
        }
       
    }
    void Update()
    {


        if (!IsOwner) return;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if(Input.GetKey(KeyCode.Space) && get){
            Debug.Log("use");
            get = false;
        }


        
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        currentSpeed = (defaultSpeed + buffSpeed) * multipySpeed;
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }
}
