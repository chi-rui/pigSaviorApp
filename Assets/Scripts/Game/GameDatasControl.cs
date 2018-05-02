using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDatasControl : MonoBehaviour {

	private static GameDatasControl saver;
	private UserDatas userData;

	public Slider loadingBar;
	public GameObject loadingImage;
	private AsyncOperation async;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Make datas always alive.
	void Awake(){
		if(saver){
			DestroyImmediate(gameObject);
			// comeback = true;
		}else{
		 	DontDestroyOnLoad(transform.gameObject);
		 	saver = this;
		}
	}

	public void GameClose(){
		Destroy(this.gameObject);
	}

	public void setUserData( string userAccount, string userPassword ){
		// check database if account is exist.
		// if()
			this.userData.account = userAccount;
			this.userData.passwd = userPassword;
		// else
			// ...

		print(this.userData.account + " / " + this.userData.passwd);
	}

	public void LoadingScene( string scene ){
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
}
