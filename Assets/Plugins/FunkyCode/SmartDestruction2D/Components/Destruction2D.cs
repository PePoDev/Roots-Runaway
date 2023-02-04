using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEngine.Tilemaps;

public class Destruction2D : MonoBehaviour {
	public enum TextureType {Sprite, Mesh, SpriteShape, Tilemap};
	public enum CenterOfMass {Default, RigidbodyOnly};
	public enum SpriteShapeBounds {Renderer, Collider};

	public TextureType textureType = TextureType.Sprite;
	public DestructionLayer destructionLayer = DestructionLayer.Layer1;

	public CenterOfMass centerOfMass = CenterOfMass.Default;

	public SpriteShapeBounds spriteShapeBounds = SpriteShapeBounds.Renderer;

	public Destruction2DSplit split = new Destruction2DSplit();

	public FilterMode filterMode = FilterMode.Point;

	public Vector2Int gridSize = new Vector2Int(1, 1);

	public Destruction2DChunks chunks = new Destruction2DChunks();

	public Destruction2DShape shape = new Destruction2DShape();

	public Destruction2DBuffer buffer;
	public RenderTexture renderTexture;

	public SpriteRenderer spriteRenderer;
	public Sprite outputSprite;
	public Material outputMaterial;
	public Texture2D outputTexture;

	public Vector2 originalSpriteOffSet = Vector2.zero;

	public Sprite originalSprite;
	public Material originalSpriteMaterial;

	public List<DestructionEvent> eraseEvents = new List<DestructionEvent>();
	public List<DestructionModifier> modifiers = new List<DestructionModifier>();

	public bool modifiersAdded = false;
	public bool replaceSprite = false;
	
	public bool initialized = false;
	
	public List<Polygon2D> erasePolygons = new List<Polygon2D>();
	public List<Mesh> eraseMeshes = new List<Mesh>();

	public Polygon2D customPolygon = null;

	public float pixelsPerUnit = 100f;
	public float scaledPixelsPerUnit = 100f;

	public bool init = false;

	static private List<Destruction2D> destructible2DList = new List<Destruction2D>();
	static private List<Destruction2D> destructible2DListLayer = new List<Destruction2D>();

	// Anchor Events
	public List<Polygon2D> anchorPolygons = new List<Polygon2D>();
	public List<Collider2D> anchorColliders = new List<Collider2D>();

	public delegate void Destruction2DEventFunction(Destruction2DEvent destruction);
	private event Destruction2DEventFunction destructionAnchorEvent;
	
	static public List<Destruction2D> GetList() {
		return(new List<Destruction2D>(destructible2DList));
	}

	void OnEnable() {
		destructible2DList.Add (this);
	}

	void OnDisable() {
		destructible2DList.Remove (this);
	}

	static public List<Destruction2D> GetListLayer(Destruction2DLayer layer) {
		destructible2DListLayer.Clear();

		foreach (Destruction2D id in  destructible2DList) {
			if (id.MatchLayers (layer)) {
				destructible2DListLayer.Add(id);
			}
		}
		
		return(destructible2DListLayer);
	}

	public void Start () {
		shape.SetOrigin(this);

		Destruction2DManager.Initialize();

		switch(textureType) {
			case TextureType.SpriteShape:
				StartSpriteShape();
				return;

			case TextureType.Tilemap:
				StartTilemap();
				return;

			default:
				StartSpriteRenderer();
				return;
		}
	}

	void StartSpriteRenderer() {
		spriteRenderer = GetComponent<SpriteRenderer>();

		if (spriteRenderer == null) {
			Debug.LogError("Destruction2D: Game object is missing SpriteRenderer Component");
		}

		if (chunks.enabled) {
			StartChunks();
		} else {
			Sprite sprite = spriteRenderer.sprite;
			pixelsPerUnit = sprite.pixelsPerUnit;

			if (originalSprite == null){
				originalSprite = sprite;
			}

			if (replaceSprite  == false) {
				originalSpriteMaterial = new Material (Shader.Find ("Sprites/Default"));
				originalSpriteMaterial.mainTexture = originalSprite.texture;
			}

			Texture2D texture = originalSprite.texture;

			renderTexture = new RenderTexture((int)sprite.rect.width, (int)sprite.rect.height, 32);
			outputTexture =  new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

			switch(textureType) {
				case TextureType.Mesh:
					outputMaterial = new Material (Shader.Find ("Sprites/Default"));
					outputMaterial.mainTexture = texture;

					Destroy(spriteRenderer);

					break;

				case TextureType.Sprite:
					break;
			}

			Destruction2DManager.RequestBufferEvent(this);
		}
	}

	void StartTilemap() {
		if (init == true) {
			return;
		}

		Tilemap tilemap = GetComponent<Tilemap>();
		TilemapRenderer renderer = GetComponent<TilemapRenderer>();

		BoundsInt bounds = tilemap.cellBounds;

		Rect rect = new Rect();

		rect.x = bounds.x * gridSize.x;
		rect.y = bounds.y * gridSize.y;

		rect.width = bounds.size.x * gridSize.x;
		rect.height = bounds.size.y * gridSize.y;

		GameObject newGamObject =  new GameObject();
		newGamObject.name = gameObject.name;
		newGamObject.transform.position = transform.position;
		newGamObject.transform.rotation = transform.rotation;
		newGamObject.transform.localScale = transform.localScale;
		newGamObject.transform.parent = transform.parent;

		foreach(Component c in gameObject.GetComponents<Component>()) {
			if (c.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
				continue;
			}
			if (c.GetType().ToString() == "UnityEngine.EdgeCollider2D") {
				continue;
			}
			if (c.GetType().ToString() != "UnityEngine.Tilemaps.Tilemap" && c.GetType().ToString() != "UnityEngine.Tilemaps.TilemapRenderer") {
				if (CopyComponent(c, newGamObject, gameObject) == true) {
					Destroy(c);
				}
			}
		}

		Destruction2D destructible = newGamObject.GetComponent<Destruction2D>();
		destructible.init = true;

		newGamObject.AddComponent<PolygonCollider2D>();

		//PolygonCollider2D collider = gameObject.GetComponent<PolygonCollider2D>();
		//if (collider != null) {
		//	newGamObject.AddComponent<PolygonCollider2D>();
		//	Destroy(collider);
		//}

		GameObject newGridObject = new GameObject();
		newGridObject.name = "Buffer: " + gameObject.name;

		Grid grid = newGridObject.AddComponent<Grid>();
		grid.cellSize = new Vector3(gridSize.x, gridSize.y, 0);

		

		newGridObject.transform.parent = Destruction2DManager.instance.transform;

		gameObject.transform.parent = newGridObject.transform;
		gameObject.name = "Tilemap";
		gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		gameObject.transform.localScale = new Vector3(1, 1, 1);
		gameObject.transform.position = new Vector3(0, 0, 0);

		destructible.buffer = Destruction2DManager.CreateCustomBuffer(newGridObject);

		destructible.pixelsPerUnit = scaledPixelsPerUnit;

		int textureWidth = (int)(rect.width * destructible.pixelsPerUnit);
		int textureHeight = (int)(rect.height * destructible.pixelsPerUnit);

		destructible.renderTexture = new RenderTexture(textureWidth, textureHeight, 32);
		destructible.outputTexture =  new Texture2D(textureWidth, textureHeight);
	
		// Filter Mode???
		// destructible.outputTexture.filterMode = filterMode;
		
		destructible.buffer.renderCamera.orthographicSize = (rect.height) / 2;
		destructible.buffer.renderCamera.transform.Translate(rect.center.x, rect.center.y, -1f);
		destructible.buffer.renderCamera.farClipPlane = 5;
		destructible.buffer.renderCamera.targetTexture = destructible.renderTexture;

		newGamObject.transform.Translate(rect.center.x, rect.center.y, -1f);

		destructible.spriteRenderer = newGamObject.AddComponent<SpriteRenderer>();

		// Set Up Destructible
		destructible.buffer.SetDestructible(destructible);

		destructible.buffer.renderCamera.enabled = true;

		destructible.originalSprite = Sprite.Create(destructible.outputTexture, new Rect(0.0f, 0.0f, destructible.renderTexture.width, destructible.renderTexture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit, 2, SpriteMeshType.FullRect);

		//Destroy(gameObject.GetComponent<EdgeCollider2D>());
	}

	void StartSpriteShape() {
		if (init == true) {
			return;
		}
		
		Rect rect = new Rect();
		switch (spriteShapeBounds) {
			case SpriteShapeBounds.Renderer:
				Bounds bounds = new Bounds();
				bool found = false;
				foreach(Component c in gameObject.GetComponents<Component>()) {
					if (c.GetType().ToString() == "UnityEngine.Experimental.U2D.SpriteShapeRenderer") {
						System.Reflection.PropertyInfo[] fields = c.GetType().GetProperties();
						foreach (System.Reflection.PropertyInfo field in fields) {
							if (field.ToString() == "UnityEngine.Bounds bounds") {
								bounds = (Bounds)field.GetValue(c, null);
								found = true;
								break;
							}
						}
						break;
					}
				}
				if (found) {
					rect = new Rect(bounds.center.x - bounds.extents.x - transform.position.x, bounds.center.y - bounds.extents.y - transform.position.y, bounds.extents.x * 2, bounds.extents.y * 2);
				} else {
					Debug.LogWarning("Destruction2D: SpriteShape game object is missing SpriteShapeRenderer");
				}
				break;
			case SpriteShapeBounds.Collider:
				List<Polygon2D> polygons = Polygon2DList.CreateFromGameObject(gameObject);
				if (polygons.Count < 1) {
					Debug.LogError("Destruction2D: SpriteShape game object is missing collider");
					return;
				}

				rect = polygons[0].GetBounds();
				break;
		}

		GameObject newGamObject =  new GameObject();
		newGamObject.name = gameObject.name;
		newGamObject.transform.position = transform.position;
		newGamObject.transform.rotation = transform.rotation;
		newGamObject.transform.localScale = transform.localScale;
		newGamObject.transform.parent = transform.parent;

		foreach(Component c in gameObject.GetComponents<Component>()) {
			if (c.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
				continue;
			}
			if (c.GetType().ToString() == "UnityEngine.EdgeCollider2D") {
				continue;
			}
			if (c.GetType().ToString() != "UnityEngine.U2D.SpriteShapeController" && c.GetType().ToString() != "UnityEngine.Experimental.U2D.SpriteShapeRenderer") {
				if (CopyComponent(c, newGamObject, gameObject) == true) {
					Destroy(c);
				}
			}
		}

		Destruction2D destructible = newGamObject.GetComponent<Destruction2D>();
		destructible.init = true;

		Destroy(gameObject.GetComponent<EdgeCollider2D>());

		PolygonCollider2D collider = gameObject.GetComponent<PolygonCollider2D>();
		if (collider != null) {
			newGamObject.AddComponent<PolygonCollider2D>();
			Destroy(collider);
		}

		gameObject.transform.parent = Destruction2DManager.instance.transform;
		gameObject.name = "Buffer: " + gameObject.name;
		gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		gameObject.transform.localScale = new Vector3(1, 1, 1);

		destructible.buffer = Destruction2DManager.CreateCustomBuffer(gameObject);

		destructible.pixelsPerUnit = scaledPixelsPerUnit;

		int textureWidth = (int)(rect.width * destructible.pixelsPerUnit);
		int textureHeight = (int)(rect.height * destructible.pixelsPerUnit);

		destructible.renderTexture = new RenderTexture(textureWidth, textureHeight, 32);
		destructible.outputTexture =  new Texture2D(textureWidth, textureHeight);
		
		destructible.buffer.renderCamera.orthographicSize = (rect.height) / 2;
		destructible.buffer.renderCamera.transform.Translate(rect.center.x, rect.center.y, -1f);
		destructible.buffer.renderCamera.farClipPlane = 5;
		destructible.buffer.renderCamera.targetTexture = destructible.renderTexture;

		newGamObject.transform.Translate(rect.center.x, rect.center.y, -1f);

		destructible.spriteRenderer = newGamObject.AddComponent<SpriteRenderer>();

		// Set Up Destructible
		destructible.buffer.SetDestructible(destructible);

		destructible.buffer.renderCamera.enabled = true;

		destructible.originalSprite = Sprite.Create(destructible.outputTexture, new Rect(0.0f, 0.0f, destructible.renderTexture.width, destructible.renderTexture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit, 2, SpriteMeshType.FullRect);
	}

	void StartChunks() {
		Sprite sprite = spriteRenderer.sprite;
		pixelsPerUnit = sprite.pixelsPerUnit;

		GameObject parentGameObject = new GameObject();
		int rowSize = sprite.texture.height / chunks.chunksRows;
		int columnSize = sprite.texture.width / chunks.chunksColumns;

		for(int column = 0; column < chunks.chunksColumns; column++){
			for(int row = 0;row < chunks.chunksRows; row++){
				GameObject newGameObject = new GameObject();
				newGameObject.transform.parent = parentGameObject.transform;
				newGameObject.name = "Chunk (" + column + ", " + row + ")";

				// Pivot or Transform offset
				float positionX;
				float positionY;

				Vector2 pivot = new Vector2(0.5f, 0.5f);

				switch(chunks.chunkOffset) {
					case Destruction2DChunks.Offset.Transform:
						positionX = (((float)column - ((float)chunks.chunksColumns - 1) / 2) * (float)sprite.texture.width / chunks.chunksColumns) / pixelsPerUnit;
						positionY = (((float)row - ((float)chunks.chunksRows - 1) / 2) * (float)sprite.texture.height / chunks.chunksRows) / pixelsPerUnit;

						newGameObject.transform.position = new Vector3(positionX, positionY, 0);

						break;

					case Destruction2DChunks.Offset.SpritePivot:
						positionX = (((float)column - ((float)chunks.chunksColumns - 1) / 2) * (float)sprite.texture.width) / pixelsPerUnit;
						positionY = (((float)row - ((float)chunks.chunksRows - 1) / 2) * (float)sprite.texture.height ) / pixelsPerUnit;

						float pivotX = -positionX / 6;
						float pivotY = -positionY / 6;
						
						pivot = new Vector2(0.5f, 0.5f) + new Vector2(pivotX, pivotY);

						break;
				}
				

				SpriteRenderer srenderer = newGameObject.AddComponent<SpriteRenderer>();

				Sprite output = Sprite.Create(GetComponent<SpriteRenderer>().sprite.texture, new Rect(column * columnSize, row * rowSize, columnSize, rowSize), pivot, pixelsPerUnit, 2, SpriteMeshType.FullRect);
				srenderer.sprite = output;
		
				if (gameObject.GetComponent<PolygonCollider2D>()) {
					PolygonCollider2D collider = newGameObject.AddComponent<PolygonCollider2D>();

					if (collider.pathCount == 1) {

						Vector2[] standardPentagon = collider.GetPath(0);
						Vector2 v0 = standardPentagon[0] / (3f / chunks.chunksColumns);  // + Rows (Y)
						Vector2 v1 = new Vector2(0, 1);
						Vector2 v2 = standardPentagon[1] / (3f / chunks.chunksColumns);  // + Rows (Y)
						Vector2 v3 = new Vector2(-0.9510565f, 0.309017f);

						switch(chunks.chunkOffset) {
							case Destruction2DChunks.Offset.SpritePivot:
								v0.x -= (1-chunks.chunksColumns + column * 2);
								v0.y -= (1-chunks.chunksRows + row * 2);
								v2.x -= (1-chunks.chunksColumns + column * 2);
								v2.y -= (1-chunks.chunksRows + row * 2);
							break;
						}

						if (v0 == v1 && v2 == v3) {
							Destroy(newGameObject);
							newGameObject = null;
						}

						//Debug.Log(newGameObject.name + " " + ((1-chunksColumns + column * 2)) + " " +((1-chunksRows + row * 2)) + " (" + v0.x + ", " + v0.y + ") - (" + v1.x + ", " + v1.y + ")");
					}
				}

				if (newGameObject != null) {
					Destruction2D destructible = newGameObject.AddComponent<Destruction2D>();
					destructible.originalSprite = sprite;
					//destructible.replaceSprite = true;
				}
			
			
			}
		}

		parentGameObject.name = gameObject.name;
		parentGameObject.transform.parent = gameObject.transform.parent;
		parentGameObject.transform.localScale = gameObject.transform.localScale;
		parentGameObject.transform.rotation = gameObject.transform.rotation;
		parentGameObject.transform.position = gameObject.transform.position;


		Destroy(gameObject);
	}

	public void UpdateCollider() {
		PolygonCollider2D polygonCollider2D = GetComponent<PolygonCollider2D>();
		if (polygonCollider2D != null) {
			Destroy(polygonCollider2D);
			PolygonCollider2D collider = gameObject.AddComponent<PolygonCollider2D>();
			float scale = (float)originalSprite.texture.width / (originalSprite.pixelsPerUnit * 2);   // Y?
			
			// Check If There Is No Proper Collide Generated ()
			if (collider.pathCount == 1) {
				Vector2[] standardPentagon = collider.GetPath(0);

				Vector2 v0 = standardPentagon[0] / scale;
				Vector2 v1 = new Vector2(0, 1);
				Vector2 v2 = standardPentagon[1] / scale;
				Vector2 v3 = new Vector2(-0.9510565f, 0.309017f);

				if (v0 == v1 && v2 == v3) {
					Destroy(gameObject);
				}
			}

			if (textureType == TextureType.Sprite) {
		
				if (split.enabled == true && (split.splitLimit == false || split.splitCount <= split.maxSplits)) {
					List<Polygon2D> polys = Polygon2DList.CreateFromPolygonColliderToLocalSpace(collider);

					List<GameObject> disattachedObjects = new List<GameObject>();

					if (polys.Count > 1) {
						int id = 1;

						Rigidbody2D originalRigidBody = GetComponent<Rigidbody2D>();

						foreach(Polygon2D poly in polys) {
							GameObject gObject = new GameObject();
							gObject.name = gameObject.name + " (" + id + ")";
							gObject.transform.parent = transform.parent;

							Polygon2D polygon = poly;
							polygon.CreatePolygonCollider(gObject);

							gObject.transform.position = transform.position;
							gObject.transform.rotation = transform.rotation;
							gObject.transform.localScale = transform.localScale;

							Destruction2D.CopyComponents(this, gObject);
							
							gObject.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
							
							Destruction2D destructible = gObject.AddComponent<Destruction2D>();
							destructible.replaceSprite = true;
							destructible.originalSprite = originalSprite;
							destructible.originalSpriteMaterial = originalSpriteMaterial;

							destructible.customPolygon = poly;

							destructible.modifiers = new List<DestructionModifier>(modifiers);
							destructible.eraseMeshes = new List<Mesh>(eraseMeshes);
							
							destructible.split.enabled = true;
							destructible.split.splitLimit = split.splitLimit;
							destructible.split.maxSplits = split.maxSplits;
							destructible.split.splitCount = split.splitCount + 1;

							destructible.centerOfMass = centerOfMass;
							
							if (anchorColliders.Count > 0) {
								if (destructible.Detach(collider) == true) {
									disattachedObjects.Add(gameObject);
								}
							}
							
							foreach(Polygon2D p in polys) {
								if (p != poly) {
									destructible.erasePolygons.Add(p);
								}
							}

							// CopyRigidbody
							if (originalRigidBody) {
								Rigidbody2D newRigidBody = gObject.GetComponent<Rigidbody2D> ();

								newRigidBody.isKinematic = originalRigidBody.isKinematic;
								newRigidBody.velocity = originalRigidBody.velocity;
								newRigidBody.angularVelocity = originalRigidBody.angularVelocity;
								newRigidBody.angularDrag = originalRigidBody.angularDrag;
								newRigidBody.constraints = originalRigidBody.constraints;
								newRigidBody.gravityScale = originalRigidBody.gravityScale;
								newRigidBody.collisionDetectionMode = originalRigidBody.collisionDetectionMode;
								//newRigidBody.sleepMode = originalRigidBody.sleepMode;
								//newRigidBody.inertia = originalRigidBody.inertia;

								// Center of Mass : Auto / Center
								if (centerOfMass == CenterOfMass.RigidbodyOnly) {
									newRigidBody.centerOfMass = Vector2.zero;
								}
							}
							
							id++;
						}
						Destroy(gameObject);
				} else {
						if (anchorColliders.Count > 0) {
							if (Detach(collider) == true) {
								disattachedObjects.Add(gameObject);
							}
						}
					}

					if (disattachedObjects.Count > 0) {
						if ((destructionAnchorEvent!= null)) {
							Destruction2DEvent destruction = new Destruction2DEvent();
							destruction.gameObjects = disattachedObjects;

							destructionAnchorEvent (destruction);
						}
					}
				}
			}
		}
	}

	public bool MatchLayers(Destruction2DLayer sliceLayer) {
		return((sliceLayer == null || sliceLayer.GetLayerType() == Destruction2DLayerType.All) || sliceLayer.GetLayerState(GetLayerID ()));
	}

	public int GetLayerID() {
		return((int)destructionLayer);
	}

	public void AddAnchorEvent(Destruction2DEventFunction e) {
		destructionAnchorEvent += e;
	}

	public float GetOrtographicSize() {
		return((float)originalSprite.texture.height / (pixelsPerUnit * 2) * transform.localScale.x);
	}

	public Polygon2D GetBoundPolygon() {
		float sizeY = GetOrtographicSize() / transform.localScale.x;
		float sizeX = sizeY * ((float)outputTexture.width / (float)outputTexture.height);

		return(Polygon2D.CreateFromRect(new Vector2(sizeX, sizeY)));
	}
	
	static public void DestroyByComplexCutAll(ComplexCut complexCut, Destruction2DLayer layer = null) {
		if (layer == null) {
			layer = Destruction2DLayer.Create();
		}
		foreach(Destruction2D gObject in GetListLayer(layer)) {
			gObject.DestroyByComplexCut(complexCut);
		}
	}

	static public void DestroyByLinearCutAll(LinearCut linearCut, Destruction2DLayer layer = null) {
		if (layer == null) {
			layer = Destruction2DLayer.Create();
		}
		foreach(Destruction2D gObject in GetListLayer(layer)) {
			gObject.DestroyByLinearCut(linearCut);
		}
	}

	static public void DestroyByPolygonAll(Polygon2D polygon, Destruction2DLayer layer = null) {
		if (layer == null) {
			layer = Destruction2DLayer.Create();
		}
		foreach(Destruction2D gObject in GetListLayer(layer)) {
			gObject.DestroyByPolygon(polygon);
		}
	}

	public void DestroyByLinearCut(LinearCut linearCut) {
		DestroyByPolygon(new Polygon2D(linearCut.GetPointsList()));
	}

	public void DestroyByComplexCut(ComplexCut complexCut) {
		DestroyByPolygon(new Polygon2D(complexCut.GetPointsList()));
	}

	public void DestroyByCollider(Collider2D collider) {
		DestroyByPolygon(Polygon2DList.CreateFromGameObject(collider.gameObject)[0].ToWorldSpace(collider.transform));
	}
	
	public void DestroyByPolygon(Polygon2D polygon) {
		if (polygon.pointsList.Count < 3) {
			return;
		}

		List<Polygon2D> polys = shape.GetWorld();
		if (polys.Count > 0) {
			bool touch = false;
			bool outside = false;

			foreach(Polygon2D p in polys) {
				if (Math2D.PolyCollidePoly(polygon, p) == true) {
					touch = true;
				}

				if (polygon.PolyInPoly(p) == false) {
					outside = true;
				}
			}

			if (touch == false) {
				return;
			}

			if (outside == false) {
				Destroy(gameObject);
				return;
			}
		} else {
			Polygon2D bound = GetBoundPolygon(); 
			bool touch = false;

			bound.ToWorldSpaceItself(transform);

			if (Math2D.PolyCollidePoly(polygon, bound) == true) {
				touch = true;
			}

			if (touch == false) {
				return;
			}
		}

		eraseEvents.Add(new DestructionEvent(polygon));

		if (textureType == TextureType.SpriteShape) {
			buffer.renderCamera.enabled = true;
		} else {
			Destruction2DManager.RequestBufferEvent(this);
		}
	}

	public void AddModifier(Texture2D texture, Vector2 position, Vector2 size, float rotation) {
		Polygon2D poly = Polygon2D.CreateFromRect(size);
		poly = poly.ToRotation(rotation * Mathf.Deg2Rad);
		poly = poly.ToOffset(new Vector2D(position));

		List<Polygon2D> polys = Polygon2DList.CreateFromGameObject(gameObject); 
		if (polys.Count > 0) {
			bool touch = false;

			foreach(Polygon2D p in polys) {
				p.pointsList = p.ToWorldSpace(transform).pointsList;

				if (Math2D.PolyCollidePoly(poly, p) == true) {
					touch = true;
				}
			}

			if (touch == false) {
				return;
			}
		} else {
			Polygon2D p = GetBoundPolygon(); 
			bool touch = false;

			p = p.ToWorldSpace(transform);

			if (Math2D.PolyCollidePoly(p, poly) == true) {
				touch = true;
			}

			if (touch == false) {
				return;
			}
		}
		Vector2 pos = transform.InverseTransformPoint(position);

		float ratioX = 1;
		
		if (transform.localScale.x != transform.localScale.y) {
			ratioX = transform.localScale.x / transform.localScale.y;
			size.y = size.y * ratioX;
		}

		size.x /= transform.localScale.x;
		size.y /= transform.localScale.y;
		size.y /= ratioX;

		DestructionModifier modifier = new DestructionModifier(texture, pos, size, rotation);
	
		modifiers.Add(modifier);

		modifiersAdded = true; // add only if colides

		if (textureType == TextureType.SpriteShape) {
			buffer.renderCamera.enabled = true;
		} else {
			Destruction2DManager.RequestBufferEvent(this); // Check if it is already requested
		}
	}

	static public void AddModifierAll(Texture2D texture, Vector2 position, Vector2 size, float rotation, Destruction2DLayer layer = null) {
		if (layer == null) {
			layer = Destruction2DLayer.Create();
		}
		foreach(Destruction2D gObject in GetListLayer(layer)) {
			gObject.AddModifier(texture,position,size, rotation);
		}
	}

	public bool Detach (PolygonCollider2D collider) {
		Polygon2D polygon = Polygon2DList.CreateFromPolygonColliderToWorldSpace(collider)[0];

		bool attached = true;
		foreach(Collider2D c in anchorColliders) {
			Polygon2D p = anchorPolygons[anchorColliders.IndexOf(c)].ToWorldSpace(c.transform);

			// Fix for PolyCollidePoly!!!
			bool inHole = false;
			
			foreach(Polygon2D hole in polygon.holesList) {
				if (hole.PolyInPoly(p)) {
					inHole = true;
				}
			}

			if (inHole == false) {
				if (Math2D.PolyCollidePoly(p, polygon) == false) {
					attached = false;
				}
			} else{
				attached = false;
			}
		}
		if (attached == false) {
			return(true);
		}
		return(false);
	}

	static public void CopyComponents(Destruction2D slicer, GameObject gObject) {
		Component[] scriptList = slicer.gameObject.GetComponents<Component>();	
		foreach (Component script in scriptList) {
			if (script.GetType().ToString() == "UnityEngine.PolygonCollider2D" || script.GetType().ToString() == "UnityEngine.BoxCollider2D" || script.GetType().ToString() == "UnityEngine.CircleCollider2D" || script.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
				continue;
			}

			if (script.GetType().ToString() == "Destruction2D") {
				continue;
			}
			CopyComponent(script, gObject, slicer.gameObject);
		}
	}

	static public bool CopyComponent(Component script, GameObject gObject, GameObject copyObject) {
		if (script.GetType().ToString() != "UnityEngine.Transform") {
			gObject.AddComponent(script.GetType());
			System.Reflection.FieldInfo[] fields = script.GetType().GetFields();

			foreach (System.Reflection.FieldInfo field in fields) {
				field.SetValue(gObject.GetComponent(script.GetType()), field.GetValue(script));
			}
		} else {
			return(false);
		}

		foreach (Behaviour childCompnent in gObject.GetComponentsInChildren<Behaviour>()) {
			foreach (Behaviour child in copyObject.GetComponentsInChildren<Behaviour>()) {
				if (child.GetType() == childCompnent.GetType()) {
					childCompnent.enabled = child.enabled;
					break;
				}
			}
		}
		return(true);
	}
}

public class DestructionEvent {
	public Polygon2D polygon;

	public DestructionEvent(Polygon2D polygonVar) {
		polygon = polygonVar;
	}
}

public class DestructionModifier {
	public Vector2 position;
	public Material material;
	public Vector2 size;
	public float rotation;

	public DestructionModifier(Texture tex, Vector2 pos, Vector2 siz, float rot) {
		material = new Material (Shader.Find ("SmartDestruction2D/ModifierShader")); 
		material.mainTexture = tex;
		position = pos;
		size = siz;
		rotation = rot;
	}
}

	//public bool recalculateCenterPivot = false;
	//public bool createChunks = false;


	//public Vector2D customPivot = null;
	//public Vector2D customOffset = null;
	//public Vector2D customOffsetSet = null;

			//float sx = (originalSprite.texture.width / originalSprite.pixelsPerUnit) ;
			//float sy = (originalSprite.texture.height / originalSprite.pixelsPerUnit);
		
			//r.customPivot = new Vector2D(poly.GetBounds().center.x / sx, poly.GetBounds().center.y / sy);
			//r.customOffset  = new Vector2D(poly.GetBounds().center);

	//	if (customPivot != null) {
	//		//Debug.Log("Offset " + customPivot.ToVector2());
	//		pos.x += (float)customPivot.x * 5;
	//		pos.y += (float)customPivot.y * 5;
	//	}
	