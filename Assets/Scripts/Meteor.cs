using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {
	public float scale = 1f;

    public void OnCollisionEnter2D (Collision2D col) {
		Polygon2D polygon = Polygon2D.Create(Polygon2D.PolygonType.Octagon, 1);
		polygon = polygon.ToScale(new Vector2(scale, scale));
		polygon = polygon.ToWorldSpace(transform);
		Destruction2D.DestroyByPolygonAll(polygon, Destruction2DLayer.Create());
	}
}
