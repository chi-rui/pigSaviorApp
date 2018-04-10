using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterEvent : MonoBehaviour {
	public float speed;
	public Animator characterAnim;
	private Vector3 newPosition, newCameraPosition;
	public GameObject mainCamera, TalkWindow;
	
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)){
			if(EventSystem.current.currentSelectedGameObject == null && !TalkWindow.active){
				characterMove();
			}else{
				print(EventSystem.current.currentSelectedGameObject.name);
			}
		}
	}

	void characterMove () {
		if (Input.mousePosition.x < (Screen.width / 2)) {
			// change character image "left move" here.
			newPosition = new Vector3(Mathf.Clamp(transform.position.x - 50f, -1750f, 1750f), transform.position.y, 0f);
			if(newPosition.x < 1150f)
				newCameraPosition = new Vector3(Mathf.Clamp(mainCamera.transform.position.x - 50f, -1200f, 1200f), mainCamera.transform.position.y, mainCamera.transform.position.z);
			else
				newCameraPosition = mainCamera.transform.position;
		} else if (Input.mousePosition.x > (Screen.width / 2)) {
			// change character image "right move" here.
			newPosition = new Vector3(Mathf.Clamp(transform.position.x + 50f, -1750f, 1750f), transform.position.y, 0f);
			if(newPosition.x > -1150f)
				newCameraPosition = new Vector3(Mathf.Clamp(mainCamera.transform.position.x + 50f, -1200f, 1200f), mainCamera.transform.position.y, mainCamera.transform.position.z);
			else
				newCameraPosition = mainCamera.transform.position;
		}
		transform.position = Vector3.Lerp(transform.position, newPosition, speed * Time.deltaTime);
		mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newCameraPosition, speed * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "npc") {
			print("碰到npc了！");
			characterAnim.Play("Image_character meet npc", -1, float.NegativeInfinity);
		}
	}

}
