using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {
	public GameObject warningPanel, mainPagePanel, loginPanel;
	public Text Text_account, Text_password;
	public static int user_id, user_progress;
	public static string user_isOpenChallenge, user_challProgress;
	public static string account;

	public DatasControl datas;
	private string password;

	// Use this for initialization
	void Start () {
		warningPanel.SetActive(false);
		datas = GameObject.Find("Datas").GetComponent<DatasControl>();
		datas.setGameObjects();

		if (user_id != 0) {
			loginPanel.SetActive(false);
			mainPagePanel.SetActive(true);
			if (datas.isChallengeModeOpen)
				mainPagePanel.transform.GetChild(4).GetComponent<Button>().interactable = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void clickLogin () {
		account = Text_account.text;
		password = Text_password.text;
		// print(account + " " + password);
		StartCoroutine(checkUserInfo());
	}

	IEnumerator checkUserInfo () {
		string URL = "http://163.21.245.192/PigSaviorAPP/userLogin.php";
		WWWForm form = new WWWForm();
		Dictionary<string, string> data = new Dictionary<string, string>();
		data.Add("account", account);
		data.Add("password", password);
		foreach (KeyValuePair<string, string> post in data) {
			form.AddField(post.Key, post.Value);
		}
		WWW www = new WWW(URL, form);
		yield return www;
		print(www.text);

		warningPanel.SetActive(true);
		if (www.text == "")
			warningPanel.transform.GetChild(1).GetComponent<Text>().text = "登入失敗：請檢查網路連線";
		else {
			if (www.text == "wrong password")		
				warningPanel.transform.GetChild(1).GetComponent<Text>().text = "登入失敗：密碼輸入錯誤";
			else if (www.text == "no register")
				warningPanel.transform.GetChild(1).GetComponent<Text>().text = "登入失敗：此帳號尚未註冊";
			else {
				string[] userData = www.text.Split('@');
				int.TryParse(userData[0], out user_id);
				user_progress = int.Parse(userData[1]);
				user_isOpenChallenge = userData[2];
				user_challProgress = userData[3];
				print("user_id: " + user_id + " / user_progress: " + user_progress + " / user_isOpenChallenge: " + user_isOpenChallenge + " / user_challProgress: " + user_challProgress);
				warningPanel.transform.GetChild(1).GetComponent<Text>().text = "登入成功";
				GameObject.Find("Panel_Login").SetActive(false);
				mainPagePanel.SetActive(true);
				if(user_progress > 5)
					datas.chapter = 2;
				else
					datas.chapter = 1;
				datas.progress = user_progress;
				datas.nowStage = user_progress;
			}
		}
		if(user_isOpenChallenge == "true")
			datas.isChallengeModeOpen = true;
		if (user_progress >= 15)
			mainPagePanel.transform.GetChild(4).GetComponent<Button>().interactable = true;
	}

	public void enterChapter(){
		// SceneManager.LoadScene("Chapter"+chapter.ToString());
		string chapterScene = "Chapter"+datas.chapter.ToString();
		datas.LoadingScene(chapterScene);
	}

	public void enterChallengeMode () {
		datas.LoadingScene("ChallengeMode");
	}
}
