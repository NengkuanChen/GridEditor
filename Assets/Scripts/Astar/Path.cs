using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path {

	public string pathName;
	public int nodeInspected;

	GridScript gridScipt;
	public List<Step> path = new List<Step>();

	public float score;
	public int steps;

	public Path(string name, GridScript gridScipt){
		this.gridScipt = gridScipt;
		pathName = name;
	}

	public Step Get(int index){
		return path[index];
	}
	
	public virtual void Insert(int index, GameObject go){
		float stepCost = gridScipt.GetMovementCost(go);
		score += stepCost;
		
		path.Insert(index, new Step(go, stepCost));
		
		steps++;
	}

	public virtual void Insert(int index, GameObject go, Vector3 gridPos){
		float stepCost = gridScipt.GetMovementCost(go);
		score += stepCost;
		
		path.Insert(index, new Step(go, stepCost, gridPos));

		steps++;
	}
}
