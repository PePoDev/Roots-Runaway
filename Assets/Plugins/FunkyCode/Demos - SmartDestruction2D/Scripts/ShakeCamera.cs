using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Smooth Camera Transition
public class ShakeCamera : MonoBehaviour {
	private Vector3 position;
	private float shake = 0;

	private static ShakeCamera instance;

	static  public void SetPosition(Vector3 pos) {
		instance.position = pos;
	}

	static public void Move(Vector3 pos) {
		instance.position += pos;
	}

	static public void SetShake(float shake) {
		instance.shake = shake;
	}

	void Start () {
		position = transform.position;
		instance = this;
	}
	
	void Update () {
		Vector3 pos = transform.position;

		pos = Vector3.Lerp(pos, position, 0.05f);
		
		transform.position = pos;

		if (shake > 0) {
			shake -= Time.deltaTime;
			transform.position = pos + new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0);
		}
	}
}
