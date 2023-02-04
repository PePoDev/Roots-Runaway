using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class anim : NetworkBehaviour
{
    public Animator m_Animator;
    public static Animator Anim;
    
    // Start is called before the first frame update
    void Start()
    {
        Anim = m_Animator;
        // Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if(Input.GetKeyDown(KeyCode.S)){
            
            Anim.Play("Throw");
            transform.rotation = Quaternion.Euler(0f,0,0);
        }
        else if(Input.GetKeyDown(KeyCode.D)){
            
            Anim.Play("Attack01");
            transform.rotation = Quaternion.Euler(0f,-90,-180);
         
        }
        else if(Input.GetKeyDown(KeyCode.A)){
            
            Anim.Play("Attack01");
            transform.rotation = Quaternion.Euler(0f,90,-180);
         
        }
        else if(Input.GetKeyDown(KeyCode.W)){
            
            Anim.Play("Throw");
            transform.rotation = Quaternion.Euler(-180,-180,0f);
         
        }
        
    }

    
}
