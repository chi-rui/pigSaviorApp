using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour {

	public GameObject character, nextArrow, lastArrow;
	// public Vector2 target = new Vector2(-5.5f, -6.5f);
	public float speed, fspeed;
	public bool goNext = true;

	// Use this for initialization
	void Start () {
		speed = 0f;
		// print(character.transform.position);
		character.transform.position = new Vector2(-2000f, -1400f);
	}
	
	// Update is called once per frame
	void Update () {

	}

	IEnumerator move( Vector2 position ){
		speed = 0.05f;
		fspeed = Vector2.Distance(character.transform.position, position) * speed;
		while(speed != 0){
			character.transform.position = Vector2.Lerp(character.transform.position, position, speed);
			speed = calculateNewSpeed(position);
			yield return 0;
		}
	}

	private float calculateNewSpeed( Vector2 target ){
		float tmp = Vector2.Distance(character.transform.position, target );
		if (tmp == 0)
			return tmp;
		else
			return (fspeed / tmp);
	}


	public void nextClicked(){
		string stageName;
		StartCoroutine(lockObject(true, 0f));
		if(GameEvents.nowStage < GameEvents.progress){
			// stageName = "Button_points_" + GameEvents.nowStage.ToString();
			stageName = "Image_points" + GameEvents.nowStage.ToString();
			print(stageName);
			StageInfo stage = GameObject.Find(stageName).GetComponent<StageInfo>();

			if(stage.isNextNeedTurn){
				if(stage.isNextHorizontalFirst){
					// print("先水平要轉彎");
					StartCoroutine(move(new Vector2(stage.next.x, character.transform.position.y)));
				}else{
					// print("先垂直要轉彎");
					StartCoroutine(move(new Vector2(character.transform.position.x, stage.next.y)));
				}
				StartCoroutine(nextMove(0.8f, stage.next));
				StartCoroutine(lockObject(false, 1.3f));
			}else{
				StartCoroutine(move(stage.next));
				StartCoroutine(lockObject(false, 0.8f));
			}
			GameEvents.nowStage++;
		}else{
			Debug.Log("error : Can't over progress.(" + GameEvents.nowStage + ")");			
		}
	}

	public void lastClicked(){
		string stageName;
		StartCoroutine(lockObject(true, 0f));
		if(GameEvents.nowStage > 1){
			// stageName = "Button_points_" + GameEvents.nowStage.ToString();
			stageName = "Image_points" + GameEvents.nowStage.ToString();
			StageInfo stage = GameObject.Find(stageName).GetComponent<StageInfo>();

			if(stage.isLastNeedTurn){
				if(stage.isLastHorizontalFirst){
					// print("先水平要轉彎");	
					StartCoroutine(move(new Vector2(stage.last.x, character.transform.position.y)));
				}else{
					// print("先垂直要轉彎");
					StartCoroutine(move(new Vector2(character.transform.position.x, stage.last.y)));
				}
				StartCoroutine(nextMove(0.8f, stage.last));
				StartCoroutine(lockObject(false, 1.3f));
			}else{
				StartCoroutine(move(stage.last));
				StartCoroutine(lockObject(false, 0.8f));
			}
			GameEvents.nowStage--;
		}else{
			Debug.Log("error : Already the first stage.(" + GameEvents.nowStage + ")");
		}
	}

	IEnumerator nextMove( float time, Vector2 position ){
		yield return new WaitForSeconds(time);
		StartCoroutine(move(position));
	}

	IEnumerator lockObject(bool l, float time){
		yield return new WaitForSeconds(time);
		if(l){
			// print("lock");
			nextArrow.SetActive(false);
			lastArrow.SetActive(false);
		}else{
			// print("open");
			nextArrow.SetActive(true);
			lastArrow.SetActive(true);
		}
	}
}