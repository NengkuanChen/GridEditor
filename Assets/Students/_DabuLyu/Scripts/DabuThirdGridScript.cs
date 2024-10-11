using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DabuLyu
{
    public class DabuThirdGridScript : GridScript
    {
        string[] gridString = new string[]{
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-------------wwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "rwwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "-wwwwwwwwwwwwwwwwwwwwwwwwwwwww",
            "---------wwwwwwwwwwwwwwwwwwwww",
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

}
