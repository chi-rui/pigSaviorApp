using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo : MonoBehaviour {

	public Vector3 last = new Vector3();
	public Vector3 next = new Vector3();
	public bool isLastNeedTurn = false;
	public bool isNextNeedTurn = false;
	public bool isLastHorizontalFirst = false;
	public bool isNextHorizontalFirst = false;
	public int stageProgress;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void clickedStage(){
		// int d = direction;
		// GameEvent.move(this, d);
	}

	public void test() {
		// Debug.Log();
    }
}
