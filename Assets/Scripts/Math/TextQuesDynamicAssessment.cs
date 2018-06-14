using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextQuesDynamicAssessment : MonoBehaviour {
	public GameObject teachingPanel, promptQues, teachPage, selectionFunc, selectionQues;

	private bool isTeaching;

	public List<string> quesList = new List<string>();
	public List<string> quesKeyword = new List<string>();
	public List<string> ansList = new List<string>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string getPropmt (int userAnsCount) {
		if (userAnsCount == 1)
			return "不同的運算符號有不同的意義";
		else
			return "加法與乘法會使結果增加\n減法與除法會使結果減少";
	}

	public void setContents (string question, List<string> keyword, string answer) {
		string keywordWrap = "";
		List<string> textKeyword = new List<string>();
		
		for (int i = 0; i < keyword.Count; i++) {
			// print(keyword[i].Substring(0, keyword[i].IndexOf("$")));
			textKeyword.Add(keyword[i].Substring(0, keyword[i].IndexOf("$")));
			keyword[i] = keyword[i].Replace("$", "：");
			keywordWrap += keyword[i];
			if (i != keyword.Count-1)
				keywordWrap += '\n';
		}

		for (int i = 0; i < textKeyword.Count; i++)
			question = question.Replace(textKeyword[i], "<color=red>"+textKeyword[i]+"</color>");
		
		quesList.Add(question);
		quesKeyword.Add(keywordWrap);
		ansList.Add(answer);
	}

	public void teachNum (int index) {
		StartCoroutine(teaching(index));
		if (index == -1) {
			selectionQues.SetActive(true);
			for (int i = 0; i < 3; i++)
				selectionQues.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text = "第"+(i+1)+"題";
			selectionQues.SetActive(false);
		}
	}

	IEnumerator teaching (int index) {
		teachingPanel.SetActive(true);
		if (index == -1) {
			for (int i = 0; i < 3; i++) {
				promptQues.SetActive(true);
				promptQues.transform.GetChild(2).GetComponent<Text>().text = "第 "+(i+1).ToString()+" 題";
				yield return new WaitForSeconds(4f);
				promptQues.SetActive(false);
				teachPage.SetActive(true);
				StartCoroutine(teachingProcess(i, quesList, quesKeyword, ansList));
				while (isTeaching)
					yield return new WaitForSeconds(0.1f);
				teachPage.SetActive(false);
			}
		} else {
			teachPage.SetActive(true);
			StartCoroutine(teachingProcess(index, quesList, quesKeyword, ansList));
			while (isTeaching)
				yield return new WaitForSeconds(0.1f);
			teachPage.SetActive(false);
		}
		selectionFunc.SetActive(true);
	}

	IEnumerator teachingProcess (int quesNo, List<string> question, List<string> keyword, List<string> answer) {
		isTeaching = true;

		teachPage.transform.GetChild(0).GetComponent<Text>().text = question[quesNo];
		teachPage.transform.GetChild(1).GetComponent<Text>().text = keyword[quesNo];
		teachPage.transform.GetChild(2).GetComponent<Text>().text = answer[quesNo];

		yield return new WaitForSeconds(15f);

		isTeaching = false;
	}

	public void finishTeaching (){
		quesList.Clear();
		quesKeyword.Clear();
		ansList.Clear();
		teachingPanel.SetActive(false);
		SceneManager.LoadScene("Chapter"+GameObject.Find("Datas").GetComponent<DatasControl>().chapter.ToString());
	}
}
