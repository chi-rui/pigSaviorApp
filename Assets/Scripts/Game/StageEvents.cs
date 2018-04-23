using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageEvents : MonoBehaviour {

	public float speed;
	public GameObject mainCharacter, mainCamera, TalkWindow;
	private Vector3 newPosition, newCameraPosition;
	public int userProgress;
	// Use this for initialization
	void Start () {
		userProgress = 0;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)){
			if(EventSystem.current.currentSelectedGameObject == null && !TalkWindow.activeInHierarchy){
				characterMove();
			}
		}
	}

	void characterMove () {
		if (Input.mousePosition.x < (Screen.width / 2)) {
			// change character image "left move" here.
			newPosition = new Vector3(Mathf.Clamp(mainCharacter.transform.position.x - 50f, -1750f, 1750f), mainCharacter.transform.position.y, 0f);
			if(newPosition.x < 1150f)
				newCameraPosition = new Vector3(Mathf.Clamp(mainCamera.transform.position.x - 50f, -1200f, 1200f), mainCamera.transform.position.y, mainCamera.transform.position.z);
			else
				newCameraPosition = mainCamera.transform.position;
		} else if (Input.mousePosition.x > (Screen.width / 2)) {
			// change character image "right move" here.
			newPosition = new Vector3(Mathf.Clamp(mainCharacter.transform.position.x + 50f, -1750f, 1750f), mainCharacter.transform.position.y, 0f);
			if(newPosition.x > -1150f)
				newCameraPosition = new Vector3(Mathf.Clamp(mainCamera.transform.position.x + 50f, -1200f, 1200f), mainCamera.transform.position.y, mainCamera.transform.position.z);
			else
				newCameraPosition = mainCamera.transform.position;
		}
		mainCharacter.transform.position = Vector3.Lerp(mainCharacter.transform.position, newPosition, speed * Time.deltaTime);
		mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newCameraPosition, speed * Time.deltaTime);
	}

	public void nextProgress(){
		userProgress += 1;
	}

	void taskStart( string challenges ){
		// ...
	}
}
