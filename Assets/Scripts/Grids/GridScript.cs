using UnityEngine;
using System.Collections;

public class GridScript : MonoBehaviour {

	public int gridWidth;
	public int gridHeight;
	public float spacing;
	
	public Material[] mats;
	public float[]   costs;

	public Vector3 start = new Vector3(0,0);
	public Vector3 goal = new Vector3(0,0);
	
	GameObject[,] gridArray;
	
	public GameObject startSprite;
	public GameObject goalSprite;
	
	public virtual GameObject[,] GetGrid(){

		if(gridArray == null){
			
			Debug.Log(start);
			Debug.Log(goal);

			gridArray = new GameObject[gridWidth, gridHeight];
			
			float offsetX = (gridWidth  * -spacing)/2f;
			float offsetY = (gridHeight * spacing)/2f;

			for(int x = 0; x < gridWidth; x++){
				for(int y = 0; y < gridHeight; y++){
					GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
					quad.transform.localScale = new Vector3(spacing, spacing, spacing);
					quad.transform.position = new Vector3(offsetX + x * spacing, 
					                                      offsetY - y * spacing, 0);

					quad.transform.parent = transform;

					gridArray[x, y] = quad;
					
					quad.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial(x, y);

					if(Mathf.Approximately(goal.x, x) && Mathf.Approximately(goal.y, y)){
						goalSprite.transform.position = quad.transform.position;
					}
					if(Mathf.Approximately(start.x, x) && Mathf.Approximately(start.y, y)){
						startSprite.transform.position = quad.transform.position;
					}
				}
			}
		}

		return gridArray;
	}

	public virtual float GetMovementCost(GameObject go){
		Material mat = go.GetComponent<MeshRenderer>().sharedMaterial;
		int i;

		for(i = 0; i < mats.Length; i++){
			if(mat.name.StartsWith(mats[i].name)){
				break;
			}
		}

		return costs[i];
	}

	protected virtual Material GetMaterial(int x, int y){
		return mats[Random.Range(0,mats.Length)];
	}
}
