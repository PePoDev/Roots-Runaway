using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {

	public Texture2D modifierTexture;

	public void OnCollisionEnter2D (Collision2D col) {
		Polygon2D polygon = Polygon2DList.CreateFromGameObject(gameObject)[0];
		// or
		//Polygon2D.Create(Polygon2D.PolygonType.Pentagon, 1);
		polygon = polygon.ToScale(new Vector2(5, 5));
		polygon = polygon.ToWorldSpace(transform);

		Destruction2D.DestroyByPolygonAll(polygon, Destruction2DLayer.Create());

		Destruction2D.AddModifierAll(modifierTexture, gameObject.transform.position, new Vector2(5, 5), 0, Destruction2DLayer.Create());

		Destroy(gameObject);
	}
}
