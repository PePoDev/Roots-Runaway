using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Destruction2DController))]
public class Destruction2DControllerEditor : Editor {
	static bool visualsFoldout = true;
	static bool foldout = true;
	static bool modifiersFoldout = true;

	override public void OnInspectorGUI()
	{
		Destruction2DController script = target as Destruction2DController;

		script.destructionType = (Destruction2DController.DestructionType)EditorGUILayout.EnumPopup ("Destruction Type", script.destructionType);
		script.destructionLayer.SetLayerType((Destruction2DLayerType)EditorGUILayout.EnumPopup ("Destruction Layer", script.destructionLayer.GetLayerType()));

		EditorGUI.indentLevel = EditorGUI.indentLevel + 2;

		if (script.destructionLayer.GetLayerType() == Destruction2DLayerType.Selected) {
			for (int i = 0; i < 8; i++) {
				script.destructionLayer.SetLayer(i, EditorGUILayout.Toggle ("Layer " + (i + 1), script.destructionLayer.GetLayerState(i)));
			}
		}

		EditorGUI.indentLevel = EditorGUI.indentLevel - 2;

		Destruction2DVisuals visuals = script.visuals;

		visualsFoldout = EditorGUILayout.Foldout(visualsFoldout, "Visuals");
		if (visualsFoldout) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			visuals.drawVisuals = EditorGUILayout.Toggle ("Enable Visuals", visuals.drawVisuals);

			if (visuals.drawVisuals == true) {
				visuals.zPosition = EditorGUILayout.FloatField ("Z Position", visuals.zPosition);
				visuals.destructionColor = (Color)EditorGUILayout.ColorField ("Color",visuals.destructionColor);
				visuals.visualScale = EditorGUILayout.Slider("Scale", visuals.visualScale, 1f, 50f);
				visuals.lineBorder = EditorGUILayout.Toggle ("Border", visuals.lineBorder);
				visuals.borderScale = EditorGUILayout.Slider("Border Scale", visuals.borderScale, 1f, 5f);
				visuals.lineWidth = EditorGUILayout.Slider ("Width", visuals.lineWidth, 0.01f, 5f);
				visuals.lineEndWidth = EditorGUILayout.Slider ("Line End Width", visuals.lineEndWidth, 0.01f, 5f);
				visuals.minVertexDistance = EditorGUILayout.Slider("Min Vertex Distance", visuals.minVertexDistance, 0.1f, 5f);
			}
			
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		DestructionTypesUpdate (script);

		if (Destruction2DController.DestructionType.Polygon == script.destructionType || Destruction2DController.DestructionType.Modifier == script.destructionType) {

			modifiersFoldout = EditorGUILayout.Foldout(modifiersFoldout, "Mofidiers" );
			if (modifiersFoldout) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				
				script.modifierSize = EditorGUILayout.Vector2Field("Size", script.modifierSize);
				script.randomRotation = EditorGUILayout.Toggle("Random Rotation", script.randomRotation);
				if (script.randomRotation == false) {
					script.modifierRotation = EditorGUILayout.FloatField("Rotation", script.modifierRotation);
				}

				script.randomModifierID = EditorGUILayout.Toggle("Random Modifier", script.randomModifierID);
				if (script.randomModifierID == false) {
					script.modifierID = EditorGUILayout.IntField("ModifierID", script.modifierID);
				}

				SerializedProperty myIterator = serializedObject.FindProperty("modifierTextures");
				while (true) {
					Rect myRect = GUILayoutUtility.GetRect(0f, 16f);
					bool showChildren = EditorGUI.PropertyField(myRect, myIterator);
					
					if (myIterator.NextVisible(showChildren) == false) {
						break;
					}

				}
				serializedObject.ApplyModifiedProperties();

					
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}
	}

	void DestructionTypesUpdate(Destruction2DController script){
		Destruction2DVisuals visuals = script.visuals;

		switch (script.destructionType) {

			case Destruction2DController.DestructionType.LinearCut:
				foldout = EditorGUILayout.Foldout(foldout, "Linear Cut" );
				if (foldout) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					
					script.cutSize = EditorGUILayout.FloatField ("Linear Cut Size", script.cutSize);
					if (script.cutSize < 0.01f) {
						script.cutSize = 0.01f;
					}

					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}
				break;

			case Destruction2DController.DestructionType.ComplexCut:
				foldout = EditorGUILayout.Foldout(foldout, "Complex Cut" );
				if (foldout) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					
					script.cutSize = EditorGUILayout.FloatField ("Complex Cut Size", script.cutSize);
					if (script.cutSize < 0.01f) {
						script.cutSize = 0.01f;
					}
					
					visuals.minVertexDistance = EditorGUILayout.FloatField ("Min Vertex Size", visuals.minVertexDistance);

					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}
				break;

			case Destruction2DController.DestructionType.Polygon:
				foldout = EditorGUILayout.Foldout(foldout, "Polygon Cut");
				if (foldout) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

					script.polygonType = (Polygon2D.PolygonType)EditorGUILayout.EnumPopup ("Type", script.polygonType);
					script.polygonSize = EditorGUILayout.FloatField ("Size", script.polygonSize);
					visuals.minVertexDistance = EditorGUILayout.FloatField ("Min Vertex Size", visuals.minVertexDistance);
					if (script.polygonType == Polygon2D.PolygonType.Circle) {
						script.polygonEdgeCount = EditorGUILayout.IntField ("Edge Count", script.polygonEdgeCount);	
					}
					
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}
				break;

			case Destruction2DController.DestructionType.PolygonBrush:
				foldout = EditorGUILayout.Foldout(foldout, "Polygon Brush");
				if (foldout) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

					script.polygonType = (Polygon2D.PolygonType)EditorGUILayout.EnumPopup ("Type", script.polygonType);
					script.polygonSize = EditorGUILayout.FloatField ("Size", script.polygonSize);
					visuals.minVertexDistance = EditorGUILayout.FloatField ("Min Vertext Size", visuals.minVertexDistance);
					if (script.polygonType == Polygon2D.PolygonType.Circle) {
						script.polygonEdgeCount = EditorGUILayout.IntField ("Edge Count", script.polygonEdgeCount);	
					}
					
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}
				break;

			case Destruction2DController.DestructionType.ComplexBrush:
				foldout = EditorGUILayout.Foldout(foldout, "Complex Brush");
				if (foldout) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

					script.cutSize = EditorGUILayout.FloatField ("Complex Cut Size", script.cutSize);
					if (script.cutSize < 0.01f) {
						script.cutSize = 0.01f;
					}

					visuals.minVertexDistance = EditorGUILayout.FloatField ("Min Vertex Size", visuals.minVertexDistance);

					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}
				break;
		}
	}
}