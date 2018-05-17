using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeMode : MonoBehaviour {
	public GameObject warningPanel, showTeamBeforeFighting, fightingPanel, fightingResult, calculatePanel, clickAnyPositionImage;
	public GameObject[] operTeamFieldImageArr, chooseOperMemberBtnArr, operTeamFailedImageArr, quesNumTextArr, quesOperImageArr;
	public Animator Anim_characterAction, Anim_npcFightingType, Anim_npcFightingResult, Anim_operFightingResult;
	public Sprite[] fightResultHintArr, windOperArr, fireOperArr, waterOperArr, groundOperArr;
	public Image Image_nextFightOper, Image_fightResultHint, Image_operFightingResult;
	public Image[] Image_operTeamMemberArr, Image_chooseOperMemberArr, Image_quesOperArr;
	public Text Text_warning, Text_userans, Text_partQues;
	public Text[] Text_quesNumArr;
	public string npc;

	private int operCount, operChooseMemberCount, operFailedCount;
	private bool isWin, isDraw, isAttackFailed;
	private string chooseNpcType, operTmpStr;
	private List<string> operChooseTypeList = new List<string>();
	private List<int> quesOperList = new List<int>();
	private List<string> typeList = new List<string> {"wind", "fire", "water", "ground"};
	private List<string> typeRanList = new List<string>();
	private List<int> operChooseBtnIndexList = new List<int>();
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
		Anim_characterAction.Play("character game action_fighting");

		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		generateQuestion(minNum, maxNum, quesTemplate);
		
		for (int i = 0; i < operCount; i++) {
			operTeamFieldImageArr[i].SetActive(true);
			chooseOperMemberBtnArr[i].SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isAttackFailed) {
			if (Input.GetMouseButtonDown(0)) {
				isAttackFailed = false;
				restartTypeModeFighting();
				clickAnyPositionImage.SetActive(false);
			}
		}
	}

	void generateQuestion (int min, int max, List<string> template) {
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

		// show question number text
		for (int i = 0; i < quesObj.question.Count; i++)
			Text_quesNumArr[i].text = quesObj.question[i];

		// unrepeat random four types
		while (typeRanList.Count < 4) {
			int index = Random.Range(0, typeList.Count);
			if (!typeRanList.Contains(typeList[index])) {
				typeRanList.Add(typeList[index]);
				typeList.Remove(typeList[index]);
			}
		}
		for (int i = 0; i < typeRanList.Count; i++)
			print(typeRanList[i]);

		// set operator type and symbol
		for (int i = 0; i < operCount; i++) {
			switch (typeRanList[i]) {
				case "wind":
					Image_quesOperArr[i].sprite = windOperArr[quesOperList[i]];
					break;
				case "fire":
					Image_quesOperArr[i].sprite = fireOperArr[quesOperList[i]];
					break;
				case "water":
					Image_quesOperArr[i].sprite = waterOperArr[quesOperList[i]];
					break;
				case "ground":
					Image_quesOperArr[i].sprite = groundOperArr[quesOperList[i]];
					break;
			}
			Image_chooseOperMemberArr[i].sprite = Image_quesOperArr[i].sprite;
		}
	}

	public void chooseOperatorMember (int num) {
		if (operChooseMemberCount < operCount) {
			for (int i = 0; i < operCount; i++)
				Image_operTeamMemberArr[operChooseMemberCount].sprite = Image_chooseOperMemberArr[num].sprite;
			operChooseTypeList.Add(typeRanList[num]);
			operChooseBtnIndexList.Add(num);
		} else {
			warningPanel.SetActive(true);
			Text_warning.text = "你指定的運算符號數目已超過"+operCount+"個了喔！";
		}
		operChooseMemberCount++;
	}

	public void clickClearTeam () {
		operChooseMemberCount = 0;
		operChooseTypeList.Clear();
		for (int i = 0; i < operCount; i++)
			Image_operTeamMemberArr[i].sprite = Resources.Load<Sprite>("transparent") as Sprite;
	}

	public void clickFightingStart () {
		// for (int i = 0; i < operChooseBtnIndexList.Count; i++)
		// 	print(operChooseBtnIndexList[i]);
		for (int i = 0; i < operChooseTypeList.Count; i++)
			print(operChooseTypeList[i]);
		if (operChooseMemberCount == 0) {
			warningPanel.SetActive(true);
			Text_warning.text = "你尚未指定運算符號攻擊順序";
		} else {
			showTeamBeforeFighting.SetActive(true);
			for (int i = 0; i < operCount; i++)
				operTeamFieldImageArr[i].SetActive(false);
			StartCoroutine(setOperTeamPosition("beforefight"));
			StartCoroutine(showFightingPanel(3.3f));
		}
	}

	IEnumerator setOperTeamPosition (string state) {
		switch (state) {
			case "beforefight":
				operTeamFieldImageArr[0].transform.position = new Vector3(-1000f, -20f, 0);
				operTeamFieldImageArr[1].transform.position = new Vector3(-830f, -20f, 0);
				operTeamFieldImageArr[2].transform.position = new Vector3(-660f, -20f, 0);
				yield return new WaitForSeconds(1.5f);
				for (int i = 0; i < operCount; i++)
					operTeamFieldImageArr[i].SetActive(true);
				break;
			case "fightpanel":
				operTeamFieldImageArr[0].transform.position = new Vector3(-810f, 110f, 0);
				operTeamFieldImageArr[1].transform.position = new Vector3(-810f, -10f, 0);
				operTeamFieldImageArr[2].transform.position = new Vector3(-810f, -130f, 0);
				yield return new WaitForSeconds(1f);
				break;
		}
	}

	IEnumerator showFightingPanel (float time) {
		yield return new WaitForSeconds(time);
		StartCoroutine(setOperTeamPosition("fightpanel"));
		showTeamBeforeFighting.SetActive(false);
		fightingPanel.SetActive(true);
		Image_nextFightOper.sprite = Image_operTeamMemberArr[0].sprite;
	}

	public void changeNpcFightingType (string type) {
		chooseNpcType = type;
		// print(chooseNpcType);
		switch (type) {
			case "wind":
				Anim_npcFightingType.Play(npc + " Wind");
				chooseNpcType = "wind";
				break;
			case "fire":
				Anim_npcFightingType.Play(npc + " Fire");
				chooseNpcType = "fire";
				break;
			case "water":
				Anim_npcFightingType.Play(npc + " Water");
				chooseNpcType = "water";
				break;
			case "ground":
				Anim_npcFightingType.Play(npc + " Ground");
				chooseNpcType = "ground";
				break;
		}
	}

	public void clickAttack () {
		print(chooseNpcType);
		if (chooseNpcType == null) {
			warningPanel.SetActive(true);
			Text_warning.text = "你尚未指定npc屬性";
		} else {
			fightingResult.SetActive(true);
			Image_operFightingResult.sprite = Image_nextFightOper.sprite;
			switch (chooseNpcType) {
				case "wind":
					if (operChooseTypeList[0] == "ground") {
						isWin = true;
						Anim_npcFightingResult.Play(npc + " Wind Win");
					} else {
						isWin = false;
						if (operChooseTypeList[0] != "fire")
							isDraw = true;
					}
					break;
				case "fire": 
					if (operChooseTypeList[0] == "wind") {
						isWin = true;
						Anim_npcFightingResult.Play(npc + " Fire Win");
					} else {
						isWin = false;
						if (operChooseTypeList[0] != "water")
							isDraw = true;
					}
					break;
				case "water":
					if (operChooseTypeList[0] == "fire") {
						isWin = true;
						Anim_npcFightingResult.Play(npc + " Water Win");
					} else {
						isWin = false;
						if (operChooseTypeList[0] != "ground")
							isDraw = true;
					}
					break;
				case "ground":
					if (operChooseTypeList[0] == "water") {
						isWin = true;
						Anim_npcFightingResult.Play(npc + " Ground Win");
					} else {
						isWin = false;
						if (operChooseTypeList[0] != "wind")
							isDraw = true;
					}
					break;
			}
			if (isWin) {
				Image_fightResultHint.sprite = fightResultHintArr[0];
				Anim_operFightingResult.Play("Operator Lose");
				StartCoroutine(showFightingFeedback("win"));
			} else {
				Image_fightResultHint.sprite = fightResultHintArr[1];
				if (isDraw) {
					Anim_npcFightingResult.Play(npc + " Draw");
					Anim_operFightingResult.Play("Operator Draw");
				} else {
					Anim_npcFightingResult.Play(npc + " Lose Die");
					Anim_operFightingResult.Play("Operator Win");
				}
				StartCoroutine(showFightingFeedback("lose"));
				isAttackFailed = true;
			}
		}
	}

	IEnumerator showFightingFeedback (string fightResult) {
		switch (fightResult) {
			case "win":
				yield return new WaitForSeconds(1.5f);
				for (int i = 0; i < operCount; i++)
					operTeamFieldImageArr[i].SetActive(false);
				if (operFailedCount != 0) {
					for (int i = 0; i < operFailedCount; i++)
						operTeamFailedImageArr[i].SetActive(false);
				}
				fightingResult.SetActive(false);
				showPartQuestion();
				calculatePanel.SetActive(true);
				break;
			case "lose":
				yield return new WaitForSeconds(0.6f);
				clickAnyPositionImage.SetActive(true);
				break;
		}
	}

	void showPartQuestion () {
		switch (quesOperList[operChooseBtnIndexList[0]]) {
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
		// Text_partQues.text = Text_quesNumArr[operChooseBtnIndexList[0]].text + operTmpStr + Text_quesNumArr[operChooseBtnIndexList[0]+1].text;
		Text_partQues.text = removeQuesBracket(Text_quesNumArr[operChooseBtnIndexList[0]].text) + operTmpStr + removeQuesBracket(Text_quesNumArr[operChooseBtnIndexList[0]+1].text);
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

	void restartTypeModeFighting () {
		fightingResult.SetActive(false);
		Anim_npcFightingType.Play(npc + " Original");
		chooseNpcType = null;
		isWin = false;
		isDraw = false;
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
		operFailedCount++;

		string tmpAns = "";
		if (Text_userans.text == null || Text_userans.text == "ANS") {
			userAnsList.Add(0);
			tmpAns = "0";
		} else {
			userAnsList.Add(int.Parse(Text_userans.text));
			tmpAns = Text_userans.text;
		}
		for (int i = 0; i < userAnsList.Count; i++)
			print(userAnsList[i]);

		for (int i = 0; i < operFailedCount; i++)
			operTeamFailedImageArr[i].SetActive(true);
		for (int i = 0; i < operCount; i++)
			operTeamFieldImageArr[i].SetActive(true);
		if (operFailedCount < operChooseMemberCount) {
			restartTypeModeFighting();
			Image_nextFightOper.sprite = Image_operTeamMemberArr[operFailedCount].sprite;
			if (operChooseTypeList.Count != 0) {
				operChooseTypeList.Remove(operChooseTypeList[0]);
				print(operChooseTypeList[0]);
			}
			quesOperImageArr[operChooseBtnIndexList[0]].SetActive(false);
			if (operChooseBtnIndexList[0] == 2) {
				quesNumTextArr[operChooseBtnIndexList[0]+1].SetActive(false);
				Text_quesNumArr[operChooseBtnIndexList[0]].text = tmpAns;
			} else {
				quesNumTextArr[operChooseBtnIndexList[0]].SetActive(false);
				Text_quesNumArr[operChooseBtnIndexList[0]+1].text = tmpAns;
			}
			operChooseBtnIndexList.Remove(operChooseBtnIndexList[0]);
			showPartQuestion();
			clickClearAnsNum();
			Text_userans.text = "ANS";
			calculatePanel.SetActive(false);
		} else {
			print("計算完成！");
			checkUserAnswer();
		}
		// print(operFailedCount);
		// print(operMemberCount);
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