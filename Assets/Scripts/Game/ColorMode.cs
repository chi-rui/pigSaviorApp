using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorMode : MonoBehaviour {
	public GameObject warningPanel, waterGunImage, colorPainterImage, mixingColorResult, calculatePanel, clickAnyPositionImage, rechallengeBtn;
	public GameObject[] quesNumTextArr, quesOperImageArr;
	public Animator Anim_npcColored, Anim_npcMixingColor, Anim_operatorPairResult;
	public Image[] colorChooseMemberArr;
	public Image Image_npc, Image_mixColorResultHint, Image_npcInResult, Image_operInResult;
	public Button[] Button_colorPaintArr;
	public Button Button_waterGun, Button_finishColorNpc;
	public Text Text_warning, Text_partQues, Text_userans;
	public string npc;

	private int colorMixCount, operCount, userCalculateCount, operChooseColorIndex;
	private bool isPair, isMixColorFailed, isSpecialCalculate;
	private string colorResult, operTmpStr;
	private List<string> chooseColorList = new List<string>();
	private List<string> colorList = new List<string> {"red", "yellow", "blue", "orange", "green", "purple"};
	private List<string> operColorRanList = new List<string>();
	private List<string> tmpColorOperList = new List<string>();
	private List<int> quesOperList = new List<int>();
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
		isPair = false;
		isMixColorFailed = false;

		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		generateNewQuestion(minNum, maxNum, quesTemplate);
	}
	
	// Update is called once per frame
	void Update () {
		if (isMixColorFailed) {
			if (Input.GetMouseButtonDown(0)) {
				restartColorMode();
				clickAnyPositionImage.SetActive(false);
				isMixColorFailed = false;
			}
		}
	}

	void generateNewQuestion (int min, int max, List<string> template) {
		// generate question and get operator counts
		quesObj = MathDatas.getQuestion(min, max, template[UnityEngine.Random.Range(0, template.Count)]);
		for (int i = 0; i < quesObj.question.Count; i++)
			testQues += quesObj.question[i];
		print(testQues);
		operCount = quesObj.answer.Count;
		// print(operCount);

		// set question numbers and operators position
		switch (operCount) {
			case 2:
				for (int i = 0; i <= operCount; i++) {
					quesNumTextArr[i].transform.position += new Vector3(50f, 0, 0);
					quesOperImageArr[i].transform.position += new Vector3(50f, 0, 0);
				}
				break;
			case 1:
				for (int i = 0; i <= operCount; i++) {
					quesNumTextArr[i].transform.position += new Vector3(200f, 0, 0);
					quesOperImageArr[i].transform.position += new Vector3(200f, 0, 0);
				}
				break;
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

		// print(quesObj.question.Count);
		// for (int i = 0; i < quesObj.question.Count; i++)
		// 	print(quesObj.question[i]);
		// for (int i = 0; i < quesOperList.Count; i++)
		// 	print(quesOperList[i]);

		// unrepeat random four types
		while (operColorRanList.Count < operCount) {
			int index = Random.Range(0, colorList.Count);
			if (!operColorRanList.Contains(colorList[index])) {
				operColorRanList.Add(colorList[index]);
				tmpColorOperList.Add(colorList[index]);
				colorList.Remove(colorList[index]);
			}
		}
		for (int i = 0; i < operColorRanList.Count; i++) {
			// print(operColorRanList[i]);
			print(tmpColorOperList[i]);
		}

		showQuestion();
	}

	void showQuestion () {
		// show question number text
		for (int i = 0; i < quesObj.question.Count; i++)
			quesNumTextArr[i].GetComponent<Text>().text = quesObj.question[i];

		// for (int i = 0; i < quesObj.question.Count; i++)
		// 	print(quesObj.question[i]);

		// set operator color and symbol
		for (int i = 0; i < operCount; i++) {
			quesOperImageArr[i].SetActive(true);
			switch (operColorRanList[i]) {
				case "red":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("redOper"+quesOperList[i]) as Sprite;
					break;
				case "yellow":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("yellowOper"+quesOperList[i]) as Sprite;
					break;
				case "blue":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("blueOper"+quesOperList[i]) as Sprite;
					break;
				case "orange":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("orangeOper"+quesOperList[i]) as Sprite;
					break;
				case "green":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("greenOper"+quesOperList[i]) as Sprite;
					break;
				case "purple":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("purpleOper"+quesOperList[i]) as Sprite;
					break;
			}
		}
	}

	void changeBtnsState (int state) {
		if (state == 0) {
			for (int i = 0; i < 3; i++)
				Button_colorPaintArr[i].interactable = false;
			Button_waterGun.interactable = false;
			Button_finishColorNpc.interactable = false;
		} else {
			for (int i = 0; i < 3; i++)
				Button_colorPaintArr[i].interactable = true;
			Button_waterGun.interactable = true;
			Button_finishColorNpc.interactable = true;
		}
	}

	public void clickColorNpc (string color) {
		changeBtnsState(0);
		colorPainterImage.SetActive(true);
		// print(colorMixCount);
		if (colorMixCount < 2) {
			switch (color) {
				case "red":
					colorPainterImage.GetComponent<Animator>().Play("Color Red on NPC");
					colorChooseMemberArr[colorMixCount].sprite = Resources.Load<Sprite>("pigment"+0) as Sprite;
					break;
				case "yellow":
					colorPainterImage.GetComponent<Animator>().Play("Color Yellow on NPC");
					colorChooseMemberArr[colorMixCount].sprite = Resources.Load<Sprite>("pigment"+1) as Sprite;
					break;
				case "blue":
					colorPainterImage.GetComponent<Animator>().Play("Color Blue on NPC");
					colorChooseMemberArr[colorMixCount].sprite = Resources.Load<Sprite>("pigment"+2) as Sprite;
					break;
			}
			chooseColorList.Add(color);
			StartCoroutine(paintColorOnNpc(1.45f));
		} else {
			warningPanel.SetActive(true);
			Text_warning.text = "最多只能混合2種顏色喔！";
			colorPainterImage.SetActive(false);
			changeBtnsState(1);
		}
		colorMixCount++;
	}

	IEnumerator paintColorOnNpc (float time) {
		yield return new WaitForSeconds(time);

		colorPainterImage.SetActive(false);

		if (chooseColorList.Count == 1) {
			colorResult = chooseColorList[0];
		} else {
			if (chooseColorList[0] == chooseColorList[1]) {
				colorResult = chooseColorList[0];
			} else {
				if ((chooseColorList[0] == "red" && chooseColorList[1] == "yellow") ||
					(chooseColorList[0] == "yellow" && chooseColorList[1] == "red")) {
					print("mix orange");
					colorResult = "orange";
				} else if ((chooseColorList[0] == "red" && chooseColorList[1] == "blue") || 
							(chooseColorList[0] == "blue" && chooseColorList[1] == "red")) {
					print("mix purple");
					colorResult = "purple";
				} else if ((chooseColorList[0] == "yellow" && chooseColorList[1] == "blue") || 
							(chooseColorList[0] == "blue" && chooseColorList[1] == "yellow")) {
					print("mix green");
					colorResult = "green";
				}
			}
		}
		Anim_npcColored.Play(npc + " Colored");
		changeColor(colorResult);
		changeBtnsState(1);
	}

	void changeColor (string color) {
		switch (color) {
			case "red":
				Image_npc.color = new Color32(255, 0, 0, 255);
				Image_npcInResult.color = new Color32(255, 0, 0, 255);
				break;
			case "yellow":
				Image_npc.color = new Color32(255, 255, 0, 255);
				Image_npcInResult.color = new Color32(255, 255, 0, 255);
				break;
			case "blue":
				Image_npc.color = new Color32(0, 0, 255, 255);
				Image_npcInResult.color = new Color32(0, 0, 255, 255);
				break;
			case "orange":
				Image_npc.color = new Color32(255, 135, 35, 255);
				Image_npcInResult.color = new Color32(255, 135, 35, 255);
				break;
			case "green":
				Image_npc.color = new Color32(50, 175, 55, 255);
				Image_npcInResult.color = new Color32(50, 175, 55, 255);
				break;
			case "purple":
				Image_npc.color = new Color32(150, 50, 220, 255);
				Image_npcInResult.color = new Color32(150, 50, 220, 255);
				break;
		}
	}

	public void clickClearColor () {
		changeBtnsState(0);
		waterGunImage.SetActive(true);
		waterGunImage.GetComponent<Animator>().Play("Water gun");

		colorResult = null;
		print(colorResult);

		StartCoroutine(playWaterGunAnim(0.95f));
	}

	IEnumerator playWaterGunAnim (float time) {
		yield return new WaitForSeconds(time);

		waterGunImage.SetActive(false);
		restartColorMode();
		changeBtnsState(1);
	}

	public void clickFinishColor () {
		changeBtnsState(0);
		mixingColorResult.SetActive(true);
		// print(colorResult);
		changeColor(colorResult);

		for (int i = 0; i < operColorRanList.Count; i++) {
			if (operColorRanList[i] == colorResult)
				operChooseColorIndex = i;
		}
		Image_operInResult.sprite = Resources.Load<Sprite>(colorResult+"Oper"+quesOperList[operChooseColorIndex]) as Sprite;
		for (int i = 0; i < tmpColorOperList.Count; i++) {
			if (tmpColorOperList[i] == colorResult)
				isPair = true;
		}

		if (isPair) {
			Image_mixColorResultHint.sprite = Resources.Load<Sprite>("mixColorResult"+1) as Sprite;
			Anim_npcMixingColor.Play(npc + " Colored Success");
			Anim_operatorPairResult.Play("Operator Color Pair Success");
			StartCoroutine(showColorModeFeedback(1.5f));
		} else {
			Image_operInResult.sprite = Resources.Load<Sprite>("transparent") as Sprite;
			Image_mixColorResultHint.sprite = Resources.Load<Sprite>("mixColorResult"+0) as Sprite;
			if (colorResult == null)
				Anim_npcMixingColor.Play(npc + " Original");
			else
				Anim_npcMixingColor.Play(npc + " Colored Failed");
			Anim_operatorPairResult.Play("Operator Color Pair Failed");
			isMixColorFailed = true;
			StartCoroutine(showColorModeFeedback(0.3f));
		}
	}

	IEnumerator showColorModeFeedback (float time) {
		yield return new WaitForSeconds(time);

		if (isPair) {
			mixingColorResult.SetActive(false);
			showPartQuestion();
			calculatePanel.SetActive(true);
		} else
			clickAnyPositionImage.SetActive(true);
	}

	void showPartQuestion () {
		switch (quesOperList[operChooseColorIndex]) {
			case 0:
				operTmpStr = "+";
				break;
			case 1:
				operTmpStr = "-";
				break;
			case 2:
				operTmpStr = "x";
				break;
			case 3:
				operTmpStr = "÷";
				break;
		}
		string tmp1 = "", tmp2 = "";
		if (quesNumTextArr[operChooseColorIndex].GetComponent<Text>().text == "") {
			tmp1 = quesNumTextArr[operChooseColorIndex-1].GetComponent<Text>().text;
			tmp2 = quesNumTextArr[operChooseColorIndex+1].GetComponent<Text>().text;
		} else if (quesNumTextArr[operChooseColorIndex+1].GetComponent<Text>().text == "") {
			tmp1 = quesNumTextArr[operChooseColorIndex].GetComponent<Text>().text;
			tmp2 = quesNumTextArr[operChooseColorIndex+2].GetComponent<Text>().text;
			isSpecialCalculate = true;
		} else {
			tmp1 = quesNumTextArr[operChooseColorIndex].GetComponent<Text>().text;
			tmp2 = quesNumTextArr[operChooseColorIndex+1].GetComponent<Text>().text;
		}
		Text_partQues.text = removeQuesBracket(tmp1) + operTmpStr + removeQuesBracket(tmp2);
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

	void restartColorMode () {
		changeBtnsState(1);
		isPair = false;
		Image_operInResult.sprite = Resources.Load<Sprite>("transparent") as Sprite;
		mixingColorResult.SetActive(false);
		colorMixCount = 0;
		chooseColorList.Clear();
		for (int i = 0; i < 2; i++)
			colorChooseMemberArr[i].sprite = Resources.Load<Sprite>("transparent") as Sprite;
		Image_npc.color = new Color32(255, 255, 255, 255);
		Image_npcInResult.color = new Color32(255, 255, 255, 255);
		Anim_npcColored.Play(npc + " Original");
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
		
		tmpColorOperList.Remove(colorResult);
		if (userCalculateCount < operCount) {
			print("operChooseColorIndex: " + operChooseColorIndex);
			quesOperImageArr[operChooseColorIndex].SetActive(false);
			if (operChooseColorIndex == 2) {
				quesNumTextArr[operChooseColorIndex+1].GetComponent<Text>().text = null;
				quesNumTextArr[operChooseColorIndex].GetComponent<Text>().text = tmpAns;
			} else {
				if (isSpecialCalculate)
					quesNumTextArr[operChooseColorIndex+2].GetComponent<Text>().text = null;
				quesNumTextArr[operChooseColorIndex].GetComponent<Text>().text = null;
				quesNumTextArr[operChooseColorIndex+1].GetComponent<Text>().text = tmpAns;
			}
			clickClearAnsNum();
			Text_userans.text = "ANS";
			restartColorMode();
			calculatePanel.SetActive(false);
		} else {
			rechallengeBtn.SetActive(false);
			print("計算完成！");
			checkUserAnswer();
		}

		// for (int i = 0; i < operColorRanList.Count; i++)
		// 	print(operColorRanList[i]);
		// for (int i = 0; i < tmpColorOperList.Count; i++)
		// 	print(tmpColorOperList[i]);
	}

	public void checkUserAnswer () {
		// for (int i = 0; i < quesObj.answer.Count; i++)
		// 	print(quesObj.answer[i].partAns);
		if (userAnsList[userAnsList.Count-1] == quesObj.answer[quesObj.answer.Count-1].partAns)
			print("答案正確");
		else
			print("答案錯誤");
	}

	public void clickRechallengeGame () {
		calculatePanel.SetActive(false);
		userCalculateCount = 0;
		Text_userans.text = "ANS";
		isSpecialCalculate = false;
		restartColorMode();
		userAnsList.Clear();
		tmpColorOperList.Clear();
		for (int i = 0; i < operColorRanList.Count; i++)
			tmpColorOperList.Add(operColorRanList[i]);
		// for (int i = 0; i < tmpColorOperList.Count; i++)
		// 	print(tmpColorOperList[i]);
		showQuestion();
	}
}