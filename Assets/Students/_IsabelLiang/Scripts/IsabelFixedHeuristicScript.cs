using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IsabelLiang
{
public class IsabelFixedHeuristicScript : HeuristicScript
{
    GameObject[,] pos;
    public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript)
    {
        // Tie breaker to add a deterministic random number to the heuristic or edge costs
        float dx1 = x - goal.x;
        float dy1 = y - goal.y;
        float dx2 = start.x - goal.x;
        float dy2 = start.y - goal.y;
        float cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
        float tieBreakerAdd = cross * 0.0001f;

        // Test out the variable D: the minimum cost D for moving from one space to an adjacent space.
        float D = 1; // simple

        // Get grid information
        GameObject[,] pos = gridScript.GetGrid();
        int gridWidth = pos.GetLength(0);  // Width of the grid
        int gridHeight = pos.GetLength(1); // Height of the grid

        // Check bounds before accessing grid positions
        if (x >= 1 && x < gridWidth - 1 && y >= 1 && y < gridHeight - 1)
        {
            GameObject go = pos[(int)x, (int)y];

            GameObject goLeft = x > 0 ? pos[(int)x - 1, (int)y] : null;
            GameObject goRight = x < gridWidth - 1 ? pos[(int)x + 1, (int)y] : null;
            GameObject goUp = y < gridHeight - 1 ? pos[(int)x, (int)y + 1] : null;
            GameObject goDown = y > 0 ? pos[(int)x, (int)y - 1] : null;

            /*Debug.Log(go.name + (goLeft != null ? goLeft.name : "") +
                      (goRight != null ? goRight.name : "") +
                      (goUp != null ? goUp.name : "") +
                      (goDown != null ? goDown.name : ""));*/

            // Check if the adjacent squares exist before calculating the movement cost
            if (goLeft != null) D = Mathf.Min(D, gridScript.GetMovementCost(goLeft));
            if (goRight != null) D = Mathf.Min(D, gridScript.GetMovementCost(goRight));
            if (goUp != null) D = Mathf.Min(D, gridScript.GetMovementCost(goUp));
            if (goDown != null) D = Mathf.Min(D, gridScript.GetMovementCost(goDown));
        }

        Debug.Log(D);

        // Manhattan + tie breaker
        return (D * (Math.Abs(x - goal.x) + Math.Abs(y - goal.y)) + tieBreakerAdd );
    }

    
}

}

