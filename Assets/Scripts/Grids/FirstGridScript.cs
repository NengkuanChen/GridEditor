using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstGridScript : GridScript {

	string[] gridString = new string[]{
		"ww--|-rw-|----|------------",
		"-ww-|-wr-rrrr-|---rrrr-----",
		"-ww-|----r----|------r--www",
		"-ww-|----r----|------r--ww-",
		"-ww-|----rrrrrrrrrrrr--dww-",
		"-wwwwwwww|----|-------dww--",
		"-wwwwwwwwww---|------d-----",
		"----|---www---|-----dd-----",
		"----|---www---|----dd------",
		"--ddd----w--www---dd-------",
		"--drd----wwww-w--dd--------",
		"--drd----|----wwdd---------",
		"--dddddd-|----|wdd---------",
		"----dddddd----|------------",
		"----|---dd----|------------",
	};

	// Use this for initialization
	void Start () {
		gridWidth = gridString[0].Length;
		gridHeight = gridString.Length;
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
		default: 
			mat = mats[0];
			break;
		}
	
		return mat;
	}
}
