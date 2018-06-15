using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterEvents : MonoBehaviour {

	private DatasControl gameDatas;
	public Stage stage;
	public GameObject character, GoBackPrompt;
	public Button nextArrow, lastArrow;
	public float speed, fspeed;
	public bool goNext;
	// public int offset;
	public Vector2 startPoint;

	// Use this for initialization
	void Start () {
		// initialization
		gameDatas = GameObject.Find("Datas").GetComponent<DatasControl>();
		stage = GameObject.Find("Image_points" + gameDatas.nowStage.ToString()).GetComponent<Stage>();
		speed = 0f;
		goNext = true;
		character.transform.position = new Vector3(stage.transform.position.x, stage.transform.position.y+100f, 0);
		
		// set game datas.
		if(gameDatas.loadingPanel == null)
			gameDatas.setGameObjects();
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
		lockObject(false);
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
		lockObject(true);
		if(gameDatas.nowStage+1 <= gameDatas.progress){
			if(gameDatas.nowStage == 11){
				// working stage
				print("still working.");
				lockObject(false);
			}else{
				// open stage.
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
			}
		}else{
			Debug.Log("error : Can't over progress.(" + gameDatas.nowStage + ")");
			lockObject(false);
		}
	}

	public void lastClicked(){
		if(gameDatas.nowStage == 6){
			print("go back");
			GoBackPrompt.SetActive(true);
			// goBack();
		}else{
			string stageName;
			lockObject(true);
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
				}else{
					StartCoroutine(move(stage.stageInfo.last));
				}
				gameDatas.nowStage--;
			}else{
				Debug.Log("error : Already the first stage.(" + gameDatas.nowStage + ")");
				lockObject(false);
			}
		}
	}

	IEnumerator nextMove( float time, Vector2 position ){
		yield return new WaitForSeconds(time);
		StartCoroutine(move(position));
	}

	private void lockObject(bool l){
		nextArrow.interactable = lastArrow.interactable = !l;
	}

	public void enterStage(){
		if(gameDatas.nowStage == 5 || gameDatas.nowStage == 16){
			gameDatas.progress++;
			gameDatas.nowStage++;
			gameDatas.chapter++;
			gameDatas.LoadingScene("Chapter" + gameDatas.chapter.ToString());
		}else if(gameDatas.nowStage == -1){
			// nothing...
			print("still working.");
		}else{
			string stageName = "Image_points" + gameDatas.nowStage.ToString();
			stage = GameObject.Find(stageName).GetComponent<Stage>();
			gameDatas.stageGoal = stage.stageInfo.stageGoal;
			print(gameDatas.stageGoal);
			gameDatas.LoadingScene("stage" + gameDatas.nowStage.ToString());
		}
	}

	public void goBack(){
		
		gameDatas.nowStage--;
		gameDatas.chapter--;
		gameDatas.LoadingScene("Chapter" + gameDatas.chapter.ToString());
	}

	public void changeImage(){
		// ...
	}

	public void setProgress(){
		gameDatas.cheat(16);
	}
}