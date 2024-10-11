using UnityEngine;
using System.Collections;

public class Step {

	public GameObject gameObject;
	public float moveCost;
	public Vector3 gridPos;

	
	public Step(GameObject gameObject, float cost){
		this.gameObject = gameObject;
		moveCost = cost;
	}

	public Step(GameObject gameObject, float cost, Vector3 gridPos){
		this.gameObject = gameObject;
		moveCost = cost;
		this.gridPos = gridPos;
	}
}
