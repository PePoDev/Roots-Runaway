using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Destruction2DChunks  {
	public enum Offset {Transform, SpritePivot};
	public bool enabled = false;
	public int chunksRows = 2;
	public int chunksColumns = 2;
	public Offset chunkOffset = Offset.Transform;
	
}
