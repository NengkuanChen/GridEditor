using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class FirstGridScript : GridScript {

	string[] gridString = new string[]{
		"ww--|-rw-|----|------------",
		"-ww-|-wr-rrrr-|---rrrr-----",
		"-ww-|----r----|------r--www",
		"-ww-|----r----|------r--ww-",
		"-ww-|----rrrrrrrrrrrr--dww-",
		"-wwwwwwww|----|-------dww--",
		"-wwwwwwwwww---|---c--d-----",
		"----|---www---|-----dd-----",
		"----|---www---|----dd------",
		"--ddd----w--www---dd-------",
		"--drd----wwww-w--dd--------",
		"--drd-c--|----wwdd---------",
		"--dddddd-|----|wdd----cc---",
		"----dddddd----|------------",
		"----|---dd----|------------",
	};

	//public Renderer _objectRenderer;

	// Use this for initialization
	void Start () {
		gridWidth = gridString[0].Length;
		gridHeight = gridString.Length;

		// _objectRenderer = GetComponent<Renderer>();
	}
	
	protected override Material GetMaterial(int x, int y){

		char c = gridString[y].ToCharArray()[x];

		Material mat;

		switch(c){
		case 'd': 
			mat = mats[1];
			break;
		case 'w': 
			mat = mats[2];
			break;
		case 'r': 
			mat = mats[3];
			break;
		case 'b':
			mat = mats[4];
			break;
		default: 
			mat = mats[0];
			break;
		}
	
		return mat;
	}

	public void Update()
	{
		
		
		if (Input.GetMouseButtonDown(0))
		{
			// Darryl tried but couldnt figure out how to change tiles on click at runtime
			
			// _objectRenderer.material = mats[3];
			// _material = new Material(mats[3]);
			// this._material = Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(transform.position.y));
			// Debug.Log("This material is:" + this.GetMaterial() );
			Debug.Log("The mouse click occured at: " + Mathf.Round(Input.mousePosition.x) + ", " + Mathf.RoundToInt(Input.mousePosition.y));
		}
		
	}
}
