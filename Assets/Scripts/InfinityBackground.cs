using UnityEngine;
using System.Collections;

public class InfinityBackground : MonoBehaviour {

	private float speed;

	// Use this for initialization
	void Start () {
		speed = 10f;
	}
	
	// Update is called once per frame
	void Update () {
		var x = GetComponent<Renderer>().material.mainTextureOffset.x;
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(x-speed, 0));
	}

	// public float scrollSpeed;
	// public float tileSize;

	// private Vector2 startPosition;

	// void Start ()
	// {
	//     startPosition = transform.position;
	// }

	// void Update ()
	// {
	//     float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSize);
	//     transform.position = startPosition + new Vector2(-1f ,0f) * newPosition;
	// }
}
