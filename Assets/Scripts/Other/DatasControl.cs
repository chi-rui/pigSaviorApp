using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class DatasControl : MonoBehaviour {

	// User
	private static DatasControl datas;
	// private UserDatas userData;	// account, passwd, gameProgress, achievement, timer.

	// Game Information
	// Chapter infomation saves.
	public int chapter;		// keep the data of the moment user's chapter.
	public int progress;	// keep the game progress of the user. 
	public int nowStage;	// keep the data of the moment user's stage in the chapter.
	public Vector3 characterPosition;	// character's position when come back to the Chapter scene.
	// Stage information saves.
	public int stageGoal = 0;	// keep the data of the max stage plots number. 

	// Game setting
	public static string GameMode;
	// music...etc

	// Loading Image
	public GameObject loadingPanel;
	public Slider loadingBar;
	private AsyncOperation async;

	// data saved
	private bool saverBlock = false;
	private List<string> actionsA = new List<string>();
	private List<string> targetsA = new List<string>();
	private List<string> scenesA = new List<string>();
	private List<string> actionsB = new List<string>();
	private List<string> targetsB = new List<string>();
	private List<string> scenesB = new List<string>();

	// Use this for initialization
	void Start () {
		// get the save of the user.
		// progress = userData.gameProgress;
		chapter = 1;
		progress = 0;
		nowStage = 0;
		GameMode = "";
		setGameObjects();
	}
	
	// Update is called once per frame
	void Update () {
		// save behavior history.
		if(Input.GetMouseButtonDown(0)){
			if(EventSystem.current.currentSelectedGameObject != null){
				if(saverBlock){
					actionsA.Add("CLICK");
					targetsA.Add(EventSystem.current.currentSelectedGameObject.name);
					scenesA.Add(SceneManager.GetActiveScene().name);
				}else{
					actionsB.Add("CLICK");
					targetsB.Add(EventSystem.current.currentSelectedGameObject.name);
					scenesB.Add(SceneManager.GetActiveScene().name);
				}

				if(actionsA.Count >= 5 || actionsB.Count > 5){
					getBehaviorData();
				}
			}
		}
	}

	// Make datas always alive.
	void Awake(){
		if(datas){
			DestroyImmediate(gameObject);
			// comeback = true;
		}else{
		 	DontDestroyOnLoad(transform.gameObject);
		 	datas = this;
		}
	}

	public void setGameObjects(){
		// set loading images. 
		// print("set loading panel.");
		if(GameObject.Find("Panel_loading")){
			loadingPanel = GameObject.Find("Panel_loading");
			loadingBar = loadingPanel.transform.GetChild(0).GetComponentInChildren<Slider>();
			loadingPanel.SetActive(false);
		}else{
			// print("not found");
		}
	}

	private void GameClose(){
		Destroy(this.gameObject);
	}

	// private void setUserData( string userAccount, string userPassword ){
	// 	// check database if account is exist.
	// 	// if()
	// 		this.userData.account = userAccount;
	// 		this.userData.passwd = userPassword;
	// 	// else
	// 		// ...

	// 	print(this.userData.account + " / " + this.userData.passwd);
	// }

	public void LoadingScene( string scene ){
		loadingPanel.SetActive(true);
		StartCoroutine(LoadSceneWithLoadingPage(scene));
	}

	private IEnumerator LoadSceneWithLoadingPage( string scene ){
		float valueBar = 0f;
		async = SceneManager.LoadSceneAsync(scene);
		async.allowSceneActivation = false;
		while(async.progress < 0.9f){
			while(valueBar < async.progress){
				valueBar += 0.01f;
				loadingBar.value = valueBar;
				yield return 0;
			}
		}
		while(valueBar < 1f){
				valueBar += 0.01f;
				loadingBar.value = valueBar;
				yield return 0;
		}
		async.allowSceneActivation = true;
	}

	public void cheat(int value){
		this.progress = value;
		print("Progress already change to " + progress + ".");	
	}

	private void getBehaviorData() {
		/***
		 	id > id(V)
			scene > scenesA, scenesB
			action > actionsA, actionB
			object > targetA, targetB
			time > solved in php
		***/
		string userBehavior = "", scenes = "", actions = "", targets = "";
		saverBlock = !saverBlock;
		if(!saverBlock){
			// upload A block to db.
			for (int i = 0; i < scenesA.Count; i++) {
				scenes += scenesA[i];
				actions += actionsA[i];
				targets += targetsA[i];
				if (i != scenesA.Count-1) {
					scenes += ", ";
					actions += ", ";
					targets += ", ";
				}
			}
			actionsA.Clear();
			targetsA.Clear();
			scenesA.Clear();
		}else{
			// upload B block to db.
			for (int i = 0; i < scenesB.Count; i++) {
				scenes += scenesB[i];
				actions += actionsB[i];
				targets += targetsB[i];
				if (i != scenesB.Count-1) {
					scenes += ", ";
					actions += ", ";
					targets += ", ";
				}
			}
			actionsB.Clear();
			targetsB.Clear();
			scenesB.Clear();
		}
		userBehavior = Login.user_id + "#" + scenes + "#" + actions + "#" + targets;
		StartCoroutine(upload_BEHAVIOR(userBehavior));
	}

	IEnumerator upload_BEHAVIOR (string userBehavior) {
		string URL = "http://163.21.245.192/PigSaviorAPP/userBehaviorUpload.php";
		WWWForm form = new WWWForm();
		Dictionary<string, string> data = new Dictionary<string, string>();
		data.Add("behavior", userBehavior);
		foreach (KeyValuePair<string, string> post in data) {
			form.AddField(post.Key, post.Value);
		}
		WWW www = new WWW(URL, form);
		yield return www;
		print(www.text);
	}

	public void getTextQuesGameData (string question, string correctAns, string userAns, bool isCorrect, string misconception) {
		string gameHistory = Login.user_id + "#" + nowStage + "#" + question + "#" + correctAns + "#" + userAns + "#" + isCorrect + "#" + misconception;
		// print(gameHistory);
		StartCoroutine(upload_HISTORY(gameHistory));
	}

	public void getGameHistoryData (List<string> question, List<AnsObj> correctAns, List<AnsObj> userAns, bool isCorrect, List<string> Mis){
		/***
		 	id > id(V)
			stage > nowStage(V)
			question > question(P)
			correctAns > correctAnswer
			userAns > userAnswer
			isCorrect > isCorrect(P)
			misconception > misconception(P)
			time > php
		***/

		string correctAnswer = "", userAnswer = "", misconception = "", q = "";
		for(int i = 0; i < correctAns.Count; i++){
			correctAnswer += JsonUtility.ToJson(correctAns[i]);
			userAnswer += JsonUtility.ToJson(userAns[i]);
		}
		for(int j = 0; j < Mis.Count; j++){
			misconception += Mis[j];
		}
		for(int k = 0; k < question.Count; k++){
			q += question[k];
		}
		// print("----- save start -----");
		// print(id);
		// print(nowStage);
		// print(q);
		// print(correctAnswer);
		// print(userAnswer);
		// print(isCorrect);
		// print(misconception);
		// print("-----  save end  -----");

		string gameHistory = Login.user_id + "#" + nowStage + "#" + q + "#" + correctAnswer + "#" + userAnswer + "#" + isCorrect + "#" + misconception;
		// print(gameHistory);
		StartCoroutine(upload_HISTORY(gameHistory));
	}

	IEnumerator upload_HISTORY (string gameHistory) {
		string URL = "http://163.21.245.192/PigSaviorAPP/gameHistoryUpload.php";
		WWWForm form = new WWWForm();
		Dictionary<string, string> data = new Dictionary<string, string>();
		data.Add("history", gameHistory);
		foreach (KeyValuePair<string, string> post in data) {
			form.AddField(post.Key, post.Value);
		}
		WWW www = new WWW(URL, form);
		yield return www;
		print(www.text);
	}
}
