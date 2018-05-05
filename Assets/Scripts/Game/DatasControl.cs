using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
	// music...etc

	// Loading Image
	public GameObject loadingPanel;
	public Slider loadingBar;
	private AsyncOperation async;

	// Use this for initialization
	void Start () {
		// get the save of the user.
		// progress = userData.gameProgress;
		chapter = 1;
		progress = 1;
		nowStage = 1;
		setGameObjects();
	}
	
	// Update is called once per frame
	void Update () {
		
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
		print("set loading panel.");
		loadingPanel = GameObject.Find("Panel_loading");
		loadingBar = loadingPanel.transform.GetChild(0).GetComponentInChildren<Slider>();
		loadingPanel.SetActive(false);
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
}
