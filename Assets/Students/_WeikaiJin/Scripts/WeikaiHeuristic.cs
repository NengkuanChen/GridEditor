using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  WeikaiJin
{
    public class WeikaiHeuristic : HeuristicScript
    {

        GridScript preprocessedGridScript = null;

        private GameObject[,] pos;
        private float[,] cost;
        int sizeX, sizeY;
        private int xg, yg;
        private float minValue = 999999f;
        void Preprocess(GridScript gridScript)
        {
            // if this grid is already preprocessed...
            if(preprocessedGridScript == gridScript)
                return;
            preprocessedGridScript = gridScript;
            
            pos = gridScript.GetGrid();
            sizeX = pos.GetLength(0);
            sizeY = pos.GetLength(1);
            cost = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
                cost[i,j] = gridScript.GetMovementCost(pos[i,j]);
        }
        

        private int[] moveX = new[] { -1, 0, 0, 1 };
        private int[] moveY = new[] { 0, 1, -1, 0 };
        private int moveCnt = 4;
        
        public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript)
        {
            
            Preprocess(gridScript);

            
            xg = (int)goal.x;
            yg = (int)goal.y;
            
            
            if (System.Math.Abs(xg - x) + System.Math.Abs(yg - y) <= 0)
                return 0f;
            if (System.Math.Abs(xg- x) + System.Math.Abs(yg- y) <= 1)
                return cost[xg,yg];

            int x1, x2;
            int y1, y2;
            minValue = 999999f;
            
            float curValue = 0f;
            
            // Enumerate the next 2 steps
            for (int i = 0; i < moveCnt; i++)
            {
                x1 = x + moveX[i];
                y1 = y + moveY[i];
                if (x1 >= sizeX || x1 < 0 || y1 >= sizeY || y1 < 0)
                    continue;
                
                for (int j = 0; j < moveCnt; j++)
                {
                    x2 = x1 + moveX[j];
                    y2 = y1 + moveY[j];
                    if(x2>= sizeX || x2 < 0 || y2>= sizeY || y2 < 0)
                        continue;
                    curValue = cost[x1,y1] + cost[x2, y2] + 0.245f * (Mathf.Abs(xg-x2) + Mathf.Abs(yg-y2));
                    minValue = Mathf.Min(curValue, minValue);
                }
            }
            
            // float k =0.245f * (Mathf.Abs(goal.x - x) + Mathf.Abs(goal.y - y));
            // if (minValue > k)
            // {
            //     Debug.Log("Diff: " +"x: "+ x + " y: "+y+"\n" + minValue + " " + k);
            //     pos[x,y].transform.rotation = Quaternion.Euler(0, 0, 45f);
            // }
            return minValue;
        }
        
        
        // public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript)
        // {
        //     var pos = gridScript.GetGrid();
        //
        //     float value = 0f;
        //     float value2 = 0f;
        //     int xg = (int)goal.x;
        //     int yg = (int)goal.y;
        //
        //     int xSign = System.Math.Sign(xg - x);
        //     int ySign = System.Math.Sign(yg - y);
        //     
        //     
        //     // Debug.Log("-- Weikai Heuristic: " + x + ", "+ y +
        //     //           "\n to: "+ goal.x + ", "+ goal.y);
        //     if (xSign != 0)
        //     {
        //         for (int i = x + xSign; xSign * (xg - i) >= 0; i += xSign)
        //         {
        //             value += gridScript.GetMovementCost(pos[i,y]);
        //             value2 += gridScript.GetMovementCost(pos[i, yg]);
        //             //Debug.Log("pos: " + i + ", " + y + " value: " + value);
        //         }
        //     }
        //
        //     if (ySign != 0)
        //     {
        //         for (int i = y + ySign; ySign * (yg - i) >= 0; i += ySign)
        //         {
        //             value += gridScript.GetMovementCost(pos[xg,i]);
        //             value2 += gridScript.GetMovementCost(pos[x, i]);
        //             //Debug.Log("pos: " + xg + ", " + i + " value: " + value);
        //         }
        //     }
        //     
        //     
        //     
        //     return 0.6f * Mathf.Min(value, value2);
        // }
        //
        
    }
}
