using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EzrealYe {
public class FixedHeuristic : HeuristicScript
{
    public float averageMovementCost = 3f; //a reasonable seeting of average movement cost among 0.245, 3, 10, and 20
    public float minCost = 0.245f;
    public float maxCost = 20f;

    public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript) {
        
        // calculate the Manhattan distance between the current node and the goal
        // it sums the absolute differences in x and z
        float dx = Mathf.Abs(x - goal.x);
        float dy = Mathf.Abs(y - goal.z);  
        float manhattanDistance = dx + dy;

        // call the gridScript's GetMovementCostFunction to get currentMovementCost
        float currentMovementCost = gridScript.GetMovementCost(gridScript.GetGrid()[x, y]);

        // declare a float t to normalize the current movement cost
        // 't' is a ratio between 0 and 1, indicating where the currentMovementCost lies between minCost and maxCost
        float t = (currentMovementCost - minCost) / (maxCost - minCost);

        // calculate the cost factor using lerp
        // the cost factor will range between 1 and 2, with 1 representing low cost (t close to 0), 2 representing high-cost terrain (t close to 1)
        // it adjusts the heuristic value to make higher terrains are less preferred
        float costFactor = Mathf.Lerp(1f, 2f, t);

        // calculate the final heuristic value using Manhattan distance & Movement Cost
        // the heuristic value is increased by the cost factor, higher movement cost areas will result in a higher heuristic value, making them less favorable

        // use Mathf.Max to ensure that the movement cost used is not lower than the averageMovementCost
        // to prevent underestimating the cost of movement: the current movement cost could be very low, but there it is actually very high-cost
        // another benefit is that it prevent an overly optimistic value when the cost is too low
        float heuristicValue = manhattanDistance * Mathf.Max(averageMovementCost, currentMovementCost) * costFactor;

        // return the final heuristicValue
        return heuristicValue;
    }
}
}
