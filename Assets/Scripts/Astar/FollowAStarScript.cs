using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FollowAStarScript : MonoBehaviour {

	protected bool move = false;

	protected Path path;
	public AStarScript astar;
	public Step startPos;
	public Step destPos;
	

	protected int currentStep = 1;

	protected float lerpPer = 0;
	
	protected float startTime;
	protected float travelStartTime;
	
	protected int playerRoll;
	public TextMeshProUGUI rollText;
	public GameObject enemyPrincess;
	public FollowAStarScript enemyPrincessScript;

	// Use this for initialization
	protected virtual void Start () {
		path = astar.path;
		startPos = path.Get(0);
		destPos  = path.Get(currentStep);

		transform.position = startPos.gameObject.transform.position;

		Debug.Log(name + " Start Delay: " + path.nodeInspected/100f);

		Invoke("StartMove", path.nodeInspected/100f);

		startTime = Time.realtimeSinceStartup;

		playerRoll = Random.Range(1, 7);
		rollText.text = "you have " + playerRoll.ToString() + " moves remaining";


	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		if (this.gameObject.name == "Daisy")
		{
			DaisyWASD();
		}
		if (this.gameObject.name == "Peach")
		{
			PeachUPDOWN();
			//
		}

		if(move){
			lerpPer += Time.deltaTime/destPos.moveCost;

			transform.position = Vector3.Lerp(startPos.gameObject.transform.position, 
			                                  destPos.gameObject.transform.position, 
			                                  lerpPer);

			if(lerpPer >= 1){
				lerpPer = 0;

				currentStep++;

				if(currentStep >= path.steps){
					currentStep = 0;
					move = false;
					Debug.Log(path.pathName + " got to the goal in: " + (Time.realtimeSinceStartup - startTime));
					Debug.Log(path.pathName + " travel time: " + (Time.realtimeSinceStartup - travelStartTime));
				} 

				startPos = destPos;
				destPos = path.Get(currentStep);
			}
		}
	}

	protected virtual void StartMove(){
		move = true;
		travelStartTime = Time.realtimeSinceStartup;
	}

	public void PrincessSpeedUp()
	{
		if (playerRoll >= 1)
		{
			lerpPer = 0.9f;
			playerRoll--;
			rollText.text = "you have " + playerRoll.ToString() + " moves remaining";
			if (playerRoll == 1)
			{
				rollText.text = "you have " + playerRoll.ToString() + " move remaining";
			}
		}
		else
		{
			rollText.text = "You have no more moves!";
		}
	}

	public void ChangeSpeed()
	{
		if (playerRoll >= 2)
		{
			enemyPrincessScript.lerpPer = 0.1f;
			playerRoll -= 2;
			rollText.text = "you have " + playerRoll.ToString() + " moves remaining";
		}
		else
		{
			rollText.text = "you do not have enough!" + playerRoll.ToString() + " move left";
		}

		if (playerRoll < 0)
		{
			rollText.text = "you have no more moves!";
		}

	}

	public void StartButton()
	{
		enemyPrincess.gameObject.SetActive(true);
		this.gameObject.SetActive(true);
	}
	
	public void PrincessSpeedDown()
	{
		lerpPer = 0.1f;
	}

	public void DaisyWASD()
	{
		if (Input.GetKeyDown(KeyCode.W))
		{
			Debug.Log("ur pressing w");
			PrincessSpeedUp();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			Debug.Log("ur pressing s");
			ChangeSpeed();
		}
		
	}
	
	
	public void PeachUPDOWN()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			Debug.Log("ur pressing w");
			PrincessSpeedUp();
		}

		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			Debug.Log("ur pressing s");
			ChangeSpeed();
		}
		
	}
}

