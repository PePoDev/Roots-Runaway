using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Destruction2DVisualsMesh {
	const float pi = Mathf.PI;
	const float pi2 = pi / 2;
	const float uv0 = 1f / 32;
	const float uv1 = 1f - uv0;

	
	static public Mesh GenerateComplexMesh(List<Vector2D> complexSlicerPointsList, Transform transform, float lineWidth, float minVertexDistance, float zPosition, float squareSize, float lineEndWidth, float vertexSpace) {
		List<Mesh2DTriangle> trianglesList = new List<Mesh2DTriangle>();

		complexSlicerPointsList = new List<Vector2D>(complexSlicerPointsList);
		complexSlicerPointsList.Add(complexSlicerPointsList.First());

		float size = squareSize;
		
		Vector2D vA, vB;
		List<Pair2D> list = Pair2D.GetList(complexSlicerPointsList, false);
		foreach(Pair2D pair in list) {
			vA = new Vector2D (pair.A);
			vB = new Vector2D (pair.B);

			vA.Push (Vector2D.Atan2 (pair.A, pair.B), -minVertexDistance * vertexSpace);
			vB.Push (Vector2D.Atan2 (pair.A, pair.B), minVertexDistance * vertexSpace);

			trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(vA, vB), transform, lineWidth, zPosition));
		}

		Pair2D linearPair = Pair2D.Zero();
		linearPair.A = new Vector2D(complexSlicerPointsList.First());
		linearPair.B = new Vector2D(complexSlicerPointsList.Last());
		
		return(Max2DMesh.ExportMesh(trianglesList));
	}


	static public Mesh GenerateLinearCutMesh(Pair2D linearPair, float cutSize, Transform transform, float lineWidth, float zPosition) {
		List<Mesh2DTriangle> trianglesList = new List<Mesh2DTriangle>();

		LinearCut linearCutLine = LinearCut.Create(linearPair, cutSize);
		foreach(Pair2D pair in Pair2D.GetList(linearCutLine.GetPointsList(), true)) {
			trianglesList.Add(Max2DMesh.CreateLine(pair, transform, lineWidth, zPosition));
		}

		return(Max2DMesh.ExportMesh(trianglesList));
	}

	static public Mesh GenerateComplexCutMesh(List<Vector2D> complexSlicerPointsList, float cutSize, Transform transform, float lineWidth, float zPosition) {
		List<Mesh2DTriangle> trianglesList = new List<Mesh2DTriangle>();

		ComplexCut complexCutLine = ComplexCut.Create(complexSlicerPointsList, cutSize);
		foreach(Pair2D pair in Pair2D.GetList(complexCutLine.GetPointsList(), true)) {
			trianglesList.Add(Max2DMesh.CreateLine(pair, transform, lineWidth, zPosition));
		}

		return(Max2DMesh.ExportMesh(trianglesList));
	}

	static public Mesh GeneratePolygonMesh(Vector2D pos, Polygon2D.PolygonType polygonType, float polygonSize, float minVertexDistance, Transform transform, float lineWidth, float zPosition) {
		List<Mesh2DTriangle> trianglesList = new List<Mesh2DTriangle>();

		Polygon2D slicePolygon = Polygon2D.Create (polygonType, polygonSize).ToOffset(pos);

		Vector2D vA, vB;
		foreach(Pair2D pair in Pair2D.GetList(slicePolygon.pointsList, true)) {
			vA = new Vector2D (pair.A);
			vB = new Vector2D (pair.B);

			vA.Push (Vector2D.Atan2 (pair.A, pair.B), -minVertexDistance / 5);
			vB.Push (Vector2D.Atan2 (pair.A, pair.B), minVertexDistance / 5);

			trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(vA, vB), transform, lineWidth, zPosition));
		}

		return(Max2DMesh.ExportMesh(trianglesList));
	}

	static public Mesh GeneratePolygon2DMesh(Transform transform, Polygon2D polygon, float lineOffset, float lineWidth, bool connectedLine) {
		List<Mesh2DTriangle> trianglesList = new List<Mesh2DTriangle>();

		Polygon2D poly = polygon;

		foreach(Pair2D p in Pair2D.GetList(poly.pointsList, connectedLine)) {
			trianglesList.Add(Max2DMesh.CreateLine(p, transform, lineWidth, lineOffset));
		}

		foreach(Polygon2D hole in poly.holesList) {
			foreach(Pair2D p in Pair2D.GetList(hole.pointsList, connectedLine)) {
				trianglesList.Add(Max2DMesh.CreateLine(p, transform, lineWidth, lineOffset));
			}
		}
		
		return(Max2DMesh.ExportMesh(trianglesList));
	}

	static public Mesh GenerateLinearMesh(Pair2D linearPair, Transform transform, float lineWidth, float zPosition, float squareSize, float lineEndWidth) {
		List<Mesh2DTriangle> trianglesList = new List<Mesh2DTriangle>();

		float size = squareSize;

		trianglesList.Add(Max2DMesh.CreateLine(linearPair, transform, lineWidth, zPosition));
		trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(linearPair.A.x - size, linearPair.A.y - size), new Vector2D(linearPair.A.x + size, linearPair.A.y - size)), transform, lineEndWidth, zPosition));
		trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(linearPair.A.x - size, linearPair.A.y - size), new Vector2D(linearPair.A.x - size, linearPair.A.y + size)), transform, lineEndWidth, zPosition));
		trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(linearPair.A.x + size, linearPair.A.y + size), new Vector2D(linearPair.A.x + size, linearPair.A.y - size)), transform, lineEndWidth, zPosition));
		trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(linearPair.A.x + size, linearPair.A.y + size), new Vector2D(linearPair.A.x - size, linearPair.A.y + size)), transform, lineEndWidth, zPosition));
	
		trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(linearPair.B.x - size, linearPair.B.y - size), new Vector2D(linearPair.B.x + size, linearPair.B.y - size)), transform, lineEndWidth, zPosition));
		trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(linearPair.B.x - size, linearPair.B.y - size), new Vector2D(linearPair.B.x - size, linearPair.B.y + size)), transform, lineEndWidth, zPosition));
		trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(linearPair.B.x + size, linearPair.B.y + size), new Vector2D(linearPair.B.x + size, linearPair.B.y - size)), transform, lineEndWidth, zPosition));
		trianglesList.Add(Max2DMesh.CreateLine(new Pair2D(new Vector2D(linearPair.B.x + size, linearPair.B.y + size), new Vector2D(linearPair.B.x - size, linearPair.B.y + size)), transform, lineEndWidth, zPosition));
	
		return(Max2DMesh.ExportMesh(trianglesList));
	}
}
