// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using Priority_Queue;
// using UnityEngine.Serialization;
//
// public class AStarScript : MonoBehaviour {
//
//     public bool visualizeGridSpacesVisited = true;
//     public GridScript gridScript;
//     public HeuristicScript heuristic;
//
//     protected int gridWidth;
//     protected int gridHeight;
//     GameObject[,] pos;
//
//     // A* related variables
//     protected Vector3 start;
//     protected Vector3 goal;
//
//     public Path path;
//     private const int MAX_LOCATIONS_IN_QUEUE = 1000;
//     protected FastPriorityQueue<PriorityQueueVector3> frontier;
//     protected Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
//     protected Dictionary<Vector3, float> costSoFar = new Dictionary<Vector3, float>();
//     protected Vector3 current;
//
//     // Use this for initialization
//     protected virtual void Start() {
//         InitAstar();
//     }
//
//     protected virtual void InitAstar(Path path = null, float weight = 1.0f) {
//         if (path == null) {
//             path = new Path(heuristic.gameObject.name, gridScript);
//         }
//         this.path = path;
//
//         // Start recording A* algorithm calculation time
//         float startTime = Time.realtimeSinceStartup;
//
//         // Get grid information
//         pos = gridScript.GetGrid();
//         start = gridScript.start;
//         goal = gridScript.goal;
//         gridWidth = gridScript.gridWidth;
//         gridHeight = gridScript.gridHeight;
//
//         // Initialize frontier, path, and costs
//         frontier = new FastPriorityQueue<PriorityQueueVector3>(MAX_LOCATIONS_IN_QUEUE);
//         frontier.Enqueue(new PriorityQueueVector3(start), 0);
//
//         cameFrom.Add(start, start);
//         costSoFar.Add(start, 0);
//
//         // Variable to track nodes explored
//         int exploredNodes = 0;
//
//         // A* algorithm main loop, start exploring nodes
//         while (frontier.Count != 0) {
//             exploredNodes++;  // Increment count every time a node is explored
//             current = frontier.Dequeue().Vector;
//
//             if (visualizeGridSpacesVisited) {
//                 // Visualize the current grid position being checked
//                 pos[(int)current.x, (int)current.y].transform.localScale =
//                     Vector3.Scale(pos[(int)current.x, (int)current.y].transform.localScale, new Vector3(.8f, .8f, .8f));
//             }
//
//             // Check if we have reached the goal
//             if (current.Equals(goal)) {
//                 Debug.Log("GOOOAL!");
//                 break;  // Early exit
//             }
//
//             // Add left and right positions to frontier
//             for (int x = -1; x < 2; x += 2) {
//                 AddNodesToFrontier((int)current.x + x, (int)current.y, weight);
//             }
//
//             // Add top and bottom positions to frontier
//             for (int y = -1; y < 2; y += 2) {
//                 AddNodesToFrontier((int)current.x, (int)current.y + y, weight);
//             }
//         }
//
//         // Record the time when algorithm calculation ends
//         float calculationTime = Time.realtimeSinceStartup - startTime;
//         Debug.Log("Explored Nodes: " + exploredNodes);
//         Debug.Log("Calculation Time: " + calculationTime);
//
//         // Backtrace the path and calculate path length
//         current = goal;
//         LineRenderer line = GetComponent<LineRenderer>();
//         line.positionCount = 0;
//
//         float pathLength = 0f;  // Path length
//         int i = 0;
//
//         // Backtrace the path until the start point
//         while (!current.Equals(start)) {
//             GameObject go = pos[(int)current.x, (int)current.y];
//             path.Insert(0, go, new Vector3((int)current.x, (int)current.y));
//
//             // Set the line renderer position
//             Vector3 vec = Util.clone(go.transform.position);
//             vec.z = -1;
//             line.positionCount++;
//             line.SetPosition(i, vec);
//
//             // Accumulate the path length (based on terrain movement cost)
//             pathLength += gridScript.GetMovementCost(go);
//             current = cameFrom[current];
//             i++;
//         }
//
//         // Add the start position to the line renderer
//         Vector3 startPos = Util.clone(pos[(int)start.x, (int)start.y].transform.position);
//         startPos.z = -1;
//         line.positionCount++;
//         line.SetPosition(i, startPos);
//         path.Insert(0, pos[(int)start.x, (int)start.y]);
//
//         Debug.Log(path.pathName + " Path Length: " + pathLength);
//
//         // Calculate total time (calculation time + movement time)
//         float totalTime = calculationTime + pathLength;  // Assuming constant movement speed, movement time is approximated by path length
//         Debug.Log("Total Time (Calculation + Movement): " + totalTime);
//     }
//
//     // Add neighboring nodes to frontier and calculate heuristic value
//     void AddNodesToFrontier(int x, int y, float weight) {
//         if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight) {
//             Vector3 next = new Vector3(x, y);
//             float newCost = costSoFar[current] + gridScript.GetMovementCost(pos[x, y]);
//
//             // If the node has not been visited yet or we found a better path
//             if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
//                 costSoFar[next] = newCost;
//                 float heuristicValue = heuristic.Heuristic(x, y, start, goal, gridScript) * weight;
//                 float priority = costSoFar[next] + heuristicValue;
//                 frontier.Enqueue(new PriorityQueueVector3(next), priority);
//                 cameFrom[next] = current;
//             }
//         }
//     }
// }
