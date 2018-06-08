using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TextQuesMode : MonoBehaviour {

	// set question
	public int minNum, maxNum, templateIndex;
	public List<string> quesTemplate, quesNumSymbol;
	public string textQuesFilePath;
	private List<string> textQuesList = new List<string>();
	private List<int> quesIndexList = new List<int>();
	private QuesObj quesObj, temp;
	private MathDatasControl MathDatas;

	void OnEnable () {
		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();

		generateNewQuestion(minNum, maxNum, quesTemplate);
		restartTextQuesMode();
	}

	// Use this for initialization
	void Start () {
		showQuestion();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void generateNewQuestion (int min, int max, List<string> template) {
		// generate question
		quesObj = MathDatas.getQuestion(min, max, template[Random.Range(0, template.Count)]);
		temp = new QuesObj();
		temp.question = new List<string>(quesObj.question);
		temp.answer = new List<AnsObj>(quesObj.answer);

		// test
		string testQues = "";
		for (int i = 0; i < temp.question.Count; i++)
			testQues += temp.question[i];
		print(testQues);

		// read text question file and set question in a list
		StreamReader textQuesFile = new StreamReader(textQuesFilePath);
		string[] textQuesArr = textQuesFile.ReadToEnd().Split('@');
		string[] templateTextQuesArr = textQuesArr[templateIndex].Split('\n');
		for (int i = 0; i < templateTextQuesArr.Length; i++)
			textQuesList.Add(templateTextQuesArr[i]);
		// for (int i = 0; i < textQuesArr.Length; i++)
		// 	print(textQuesArr[i]);
		// for (int i = 0; i < textQuesList.Count; i++)
		// 	print(textQuesList[i]);
		textQuesFile.Close();
	}

	void showQuestion () {
		// random question index
		List<int> tmpQuesIndexList = new List<int> {0, 1, 2};
		int tmpQuesIndexCount = tmpQuesIndexList.Count;
		while (quesIndexList.Count < tmpQuesIndexCount) {
			int index = Random.Range(0, tmpQuesIndexList.Count);
			if (!quesIndexList.Contains(tmpQuesIndexList[index])) {
				quesIndexList.Add(tmpQuesIndexList[index]);
				tmpQuesIndexList.Remove(tmpQuesIndexList[index]);
			}
		}
		for (int i = 0; i < quesIndexList.Count; i++)
			print(quesIndexList[i]);

		// replace template number and show question
		List<string> tmpQuesNumList = new List<string>();
		for (int i = 0; i < temp.question.Count; i++) {
			if (temp.question[i] != "+" && temp.question[i] != "-" && temp.question[i] != "x" && temp.question[i] != "÷" && temp.question[i] != "(" && temp.question[i] != ")")
				tmpQuesNumList.Add(temp.question[i]);
		}
		// for (int i = 0; i < tmpQuesNumList.Count; i++)
		// 	print(tmpQuesNumList[i]);
		for (int i = 0; i < tmpQuesNumList.Count; i++)
			textQuesList[quesIndexList[0]] = textQuesList[quesIndexList[0]].Replace(quesNumSymbol[i], tmpQuesNumList[i]);
		GameObject.Find("Text_text question").GetComponent<Text>().text = textQuesList[quesIndexList[0]];
	}

	void restartTextQuesMode () {
		if (quesIndexList.Count != 0)
			quesIndexList.Remove(quesIndexList[0]);
	}

}
