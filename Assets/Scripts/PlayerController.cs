using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed;
    public float currentSpeed;
    public static float defaultSpeed;
    public static float buffSpeed;
    public static float multipySpeed = 1;
    public Rigidbody2D rb;
    
    public static bool get;
    Vector2 movement;



    void Start()
    {
        defaultSpeed = moveSpeed;
        currentSpeed = moveSpeed;
       
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
