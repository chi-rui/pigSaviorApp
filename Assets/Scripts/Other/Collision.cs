using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour {
	public Animator characterAnim;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "npc") {
			// print("碰到npc了！");
			characterAnim.Play("Image_character meet npc");
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		// print("離開npc了！");
		characterAnim.Play("Image_character stage");
    }
}
