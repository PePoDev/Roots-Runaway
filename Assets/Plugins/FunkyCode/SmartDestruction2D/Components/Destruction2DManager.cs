using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction2DManager : MonoBehaviour {
	public float bufferMaxIdle = 1;

	public bool debug = false;

	public static Destruction2DManager instance = null;

	public static void Initialize() {
		if (instance != null) {
			return;
		}

		GameObject manager = new GameObject();
		manager.name = "Destruction 2D Manager";

		instance = manager.AddComponent<Destruction2DManager>();
	}
	
	void Start () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Debug.LogError("Destruction2D: Multiple Managers Detected!");
			Destroy(this);
		}
	}
	
	static public void RequestBufferEvent(Destruction2D destructible) {
		Destruction2DBuffer buffer = GetBuffer(destructible);

		buffer.Set(destructible);
	}

	static public Destruction2DBuffer GetBuffer(Destruction2D destructible) {
		foreach(Destruction2DBuffer buffer in Destruction2DBuffer.GetList()) {
			if (buffer.destructible == destructible) {
				return(buffer);
			}
		}

		foreach(Destruction2DBuffer buffer in Destruction2DBuffer.GetList()) {
			if (buffer.destructible == null) {
				return(buffer);
			}
		}
		return(CreateBuffer());
	}

	public static Destruction2DBuffer CreateCustomBuffer (GameObject gameObject) {
		gameObject.transform.parent = instance.transform;
		gameObject.transform.position = new Vector3(Destruction2DBuffer.count * 200, 0, -1000);

		GameObject cameraObject = new GameObject();
		cameraObject.name = "Buffer Camera";
		cameraObject.transform.parent = gameObject.transform;
		cameraObject.transform.position = gameObject.transform.position;

		Destruction2DBuffer buffer = SetupBuffer(gameObject);
		buffer.Init(SetupCamera(cameraObject));

		return(buffer);
	}

	public static Destruction2DBuffer CreateBuffer () {
		GameObject gameObject = new GameObject();
		gameObject.name = "Buffer (" + (Destruction2DBuffer.count + 1) + ")";
		gameObject.transform.parent = instance.transform;
		gameObject.transform.position = new Vector3(Destruction2DBuffer.count * 200, 0, -1000);

		Destruction2DBuffer buffer = SetupBuffer(gameObject);
		buffer.Init(SetupCamera(gameObject));

		return(buffer);
	}

	public static Camera SetupCamera(GameObject gameObject) {
		Camera renderCamera = gameObject.AddComponent<Camera>();
		renderCamera.orthographic = true;
		renderCamera.orthographicSize = 1f; 
		renderCamera.allowHDR = false;
		renderCamera.allowMSAA = false;
		renderCamera.clearFlags = CameraClearFlags.SolidColor;
		renderCamera.nearClipPlane = 0;
		renderCamera.farClipPlane = 0.5f;
		renderCamera.enabled = false;
		
		return(renderCamera);
	}

	public static Destruction2DBuffer SetupBuffer (GameObject gameObject) {
		Destruction2DBuffer buffer = gameObject.AddComponent<Destruction2DBuffer>();

		buffer.idleTime = instance.bufferMaxIdle;

		return(buffer);
	}

	void OnGUI() {
		if (debug) {
			DestructionDebug.OnGUI();
		}
	}

	public class DestructionDebug {
		static public int bufferUpdates = 0;

		static public TimerHelper timer;

		static public void OnGUI() {
			if (timer == null) {
				timer = TimerHelper.Create();
			}
			if (timer.GetMillisecs() > 1000) {
			

				timer = TimerHelper.Create();				
			}


			
			GUI.Label(new Rect(10, 10, 500, 20), "Buffer Updates: " + bufferUpdates);
		
		}
	}

}