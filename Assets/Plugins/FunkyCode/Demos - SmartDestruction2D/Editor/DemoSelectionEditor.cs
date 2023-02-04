/* 
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DemoSelection))]
public class DemoSelectionEditor : Editor {

	override public void OnInspectorGUI() {
		DemoSelection script = target as DemoSelection;

		script.scenesCount = EditorGUILayout.IntField(script.scenesCount);
		
		if (script.scenes == null) {
			System.Array.Resize(ref script.scenes, script.scenesCount);
		}

		if (script.scenes.Length < script.scenesCount) {
			System.Array.Resize(ref script.scenes, script.scenesCount);
		}

		for(int i = 0; i < script.scenesCount; i++) {
			if (script.scenes[i] == null) {
				script.scenes[i] = new DemoScene();
			}

			script.scenes[i].transform = (Transform)EditorGUILayout.ObjectField(script.scenes[i].transform, typeof(Transform), true);
			script.scenes[i].tracked = (Transform)EditorGUILayout.ObjectField(script.scenes[i].tracked, typeof(Transform), true);
			script.scenes[i].cameraSize = EditorGUILayout.FloatField(script.scenes[i].cameraSize);
			script.scenes[i].distance = EditorGUILayout.FloatField(script.scenes[i].cameraSize);
		}


		if (GUI.changed) {
			EditorUtility.SetDirty (script);
		}
	}
}
*/