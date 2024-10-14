using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
//new data structure that takes a material and a cost
public struct MapElement
{
	public Material material;
	public float cost;
}

// This struct stores the full data of an editable map
[System.Serializable]
public struct EditableMap
{
	public int sizeX; //number of columns in the map
	public int sizeY; //number of rows in the map
	
	private int[,] grids; //the map
	// For example:
	// grid[1, 1] = 0 means
	// the 0th element in the MapElement (below)
	// is placed on (1, 1) on the map
	
	public List<MapElement> elements;
}




public class EditableGridScript : GridScript
{
	protected override Material GetMaterial(int x, int y)
	{
		return base.GetMaterial(x, y);
	}

	public override float GetMovementCost(GameObject go)
	{
		return base.GetMovementCost(go);
	}
}
