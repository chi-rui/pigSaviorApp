using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour {

 	public int userProgress;
 	public int goalProgress;

	// Use this for initialization
	void Start () {
		userProgress = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void nextPlot(){
		userProgress += 1;
	}
}

