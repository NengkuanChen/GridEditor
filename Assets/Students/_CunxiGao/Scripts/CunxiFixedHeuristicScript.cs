using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CunxiGao
{
    public class CunxiFixedHeuristicScript : HeuristicScript
    {
        public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript)
        {
            //Added manhattan distance as part of the priority
            return Mathf.Abs(goal.x - x) + Mathf.Abs(goal.y - y);
        }
    }

}
