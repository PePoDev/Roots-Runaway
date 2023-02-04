using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity2D : MonoBehaviour {
	Rigidbody2D body;
	
	void Start () {
		body = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		Vector3 worldPosition =  (Vector2)(transform.position + new Vector3(body.centerOfMass.x, body.centerOfMass.y, 0)) ;
		body.AddForce(Vector2D.RotToVec(Vector2D.Atan2(transform.parent.position, worldPosition)).ToVector2() * 10f);
	}
}
