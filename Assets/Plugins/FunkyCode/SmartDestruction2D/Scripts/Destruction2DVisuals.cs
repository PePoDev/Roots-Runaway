using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Destruction2DVisuals {
	public Max2DMesh.LineType lineType =  Max2DMesh.LineType.Default;
	public bool drawVisuals = true;
	public float visualScale = 1f;
	public float lineWidth = 1.0f;
	public float lineEndWidth = 1.0f;
	public float zPosition = 0f;
	public Color destructionColor = Color.white;
	public bool lineBorder = true;
	public float lineEndSize = 0.5f;
	public float vertexSpace = 0.25f;
	public float borderScale = 2f;
	public float minVertexDistance = 1f;

	// Mesh & Material
	private Mesh mesh = null;
	private Mesh meshBorder = null;

	private Material lineMaterial;
	private Material lineMaterialBorder;
	private Material lineLegacyMaterial;

	public void GenerateComplexMesh(List<Vector2D> points, Transform transform) {
		meshBorder = Destruction2DVisualsMesh.GenerateComplexMesh(points, transform, lineWidth * visualScale * borderScale, minVertexDistance, zPosition - 0.001f, lineEndSize * visualScale,  lineEndWidth * visualScale * borderScale, vertexSpace);
		mesh = Destruction2DVisualsMesh.GenerateComplexMesh(points, transform, lineWidth * visualScale, minVertexDistance, zPosition - 0.002f, lineEndSize * visualScale, lineEndWidth * visualScale, vertexSpace);
	}

	public void GeneratePolygonMesh(Vector2D pos, Polygon2D.PolygonType polygonType, float polygonSize, Transform transform) {
		meshBorder = Destruction2DVisualsMesh.GeneratePolygonMesh(pos, polygonType, polygonSize * visualScale, minVertexDistance, transform, lineWidth * visualScale * borderScale, zPosition - 0.001f);
		mesh = Destruction2DVisualsMesh.GeneratePolygonMesh(pos, polygonType, polygonSize * visualScale, minVertexDistance, transform, lineWidth * visualScale, zPosition - 0.002f);
	}
	
	public void GenerateLinearMesh(Pair2D linearPair, Transform transform) {
		meshBorder = Destruction2DVisualsMesh.GenerateLinearMesh(linearPair, transform, lineWidth * visualScale * borderScale, zPosition - 0.001f, lineEndSize * visualScale, lineEndWidth * visualScale * borderScale);
		mesh = Destruction2DVisualsMesh.GenerateLinearMesh(linearPair, transform, lineWidth * visualScale, zPosition - 0.002f, lineEndSize * visualScale, lineEndWidth * visualScale);
	}

	public void GenerateLinearCutMesh(Pair2D linearPair, float cutSize, Transform transform) {
		meshBorder = Destruction2DVisualsMesh.GenerateLinearCutMesh(linearPair, cutSize * visualScale, transform, lineWidth * visualScale * borderScale, zPosition - 0.001f);
		mesh = Destruction2DVisualsMesh.GenerateLinearCutMesh(linearPair, cutSize * visualScale, transform, lineWidth * visualScale, zPosition - 0.002f);
	}

	public void GenerateComplexCutMesh(List<Vector2D> pointsList, float cutSize, Transform transform) {
		meshBorder = Destruction2DVisualsMesh.GenerateComplexCutMesh(pointsList, cutSize * visualScale, transform, lineWidth * visualScale * borderScale, zPosition - 0.001f);
		mesh = Destruction2DVisualsMesh.GenerateComplexCutMesh(pointsList, cutSize * visualScale, transform, lineWidth * visualScale, zPosition - 0.002f);
	}

	public void Initialize() {
		Max2D.Check();

		lineMaterial = new Material(Max2D.lineMaterial);
		lineMaterialBorder = new Material(Max2D.lineMaterial);
		lineLegacyMaterial = new Material(Max2D.lineLegacyMaterial);
	}

	public void Draw() {
		if (lineType == Max2DMesh.LineType.Legacy) {
			lineLegacyMaterial.SetColor ("_Emission", destructionColor);
			Max2DMesh.Draw(mesh, lineLegacyMaterial);

		} else {
			if (lineBorder && meshBorder != null) {
				//lineMaterialBorder.color = Color.black;
				lineMaterial.SetColor ("_Emission", Color.black);
				Max2DMesh.Draw(meshBorder, lineMaterialBorder);
			}
			
			//lineMaterial.color = destructionColor;
			lineMaterial.SetColor ("_Emission", destructionColor);
			Max2DMesh.Draw(mesh, lineMaterial);
		}
	}
}
