using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandraAnderson
{
    public class AlexandraAndersonFixHeuristic : HeuristicScript
    {
        GameObject[,] pos; //positions
        
        //Minimum cost for moving from one space to an adjacent space
        public float weight = 1f;
        
        public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript)
        {
            //Getting Grid Information
            pos = gridScript.GetGrid();
            int gridW = pos.GetLength(0);  // Width of the grid
            int gridH = pos.GetLength(1); // Height of the grid
            
            /*
            //Calculate Movement Cost
            cost = new float[gridW, gridH];
            for (int i = 0; i < sizeX; i++){
                for (int l = 0; l < sizeY; l++){
                    cost[i,l] = gridScript.GetMovementCost(pos[i,l]);
                }
            }
            */
            
            //Test
            Debug.Log(weight);
            
            //Calculate Manhattan Distance
            return (Mathf.Abs(goal.x - x) + Mathf.Abs(goal.y - y)) * weight;
            
        }
    }
}

