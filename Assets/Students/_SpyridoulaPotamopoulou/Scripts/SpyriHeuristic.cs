using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyriHeuristic : HeuristicScript
{
    public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript){
        x = Mathf.RoundToInt(transform.position.x - goal.x);
        y = Mathf.RoundToInt(transform.position.y - goal.y);
        return 1 * (x + y);
    }
}
