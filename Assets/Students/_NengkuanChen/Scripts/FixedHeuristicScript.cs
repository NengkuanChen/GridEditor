using UnityEngine;

namespace Students._NengkuanChen.Scripts
{
    public class FixedHeuristicScript : HeuristicScript
    {
        public enum HeuristicType
        {
            Manhattan,
            Euclidean,
            Chebyshev,
            Octile,
        }
        
        [SerializeField]
        private HeuristicType heuristicType;
        
        [SerializeField]
        private float weight = 1;
        
        public override float Heuristic(int x, int y, Vector3 start, Vector3 goal, GridScript gridScript)
        {
            switch (heuristicType)
            {
                case HeuristicType.Manhattan:
                    return (Mathf.Abs(x - goal.x) + Mathf.Abs(y - goal.y)) * weight;
                case HeuristicType.Euclidean:
                    return Mathf.Sqrt(Mathf.Pow(x - goal.x, 2) + Mathf.Pow(y - goal.y, 2)) * weight;
                case HeuristicType.Chebyshev:
                    return Mathf.Max(Mathf.Abs(x - goal.x), Mathf.Abs(y - goal.y)) * weight;
                case HeuristicType.Octile:
                    return (Mathf.Max(Mathf.Abs(x - goal.x), Mathf.Abs(y - goal.y)) + (Mathf.Sqrt(2) - 1) *
                        Mathf.Min(Mathf.Abs(x - goal.x), Mathf.Abs(y - goal.y))) * weight;
                default:
                    return 0;
            }
        }
    }
}