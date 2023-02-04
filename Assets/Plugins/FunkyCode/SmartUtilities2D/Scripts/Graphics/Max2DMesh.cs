using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mesh2DTriangle {
	public List<Vector2> uv = new List<Vector2>();
	public List<Vector3> vertices = new List<Vector3>();
}

public class Max2DMesh {
	const float pi = Mathf.PI;
	const float pi2 = pi / 2;
	const float uv0 = 1f / 32;
	const float uv1 = 1f - uv0;

	public enum LineType {Default, Legacy};

	public static LineType lineType = LineType.Default;

	static public void Draw(Mesh mesh, Transform transform, Material material) {
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 scale = transform.lossyScale;
		Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);

		Graphics.DrawMesh(mesh, matrix, material, 0);
	}

	static public void Draw(Mesh mesh, Material material) {
		Vector3 position = Vector3.zero;
		Quaternion rotation = Quaternion.Euler(0, 0, 0);
		Vector3 scale = new Vector3(1, 1, 1);
		Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);

		Graphics.DrawMesh(mesh, matrix, material, 0);
	}

	static public Mesh2DTriangle CreateBox(float size) {
		Mesh2DTriangle result = new Mesh2DTriangle();

		result.uv.Add(new Vector2(uv0, uv0));
		result.vertices.Add(new Vector3(-size, -size, 0));
		result.uv.Add(new Vector2(uv1, uv0));
		result.vertices.Add(new Vector3(size, -size, 0));
		result.uv.Add(new Vector2(uv1, uv1));
		result.vertices.Add(new Vector3(size, size, 0));

		result.uv.Add(new Vector2(uv1, uv1));
		result.vertices.Add(new Vector3(size, size, 0));
		result.uv.Add(new Vector2(uv1, uv0));
		result.vertices.Add(new Vector3(-size, size, 0));
		result.uv.Add(new Vector2(uv0, uv0));
		result.vertices.Add(new Vector3(-size, -size, 0));
		
		return(result);
	}

	static public Mesh2DTriangle CreateLineLegacy(Pair2D pair, float lineWidth, float z = 0f) {
		Mesh2DTriangle result = new Mesh2DTriangle();

		float size = lineWidth / 6;
		float rot = (float)Vector2D.Atan2 (pair.A, pair.B);

		Vector2D A1 = new Vector2D (pair.A);
		Vector2D A2 = new Vector2D (pair.A);
		Vector2D B1 = new Vector2D (pair.B);
		Vector2D B2 = new Vector2D (pair.B);

		Vector2 scale = new Vector2(1f, 1f);

		A1.Push (rot + pi2, size, scale);
		A2.Push (rot - pi2, size, scale);
		B1.Push (rot + pi2, size, scale);
		B2.Push (rot - pi2, size, scale);

		result.uv.Add(new Vector2(0.5f + uv0, 0));
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		result.uv.Add(new Vector2(uv1, 0));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));
		result.uv.Add(new Vector2(uv1, 1));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(0.5f + uv0, 1));
		result.vertices.Add(new Vector3((float)B2.x, (float)B2.y, z));
	
		A1 = new Vector2D (pair.A);
		A2 = new Vector2D (pair.A);
		Vector2D A3 = new Vector2D (pair.A);
		Vector2D A4 = new Vector2D (pair.A);

		A1.Push (rot + pi2, size, scale);
		A2.Push (rot - pi2, size, scale);

		A3.Push (rot + pi2, size, scale);
		A4.Push (rot - pi2, size, scale);
		A3.Push (rot + pi, -size, scale);
		A4.Push (rot + pi, -size, scale);

		result.uv.Add(new Vector2(uv0, 0));
		result.vertices.Add(new Vector3((float)A3.x, (float)A3.y, z));
		result.uv.Add(new Vector2(uv0, 1));
		result.vertices.Add(new Vector3((float)A4.x, (float)A4.y, z));
		result.uv.Add(new Vector2(0.5f - uv0, 1));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(0.5f - uv0, 0));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));

		B1 = new Vector2D (pair.B);
		B2 = new Vector2D (pair.B);
		Vector2D B3 = new Vector2D (pair.B);
		Vector2D B4 = new Vector2D (pair.B);

		B1.Push (rot + pi2, size, scale);
		B2.Push (rot - pi2, size, scale);

		B3.Push (rot + pi2, size, scale);
		B4.Push (rot - pi2, size, scale);
		B3.Push (rot + pi, size, scale);
		B4.Push (rot + pi , size, scale);

		result.uv.Add(new Vector2(uv0, 0));
		result.vertices.Add(new Vector3((float)B4.x, (float)B4.y, z));
		result.uv.Add(new Vector2(uv0, 1));
		result.vertices.Add(new Vector3((float)B3.x, (float)B3.y, z));
		result.uv.Add(new Vector2(0.5f - uv0, 1));
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		result.uv.Add(new Vector2(0.5f - uv0, 0));
		result.vertices.Add(new Vector3((float)B2.x, (float)B2.y, z));

		return(result);
	}

	static public Mesh2DTriangle CreateLine(Pair2D pair, float lineWidth, float z = 0f) {
		Mesh2DTriangle result = new Mesh2DTriangle();

		float xuv0 = 0;
		float xuv1 = 1f;

		float yuv0 = 0;
		float yuv1 = 1f;

		float size = lineWidth / 6;
		float rot = (float)Vector2D.Atan2 (pair.A, pair.B);

		Vector2D A1 = pair.A.Copy();
		Vector2D A2 = pair.A.Copy();
		Vector2D B1 = pair.B.Copy();
		Vector2D B2 = pair.B.Copy();

		A1.Push (rot + pi2, size);
		A2.Push (rot - pi2, size);
		B1.Push (rot + pi2, size);
		B2.Push (rot - pi2, size);

		result.uv.Add(new Vector2(xuv1 / 3, yuv1)); 
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		result.uv.Add(new Vector2(1 - xuv1 / 3, yuv1));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));
		result.uv.Add(new Vector2(1 - xuv1 / 3, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		
		result.uv.Add(new Vector2(1 - xuv1 / 3, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(yuv1 / 3, xuv0));
		result.vertices.Add(new Vector3((float)B2.x, (float)B2.y, z));
		result.uv.Add(new Vector2(xuv1 / 3, yuv1));
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));

		Vector2D A3 = A1.Copy();
		Vector2D A4 = A1.Copy();
	
		A3.Push (rot - pi2, size);

		A3 = A1.Copy();
		A4 = A2.Copy();

		A1.Push (rot, size);
		A2.Push (rot, size);

		result.uv.Add(new Vector2(xuv1 / 3, yuv1)); 
		result.vertices.Add(new Vector3((float)A3.x, (float)A3.y, z));
		result.uv.Add(new Vector2(xuv0, yuv1));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));
		result.uv.Add(new Vector2(xuv0, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		
		result.uv.Add(new Vector2(xuv0, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(yuv1 / 3, xuv0));
		result.vertices.Add(new Vector3((float)A4.x, (float)A4.y, z));
		result.uv.Add(new Vector2(xuv1 / 3, yuv1));
		result.vertices.Add(new Vector3((float)A3.x, (float)A3.y, z));

		A1 = B1.Copy();
		A2 = B2.Copy();

		B1.Push (rot - Mathf.PI, size);
		B2.Push (rot - Mathf.PI, size);
		
		result.uv.Add(new Vector2(xuv0, yuv1)); 
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		result.uv.Add(new Vector2(xuv1 / 3, yuv1));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));
		result.uv.Add(new Vector2(xuv1 / 3, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		
		result.uv.Add(new Vector2(xuv1 / 3, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(yuv0, xuv0));
		result.vertices.Add(new Vector3((float)B2.x, (float)B2.y, z));
		result.uv.Add(new Vector2(xuv0, yuv1));
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		
		return(result);
	}
	static public Mesh2DTriangle CreateLine(Pair2D pair, Transform transform, float lineWidth, float z = 0f) {
		if (lineType == LineType.Legacy) {
			return(CreateLine(pair, lineWidth, z));
		}

		Mesh2DTriangle result = new Mesh2DTriangle();

		float xuv0 = 0; //1f / 128;
		float xuv1 = 1f - xuv0;
		float yuv0 = 0; //1f / 192;
		float yuv1 = 1f - xuv0;

		float size = lineWidth / 6;
		float rot = (float)Vector2D.Atan2 (pair.A, pair.B);

		Vector2D A1 = new Vector2D (pair.A);
		Vector2D A2 = new Vector2D (pair.A);
		Vector2D B1 = new Vector2D (pair.B);
		Vector2D B2 = new Vector2D (pair.B);

		Vector2 scale = new Vector2(1f / transform.localScale.x, 1f / transform.localScale.y);

		A1.Push (rot + pi2, size, scale);
		A2.Push (rot - pi2, size, scale);
		B1.Push (rot + pi2, size, scale);
		B2.Push (rot - pi2, size, scale);

		result.uv.Add(new Vector2(xuv1 / 3, yuv1)); 
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		result.uv.Add(new Vector2(1 - xuv1 / 3, yuv1));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));
		result.uv.Add(new Vector2(1 - xuv1 / 3, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		
		result.uv.Add(new Vector2(1 - xuv1 / 3, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(yuv1 / 3, xuv0));
		result.vertices.Add(new Vector3((float)B2.x, (float)B2.y, z));
		result.uv.Add(new Vector2(xuv1 / 3, yuv1));
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));

		Vector2D A3 = A1.Copy();
		Vector2D A4 = A1.Copy();
	
		A3.Push (rot - pi2, size, scale);

		A3 = A1.Copy();
		A4 = A2.Copy();

		A1.Push (rot, size, scale);
		A2.Push (rot, size, scale);

		result.uv.Add(new Vector2(xuv1 / 3, yuv1)); 
		result.vertices.Add(new Vector3((float)A3.x, (float)A3.y, z));
		result.uv.Add(new Vector2(xuv0, yuv1));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));
		result.uv.Add(new Vector2(xuv0, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		
		result.uv.Add(new Vector2(xuv0, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(yuv1 / 3, xuv0));
		result.vertices.Add(new Vector3((float)A4.x, (float)A4.y, z));
		result.uv.Add(new Vector2(xuv1 / 3, yuv1));
		result.vertices.Add(new Vector3((float)A3.x, (float)A3.y, z));

		A1 = B1.Copy();
		A2 = B2.Copy();

		B1.Push (rot - Mathf.PI, size, scale);
		B2.Push (rot - Mathf.PI, size, scale);
		
		result.uv.Add(new Vector2(xuv0, yuv1)); 
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		result.uv.Add(new Vector2(xuv1 / 3, yuv1));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));
		result.uv.Add(new Vector2(xuv1 / 3, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		
		result.uv.Add(new Vector2(xuv1 / 3, yuv0));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(yuv0, xuv0));
		result.vertices.Add(new Vector3((float)B2.x, (float)B2.y, z));
		result.uv.Add(new Vector2(xuv0, yuv1));
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		
		return(result);
	}

	static public Mesh ExportMesh(List<Mesh2DTriangle> trianglesList) {
		if (lineType == LineType.Legacy) {
			return(ExportMeshLegacy(trianglesList));
		}

		Mesh mesh = new Mesh();
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		int count = 0;
		foreach(Mesh2DTriangle triangle in trianglesList) {
			foreach(Vector3 v in triangle.vertices) {
				vertices.Add(v);
			}
			foreach(Vector2 u in triangle.uv) {
				uv.Add(u);
			}
			
			int iCount = triangle.vertices.Count;
			for(int i = 0; i < iCount; i++) {
				triangles.Add(count + i);
			}
			
			count += iCount;
		}

		mesh.vertices = vertices.ToArray();
		mesh.uv = uv.ToArray();
		mesh.triangles = triangles.ToArray();

		return(mesh);
	}

	static public Mesh GeneratePolygon2DMesh(Transform transform, Polygon2D polygon, float lineOffset, float lineWidth, bool connectedLine) {
		List<Mesh2DTriangle> trianglesList = new List<Mesh2DTriangle>();

		Polygon2D poly = polygon;

		foreach(Pair2D p in Pair2D.GetList(poly.pointsList, connectedLine)) {
			trianglesList.Add(CreateLine(p, transform, lineWidth, lineOffset));
		}

		foreach(Polygon2D hole in poly.holesList) {
			foreach(Pair2D p in Pair2D.GetList(hole.pointsList, connectedLine)) {
				trianglesList.Add(CreateLine(p, transform, lineWidth, lineOffset));
			}
		}
		
		return(ExportMesh(trianglesList));
	}

	static public Mesh GeneratePolygon2DMeshNew(Transform transform, Polygon2D polygon, float lineOffset, float lineWidth, bool connectedLine) {
		List<Mesh2DTriangle> trianglesList = new List<Mesh2DTriangle>();

		Polygon2D poly = polygon;

		foreach(Pair2D p in Pair2D.GetList(poly.pointsList, connectedLine)) {
			trianglesList.Add(CreateLine(p, transform, lineWidth, lineOffset));
		}

		foreach(Polygon2D hole in poly.holesList) {
			foreach(Pair2D p in Pair2D.GetList(hole.pointsList, connectedLine)) {
				trianglesList.Add(CreateLine(p, transform, lineWidth, lineOffset));
			}
		}
		
		return(ExportMesh(trianglesList));
	}


	static public Mesh ExportMeshLegacy(List<Mesh2DTriangle> trianglesList) {
		Mesh mesh = new Mesh();
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		int count = 0;
		foreach(Mesh2DTriangle triangle in trianglesList) {
			foreach(Vector3 v in triangle.vertices) {
				vertices.Add(v);
			}
			foreach(Vector2 u in triangle.uv) {
				uv.Add(u);
			}
			
			triangles.Add(count + 0);
			triangles.Add(count + 1);
			triangles.Add(count + 2);

			triangles.Add(count + 4);
			triangles.Add(count + 5);
			triangles.Add(count + 6);

			triangles.Add(count + 8);
			triangles.Add(count + 9);
			triangles.Add(count + 10);

			triangles.Add(count + 3);
			triangles.Add(count + 0);
			triangles.Add(count + 2);

			triangles.Add(count + 7);
			triangles.Add(count + 4);
			triangles.Add(count + 6);

			triangles.Add(count + 11);
			triangles.Add(count + 8);
			triangles.Add(count + 10);
			
			count += 12;
		}

		mesh.vertices = vertices.ToArray();
		mesh.uv = uv.ToArray();
		mesh.triangles = triangles.ToArray();

		return(mesh);
	}
}
/* 
	
	}*/


/* 
	static public Mesh2DTriangle CreateLine(Pair2D pair, Transform transform, float lineWidth, float z = 0f) {
		Mesh2DTriangle result = new Mesh2DTriangle();

		float size = lineWidth / 6;
		float rot = (float)Vector2D.Atan2 (pair.A, pair.B);

		Vector2D A1 = new Vector2D (pair.A);
		Vector2D A2 = new Vector2D (pair.A);
		Vector2D B1 = new Vector2D (pair.B);
		Vector2D B2 = new Vector2D (pair.B);

		Vector2 scale = new Vector2(1f / transform.localScale.x, 1f / transform.localScale.y);

		A1.Push (rot + pi2, size, scale);
		A2.Push (rot - pi2, size, scale);
		B1.Push (rot + pi2, size, scale);
		B2.Push (rot - pi2, size, scale);

		result.uv.Add(new Vector2(0.5f + uv0, 0));
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		result.uv.Add(new Vector2(uv1, 0));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));
		result.uv.Add(new Vector2(uv1, 1));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(0.5f + uv0, 1));
		result.vertices.Add(new Vector3((float)B2.x, (float)B2.y, z));
	
		A1 = new Vector2D (pair.A);
		A2 = new Vector2D (pair.A);
		Vector2D A3 = new Vector2D (pair.A);
		Vector2D A4 = new Vector2D (pair.A);

		A1.Push (rot + pi2, size, scale);
		A2.Push (rot - pi2, size, scale);

		A3.Push (rot + pi2, size, scale);
		A4.Push (rot - pi2, size, scale);
		A3.Push (rot + pi, -size, scale);
		A4.Push (rot + pi, -size, scale);

		result.uv.Add(new Vector2(uv0, 0));
		result.vertices.Add(new Vector3((float)A3.x, (float)A3.y, z));
		result.uv.Add(new Vector2(uv0, 1));
		result.vertices.Add(new Vector3((float)A4.x, (float)A4.y, z));
		result.uv.Add(new Vector2(0.5f - uv0, 1));
		result.vertices.Add(new Vector3((float)A2.x, (float)A2.y, z));
		result.uv.Add(new Vector2(0.5f - uv0, 0));
		result.vertices.Add(new Vector3((float)A1.x, (float)A1.y, z));

		B1 = new Vector2D (pair.B);
		B2 = new Vector2D (pair.B);
		Vector2D B3 = new Vector2D (pair.B);
		Vector2D B4 = new Vector2D (pair.B);

		B1.Push (rot + pi2, size, scale);
		B2.Push (rot - pi2, size, scale);

		B3.Push (rot + pi2, size, scale);
		B4.Push (rot - pi2, size, scale);
		B3.Push (rot + pi, size, scale);
		B4.Push (rot + pi , size, scale);

		result.uv.Add(new Vector2(uv0, 0));
		result.vertices.Add(new Vector3((float)B4.x, (float)B4.y, z));
		result.uv.Add(new Vector2(uv0, 1));
		result.vertices.Add(new Vector3((float)B3.x, (float)B3.y, z));
		result.uv.Add(new Vector2(0.5f - uv0, 1));
		result.vertices.Add(new Vector3((float)B1.x, (float)B1.y, z));
		result.uv.Add(new Vector2(0.5f - uv0, 0));
		result.vertices.Add(new Vector3((float)B2.x, (float)B2.y, z));

		return(result);
	}
*/