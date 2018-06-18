using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TextQuesMode : MonoBehaviour {
	public GameObject[] elementBtnArr;
	public string npc;

	private int userAnswerCount;

	// set question
	public int minNum, maxNum, templateIndex;
	public List<string> quesTemplate, quesNumSymbol;
	public string textQuesFilePath;
	private List<string> textQuesList = new List<string>();
	private List<int> quesIndexList = new List<int>();
	private List<string> quesKeywordList = new List<string>();
	private string quesAnsFormula;
	private QuesObj quesObj, temp;
	private MathDatasControl MathDatas;

	// set text question dynamic assessment
	private StageEvents stageEvents;
	private TextQuesDynamicAssessment textQuesDynamicAssessment;

	void OnEnable () {
		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		stageEvents = GameObject.Find("EventSystem").GetComponent<StageEvents>();
		textQuesDynamicAssessment = GameObject.Find("EventSystem").GetComponent<TextQuesDynamicAssessment>();

		restartTextQuesMode();
		generateNewQuestion(minNum, maxNum, quesTemplate);
	}

	// Use this for initialization
	void Start () {
		DatasControl.GameMode = "TEXTQUES";
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

		quesAnsFormula = "";
		for (int i = 0; i < temp.question.Count; i++)
			quesAnsFormula += temp.question[i];
		print(quesAnsFormula);

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

		showQuestion();
	}

	void showQuestion () {
		// random question index
		if (userAnswerCount == 0) {
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
		}

		// add template number in a list
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

		// store text question keywords
		string[] keywordArr = textQuesList[quesIndexList[0]].Split('#');
		for (int i = 0; i < keywordArr.Length; i++) {
			// print(keywordArr[i]);
			if (i != 0)
				quesKeywordList.Add(keywordArr[i]);
		}
		// for (int i = 0; i < quesKeywordList.Count; i++)
		// 	print(quesKeywordList[i]);

		for (int i = 0; i < tmpQuesNumList.Count; i++)
			keywordArr[0] = keywordArr[0].Replace(quesNumSymbol[i], tmpQuesNumList[i]);
		textQuesList[quesIndexList[0]] = keywordArr[0];
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
		elementBtnArr[index].SetActive(false);
	}

	public void clickClearFormula () {
		GameObject.Find("Text_user formula").GetComponent<Text>().text = null;
		for (int i = 0; i < 12; i++)
			elementBtnArr[i].SetActive(true);
	}

	void restartTextQuesMode () {
		if (userAnswerCount > 3)
			userAnswerCount = 0;
		quesKeywordList.Clear();
		GameObject.Find("Text_user formula").GetComponent<Text>().text = null;
		for (int i = 0; i < 12; i++)
			elementBtnArr[i].SetActive(true);
	}

	public void clickFinish () {
		StartCoroutine(checkAnswer());
	}

	public IEnumerator checkAnswer () {
		userAnswerCount++;
		print("userAnswerCount: " + userAnswerCount);

		string userAns = "", userAnsFormula = "", misConceptions = "";
		userAnsFormula = GameObject.Find("Text_user formula").GetComponent<Text>().text;
		userAns = userAnsFormula.Replace("x", "*").Replace("÷", "/");
		print("userAns: " + evaluateAns(userAns) + " / trueAns: " + quesObj.answer[quesObj.answer.Count-1].partAns);

		if (evaluateAns(userAns) == quesObj.answer[quesObj.answer.Count-1].partAns) {
			stageEvents.showFeedBack(true, "");
			GameObject.Find("Datas").GetComponent<DatasControl>().getTextQuesGameData(textQuesList[quesIndexList[0]], quesAnsFormula, userAnsFormula, true, misConceptions);
		} else {
			misConceptions = textQuesDynamicAssessment.getPropmt(userAnswerCount);
			textQuesDynamicAssessment.setContents(textQuesList[quesIndexList[0]], new List<string>(quesKeywordList), quesAnsFormula);
			stageEvents.showFeedBack(false, misConceptions);
			GameObject.Find("Datas").GetComponent<DatasControl>().getTextQuesGameData(textQuesList[quesIndexList[0]], quesAnsFormula, userAnsFormula, false, misConceptions);
		}

		if (quesIndexList.Count != 0)
			quesIndexList.Remove(quesIndexList[0]);

		yield return new WaitForSeconds(2f);
		GameObject.Find("Panel_" + npc + " TextQues").SetActive(false);
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
