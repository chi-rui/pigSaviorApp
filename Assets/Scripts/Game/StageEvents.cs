using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StageEvents : MonoBehaviour {

	private DatasControl dataControl;

	public float speed;
	public GameObject mainCharacter, mainCamera, TalkWindow, gamePanel, correctPanel, wrongPanel, enterPanel, teachingPanel, plotsImage, NPCs, stageClear;
	private Vector3 newPosition, newCameraPosition;
	public int userProgress;
	private bool isGameStart = false;
	private bool isProgressIncrease = false;
	// saved npc plots contents 
	private List<string> plots = new List<string>();
	private GameObject npc, newNpc;
	private int page, userLife;
	// prompt for next plots
	public List<StagePrompts> prompts = new List<StagePrompts>();


	// Use this for initialization
	void Start () {
		dataControl = GameObject.Find("Datas").GetComponent<DatasControl>();
		userProgress = 0;
		userLife = 3;

	}

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0) && !isGameStart && !plotsImage.activeInHierarchy && !enterPanel.activeInHierarchy){
			if(EventSystem.current.currentSelectedGameObject == null && !TalkWindow.activeInHierarchy){
				characterMove();
			}
		}
	}
	// 1575 1600
	// Control the move of the main character.
	void characterMove () {
		if(Input.mousePosition.x < (Screen.width / 2)){
			mainCharacter.transform.eulerAngles = new Vector3(0,180,0);
			newPosition = new Vector3(Mathf.Clamp(mainCharacter.transform.position.x - 50f, -1575f, 1600f), mainCharacter.transform.position.y, 0f);
			if(newPosition.x < 1150f)
				newCameraPosition = new Vector3(Mathf.Clamp(mainCamera.transform.position.x - 50f, -1200f, 1200f), mainCamera.transform.position.y, mainCamera.transform.position.z);
			else
				newCameraPosition = mainCamera.transform.position;
		}else if (Input.mousePosition.x > (Screen.width / 2)){
			mainCharacter.transform.eulerAngles = new Vector3(0,0,0);
			newPosition = new Vector3(Mathf.Clamp(mainCharacter.transform.position.x + 50f, -1575f, 1600f), mainCharacter.transform.position.y, 0f);
			if(newPosition.x > -1150f)
				newCameraPosition = new Vector3(Mathf.Clamp(mainCamera.transform.position.x + 50f, -1200f, 1200f), mainCamera.transform.position.y, mainCamera.transform.position.z);
			else
				newCameraPosition = mainCamera.transform.position;
		}
		mainCharacter.transform.position = Vector3.Lerp(mainCharacter.transform.position, newPosition, speed * Time.deltaTime);
		mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newCameraPosition, speed * Time.deltaTime);
	}

	// Set the game info of the plots.
	public void setGameInfo( GameObject npcObj, GameObject npcFinish, GameObject game, Sprite header, string title, string npcPlots, bool isCorrectPlot ){
		TalkWindow.SetActive(true);
		TalkWindow.transform.GetChild(0).GetComponentInChildren<Image>().sprite = header;
		TalkWindow.transform.GetChild(1).GetComponentInChildren<Text>().text = title;
		TalkWindow.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "繼續";
		plots = npcPlots.Split('#').ToList();
		gamePanel = game;
		isProgressIncrease = isCorrectPlot;
		page = 0;
		npc = npcObj;
		newNpc = npcFinish;
		nextPagePlots();
	}
	
	public void nextPagePlots(){
		if(page == plots.Count){
			TalkWindow.SetActive(false);
			if(isProgressIncrease){
				if(gamePanel == null)
					StartCoroutine(checkStageProgress());
				else
					enterPanel.SetActive(true);
			}else{
				// print("Wrong plots");
			}
		}else{
			TalkWindow.transform.GetChild(2).GetComponentInChildren<Text>().text = plots[page];
			page++;
			if(page == plots.Count){
				TalkWindow.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "結束";
			}
		}
	}

	// Show the game panel and start the game.
	public void taskStart(){
		gamePanel.SetActive(true);
		isGameStart = true;
		NPCs.SetActive(false);
		mainCharacter.SetActive(false);
	}

	// Show the suit feedback after check the result of the game.(check is in game script)
	public void showFeedBack( bool ans, string prompts ){
		if(ans){
			StartCoroutine(Feedback(correctPanel));
			StartCoroutine(checkStageProgress());
		}else{
			GameObject.Find("Feedbacks").transform.GetChild(1).GetChild(2).GetComponent<Text>().text = prompts;
			// ... set wrong panel hints.
			userLife--;
			if(userLife == 0){
				GameObject.Find("player life").transform.GetChild(1).gameObject.SetActive(false); //GetComponent<Image>().sprite = null;
				GameObject.Find("player life").transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("gg") as Sprite;
			}else{
				string life = "Life" + userLife.ToString();
				GameObject.Find("player life").transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(life) as Sprite;
			}
			StartCoroutine(Feedback(wrongPanel));
		}
	}

	public IEnumerator Feedback( GameObject imageFeedBack ){
		imageFeedBack.SetActive(true);
		yield return new WaitForSeconds(2f);
		imageFeedBack.SetActive(false);
		NPCs.SetActive(true);
		mainCharacter.SetActive(true);
		if(userLife > 0){
			isGameStart = false;
		}else{
			if (DatasControl.GameMode == "PICK")
				print("PICK");
			else if (DatasControl.GameMode == "TEXTQUES")
				GameObject.Find("EventSystem").GetComponent<TextQuesDynamicAssessment>().teachNum(-1);
			else
				GameObject.Find("EventSystem").GetComponent<DynamicAssessment>().teachNum(-1);
		}
	}

	// Increase the stage progress and check if the stage is finish.
	public IEnumerator checkStageProgress(){
		yield return new WaitForSeconds(0f);
		userProgress += 1;
		if(newNpc != null){
			npc.SetActive(false);
			newNpc.SetActive(true);
		}
		if(userProgress == dataControl.stageGoal){
			stageClear.SetActive(true);
			StartCoroutine(showPlots());
			// wait
			yield return new WaitForSeconds(2f);
			dataControl.progress += 1;
			SceneManager.LoadScene("Chapter"+dataControl.chapter.ToString());
		}else{
			StartCoroutine(showPlots());
		}
	}

	private IEnumerator showPlots(){
		// prompts > userProgress --;
		// print(userProgress);
		while(correctPanel.activeInHierarchy || wrongPanel.activeInHierarchy){
			yield return new WaitForSeconds(0.1f);
		}
		if(prompts[userProgress-1].pictures.Count != 0){
			plotsImage.SetActive(true);
			for(int i = 0; i < prompts[userProgress-1].pictures.Count; i++){
				plotsImage.transform.GetChild(0).GetComponent<Image>().sprite = prompts[userProgress-1].pictures[i];
				plotsImage.transform.GetChild(1).GetComponent<Text>().text = prompts[userProgress-1].words[i];
				yield return new WaitForSeconds(4f);
			}
			plotsImage.SetActive(false);
		}
	}

}
