using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour {
	public Animator Anim_character;
	public static bool isCollision;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "npc") {
			// print("碰到npc了！");
			isCollision = true;
			Anim_character.Play("Image_character meet npc");
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		// print("離開npc了！");
		isCollision = false;
		Anim_character.Play("Image_character stage");
    }
}
