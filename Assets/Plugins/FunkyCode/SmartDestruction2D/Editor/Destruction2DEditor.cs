using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Destruction2D))]
public class Destruction2DEditor : Editor {
	override public void OnInspectorGUI() {
		Destruction2D script = target as Destruction2D;

		script.textureType = (Destruction2D.TextureType)EditorGUILayout.EnumPopup ("Texture Type", script.textureType);
		if (script.textureType == Destruction2D.TextureType.SpriteShape) {
			// Only Sprite Shape?
			script.spriteShapeBounds = (Destruction2D.SpriteShapeBounds)EditorGUILayout.EnumPopup ("Bounds", script.spriteShapeBounds);
			script.scaledPixelsPerUnit = EditorGUILayout.FloatField("Scaled Pixels Per Unit", script.scaledPixelsPerUnit);
		}

		if (script.textureType == Destruction2D.TextureType.Tilemap) {
			// Only Tile Map
			script.scaledPixelsPerUnit = EditorGUILayout.FloatField("Scaled Pixels Per Unit", script.scaledPixelsPerUnit);
		}

		// DEBUG

		//script.buffer = (Destruction2DBuffer)EditorGUILayout.ObjectField(script.buffer, typeof(Destruction2DBuffer));


		script.filterMode = (FilterMode)EditorGUILayout.EnumPopup ("Filter Mode", script.filterMode);

		script.destructionLayer = (DestructionLayer)EditorGUILayout.EnumPopup ("Destruction Layer", script.destructionLayer);
	
		EditorGUI.BeginDisabledGroup(script.chunks.enabled == true);
		script.centerOfMass = (Destruction2D.CenterOfMass)EditorGUILayout.EnumPopup ("Center of Mass", script.centerOfMass);
		EditorGUI.EndDisabledGroup();

		
		script.gridSize = EditorGUILayout.Vector2IntField("Grid Size", script.gridSize);

		EditorGUI.BeginDisabledGroup(script.split.enabled == true);

		script.chunks.enabled = EditorGUILayout.Toggle("Chunks", script.chunks.enabled);
		if (script.chunks.enabled) {
			script.centerOfMass = Destruction2D.CenterOfMass.Default;

			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			script.split.enabled = false;

			script.chunks.chunkOffset = (Destruction2DChunks.Offset)EditorGUILayout.EnumPopup ("Offsets", script.chunks.chunkOffset);

			script.chunks.chunksRows = EditorGUILayout.IntSlider("Rows", script.chunks.chunksRows, 1, 20);
			script.chunks.chunksColumns = EditorGUILayout.IntSlider("Columns", script.chunks.chunksColumns, 1, 20);

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		EditorGUI.EndDisabledGroup();

		
		EditorGUI.BeginDisabledGroup(script.chunks.enabled == true);

		script.split.enabled = EditorGUILayout.Toggle("Split", script.split.enabled);

		if (script.split.enabled) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			script.chunks.enabled = false;
			
			script.split.splitLimit = GUILayout.Toggle(script.split.splitLimit, "Split Limit");

			if (script.split.splitLimit) {
				script.split.maxSplits = EditorGUILayout.IntSlider("Max Splits", script.split.maxSplits, 1, 10);
			}
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		EditorGUI.EndDisabledGroup();

		//script.createChunks = EditorGUILayout.Toggle("Create Chunks", script.createChunks);
		//script.recalculateCenterPivot = EditorGUILayout.Toggle("Recalculate Pivot", script.recalculateCenterPivot);
	}
}
 