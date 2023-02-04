using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if(Input.GetKey(KeyCode.Space) && get){
            Debug.Log("use");
            get = false;
        }
        
    }

    void FixedUpdate()
    {
        currentSpeed = (defaultSpeed + buffSpeed) * multipySpeed;
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }
}
