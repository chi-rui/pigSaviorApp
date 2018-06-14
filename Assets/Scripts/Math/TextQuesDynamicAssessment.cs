using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextQuesDynamicAssessment : MonoBehaviour {
	public GameObject teachingPanel, promptQues, teachPage;

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
		StartCoroutine(teaching());
		if (index == -1) {
			// selectionQues.SetActive(true);
			// for(int i = 0; i < quesList.Count; i++)
			// 	print(quesList[i]);
			// selectionQues.SetActive(false);

			// for (int i = 0; i < quesKeyword.Count; i++)
			// 	print(quesKeyword[i]);
			// for (int i = 0; i < ansList.Count; i++)
			// 	print(ansList[i]);
		}
	}

	IEnumerator teaching() {
		teachingPanel.SetActive(true);
		for (int i = 0; i < 3; i++) {
			promptQues.SetActive(true);
			promptQues.transform.GetChild(2).GetComponent<Text>().text = "第 "+(i+1).ToString()+" 題";
			yield return new WaitForSeconds(4f);
			promptQues.SetActive(false);
			teachPage.SetActive(true);
			StartCoroutine(teachingProcess(quesList, quesKeyword, ansList));
			while (isTeaching)
				yield return new WaitForSeconds(0.1f);
		}
	}

	IEnumerator teachingProcess(List<string> question, List<string> keyword, List<string> answer) {
		isTeaching = true;
		yield return new WaitForSeconds(2f);

		for(int i = 0; i < question.Count; i++)
			print(question[i]);
	}
}
