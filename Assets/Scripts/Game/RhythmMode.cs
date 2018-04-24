using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmMode : MonoBehaviour {
	public GameObject pointer;
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	// 540 1850
	void Update () {
		// pointer.transform.position += new Vector3(5f, 0, 0);
		if (pointer.transform.position.x > -540) {
			print("超過了！");
			pointer.transform.position -= new Vector3(5f, 0, 0);
		}
	}
}
