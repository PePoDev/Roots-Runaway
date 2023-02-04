using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Destruction2DShape {
	private List<Polygon2D> polygons = null;

	private List<Polygon2D> polygons_world = null;
	private List<Polygon2D> polygons_world_cache = null;

	private Destruction2D destructible = null;
	
	public Destruction2DMovement movement = new Destruction2DMovement();

	// Is It Necessary?
	public void ForceUpdate() {
		movement.ForceUpdate();

		polygons_world = null;
		polygons_world_cache = null;
	}

	public void SetOrigin(Destruction2D slicerPass) {
		destructible = slicerPass;
	}

	// Shape API
	public List<Polygon2D> GetLocal() {
		if (polygons == null) {
			polygons= Polygon2DList.CreateFromGameObject(destructible.gameObject);
		}
		return(polygons);
	}

	public List<Polygon2D> GetWorld() {
		movement.Update(destructible);

		if (movement.update) {
			movement.update = false;

			polygons_world = null;
		}

		if (polygons_world == null) {
			if (polygons_world_cache == null) {
                List<Polygon2D> polys = GetLocal();
                polygons_world  = new List<Polygon2D>();

                foreach(Polygon2D poly in polys) {
                    polygons_world.Add(poly.ToWorldSpace(destructible.transform));
                }

				polygons_world_cache = polygons_world;

			} else {
				List<Polygon2D> newPolygon = polygons_world_cache;
				List<Polygon2D> polys = GetLocal();

               
                for(int id = 0; id < polys.Count; id++) {
                    
                    List<Vector2D> pointsList = polys[id].pointsList;
                    Vector2 v;

                    for(int i = 0; i < pointsList.Count; i++) {
                        v = destructible.transform.TransformPoint (pointsList[i].ToVector2());
                        newPolygon[id].pointsList[i].x = v.x;
                        newPolygon[id].pointsList[i].y = v.y;
                    }

                    for(int x = 0; x < newPolygon[id].holesList.Count; x++) {
                        pointsList = polys[id].holesList[x].pointsList;

                        for(int i = 0; i < pointsList.Count; i++) {
                            v = destructible.transform.TransformPoint (pointsList[i].ToVector2());
                            newPolygon[id].holesList[x].pointsList[i].x = v.x;
                            newPolygon[id].holesList[x].pointsList[i].y = v.y;
                        }
                    }
                }

                 polygons_world = newPolygon;
				
			}
		}

	    return(polygons_world);
	}
}
