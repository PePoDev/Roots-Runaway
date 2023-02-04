using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction2DAnchors : MonoBehaviour {
	public enum AnchorType {AttachRigidbody, Nothing};

	public Collider2D[] anchorColliders = new Collider2D[1];
	public AnchorType anchorType = AnchorType.AttachRigidbody;

	void Start () {
		bool addEvents = false;

		foreach(Collider2D collider in anchorColliders) {
			addEvents = true;
		}

		if (addEvents == false) {
			return;
		}

		Destruction2D destruction = GetComponent<Destruction2D>();

		foreach(Collider2D collider in anchorColliders) {
			if (collider == null) {
				continue;
			}
			destruction.anchorPolygons.Add(Polygon2DList.CreateFromGameObject (collider.gameObject)[0]);
			destruction.anchorColliders.Add(collider);
		}

		Destruction2D destructionEvent = GetComponent<Destruction2D>();
		destructionEvent.AddAnchorEvent(AnchorEvent);
	}

	void AnchorEvent(Destruction2DEvent destruction) {
		switch(anchorType) {
			case AnchorType.AttachRigidbody: 
				foreach(GameObject g in destruction.gameObjects) {
					if (g.GetComponent<Rigidbody2D>() == null) {
						g.AddComponent<Rigidbody2D>();
					}
				}
			break;
		}
	}
}
