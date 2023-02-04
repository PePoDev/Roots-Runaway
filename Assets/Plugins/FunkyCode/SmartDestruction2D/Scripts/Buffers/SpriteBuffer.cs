using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBuffer : BufferBase {

	void Initialize() {
		Transform transform = renderCamera.transform;
		
		DrawSelf ();

		foreach(DestructionModifier modifier in destructible.modifiers) {
			Vector3 pos = (Vector3)modifier.position;

			float ratioX = (destructible.transform.localScale.x / destructible.transform.localScale.y);

			Vector2 size = modifier.size;

			pos.x *= transform.localScale.x;
			pos.y *= transform.localScale.y * ratioX;

			size.y *= ratioX;
		
			Vector2 scale = new Vector2(destructible.transform.localScale.x, destructible.transform.localScale.y);
				
			Max2D.DrawImage(transform, modifier.material, pos, size, modifier.rotation, 0.3f, new Vector2D(scale));
		}

		foreach(Polygon2D p in destructible.erasePolygons) {
			Vector2 scale = new Vector2(1, 1);
		
			Polygon2D polygon = p.ToScale(scale);

			Mesh mesh = polygon.CreateMesh(Vector2.zero, Vector2.zero);

			destructible.eraseMeshes.Add(mesh);
		}
		
		// Not Necessary? Why?
		foreach(Mesh e in destructible.eraseMeshes) {
			float ratioX = destructible.transform.localScale.x / destructible.transform.localScale.y;

			Vector2 scale = destructible.transform.localScale;
			scale.y *= ratioX;
			
			Max2D.DrawMesh(GetEraseMaterial(), e, transform, Vector2D.Zero(), transform.position.z + 0.2f, new Vector2D(scale));
		}

		SaveRenderTextureToSprite();
		
		destructible.initialized = true;
	}

	void Update() {
		Transform transform = renderCamera.transform;

		if (destructible.initialized == false) {
			Initialize();
			return;
		}

		if (destructible.eraseEvents.Count > 0 || destructible.modifiersAdded == true) {
			DrawSelf ();

			foreach(DestructionModifier modifier in destructible.modifiers) {
				Vector3 pos = (Vector3)modifier.position;

				float ratioX = (destructible.transform.localScale.x / destructible.transform.localScale.y);

				Vector2 size = modifier.size;

				pos.x *= transform.localScale.x;
				pos.y *= transform.localScale.y * ratioX;

				size.y *= ratioX;
			
				Vector2 scale = new Vector2(destructible.transform.localScale.x, destructible.transform.localScale.y);
				 
				Max2D.DrawImage(transform, modifier.material, pos, size, modifier.rotation, 0.3f, new Vector2D(scale));
			}

			// New Event Meshes That Erase Generation
			foreach(DestructionEvent e in destructible.eraseEvents) {
				Polygon2D polygon = e.polygon.ToLocalSpace(transform);

				polygon = polygon.ToOffset(new Vector2D(transform.position - destructible.transform.position));
				polygon = polygon.ToRotation(destructible.transform.rotation.eulerAngles.z * -Mathf.Deg2Rad);
				polygon = polygon.ToScale(new Vector2(1.0f / destructible.transform.localScale.x, 1.0f / destructible.transform.localScale.y));

				Mesh mesh = PolygonTriangulator2D.Triangulate(polygon, Vector2.zero, Vector2.zero, PolygonTriangulator2D.Triangulation.Advanced);

				destructible.eraseMeshes.Add(mesh);
			}

			// Erase Mesh
			foreach(Mesh e in destructible.eraseMeshes) {
				float ratioX = destructible.transform.localScale.x / destructible.transform.localScale.y;

				Vector2 scale = destructible.transform.localScale;
				scale.y *= ratioX;
				
				Max2D.DrawMesh(GetEraseMaterial(), e, transform, Vector2D.Zero(), transform.position.z + 0.2f, new Vector2D(scale));
			}

			SaveRenderTextureToSprite();

			destructible.eraseEvents.Clear();
			destructible.modifiersAdded = false;
		}
	} 

	public void OnRenderObject() {
		Update();

		destructible.UpdateCollider();
	}
}
