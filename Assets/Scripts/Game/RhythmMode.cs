using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// only onion npc problem
public class RhythmMode : MonoBehaviour {
	public GameObject pointer, characterAction, challengeFailedPanel, chooseOperatorPanel, calculatePanel, hitResult, remainingText, hitResultText, clickAnyPositionImage;
	public GameObject[] quesNumTextArr, quesOperTextArr, quesNumTextChooseArr, quesOperBtnChooseArr;
	public Animator Anim_characterPerfect, Anim_npcPerfect;
	public Sprite Sprite_characterAction;
	public Image[] hitbarArr;
	public Text Text_remainCounts, Text_userans, Text_partQues;
	public float speed;
	public string characterPerfectAnimStr, npcPerfectAnimStr;
	
	private Vector3 pos_L, pos_R;
	private int remainCounts, hitbarCounts, operCount, operChooseBtnIndex, userCalculateCount, numA, numB;
	private bool isRhythmStart, isPerfectHit, isChallengeFailed, isSpecialCalculate, isInBracketA, isInBracketB;
	private List<int> tmpSortingList = new List<int>();
	private List<int> hitBarsIndexList = new List<int>();
	private List<string> quesOperList = new List<string>();
	private List<int> quesOperIndexList = new List<int>();
	
	// set question
	public int minNum, maxNum;
	public List<string> quesTemplate;
	private MathDatasControl MathDatas;
	private MisIdentify MisIdent;
	private QuesObj quesObj;

	// set user answer
	private List<AnsObj> userAnsList = new List<AnsObj>();

// test
	private string testQues;

	// Use this for initialization
	void Start () {
		pos_L = new Vector3(-1860f, pointer.transform.position.y, 0);
		pos_R = new Vector3(-530f, pointer.transform.position.y, 0);
		speed = 0.5f;
		remainCounts = 10;
		// isInBracket = true;
		hitbarCounts = 3;
		generateHitBars(hitbarCounts);

		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		MisIdent = GameObject.Find("EventSystem").GetComponent<MisIdentify>();
		generateNewQuestion(minNum, maxNum, quesTemplate);
	}
	
	// Update is called once per frame
	// -530 -1860
	// Mathf.PingPong(speed * Time.time, 1.0f)
	void Update () {
		if (isRhythmStart) {
			pointer.transform.position = Vector3.Lerp(pos_L, pos_R, Mathf.PingPong(speed * Time.time, 1.0f));

			// set roles' action
			characterAction.transform.position = new Vector3(characterAction.transform.position.x, 60, characterAction.transform.position.z);
			characterAction.GetComponent<Animator>().enabled = true;

			if (Input.GetMouseButtonDown(0)) {
				if (isPerfectHit)
					StartCoroutine(clickInRhythm(1.3f));
				else
					StartCoroutine(clickInRhythm(0.8f));
			}
		}

		if (isChallengeFailed) {
			if (Input.GetMouseButtonDown(0)) {
				restartRhythmMode();
				clickAnyPositionImage.SetActive(false);
				isChallengeFailed = false;
			}
		}
	}

	void generateNewQuestion (int min, int max, List<string> template) {
		// generate question and get operator counts
		quesObj = MathDatas.getQuestion(min, max, template[Random.Range(0, template.Count)]);
		for (int i = 0; i < quesObj.question.Count; i++)
			testQues += quesObj.question[i];
		print(testQues);
		operCount = quesObj.answer.Count;
		// print(operCount);

		// set question numbers and operators position
		switch (operCount) {
			case 2:
				for (int i = 0; i <= operCount; i++) {
					quesNumTextArr[i].transform.position += new Vector3(100f, 0, 0);
					quesOperTextArr[i].transform.position += new Vector3(100f, 0, 0);
					quesNumTextChooseArr[i].transform.position += new Vector3(150f, 0, 0);
					quesOperBtnChooseArr[i].transform.position += new Vector3(150f, 0, 0);
				}
				break;
			case 1:
				for (int i = 0; i <= operCount; i++) {
					quesNumTextArr[i].transform.position += new Vector3(200f, 0, 0);
					quesOperTextArr[i].transform.position += new Vector3(200f, 0, 0);
					quesNumTextChooseArr[i].transform.position += new Vector3(300f, 0, 0);
					quesOperBtnChooseArr[i].transform.position += new Vector3(300f, 0, 0);
				}
				break;
		}

		// store operators index and operators in each list
		for (int i = 0; i < quesObj.question.Count; i++) {
			if (quesObj.question[i] == "+" || quesObj.question[i] == "-" || quesObj.question[i] == "x" || quesObj.question[i] == "÷") {
				quesOperIndexList.Add(i);
				quesOperList.Add(quesObj.question[i]);
			}
		}

		// delete operators and brackets in question
		for (int i = 0; i < quesObj.question.Count; i++) {
			if (quesObj.question[i] == "+" || quesObj.question[i] == "-" || quesObj.question[i] == "x" || quesObj.question[i] == "÷")
				quesObj.question.Remove(quesObj.question[i]);
			if (quesObj.question[i] == "(") {
				quesObj.question[i+1] = quesObj.question[i] + quesObj.question[i+1];
			} else if (quesObj.question[i] == ")") {
				quesObj.question[i-1] = quesObj.question[i-1] + quesObj.question[i];
			}
		}
		for (int i = 0; i < quesObj.question.Count; i++) {
			if (quesObj.question[i] == "(")
				quesObj.question.Remove(quesObj.question[i]);
		}
		for (int i = 0; i < quesObj.question.Count; i++) {
			if (quesObj.question[i] == ")")
				quesObj.question.Remove(quesObj.question[i]);
		}

		// print(quesObj.question.Count);
		// for (int i = 0; i < quesObj.question.Count; i++)
		// 	print(quesObj.question[i]);
		// for (int i = 0; i < quesOperList.Count; i++)
		// 	print(quesOperList[i]);
		// for (int i = 0; i < quesOperIndexList.Count; i++)
		// 	print(quesOperIndexList[i]);

		showQuestion();
	}

	void showQuestion () {
		// show question number text
		for (int i = 0; i < quesObj.question.Count; i++) {
			quesNumTextArr[i].GetComponent<Text>().text = quesObj.question[i];
			quesNumTextChooseArr[i].GetComponent<Text>().text = quesObj.question[i];
		}

		// set operator symbol
		for (int i = 0; i < operCount; i++) {
			quesOperTextArr[i].GetComponent<Text>().text = quesOperList[i];
			switch (quesOperList[i]) {
				case "+":
					quesOperBtnChooseArr[i].GetComponent<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("plusButton") as Sprite;
					break;
				case "-":
					quesOperBtnChooseArr[i].GetComponent<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("minusButton") as Sprite;
					break;
				case "x":
					quesOperBtnChooseArr[i].GetComponent<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("multipledButton") as Sprite;
					break;
				case "÷":
					quesOperBtnChooseArr[i].GetComponent<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("dividedButton") as Sprite;
					break;
			}
		}
	}

	void generateHitBars (int counts) {
		isRhythmStart = true;
		// clear random hitbar number list and change to original hitbar
		tmpSortingList.Clear();
		hitBarsIndexList.Clear();
		for (int i = 0; i < hitbarArr.Length; i++) {
			hitbarArr[i].color = new Color32(255, 255, 255, 0);
			hitbarArr[i].gameObject.tag = "rhythmbar";
		}
		// generate random hitbar
		for (int i = 0; i < 7; i++)
			tmpSortingList.Add(i);
		int tmpListCount = tmpSortingList.Count;
		// print("tmpListCount:" + tmpListCount);
		while (hitBarsIndexList.Count < tmpListCount) {
			int index = Random.Range(0, tmpSortingList.Count);
			if (!hitBarsIndexList.Contains(tmpSortingList[index])) {
				hitBarsIndexList.Add(tmpSortingList[index]);
				tmpSortingList.Remove(tmpSortingList[index]);
			}
		}
		// print(hitBarsIndexList.Count + " " + tmpSortingList.Count);
		for (int i = 0; i < counts; i++) {
			// print(hitBarsIndexList[i]);
			hitbarArr[hitBarsIndexList[i]].color = new Color32(50, 120, 255, 255);
			hitbarArr[hitBarsIndexList[i]].gameObject.tag = "hitbar";
		}
	}

	IEnumerator clickInRhythm (float time) {
		isRhythmStart = false;
		
		// set roles' action
		characterAction.transform.position = new Vector3(characterAction.transform.position.x, 40, characterAction.transform.position.z);

		// show hint result and Perfect hit scenario
		remainingText.SetActive(false);
		hitResultText.SetActive(true);

		if (isPerfectHit) {
			hitResultText.GetComponent<Text>().text = "Perfect";
			hitResult.SetActive(true);
			Anim_characterPerfect.Play(characterPerfectAnimStr);
			Anim_npcPerfect.Play(npcPerfectAnimStr);
		} else {
			hitResultText.GetComponent<Text>().text = "Miss";
			characterAction.GetComponent<Animator>().enabled = false;
			characterAction.GetComponent<Image>().sprite = Sprite_characterAction;
		}

		yield return new WaitForSeconds(time);		

		if (isPerfectHit)
			clickPerfectHit();
		else {
			remainCounts--;
			isRhythmStart = true;
			// show remain counts text
			remainingText.SetActive(true);
			hitResultText.SetActive(false);
		}
		Text_remainCounts.text = remainCounts.ToString();
		if (remainCounts == 0) {
			isRhythmStart = false;
			isChallengeFailed = true;
			challengeFailedPanel.SetActive(true);
			StartCoroutine(showClickAnyPosition(0.3f));
		}
	}

	void clickPerfectHit () {
		print("isPerfectHit: " + isPerfectHit);

		hitResult.SetActive(false);

		// show choose operator panel
		chooseOperatorPanel.SetActive(true);
	}

	// Collision
	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == "hitbar") {
			isPerfectHit = true;
		} else if (collider.gameObject.tag == "rhythmbar") {
			isPerfectHit = false;
		} else {
			isPerfectHit = false;
		}
		// print("isPerfectHit: " + isPerfectHit);
	}

	IEnumerator showClickAnyPosition (float time) {
		yield return new WaitForSeconds(time);

		clickAnyPositionImage.SetActive(true);
	}

	void restartRhythmMode () {
		challengeFailedPanel.SetActive(false);
		remainCounts = 10;
		Text_remainCounts.text = remainCounts.ToString();
		isPerfectHit = false;
		isRhythmStart = true;
	}

	// choose operator panel
	public void clickOperBtn (int num) {
		operChooseBtnIndex = num;
		// print(operChooseBtnIndex);
		// print(quesOperIndexList[operChooseBtnIndex]);
		calculatePanel.SetActive(true);
		isInBracketA = false; isInBracketB = false;
		string tmp1 = "", tmp2 = "";
		if (quesNumTextChooseArr[operChooseBtnIndex].GetComponent<Text>().text == "") {
			tmp1 = quesNumTextChooseArr[operChooseBtnIndex-1].GetComponent<Text>().text;
			tmp2 = quesNumTextChooseArr[operChooseBtnIndex+1].GetComponent<Text>().text;
		} else if (quesNumTextChooseArr[operChooseBtnIndex+1].GetComponent<Text>().text == "") {
			tmp1 = quesNumTextChooseArr[operChooseBtnIndex].GetComponent<Text>().text;
			tmp2 = quesNumTextChooseArr[operChooseBtnIndex+2].GetComponent<Text>().text;
			isSpecialCalculate = true;
		} else {
			tmp1 = quesNumTextChooseArr[operChooseBtnIndex].GetComponent<Text>().text;
			tmp2 = quesNumTextChooseArr[operChooseBtnIndex+1].GetComponent<Text>().text;
		}
		// print(tmp1 + " " + tmp2);
		Text_partQues.text = removeLeftNumBracket(tmp1) + quesOperList[operChooseBtnIndex] + removeRightNumBracket(tmp2);
		numA = int.Parse(removeLeftNumBracket(tmp1));
		numB = int.Parse(removeRightNumBracket(tmp2));
		// print(isInBracketA + " " + isInBracketB);
	}

	string removeLeftNumBracket (string str) {
		string str2;
		if (str.Contains("(")) {
			str2 = str.Replace("(", "");
			isInBracketA = true;
		} else if (str.Contains(")")) {
			str2 = str.Replace(")", "");
		} else {
			str2 = str;
		}
		return str2;
	}

	string removeRightNumBracket (string str) {
		string str2;
		if (str.Contains("(")) {
			str2 = str.Replace("(", "");
		} else if (str.Contains(")")) {
			str2 = str.Replace(")", "");
			isInBracketB = true;
		} else {
			str2 = str;
		}
		return str2;
	}

	// calculate panel and check answer
	public void clickNumBtn (int num) {
		if (Text_userans.text == "ANS") {
			Text_userans.text = "";
			Text_userans.text += num.ToString();
		} else {
			Text_userans.text += num.ToString();
		}
	}

	public void clickClearAnsNum () {
		Text_userans.text = "";
	}

	public void clickPreviousStep () {
		calculatePanel.SetActive(false);
		Text_userans.text = "ANS";
	}

	public void clickFinishCalculate () {
		userCalculateCount++;
		// print(userCalculateCount);
		hitbarCounts--;
		// print(hitbarCounts);
		string tmpAns = ""; string bracketStr = "";

		// set user answer object and add to user answer list
		AnsObj userAnsObj = new AnsObj();
		userAnsObj.index = quesOperIndexList[operChooseBtnIndex];
		userAnsObj.operators = System.Convert.ToChar(quesOperList[operChooseBtnIndex]);
		if (Text_userans.text == null || Text_userans.text == "ANS") {
			userAnsObj.partAns = 0;
			tmpAns = "0";
		} else {
			userAnsObj.partAns = int.Parse(Text_userans.text);
			tmpAns = int.Parse(Text_userans.text).ToString();
		}
		if (isInBracketA || isInBracketB)
			userAnsObj.isInBracket = true;
		userAnsObj.numA = numA;
		userAnsObj.numB = numB;
		userAnsList.Add(userAnsObj);

		if (userCalculateCount < operCount) {
			quesOperBtnChooseArr[operChooseBtnIndex].SetActive(false);
			quesOperTextArr[operChooseBtnIndex].SetActive(false);
			if (operChooseBtnIndex == 2) {
				quesNumTextChooseArr[operChooseBtnIndex+1].GetComponent<Text>().text = null;
				quesNumTextArr[operChooseBtnIndex+1].GetComponent<Text>().text = null;
				bracketStr = quesNumTextArr[operChooseBtnIndex-1].GetComponent<Text>().text;
				if (bracketStr.Contains("(")) {
					quesNumTextChooseArr[operChooseBtnIndex].GetComponent<Text>().text = tmpAns + ")";
					quesNumTextArr[operChooseBtnIndex].GetComponent<Text>().text = tmpAns + ")";
				} else {
					quesNumTextChooseArr[operChooseBtnIndex].GetComponent<Text>().text = tmpAns;
					quesNumTextArr[operChooseBtnIndex].GetComponent<Text>().text = tmpAns;
				}
			} else {
				quesNumTextChooseArr[operChooseBtnIndex].GetComponent<Text>().text = null;
				quesNumTextArr[operChooseBtnIndex].GetComponent<Text>().text = null;
				bracketStr = quesNumTextArr[operChooseBtnIndex+2].GetComponent<Text>().text;
				if (bracketStr.Contains(")")) {
					quesNumTextChooseArr[operChooseBtnIndex+1].GetComponent<Text>().text = "(" + tmpAns;
					quesNumTextArr[operChooseBtnIndex+1].GetComponent<Text>().text = "(" + tmpAns;
				} else {
					quesNumTextChooseArr[operChooseBtnIndex+1].GetComponent<Text>().text = tmpAns;
					quesNumTextArr[operChooseBtnIndex+1].GetComponent<Text>().text = tmpAns;
				}
				
				if (isSpecialCalculate) {
					quesNumTextChooseArr[operChooseBtnIndex+2].GetComponent<Text>().text = null;
					quesNumTextArr[operChooseBtnIndex+2].GetComponent<Text>().text = null;
				}
			}
			clickClearAnsNum();
			Text_userans.text = "ANS";

			// show remain counts text
			remainingText.SetActive(true);
			hitResultText.SetActive(false);
			generateHitBars(hitbarCounts);
			
			calculatePanel.SetActive(false);
			chooseOperatorPanel.SetActive(false);
		} else {
			print("計算完成！");
			checkUserAnswer();
		}
	}

	public void checkUserAnswer () {
		List<string> misConceptions = new List<string>();
		misConceptions = MisIdent.getMisConception(quesObj.answer, userAnsList);

		for(int i = 0; i < misConceptions.Count; i++){
			print("你有 : " + misConceptions[i]);
		}

		if(misConceptions.Count == 0){
			
		}else{
			
		}
		// print("userAnsList.Count: " + userAnsList.Count);
		// for (int i = 0; i < userAnsList.Count; i++)
		// 	print(userAnsList[i].index + " " + userAnsList[i].operators + " " + userAnsList[i].partAns + " " + userAnsList[i].isInBracket + " " + userAnsList[i].numA + " " + userAnsList[i].numB);
		
		// if (userAnsList[userAnsList.Count-1].partAns == quesObj.answer[quesObj.answer.Count-1].partAns)
		// 	print("答案正確");
		// else
		// 	print("答案錯誤");
	}
}