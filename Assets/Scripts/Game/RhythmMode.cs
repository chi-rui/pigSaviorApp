﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// , IPointerClickHandler
// only onion npc problem
public class RhythmMode : MonoBehaviour {
	public GameObject pointer, characterAction, challengeFailedPanel, chooseOperatorPanel, calculatePanel, hitResult, remainingText, hitResultText, clickAnyPositionImage;
	public GameObject[] quesNumTextArr, quesOperTextArr, quesNumTextChooseArr, quesOperBtnChooseArr;
	public Animator Anim_characterAction, Anim_characterActionPerfect, Anim_onion, Anim_onionPerfect;
	public Sprite Sprite_characterGrab;
	public Sprite[] operBtnsArr;
	public Image Image_characterAction;
	public Image[] hitbarArr;
	public Text Text_remainCounts, Text_hitResult, Text_userans, Text_partQues;
	public Text[] Text_quesNumArr, Text_quesOperArr, Text_quesNumChooseArr;
	public Button[] Button_quesOperChooseArr;
	public float speed;
	
	private Vector3 pos_L, pos_R;
	private int remainCounts, rankTimes, operCount;
	private List<int> tmpSortingList = new List<int>();
	private List<int> hitBarsIndexList = new List<int>();
	private List<int> quesOperList = new List<int>();
	private List<string> partQuesList = new List<string>();
	private bool isRhythmStart, isPerfectHit, isChallengeFailed;
	
	// setting question
	public int maxNum;
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
		generateQuestion(maxNum, quesTemplate);
	}
	
	// Update is called once per frame
	// -530 -1860
	// Mathf.PingPong(speed * Time.time, 1.0f)
	void Update () {
		if (isRhythmStart) {
			pointer.transform.position = Vector3.Lerp(pos_L, pos_R, Mathf.PingPong(speed * Time.time, 1.0f));

			// set roles' action
			characterAction.transform.position = new Vector3(characterAction.transform.position.x, 60, characterAction.transform.position.z);
			Anim_characterAction.enabled = true;

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

	void generateQuestion (int max, List<string> template) {
		// generate question and get operator counts
		quesObj = MathDatas.getQuestion(max, template[UnityEngine.Random.Range(0, template.Count)]);
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

		// store question without brackets in a list
		for (int i = 0; i < quesObj.question.Count; i++) {
			if (quesObj.question[i] != "(" && quesObj.question[i] != ")")
				partQuesList.Add(quesObj.question[i]);
		}
		// separate question without brackets in several parts
		for (int i = 0; i < partQuesList.Count; i++) {
			if (partQuesList[i] == "+" || partQuesList[i] == "-" || partQuesList[i] == "x" || partQuesList[i] == "÷")
				partQuesList.Add(partQuesList[i-1]+partQuesList[i]+partQuesList[i+1]);
		}

		// delete operators and brackets in question
		for (int i = 0; i < quesObj.question.Count; i++) {
			if (quesObj.question[i] == "+" || quesObj.question[i] == "-" || quesObj.question[i] == "x" || quesObj.question[i] == "÷") {
				switch (quesObj.question[i]) {
					case "+":
						quesOperList.Add(0);
						break;
					case "-":
						quesOperList.Add(1);
						break;
					case "x":
						quesOperList.Add(2);
						break;
					case "÷":
						quesOperList.Add(3);
						break;
				}
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

		// for (int i = 0; i < partQuesList.Count; i++) 
		// 	print(partQuesList[i]);
		// for (int i = (partQuesList.Count-operCount); i < partQuesList.Count; i++)
		// 	print(partQuesList[i]);
		// print(quesObj.question.Count);
		// for (int i = 0; i < quesObj.question.Count; i++)
		// 	print(quesObj.question[i]);

		// show question number text
		for (int i = 0; i < quesObj.question.Count; i++) {
			Text_quesNumArr[i].text = quesObj.question[i];
			Text_quesNumChooseArr[i].text = quesObj.question[i];
		}

		// set operator symbol
		for (int i = 0; i < operCount; i++) {
			switch (quesOperList[i]) {
				case 0:
					Text_quesOperArr[i].text = "+";
					break;
				case 1:
					Text_quesOperArr[i].text = "-";
					break;
				case 2:
					Text_quesOperArr[i].text = "x";
					break;
				case 3:
					Text_quesOperArr[i].text = "÷";
					break;
			}
			Button_quesOperChooseArr[i].GetComponent<Image>().sprite = operBtnsArr[quesOperList[i]];
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
		Anim_characterAction.enabled = false;
		Image_characterAction.sprite = Sprite_characterGrab;

		// show hint result and Perfect hit scenario
		remainingText.SetActive(false);
		hitResultText.SetActive(true);

		if (isPerfectHit) {
			Text_hitResult.text = "Perfect";
			Anim_onion.enabled = false;
			hitResult.SetActive(true);
			Anim_characterActionPerfect.Play("character game action_grab");
			Anim_onionPerfect.Play("onion grabbed");
		}
		else
			Text_hitResult.text = "Miss";

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

	public void clickOperBtn (int num) {
		num += (partQuesList.Count-operCount);
		calculatePanel.SetActive(true);
		Text_partQues.text = partQuesList[num];
	}
}
