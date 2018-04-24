using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmMode : MonoBehaviour {
	public GameObject pointer;
	public float speed;
	
	private Vector3 pos_L, pos_R;

	// Use this for initialization
	void Start () {
		pos_L = new Vector3(-1860f, pointer.transform.position.y, pointer.transform.position.z);
		pos_R = new Vector3(-530f, pointer.transform.position.y, pointer.transform.position.z);
		speed = 0.5f;
	}
	
	// Update is called once per frame
	// -540 -1850
	void Update () {
		pointer.transform.position = Vector3.Lerp(pos_L, pos_R, Mathf.PingPong(speed * Time.time, 1.0f));
	}
}
