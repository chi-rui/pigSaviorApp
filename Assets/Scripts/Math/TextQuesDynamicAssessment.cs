using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextQuesDynamicAssessment : MonoBehaviour {
	public GameObject teachingPanel;

	public List<QuesObj> quesList = new List<QuesObj>();
	public List<string> quesKeyword = new List<string>();

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

	public void setContents (QuesObj question, List<string> keyword) {
		quesList.Add(question);
		quesKeyword = new List<string>(keyword);
		// for (int i = 0; i < keyword.Count; i++)
		// 	print(keyword[i]);
	}

	public void teachNum (int index) {
		teachingPanel.SetActive(true);
		if (index == -1) {
			// selectionQues.SetActive(true);
			string question = "";
			for(int i = 0; i < 3; i++ ){
				for(int j = 0; j < quesList[i].question.Count; j++)
					question += quesList[i].question[j];
			}
			print(question);
			// selectionQues.SetActive(false);

			
		}
	}
}
