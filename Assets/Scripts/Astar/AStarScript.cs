//https://www.redblobgames.com/pathfinding/a-star/introduction.html

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine.Serialization;

public class AStarScript : MonoBehaviour {

	public bool visualizeGridSpacesVisited = true;

	public GridScript gridScript;
	public HeuristicScript heuristic;

	protected int gridWidth;
	protected int gridHeight;

	GameObject[,] pos;

	//A Star stuff
	protected Vector3 start;
	protected Vector3 goal;

	public Path path;
	private const int MAX_LOCATIONS_IN_QUEUE = 1000;
	//FastPriorityQueue from https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
	protected FastPriorityQueue<PriorityQueueVector3> frontier;

	protected Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
	protected Dictionary<Vector3, float> costSoFar = new Dictionary<Vector3, float>();
	protected Vector3 current;

	// Use this for initialization
	protected virtual void Start () {
		InitAstar();
		
	}

	protected virtual void InitAstar(){
		InitAstar(new Path(heuristic.gameObject.name, gridScript));
	}

	protected virtual void InitAstar(Path path){
		this.path = path;

		//Get Grid information
		pos = gridScript.GetGrid();
		
		start = gridScript.start;
		goal = gridScript.goal;
		
		gridWidth = gridScript.gridWidth;
		gridHeight = gridScript.gridHeight;
		
		//set up frontier, path, and cost
		frontier = new FastPriorityQueue<PriorityQueueVector3>(MAX_LOCATIONS_IN_QUEUE);
		frontier.Enqueue(new PriorityQueueVector3(start), 0);

		cameFrom.Add(start, start);
		costSoFar.Add(start, 0);

		//var to track nodes visited
		int exploredNodes = 0;

		//go through all the frontier nodes (begins with just the start node)
		while(frontier.Count != 0){
			exploredNodes++;
			current = frontier.Dequeue().Vector;

			if (visualizeGridSpacesVisited)
			{
				//decrease the scale of the grid space being checked, just to give a visual que of how often a grid position was visited
				pos[(int)current.x, (int)current.y].transform.localScale =
					Vector3.Scale(pos[(int)current.x, (int)current.y].transform.localScale, new Vector3(.8f, .8f, .8f));
			}

			//if the current position being checked is the goal
			if(current.Equals(goal)){
				Debug.Log("GOOOAL!");
				break; //EARLY EXIT
			}
			
			//otherwise, add the positions left and right to the frontier
			for(int x = -1; x < 2; x+=2){
				AddNodesToFrontier((int)current.x + x, (int)current.y);
			}
			
			//add the positions top and bottom to the frontier
			for(int y = -1; y < 2; y+=2){
				AddNodesToFrontier((int)current.x, (int)current.y + y);
			}
		}

		path.nodeInspected = exploredNodes;

		//start at the goal
		current = goal;

		//get the lineRenderer to show the path
		LineRenderer line = GetComponent<LineRenderer>();

		line.positionCount = 0;

		int i = 0;
		float score = 0;

		//while we haven't gotten back to the start yet
		while(!current.Equals(start)){
			//get the position of the grid tile and add it to the path
			GameObject go = pos[(int)current.x, (int)current.y];
			path.Insert(0, go, new Vector3((int)current.x, (int)current.y));
			
			//set the position the current line position to the just above the current tile position
			Vector3 vec = Util.clone(go.transform.position);
			vec.z = -1;
			
			//increment line position
			line.positionCount++;
			line.SetPosition(i, vec);
			
			//add the movement score to the cost;
			score += gridScript.GetMovementCost(go);
			
			//change "current" to the the previous node (ie, move towards the start position)
			current = cameFrom[current];
			i++; //increase the line position by one
		}
		
		//add start position to lineRenderer
		Vector3 startPos = Util.clone(pos[(int)current.x, (int)current.y].transform.position);
		startPos.z = -1;
		line.positionCount++;
		line.SetPosition(i, startPos);

		//add the start position to the path
		path.Insert(0, pos[(int)start.x, (int)current.y]);
		
		//Log the core, nodes checked and total score
		Debug.Log(path.pathName + " Terrian Score: " + score);
		Debug.Log(path.pathName + " Nodes Checked: " + exploredNodes);
		Debug.Log(path.pathName + " Total Score: " + (score + exploredNodes));
	}

	//Add new nodes to the frontier to be checked
	void AddNodesToFrontier(int x, int y){
		//if the node position is valid
		if(x >=0 && x < gridWidth && 
		   y >=0 && y < gridHeight)
		{
			//make a node at position x,y
			Vector3 next = new Vector3(x, y);
			//MOVEMENT COST
			//get the cost of coming to the current node from the path it took to get here and the movement cost of this node
			float newCost = costSoFar[current] + gridScript.GetMovementCost(pos[x, y]);
			
			//if we haven't visited this node already, or we've found a cheaper path to get there
			if(!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
			{
				//DIJKSTRA'S ALGORITHM
				//set the cost to the current cost (update it if it's cheaper)
				costSoFar[next] = newCost;
				//HEURISTIC SEARCH
				float hueristicValue = heuristic.Heuristic(x, y, start, goal, gridScript);
				//ASTAR = DIJKSTRA'S ALGORITHM + HEURISTIC SEARCH
				//get the priority of for checking this node based on the cost and the heuristic
				//and insert it into the priority queue with that cost
				float priority = costSoFar[next] + hueristicValue;

				//put it into the queue with that cost
				frontier.Enqueue(new PriorityQueueVector3(next), priority);
				//track in previous position for the new node to be the node we had previously explored
				cameFrom[next] = current;
			}
		}
	}
}
