using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VivianChen
{
    public class VivianFixHeuristicScript : HeuristicScript
    {
        // Weighted Heuristic ??
        // If "w" = 1 ---- it's balancing exploration and exploitation.
        // If "w" > 1 ---- A* will be more focused on the goal,
        //                 rely more heavily on the heuristic,
        //                 becoming more "greedy" about moving toward the goal
        public float weight = 1.0f;
        
        public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript)
        {
            // Calculating the Manhattan Distance
            float value = Mathf.Abs(goal.x - x) + Mathf.Abs(goal.y - y) * weight;

            return value;
        }
    }
}
