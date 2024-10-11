using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllisonTerry
{
    
    
    public class AllisonTerryHeuristic : HeuristicScript
    {
        public float heursticValue = 0f;
        public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript)
        {
            //This is the Manhattan distance from the article, not sure how to make other ones
            //this one returns the distance between the two points, start and goal
            //this number then gets added to the cost of the tile to decide the priority in the a star script
            //since each tile will be the same number, the path will be the same every time
            //unsure on how to get other heuristics
            heursticValue = Mathf.Abs(start.x - goal.x) + Mathf.Abs(start.y - goal.y);
            Debug.Log(heursticValue);
            return heursticValue;

        }
    }

}