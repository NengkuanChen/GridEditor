using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecondGridScript : GridScript {

	string[] gridString = new string[]{
		"----|----}wwwwwwww-|----|----|",
		"--r-|----|---wpppw-|-f--r----|",
		"----|--f-|---rpppppp-f--|----|",
		"--f-r----|---wpppw-p-f--|--r-|",
		"----|----|---wwrww-p-f--|----|",
		"----|----|---p|----ppf--|----|",
		"ff-f|ff--|--wwwwww-fpffwwwwwwww",
		"ff---fffpwwwwwwwwwfffwwwwwwwww",
		"fffrffff-|wwwwrwwwwwww--|----|",
		"f--ffff--|--wrlr---fgppp|-r--|",
		"frffff---f---rllr--|-ffppp-f-|",
		"----|----|--rrlllrrr----|p---|",
		"---rrrrrrrrrrllllllrrrrrrrrrrr",
		"rr--|----rrrrlllllrrrrr-|-r--|",
		"---r|--r-|-r-lrrl--r---r|----|",
		"-r--|r--r|---lrrl--|-r--|r-r-|",
		"--r-|----|r---rr---|----|----|"
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
		case 'r': 
			mat = mats[1];
			break;
		case 'w': 
			mat = mats[2];
			break;
		case 'l': 
			mat = mats[3];
			break;
		case 'p': 
			mat = mats[4];
			break;
		case 'f': 
			mat = mats[5];
			break;
		default: 
			mat = mats[0];
			break;
		}
	
		return mat;
	}
}
