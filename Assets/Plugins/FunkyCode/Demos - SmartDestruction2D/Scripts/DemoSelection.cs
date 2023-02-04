using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSelection : MonoBehaviour {
	public GameObject UI;
	public int sceneID;

	public DemoScene[] scenes;

	public static DemoSelection instance;

	public void Start() {
		Application.targetFrameRate = 60;
		
		instance = this;

		Physics2D.gravity = Vector2.zero;
	}

	public void SetLayer(int layerID) {
		if (scenes[sceneID].controller != null) {
			scenes[sceneID].controller.GetComponent<Destruction2DController>().SetLayerType(layerID);
		}
	}

	public void OpenInfo(int id) {
		if (id > 0 && id < scenes.Length) {
			scenes[sceneID].information.SetActive(true);
		}
	}

	public void DisableInfo() {
		for(int i = 1; i < scenes.Length; i++) {
			scenes[i].information.SetActive(false);
		}
	}

	public void ResetCurrentScene() {
		GameObject g = Instantiate(scenes[sceneID].prefab,scenes[sceneID].center.position, Quaternion.Euler(0, 0, 0));
		g.transform.parent = scenes[sceneID].center.parent;

		Destroy(scenes[sceneID].center.gameObject);

		scenes[sceneID].center = g.transform;

		foreach (Transform child in scenes[sceneID].center.transform) {
			if (child.gameObject.name == "Pivot") {
				scenes[sceneID].tracked = child;
			}
			if (child.gameObject.name == "Controller") {
				scenes[sceneID].controller = child.gameObject;
			}
		}

		ResetScene(sceneID);
	}
	
	public void ResetScene(int id) {
		sceneID = id;

		for(int i = 1; i < scenes.Length; i++) {
			if (scenes[i].controller != null) {
				scenes[i].controller.SetActive(false);
			}
			scenes[i].UI.SetActive(false);
		}

		if (id > 0 && id < scenes.Length) {
			scenes[sceneID].controller.SetActive(true);
			scenes[sceneID].UI.SetActive(true);
		}

		if (id == 0) {
			UI.SetActive(true);
		}  else {
			UI.SetActive(false);
		}
	}

	public void SetScene(int id) {
		if (id == 0) {
			if (sceneID > 0) {
				ResetCurrentScene();
			}
		}
		sceneID = id;

		for(int i = 1; i < scenes.Length; i++) {
			if (scenes[i].controller != null) {
				scenes[i].controller.SetActive(false);
			}
			scenes[i].UI.SetActive(false);
			scenes[i].information.SetActive(false);
		}

		if (id > 0 && id < scenes.Length) {
			if (scenes[sceneID].controller != null) {
				scenes[sceneID].controller.SetActive(true);
			}
			scenes[sceneID].UI.SetActive(true);
			scenes[sceneID].information.SetActive(true);
		}

		if (id == 0) {
			UI.SetActive(true);
		}  else {
			UI.SetActive(false);
		}
	}

	void Update () {
		DemoScene scene = scenes[sceneID];

		if (scene == null) {
			return;
		}

		Vector2D newPosition = new Vector2D(scene.center.position);

		float rot = (float)Vector2D.Atan2(new Vector2D(scene.tracked.position), new Vector2D(scene.center.position));
		newPosition.Push(rot, scene.distance);

		Vector3 pos = Camera.main.transform.position;
		pos.x = pos.x * 0.95f + newPosition.ToVector2().x * 0.05f;
		pos.y = pos.y * 0.95f + newPosition.ToVector2().y * 0.05f;
		pos.z = -10;

		Camera.main.orthographicSize = Camera.main.orthographicSize * 0.95f + scene.cameraSize * 0.05f;

		Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Euler(0, 0, (rot - Mathf.PI / 2) * Mathf.Rad2Deg), 0.05f);

		Camera.main.transform.position = pos;
		Color color;

		for(int i = 1; i < scenes.Length; i++) {
			if (scenes[i] == scene) {
				 scenes[i].backgroundAlpha = scenes[i].backgroundAlpha * 0.95f + 0.05f;
			} else {
				scenes[i].backgroundAlpha = scenes[i].backgroundAlpha * 0.95f;
				color = scenes[i].background.color;
				color.a = scenes[i].backgroundAlpha;
				scenes[i].background.color = color;
			}
		
		}

		color = scene.background.color;
		color.a = scene.backgroundAlpha;
		scene.background.color = color;

		color = scenes[0].background.color;
		color.a = 1;
		scenes[0].background.color = color;
	}
}

[System.Serializable]
public class DemoScene {
	public Transform center;
	public Transform tracked;

	public float cameraSize;
	public float distance;
	public GameObject controller;
	public GameObject UI;
	public GameObject prefab;
	public GameObject information;
	public SpriteRenderer background;
	public float backgroundAlpha = 1f;
} 