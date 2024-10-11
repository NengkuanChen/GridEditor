using UnityEngine;
using System.Collections;

namespace DabuLyu
{
    public class DabuFixedHeuristicScript : HeuristicScript {
		
        public float weight = 1;
         //Manhattan Distance
         public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript) {
             return (Mathf.Abs(goal.x - x) + Mathf.Abs(goal.y - y)) * weight;
         }
        
        
  

    }
    
    


}
