﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterEvents : MonoBehaviour {

	private DatasControl gameDatas;
	public Stage stage;
	public GameObject character, nextArrow, lastArrow;
	public float speed, fspeed;
	public bool goNext = true;


	// Use this for initialization
	void Start () {
		// initialization
		gameDatas = GameObject.Find("Datas").GetComponent<DatasControl>();
		stage = GameObject.Find("Image_points1").GetComponent<Stage>();
		speed = 0f;
		character.transform.position = new Vector2(-2000f, -1400f);
		
		// set game datas.
		if(gameDatas.loadingPanel == null)
			gameDatas.setGameObjects();
		gameDatas.stageGoal = 0;
		// set the situation of return from the stage scenes.
	}
	
	// Update is called once per frame
	void Update () {

	}

	IEnumerator move( Vector2 position ){
		speed = 0.01f;
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
		// StartCoroutine(lockObject(true, 0f));
		if(gameDatas.nowStage+1 <= gameDatas.progress){
			stageName = "Image_points" + gameDatas.nowStage.ToString();
			// print(stageName);
			stage = GameObject.Find(stageName).GetComponent<Stage>();

			if(stage.stageInfo.isNextNeedTurn){
				if(stage.stageInfo.isNextHorizontalFirst){
					// print("先水平要轉彎");
					StartCoroutine(move(new Vector2(stage.stageInfo.next.x, character.transform.position.y)));
				}else{
					// print("先垂直要轉彎");
					StartCoroutine(move(new Vector2(character.transform.position.x, stage.stageInfo.next.y)));
				}
				StartCoroutine(nextMove(0.8f, stage.stageInfo.next));
				// StartCoroutine(lockObject(false, 1.3f));
			}else{
				StartCoroutine(move(stage.stageInfo.next));
				// StartCoroutine(lockObject(false, 0.8f));
			}
			gameDatas.nowStage++;
		}else{
			Debug.Log("error : Can't over progress.(" + gameDatas.nowStage + ")");			
		}
	}

	public void lastClicked(){
		string stageName;
		// StartCoroutine(lockObject(true, 0f));
		if(gameDatas.nowStage > 1){
			stageName = "Image_points" + gameDatas.nowStage.ToString();
			stage = GameObject.Find(stageName).GetComponent<Stage>();

			if(stage.stageInfo.isLastNeedTurn){
				if(stage.stageInfo.isLastHorizontalFirst){
					// print("先水平要轉彎");	
					StartCoroutine(move(new Vector2(stage.stageInfo.last.x, character.transform.position.y)));
				}else{
					// print("先垂直要轉彎");
					StartCoroutine(move(new Vector2(character.transform.position.x, stage.stageInfo.last.y)));
				}
				StartCoroutine(nextMove(0.8f, stage.stageInfo.last));
				// StartCoroutine(lockObject(false, 1.3f));
			}else{
				StartCoroutine(move(stage.stageInfo.last));
				// StartCoroutine(lockObject(false, 0.8f));
			}
			gameDatas.nowStage--;
		}else{
			Debug.Log("error : Already the first stage.(" + gameDatas.nowStage + ")");
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

	public void enterStage(){
		gameDatas.stageGoal = stage.stageInfo.stageGoal;
		gameDatas.LoadingScene("stage" + gameDatas.nowStage.ToString());
	}

	public void changeImage(){
		// ...
	}

	public void setProgress(){
		gameDatas.cheat(5);
	}
}