using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TextQuesMode : MonoBehaviour {
	public string npc;

	// set question
	public int minNum, maxNum, templateIndex;
	public List<string> quesTemplate, quesNumSymbol, quesKeywords;
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
		// for (int i = 0; i < quesIndexList.Count; i++)
		// 	print(quesIndexList[i]);

		// replace template number
		List<string> tmpQuesNumList = new List<string>();
		List<string> quesElementsList = new List<string>();
		for (int i = 0; i < temp.question.Count; i++) {
			quesElementsList.Add(temp.question[i]);
			if (temp.question[i] != "+" && temp.question[i] != "-" && temp.question[i] != "x" && temp.question[i] != "÷" && temp.question[i] != "(" && temp.question[i] != ")")
				tmpQuesNumList.Add(temp.question[i]);
		}
		// for (int i = 0; i < tmpQuesNumList.Count; i++)
		// 	print(tmpQuesNumList[i]);
		// for (int i = 0; i < quesElementsList.Count; i++)
		// 	print(quesElementsList[i]);
		for (int i = 0; i < tmpQuesNumList.Count; i++)
			textQuesList[quesIndexList[0]] = textQuesList[quesIndexList[0]].Replace(quesNumSymbol[i], tmpQuesNumList[i]);
		GameObject.Find("Text_text question").GetComponent<Text>().text = textQuesList[quesIndexList[0]];

		// random question elements
		List<int> elementIndexList = new List<int>();
		List<int> tmpElementIndexList = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};
		int tmpElementIndexCount = tmpElementIndexList.Count;
		while (elementIndexList.Count < tmpElementIndexCount) {
			int index = Random.Range(0, tmpElementIndexList.Count);
			if (!elementIndexList.Contains(tmpElementIndexList[index])) {
				elementIndexList.Add(tmpElementIndexList[index]);
				tmpElementIndexList.Remove(tmpElementIndexList[index]);
			}
		}
		// for (int i = 0; i < elementIndexList.Count; i++)
		// 	print(elementIndexList[i]);
		// show elements on btns
		for (int i = 0; i < quesElementsList.Count; i++)
			GameObject.Find("Text_element "+elementIndexList[i]).GetComponent<Text>().text = quesElementsList[i];

		// random remaining no elements' operator or symbol
		List<string> elementNoNumList = new List<string>();
		List<string> tmpElementNoNumList = new List<string> {"+", "-", "x", "÷", "(", ")"};
		int tmpElementNoNumCount = elementIndexList.Count-quesElementsList.Count;
		while (elementNoNumList.Count < tmpElementNoNumCount) {
			int index = Random.Range(0, tmpElementNoNumList.Count);
			elementNoNumList.Add(tmpElementNoNumList[index]);
		}
		// show remaining no text elements on btns
		for (int i = 0; i < elementNoNumList.Count; i++) {
			// print(elementNoNumList[i]);
			GameObject.Find("Text_element "+elementIndexList[i+quesElementsList.Count]).GetComponent<Text>().text = elementNoNumList[i];
		}
	}

	public void clickElementBtn (int index) {
		GameObject.Find("Text_user formula").GetComponent<Text>().text += GameObject.Find("Text_element "+index).GetComponent<Text>().text;
	}

	public void clickClearFormula () {
		GameObject.Find("Text_user formula").GetComponent<Text>().text = null;
	}

	void restartTextQuesMode () {
		if (quesIndexList.Count != 0)
			quesIndexList.Remove(quesIndexList[0]);
	}

	public void checkAnswer () {
		string userAnsFormula = "";
		userAnsFormula = GameObject.Find("Text_user formula").GetComponent<Text>().text;
		userAnsFormula = userAnsFormula.Replace("x", "*").Replace("÷", "/");
		print("userAns: " + evaluateAns(userAnsFormula) + " / trueAns: " + quesObj.answer[quesObj.answer.Count-1].partAns);
		if (evaluateAns(userAnsFormula) == quesObj.answer[quesObj.answer.Count-1].partAns)
			print("答案正確");
		else
			print("答案錯誤");
	}

	// compute formula
	double evaluateAns (string expression) {
		try {
			var doc = new System.Xml.XPath.XPathDocument(new System.IO.StringReader("<r/>"));
			var nav = doc.CreateNavigator();
			var newString = expression;
			newString = (new System.Text.RegularExpressions.Regex(@"([\+\-\*])")).Replace(newString, " ${1} ");
			newString = newString.Replace("/", " div ").Replace("%", " mod ");
			return (double)nav.Evaluate("number(" + newString + ")");
		} catch {
			return -1;
		}
	}
}
