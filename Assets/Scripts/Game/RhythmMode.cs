﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// only onion npc problem
public class RhythmMode : MonoBehaviour {
	public GameObject pointer, characterAction, challengeFailedPanel, chooseOperatorPanel, calculatePanel, hitResult, remainingText, hitResultText, clickAnyPositionImage;
	public GameObject[] quesNumTextArr, quesOperTextArr, quesNumTextChooseArr, quesOperBtnChooseArr;
	public Animator Anim_characterPerfect, Anim_npcPerfect;
	public Sprite Sprite_characterAction; //if need?
	public Image[] hitbarArr;
	public Text Text_remainCounts, Text_userans, Text_partQues;
	public float speed;
	public string characterPerfectAnimStr, npcPerfectAnimStr;
	
	private Vector3 pos_L, pos_R;
	private int remainCounts, rankTimes, operCount, operChooseBtnIndex, userCalculateCount;
	private bool isRhythmStart, isPerfectHit, isChallengeFailed, isSpecialCalculate;
	private List<int> tmpSortingList = new List<int>();
	private List<int> hitBarsIndexList = new List<int>();
	private List<string> quesOperList = new List<string>();
	private List<int> userAnsList = new List<int>();
	
	// setting question
	public int minNum, maxNum;
	public List<string> quesTemplate;
	private MathDatasControl MathDatas;
	private QuesObj quesObj;

// test
	private string testQues;

	// Use this for initialization
	void Start () {
		pos_L = new Vector3(-1860f, pointer.transform.position.y, 0);
		pos_R = new Vector3(-530f, pointer.transform.position.y, 0);
		speed = 0.5f;
		remainCounts = 10;
		isChallengeFailed = false;
		isPerfectHit = false;
		isRhythmStart = true;
		rankTimes = 1;
		generateHitBars(rankTimes);

		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		generateQuestion(minNum, maxNum, quesTemplate);
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

	void generateQuestion (int min, int max, List<string> template) {
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

		// delete operators and brackets in question
		for (int i = 0; i < quesObj.question.Count; i++) {
			if (quesObj.question[i] == "+" || quesObj.question[i] == "-" || quesObj.question[i] == "x" || quesObj.question[i] == "÷") {
				quesOperList.Add(quesObj.question[i]);
				quesObj.question.Remove(quesObj.question[i]);
			}
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

	void generateHitBars (int times) {
		switch (times) {
			case 1:
				ranNum(3);
				break;
			case 2:
				ranNum(2);
				break;
			case 3:
				ranNum(1);
				break;
			default:
				break;
		}
	}

	void ranNum (int counts) {
		for (int i = 0; i < 7; i++){
			tmpSortingList.Add(i);
			// print(tmpSortingList[i]);
		}
		// print(hitBarsIndexList.Count + " " + tmpSortingList.Count);
		int tmpListCount = tmpSortingList.Count;
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
		}
		else {
			hitResultText.GetComponent<Text>().text = "Miss";
			characterAction.GetComponent<Animator>().enabled = false;
			characterAction.GetComponent<Image>().sprite = Sprite_characterAction;
		}

		yield return new WaitForSeconds(time);

		// set remain counts text
		remainCounts--;
		Text_remainCounts.text = remainCounts.ToString();

		if (isPerfectHit)
			clickPerfectHit();
		else {
			isRhythmStart = true;
			// show remain counts text
			remainingText.SetActive(true);
			hitResultText.SetActive(false);
		}

		if (remainCounts == 0) {
			isRhythmStart = false;
			isChallengeFailed = true;
			challengeFailedPanel.SetActive(true);
			StartCoroutine(showClickAnyPosition(0.6f));
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
			// print("isPerfectHit: " + isPerfectHit);
		} else if (collider.gameObject.tag == "rhythmbar") {
			isPerfectHit = false;
			// print("isPerfectHit: " + isPerfectHit);
		} else {
			isPerfectHit = false;
			// print("Error for collision");
		}
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
		generateHitBars(rankTimes);
	}

	// choose operator panel
	public void clickOperBtn (int num) {
		operChooseBtnIndex = num;
		// print(operChooseBtnIndex);
		calculatePanel.SetActive(true);
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
		Text_partQues.text = removeQuesBracket(tmp1) + quesOperList[operChooseBtnIndex] + removeQuesBracket(tmp2);
	}

	string removeQuesBracket (string str) {
		string str2 = "";
		if (str.Contains("(")) {
			str2 = str.Replace("(", "");
		} else if (str.Contains(")")) {
			str2 = str.Replace(")", "");
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
	}

	public void clickFinishCalculate () {
		userCalculateCount++;
		
		string tmpAns = "";
		if (Text_userans.text == null || Text_userans.text == "ANS") {
			userAnsList.Add(0);
			tmpAns = "0";
		} else {
			userAnsList.Add(int.Parse(Text_userans.text));
			tmpAns = int.Parse(Text_userans.text).ToString();
		}
		for (int i = 0; i < userAnsList.Count; i++)
			print(userAnsList[i]);

		if (userCalculateCount < operCount) {
			quesOperBtnChooseArr[operChooseBtnIndex].SetActive(false);
			if (operChooseBtnIndex == 2) {
				quesNumTextChooseArr[operChooseBtnIndex+1].GetComponent<Text>().text = null;
				quesNumTextChooseArr[operChooseBtnIndex].GetComponent<Text>().text = tmpAns;
			} else {
				if (isSpecialCalculate)
					quesNumTextChooseArr[operChooseBtnIndex+2].GetComponent<Text>().text = null;
				quesNumTextChooseArr[operChooseBtnIndex].GetComponent<Text>().text = null;
				quesNumTextChooseArr[operChooseBtnIndex+1].GetComponent<Text>().text = tmpAns;
			}
			clickClearAnsNum();
			Text_userans.text = "ANS";
			calculatePanel.SetActive(false);
		} else {
			print("計算完成！");
			checkUserAnswer();
		}
	}

	public void checkUserAnswer () {
		// for (int i = 0; i < quesObj.answer.Count; i++)
		// 	print(quesObj.answer[i].partAns);
		if (userAnsList[userAnsList.Count-1] == quesObj.answer[quesObj.answer.Count-1].partAns)
			print("答案正確");
		else
			print("答案錯誤");
	}
}