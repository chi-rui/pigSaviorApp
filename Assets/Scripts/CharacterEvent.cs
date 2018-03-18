using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterEvent : MonoBehaviour {
	public float speed;
	public Animator characterAnim;

	private Vector3 leftMove, rightMove, newPosition;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		characterMove ();
	}

	void characterMove () {
		// transform.position += new Vector3(0.3f, 0, 0);

		leftMove = new Vector3(-50f, transform.position.y, 0);
		rightMove = new Vector3(50f, transform.position.y, 0);
		if (Input.GetMouseButtonDown(0)) {
	        if (Input.mousePosition.x < (Screen.width / 2)) {
	        	print("Left: " + Input.mousePosition);
	        	newPosition = leftMove;
	        } else if (Input.mousePosition.x > (Screen.width / 2)) {
	        	print("Right: " + Input.mousePosition);
	        	newPosition = rightMove;
	        }
	        transform.position = Vector3.Lerp(transform.position, newPosition, speed * Time.deltaTime);
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "npc") {
			print("碰到npc了！");
			characterAnim.Play("Image_character meet npc", -1, float.NegativeInfinity);
		}
	}
}
