using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoClouds : MonoBehaviour {
	public int cloudsCount;

	public DemoRange cloudRange;
	public DemoRange speedRange;

	public int cloudDistance;
	public float scale = 1;
	public float zOffset = 1;
	
	public Sprite[] sprites;
	private GameObject cloudsParent;
	private List<GameObject> cloudsList = new List<GameObject>();

	void Start () {
		cloudsParent = new GameObject();
		cloudsParent.name = "Clouds";
		cloudsParent.transform.parent = transform;
		cloudsParent.transform.position = transform.position;

		for(int i = 0; i < cloudsCount; i++) {
			GameObject cloud = new GameObject();
			cloud.name = "Cloud " + i;
			cloud.transform.position = cloudsParent.transform.position;
			cloud.transform.parent = cloudsParent.transform;

			SpriteRenderer spriteRenderer = cloud.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

			Cloud cloudScript = cloud.AddComponent<Cloud>();
			cloudScript.distance = cloudDistance + Random.Range(cloudRange.min, cloudRange.max);
			cloudScript.rotation = ((float)i / cloudsCount) * 360f;
			cloudScript.parent = cloudsParent.transform;
			cloudScript.zOffset = zOffset;
			cloudScript.speed = Random.Range(speedRange.min, speedRange.max);

			cloudScript.transform.localScale = new Vector3(scale, scale, 1);

			cloudsList.Add(cloud);
		}
	}
}
[System.Serializable]
public class DemoRange {
	public float min = 1;
	public float max = 2;
}
