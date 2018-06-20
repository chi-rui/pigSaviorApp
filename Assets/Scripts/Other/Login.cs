﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {
	public GameObject warningPanel, mainPagePanel;
	public Text Text_account, Text_password;
	public static int user_id, user_progress;
	public static string account;

	public DatasControl datas;
	private string password;

	// Use this for initialization
	void Start () {
		warningPanel.SetActive(false);
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
				user_id = int.Parse(userData[0]);
				user_progress = int.Parse(userData[1]);
				print("user_id: " + user_id + " / " + "user_progress: " + user_progress);
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
	}
}
