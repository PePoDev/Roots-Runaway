using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction2DBuffer : MonoBehaviour {
	static private List<Destruction2DBuffer> bufferList = new List<Destruction2DBuffer>();
	
	public Camera renderCamera;
	public Destruction2D destructible = null;

	TimerHelper timer = null;
	public float idleTime = 1f;

	public SpriteBuffer spriteBufferObject = null;
	public MeshBuffer meshBufferObject = null;
	public SpriteShapeBuffer spriteShapeBufferObject = null;
	public TilemapBuffer tilemapBufferObject = null;

	public static int count = 0;

	static public List<Destruction2DBuffer> GetList() {
		return(new List<Destruction2DBuffer>(bufferList));
	}

	void OnDestroy() {
		bufferList.Remove (this);
	}

	void Awake() {
		count += 1;

		bufferList.Add (this);

		timer = TimerHelper.Create();
	}

	void Update() {
		if (destructible == null && timer.Get() > idleTime) {
			Destroy(gameObject);
		}
	}

	public void Init(Camera camera) {
		renderCamera = camera;

		spriteBufferObject = new SpriteBuffer();
		spriteBufferObject.renderCamera = camera;

		meshBufferObject = new MeshBuffer();
		meshBufferObject.renderCamera = camera;

		spriteShapeBufferObject = new SpriteShapeBuffer();
		spriteShapeBufferObject.renderCamera = camera;

		tilemapBufferObject = new TilemapBuffer();
		tilemapBufferObject.renderCamera = camera;
	}

	public void SetDestructible(Destruction2D d) {
		destructible = d;
		spriteBufferObject.destructible = destructible;
		meshBufferObject.destructible = destructible;
		spriteShapeBufferObject.destructible = destructible;
		tilemapBufferObject.destructible = destructible;
	}

	public void Set(Destruction2D destructibleObject) {	
		SetDestructible(destructibleObject);

		//Sprite Shape
		renderCamera.enabled = true;
		renderCamera.targetTexture = destructible.renderTexture;
		renderCamera.orthographicSize = (float)destructible.originalSprite.texture.height / (destructible.originalSprite.pixelsPerUnit * 2) * destructible.transform.localScale.x;
	}

	public void OnRenderObject() {
		if (Camera.current != renderCamera) {
			return;
		}

		Destruction2DManager.DestructionDebug.bufferUpdates ++;

		Destruction2D destruction;

		switch(destructible.textureType) {
			case Destruction2D.TextureType.Sprite:
				spriteBufferObject.OnRenderObject();

				break;

			case Destruction2D.TextureType.Mesh:
				meshBufferObject.OnRenderObject();

				break;

			case Destruction2D.TextureType.SpriteShape:
				spriteShapeBufferObject.OnRenderObject();

				Destroy(destructible);
				Destroy(gameObject);

				destruction = destructible.gameObject.AddComponent<Destruction2D>();
				destruction.split = destructible.split;
				
				break;

			case Destruction2D.TextureType.Tilemap:
				tilemapBufferObject.OnRenderObject();

				Destroy(destructible);
				Destroy(gameObject);

				destruction = destructible.gameObject.AddComponent<Destruction2D>();
				destruction.split = destructible.split;
				
				break;
		}

		renderCamera.enabled = false;

		if (destructible.textureType != Destruction2D.TextureType.SpriteShape && destructible.textureType != Destruction2D.TextureType.Tilemap) {
			destructible = null;
			timer.Reset();

			renderCamera.targetTexture = null;
			renderCamera.orthographicSize = 1f; 
		}
	}
}

//if (destructible.customPivot != null) {
//	polygon = polygon.ToOffset(new Vector2D(destructible.customPivot.ToVector2() * 12));
//}
				
//Max2D.DrawImage(transform, DestructionController.instance.creater, new Vector2D(transform.InverseTransformPoint(dEvent.collider.transform.position)), new Vector2D(1.5f, 1.5f), 20);



//Debug.Log(t.GetMillisecs());
//List<Polygon2D> polygons = Polygon2DList.CreateFromGameObject(destructible.gameObject);
//Polygon2D polygon = polygons[0];

/* 

	Sprite sprite = destructible.originalSprite;
	Rect bounds = polygon.GetBounds();

	Vector2 c = bounds.center;
	bounds.x += sprite.bounds.extents.x; // 600px
	bounds.y += sprite.bounds.extents.y; // 600px

	bounds.x *= sprite.pixelsPerUnit;
	bounds.y *= sprite.pixelsPerUnit;

	bounds.width *= sprite.pixelsPerUnit;
	bounds.height *= sprite.pixelsPerUnit;

	float cx = (destructible.spriteRenderer.sprite.pivot.x - 300f) / 600;
	float cy = (destructible.spriteRenderer.sprite.pivot.y - 300f) / 600;
	Vector2 d = new Vector2(cx, cy);
	float sx = (bounds.width / destructible.spriteRenderer.sprite.texture.width);
	float sy = (bounds.height / destructible.spriteRenderer.sprite.texture.height);

	c.x = c.x / sx;
	c.y = c.y / sy;
	
	//- c / 4
//	destructible.outputSprite = Sprite.Create(destructible.outputTexture, new Rect(bounds.x, bounds.y, bounds.width, bounds.height), new Vector2(0.5f, 0.5f) , 100.0f, 2, SpriteMeshType.FullRect);
	destructible.outputSprite = Sprite.Create(destructible.outputTexture, new Rect(0.0f, 0.0f, destructible.renderTexture.width, destructible.renderTexture.height), new Vector2(0.5f, 0.5f), 100.0f, 2, SpriteMeshType.FullRect);

	destructible.spriteRenderer.sprite = destructible.outputSprite;
	
	destructible.outputMaterial.mainTexture = destructible.outputTexture;

	/*
	if (destructible.customPivot != null) {
		Polygon2D polygon = null;
		if (destructible.customPolygon != null) {
			polygon = destructible.customPolygon;
			
			destructible.customPolygon = null;
		} else {
			polygon = Polygon2D.CreateFromCollider(destructible.gameObject)[0];
		}

		//if (destructible.customOffsetSet != null) {
		//	polygon = polygon.ToOffset(new Vector2D(-destructible.customOffsetSet.ToVector2() * 0.5f));
		//}
			
		Rect bounds = polygon.GetBounds();
		Sprite sprite = destructible.originalSprite;

		bounds.x += sprite.bounds.extents.x; // 600px
		bounds.y += sprite.bounds.extents.y; // 600px

		bounds.x *= sprite.pixelsPerUnit;
		bounds.y *= sprite.pixelsPerUnit;

		bounds.width *= sprite.pixelsPerUnit;
		bounds.height *= sprite.pixelsPerUnit;

		Debug.Log(destructible.name + " bounds " + bounds);

		destructible.outputSprite = Sprite.Create(destructible.outputTexture, new Rect(bounds.x, bounds.y, bounds.width, bounds.height), new Vector2(0.5f, 0.5f), 100.0f, 2, SpriteMeshType.FullRect);

		//if (destructible.customOffset != null) {
		//	Debug.Log("do offset");
		//	destructible.transform.Translate(destructible.customOffset.ToVector2() * 2);
		//	destructible.customOffsetSet = destructible.customOffset;
			
		//	destructible.customOffset = null;
		//}
	} else {
		}
*/