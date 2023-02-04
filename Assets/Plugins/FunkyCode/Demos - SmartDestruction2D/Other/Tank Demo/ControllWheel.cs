using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllWheel : MonoBehaviour {

	void Update () {
		if (Input.GetKey(KeyCode.Space)) {
			GetComponent<Rigidbody2D>().AddTorque(-15);
		}
	}
}
