using UnityEngine;
using System.Collections;

public class HeuristicScript : MonoBehaviour {
		
	public virtual float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript){
		return Random.Range(0, 1000);
	}
}
