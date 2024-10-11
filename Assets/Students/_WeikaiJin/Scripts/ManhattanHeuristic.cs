using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace  WeikaiJin
{
    public class ManhattanHeuristic : HeuristicScript
    {
        public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript)
        {
            
            
            return 0.245f * (Mathf.Abs(goal.x - x) + Mathf.Abs(goal.y - y));
        }
    }
}
