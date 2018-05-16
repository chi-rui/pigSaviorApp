using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorMode : MonoBehaviour {
	public GameObject warningPanel, waterGunImage, colorPainterImage, mixingColorResult, calculatePanel, clickAnyPositionImage;
	public GameObject[] quesNumTextArr, quesOperImageArr;
	public Animator Anim_npcColored, Anim_painter, Anim_waterGun, Anim_npcMixingColor, Anim_operatorPairResult;
	public Sprite[] colorArr, mixColorResultHintArr, redOperArr, yellowOperArr, blueOperArr, orangeOperArr, greenOperArr, purpleOperArr;
	public Image[] colorChooseMemberArr, Image_quesOperArr;
	public Image Image_npc, Image_mixColorResultHint, Image_npcInResult, Image_operInResult;
	public Button[] Button_colorPaintArr;
	public Button Button_waterGun, Button_finishColorNpc;
	public Text Text_warning;
	public Text[] Text_quesNumArr;
	public string npc;

	private int colorMixCount, operCount;
	private bool isPair, isMixColorFailed;
	private string colorResult;
	private List<string> chooseColorList = new List<string>();
	private List<string> colorList = new List<string> {"red", "yellow", "blue", "orange", "green", "purple"};
	private List<string> operColorRanList = new List<string>();
	private List<int> quesOperList = new List<int>();

	// setting question
	public int maxNum;
	public List<string> quesTemplate;
	private MathDatasControl MathDatas;
	private QuesObj quesObj;

// test
	private string testQues;

	// Use this for initialization
	void Start () {
		waterGunImage.SetActive(false);
		colorPainterImage.SetActive(false);
		isMixColorFailed = false;

		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		generateQuestion(maxNum, quesTemplate);
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

		// show question number text
		for (int i = 0; i < quesObj.question.Count; i++)
			Text_quesNumArr[i].text = quesObj.question[i];

		// unrepeat random four types
		while (operColorRanList.Count < 6) {
			int index = Random.Range(0, colorList.Count);
			if (!operColorRanList.Contains(colorList[index])) {
				operColorRanList.Add(colorList[index]);
				colorList.Remove(colorList[index]);
			}
		}
		for (int i = 0; i < operColorRanList.Count; i++)
			print(operColorRanList[i]);

		// set operator color and symbol
		for (int i = 0; i < operCount; i++) {
			switch (operColorRanList[i]) {
				case "red":
					Image_quesOperArr[i].sprite = redOperArr[quesOperList[i]];
					break;
				case "yellow":
					Image_quesOperArr[i].sprite = yellowOperArr[quesOperList[i]];
					break;
				case "blue":
					Image_quesOperArr[i].sprite = blueOperArr[quesOperList[i]];
					break;
				case "orange":
					Image_quesOperArr[i].sprite = orangeOperArr[quesOperList[i]];
					break;
				case "green":
					Image_quesOperArr[i].sprite = greenOperArr[quesOperList[i]];
					break;
				case "purple":
					Image_quesOperArr[i].sprite = purpleOperArr[quesOperList[i]];
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
					Anim_painter.Play("Color Red on NPC");
					colorChooseMemberArr[colorMixCount].sprite = colorArr[0];
					break;
				case "yellow":
					Anim_painter.Play("Color Yellow on NPC");
					colorChooseMemberArr[colorMixCount].sprite = colorArr[1];
					break;
				case "blue":
					Anim_painter.Play("Color Blue on NPC");
					colorChooseMemberArr[colorMixCount].sprite = colorArr[2];
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
		Anim_waterGun.Play("Water gun");

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
		print(colorResult);
		changeColor(colorResult);

		switch (colorResult) {
			case "red":
				for (int i = 0; i < operCount; i++) {
					if (operColorRanList[i] == "red") {
						isPair = true;
						Image_operInResult.sprite = redOperArr[quesOperList[i]];
					}
				}
				break;
			case "yellow":
				for (int i = 0; i < operCount; i++) {
					if (operColorRanList[i] == "yellow") {
						isPair = true;
						Image_operInResult.sprite = yellowOperArr[quesOperList[i]];
					}
				}
				break;
			case "blue":
				for (int i = 0; i < operCount; i++) {
					if (operColorRanList[i] == "blue") {
						isPair = true;
						Image_operInResult.sprite = blueOperArr[quesOperList[i]];
					}
				}
				break;
			case "orange":
				for (int i = 0; i < operCount; i++) {
					if (operColorRanList[i] == "orange") {
						isPair = true;
						Image_operInResult.sprite = orangeOperArr[quesOperList[i]];
					}
				}
				break;
			case "green":
				for (int i = 0; i < operCount; i++) {
					if (operColorRanList[i] == "green") {
						isPair = true;
						Image_operInResult.sprite = greenOperArr[quesOperList[i]];
					}
				}
				break;
			case "purple":
				for (int i = 0; i < operCount; i++) {
					if (operColorRanList[i] == "purple") {
						isPair = true;
						Image_operInResult.sprite = purpleOperArr[quesOperList[i]];
					}
				}
				break;
			default:
				isPair = false;
				break;
		}

		if (isPair) {
			Image_mixColorResultHint.sprite = mixColorResultHintArr[0];
			Anim_npcMixingColor.Play(npc + " Colored Success");
			Anim_operatorPairResult.Play("Operator Color Pair Success");
		} else {
			Image_mixColorResultHint.sprite = mixColorResultHintArr[1];
			if (colorResult == null)
				Anim_npcMixingColor.Play(npc + " Original");
			else
				Anim_npcMixingColor.Play(npc + " Colored Failed");
			Anim_operatorPairResult.Play("Operator Color Pair Failed");
			isMixColorFailed = true;
		}
		StartCoroutine(showColorModeFeedback(1.5f));
	}

	IEnumerator showColorModeFeedback (float time) {
		yield return new WaitForSeconds(time);

		if (isPair) {
			mixingColorResult.SetActive(false);
			calculatePanel.SetActive(true);
		} else
			clickAnyPositionImage.SetActive(true);
	}

	void restartColorMode () {
		changeBtnsState(1);
		mixingColorResult.SetActive(false);
		colorMixCount = 0;
		chooseColorList.Clear();
		for (int i = 0; i < 2; i++)
			colorChooseMemberArr[i].sprite = colorArr[3];
		Image_npc.color = new Color32(255, 255, 255, 255);
		Image_npcInResult.color = new Color32(255, 255, 255, 255);
		Anim_npcColored.Play(npc + " Original");
	}
}
