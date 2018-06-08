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
	public GameObject mainCharacter, mainCamera, TalkWindow, gamePanel, correctPanel, wrongPanel, warningPanel, enterPanel, teachingPanel, plotsImage, NPCs;
	private Vector3 newPosition, newCameraPosition;
	public int userProgress;
	private bool isGameStart = false;
	private bool isProgressIncrease = false;
	// saved npc plots contents 
	private List<string> plots = new List<string>();
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
		if(Input.GetMouseButton(0) && !isGameStart){
			if(EventSystem.current.currentSelectedGameObject == null && !TalkWindow.activeInHierarchy){
				characterMove();
			}
		}
	}

	// Control the move of the main character.
	void characterMove () {
		if(Input.mousePosition.x < (Screen.width / 2)){
			mainCharacter.transform.eulerAngles = new Vector3(0,180,0);
			newPosition = new Vector3(Mathf.Clamp(mainCharacter.transform.position.x - 50f, -1750f, 1750f), mainCharacter.transform.position.y, 0f);
			if(newPosition.x < 1150f)
				newCameraPosition = new Vector3(Mathf.Clamp(mainCamera.transform.position.x - 50f, -1200f, 1200f), mainCamera.transform.position.y, mainCamera.transform.position.z);
			else
				newCameraPosition = mainCamera.transform.position;
		}else if (Input.mousePosition.x > (Screen.width / 2)){
			mainCharacter.transform.eulerAngles = new Vector3(0,0,0);
			newPosition = new Vector3(Mathf.Clamp(mainCharacter.transform.position.x + 50f, -1750f, 1750f), mainCharacter.transform.position.y, 0f);
			if(newPosition.x > -1150f)
				newCameraPosition = new Vector3(Mathf.Clamp(mainCamera.transform.position.x + 50f, -1200f, 1200f), mainCamera.transform.position.y, mainCamera.transform.position.z);
			else
				newCameraPosition = mainCamera.transform.position;
		}
		mainCharacter.transform.position = Vector3.Lerp(mainCharacter.transform.position, newPosition, speed * Time.deltaTime);
		mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newCameraPosition, speed * Time.deltaTime);
	}

	// Set the game info of the plots.
	public void setGameInfo( GameObject game, Sprite header, string title, string npcPlots, bool isCorrectPlot ){
		TalkWindow.SetActive(true);
		TalkWindow.transform.GetChild(0).GetComponentInChildren<Image>().sprite = header;
		TalkWindow.transform.GetChild(1).GetComponentInChildren<Text>().text = title;
		plots = npcPlots.Split('#').ToList();
		gamePanel = game;
		isProgressIncrease = isCorrectPlot;
		page = 0;
		nextPagePlots();
	}
	
	public void nextPagePlots(){
		if(page == plots.Count){
			TalkWindow.SetActive(false);
			if(isProgressIncrease){
				if(gamePanel == null)
					checkStageProgress();	
				else
					enterPanel.SetActive(true);
			}else{
				// print("Wrong plots");
			}
		}else{
			TalkWindow.transform.GetChild(2).GetComponentInChildren<Text>().text = plots[page];
			page++;
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
			checkStageProgress();
		}else{
			StartCoroutine(Feedback(wrongPanel));
			GameObject.Find("Feedbacks").transform.GetChild(1).GetChild(2).GetComponent<Text>().text = prompts;
			// ... set wrong panel hints.
			userLife--;
			string life = "Life" + userLife.ToString();
			if(userLife == 0){
				GameObject.Find("player life").transform.GetChild(1).gameObject.SetActive(false); //GetComponent<Image>().sprite = null;
				GameObject.Find("player life").transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("gg") as Sprite;
			}else
				GameObject.Find("player life").transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(life) as Sprite;
			
		}
	}

	public IEnumerator Feedback( GameObject imageFeedBack ){
		imageFeedBack.SetActive(true);
		yield return new WaitForSeconds(2f);
		imageFeedBack.SetActive(false);
		if(userLife > 0){
			isGameStart = false;
			NPCs.SetActive(true);
			mainCharacter.SetActive(true);
		}
		else{
			// if(gameMode != "PICK")
				GameObject.Find("EventSystem").GetComponent<DynamicAssessment>().teachNum(-1);
		}
	}

	// Increase the stage progress and check if the stage is finish.
	public void checkStageProgress(){
		userProgress += 1;
		// print(userProgress);
		// show prompts.
		StartCoroutine(showPlots());
		if(userProgress == dataControl.stageGoal){
			// show stage finish animation.
			// print("stage finish.");
			dataControl.progress += 1;
			SceneManager.LoadScene("Chapter"+dataControl.chapter.ToString());
		}
	}

	private IEnumerator showPlots(){
		// prompts > userProgress --;
		print(userProgress);
		if(prompts[userProgress-1].pictures.Count == 0){
			print("nothing");
		}else{
			plotsImage.SetActive(true);
			for(int i = 0; i < prompts[userProgress-1].pictures.Count; i++){
				plotsImage.transform.GetChild(0).GetComponent<Image>().sprite = prompts[userProgress-1].pictures[i];
				plotsImage.transform.GetChild(1).GetComponent<Text>().text = prompts[userProgress-1].words[i];
				yield return new WaitForSeconds(5f);
			}
			plotsImage.SetActive(false);
		}
	}

}
