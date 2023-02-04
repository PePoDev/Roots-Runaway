using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {
	public float rotation = 0;
	public float distance = 0;
	public float zOffset = 1;
	public float speed = 10;

	public Transform parent;

	void Update () {
		Vector2D pos = Vector2D.RotToVec(rotation * Mathf.Deg2Rad);

		transform.position = transform.parent.position + new Vector3((float)pos.x * distance, (float)pos.y * distance, zOffset);
		
		transform.rotation = Quaternion.Euler(0, 0, rotation - 90);

		rotation += speed * Time.deltaTime;
	}
}
