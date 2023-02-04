using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAttachGravity : MonoBehaviour {

	void Start () {
		Destruction2D destruction = GetComponent<Destruction2D>();
		destruction.AddAnchorEvent(AnchorEvent);
	}
	
	void AnchorEvent(Destruction2DEvent destruction) {
		foreach(GameObject g in destruction.gameObjects) {
			g.AddComponent<Gravity2D>();
			Destroy(g.GetComponent<EventAttachGravity>());
		}
	}
}
