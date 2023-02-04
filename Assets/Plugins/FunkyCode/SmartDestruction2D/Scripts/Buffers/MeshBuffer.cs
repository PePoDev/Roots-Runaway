using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuffer : BufferBase {

	void Initialize() {
		MeshRenderer meshRenderer = destructible.gameObject.AddComponent<MeshRenderer>();
		MeshFilter meshFilter = destructible.gameObject.AddComponent<MeshFilter>();

		Mesh mesh = new Mesh();

		float sizeY = renderCamera.orthographicSize / destructible.transform.localScale.x;
		float sizeX = sizeY * ((float)destructible.outputTexture.width / (float)destructible.outputTexture.height);

		mesh.vertices = new Vector3[]{new Vector3(- sizeX, - sizeY, 0), new Vector3(sizeX, -sizeY, 0), new Vector3(sizeX, sizeY, 0), new Vector3(-sizeX, sizeY, 0)};
		mesh.triangles = new int[]{2, 1, 0, 0, 3, 2};
		mesh.uv = new Vector2[]{new Vector2(0, 0),new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

		meshFilter.mesh = mesh;
		
		DrawSelf ();

		RenderTexture.active = destructible.renderTexture;
		destructible.outputTexture.ReadPixels(new Rect(0, 0, destructible.outputTexture.width, destructible.outputTexture.height), 0, 0);
		destructible.outputTexture.Apply();

		destructible.outputMaterial.mainTexture = destructible.renderTexture;
		meshRenderer.material = destructible.outputMaterial;

		destructible.initialized = true;
	}

	void Update() {
		Transform transform = renderCamera.transform;
		
		if (destructible.initialized == false) {
			Initialize();
			return;
		} 

		if (destructible.eraseEvents.Count > 0 || destructible.modifiersAdded == true) {
			MeshRenderer meshRenderer = destructible.gameObject.GetComponent<MeshRenderer>();				
			destructible.outputMaterial.mainTexture = destructible.outputTexture;

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

			RenderTexture.active = destructible.renderTexture;
			destructible.outputTexture.ReadPixels(new Rect(0, 0, destructible.outputTexture.width, destructible.outputTexture.height), 0, 0);
			destructible.outputTexture.Apply();
			
			destructible.outputMaterial.mainTexture = destructible.renderTexture;
			meshRenderer.material = destructible.outputMaterial;

			destructible.eraseEvents.Clear();
		}

	} 

	public void OnRenderObject() {
		Update();

		destructible.UpdateCollider();
	}
}
