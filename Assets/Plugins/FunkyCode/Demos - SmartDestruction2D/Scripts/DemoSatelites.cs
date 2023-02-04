using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSatelites : MonoBehaviour {
	public Sprite[] sprites;
	public float sateliteDistance = 1;
	public float sateliteSpeed = 1;
	private GameObject sateliteParent;

	void Start () {
		sateliteParent = new GameObject();
		sateliteParent.name = "Satelites";
		sateliteParent.transform.parent = transform;
		sateliteParent.transform.position = transform.position;

		for(int i = 0; i < sprites.Length; i++) {
			GameObject satelite = new GameObject();
			satelite.name = "Satelite " + (i + 1);
			satelite.transform.position = sateliteParent.transform.position;
			satelite.transform.parent = sateliteParent.transform;

			SpriteRenderer spriteRenderer = satelite.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprites[i];

			Cloud cloudScript = satelite.AddComponent<Cloud>();
			cloudScript.distance = sateliteDistance;
			cloudScript.rotation = ((float)i / sprites.Length) * 360f;
			cloudScript.parent = sateliteParent.transform;
			cloudScript.zOffset = -5;
			cloudScript.speed = sateliteSpeed;

			Destruction2D destruction2D = satelite.AddComponent<Destruction2D>();
			if (i == 0) {
				destruction2D.destructionLayer = DestructionLayer.Layer2;
			} else {
				destruction2D.destructionLayer = DestructionLayer.Layer3;
			}

			cloudScript.transform.localScale = new Vector3(1, 1, 1);
		}
	}
}
