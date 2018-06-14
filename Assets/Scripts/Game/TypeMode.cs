using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeMode : MonoBehaviour {
	public GameObject warningPanel, showTeamBeforeFighting, fightingPanel, fightingResult, calculatePanel, clickAnyPositionImage;
	public GameObject[] operTeamFieldImageArr, chooseOperMemberBtnArr, operTeamFailedImageArr, quesNumTextArr, quesOperImageArr;
	public Animator Anim_characterAction, Anim_npcFightingType, Anim_npcFightingResult, Anim_operFightingResult;
	public Image Image_nextFightOper, Image_fightResultHint;
	public Image[] Image_operTeamMemberArr, Image_chooseOperMemberArr;
	public Text Text_warning, Text_userans, Text_partQues;
	public string npc;

	private int operCount, operChooseMemberCount, operFailedCount, numA, numB;
	private bool isWin, isDraw, isAttackFailed, isSpecialCalculate, isInBracketA, isInBracketB;
	private string chooseNpcType, operTmpStr;
	private List<string> typeRanList = new List<string>();
	private List<string> operChooseTypeList = new List<string>();
	private List<int> operChooseBtnIndexList = new List<int>();
	private List<int> quesOperList = new List<int>();
	private List<int> quesOperIndexList = new List<int>();
	private	List<string> misConceptions = new List<string>();

	// set question
	public int minNum, maxNum;
	public List<string> quesTemplate;
	private QuesObj quesObj, temp;
	private MathDatasControl MathDatas;
	private MisIdentify MisIdent;

	// set user answer
	private List<AnsObj> userAnsList = new List<AnsObj>();

	private StageEvents stageEvents;
	private DynamicAssessment dynamicAssessment;

	void OnEnable () {
		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		MisIdent = GameObject.Find("EventSystem").GetComponent<MisIdentify>();
		stageEvents = GameObject.Find("EventSystem").GetComponent<StageEvents>();
		dynamicAssessment = GameObject.Find("EventSystem").GetComponent<DynamicAssessment>();
		
		Anim_characterAction.Play("character game action_fighting");
		generateNewQuestion(minNum, maxNum, quesTemplate);
		clickRechallengeGame();
	}

	// Use this for initialization
	void Start () {
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

	void generateNewQuestion (int min, int max, List<string> template) {
		// generate question and get operator counts
		quesObj = MathDatas.getQuestion(min, max, template[UnityEngine.Random.Range(0, template.Count)]);
		temp = new QuesObj();
		temp.question = new List<string>(quesObj.question);
		temp.answer = new List<AnsObj>(quesObj.answer);

		// initial and clear question setting lists
		quesOperList.Clear();
		quesOperIndexList.Clear();
		typeRanList.Clear();

		operCount = temp.answer.Count;
		// print(operCount);

		string testQues = "";
		for (int i = 0; i < temp.question.Count; i++)
			testQues += temp.question[i];
		print(testQues);

		// store operators index and operators in each list
		for (int i = 0; i < temp.question.Count; i++) {
			if (temp.question[i] == "+" || temp.question[i] == "-" || temp.question[i] == "x" || temp.question[i] == "÷") {
				quesOperIndexList.Add(i);
				switch (temp.question[i]) {
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
			}
		}

		// delete operators and brackets in question
		for (int i = 0; i < temp.question.Count; i++) {
			if (temp.question[i] == "+" || temp.question[i] == "-" || temp.question[i] == "x" || temp.question[i] == "÷")
				temp.question.Remove(temp.question[i]);
			if (temp.question[i] == "(") {
				temp.question[i+1] = temp.question[i] + temp.question[i+1];
			} else if (temp.question[i] == ")") {
				temp.question[i-1] = temp.question[i-1] + temp.question[i];
			}
		}
		for (int i = 0; i < temp.question.Count; i++) {
			if (temp.question[i] == "(")
				temp.question.Remove(temp.question[i]);
		}
		for (int i = 0; i < temp.question.Count; i++) {
			if (temp.question[i] == ")")
				temp.question.Remove(temp.question[i]);
		}

		// print(temp.question.Count);
		// for (int i = 0; i < temp.question.Count; i++)
		// 	print(temp.question[i]);
		// for (int i = 0; i < quesOperList.Count; i++)
		// 	print(quesOperList[i]);
		// for (int i = 0; i < quesOperIndexList.Count; i++)
		// 	print(quesOperIndexList[i]);

		// unrepeat random four types
		List<string> typeList = new List<string> {"wind", "fire", "water", "ground"};
		while (typeRanList.Count < operCount) {
			int index = Random.Range(0, typeList.Count);
			if (!typeRanList.Contains(typeList[index])) {
				typeRanList.Add(typeList[index]);
				typeList.Remove(typeList[index]);
			}
		}
		print("typeRanList.Count: " + typeRanList.Count);
		for (int i = 0; i < typeRanList.Count; i++)
			print(typeRanList[i]);

		showQuestion();
	}

	void showQuestion () {
		// show question number text
		for (int i = 0; i < temp.question.Count; i++)
			quesNumTextArr[i].GetComponent<Text>().text = temp.question[i];

		// set operator type and symbol
		for (int i = 0; i < operCount; i++) {
			quesOperImageArr[i].SetActive(true);
			switch (typeRanList[i]) {
				case "wind":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("windOper"+quesOperList[i]) as Sprite;
					break;
				case "fire":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("fireOper"+quesOperList[i]) as Sprite;
					break;
				case "water":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("waterOper"+quesOperList[i]) as Sprite;
					break;
				case "ground":
					quesOperImageArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("groundOper"+quesOperList[i]) as Sprite;
					break;
			}
			Image_chooseOperMemberArr[i].sprite = quesOperImageArr[i].GetComponent<Image>().sprite;
		}
	}

	public void chooseOperatorMember (int num) {
		chooseOperMemberBtnArr[num].GetComponent<Button>().interactable = false;
		if (operChooseMemberCount < operCount) {
			for (int i = 0; i < operCount; i++)
				Image_operTeamMemberArr[operChooseMemberCount].sprite = Image_chooseOperMemberArr[num].sprite;
			operChooseTypeList.Add(typeRanList[num]);
			operChooseBtnIndexList.Add(num);
		}
		operChooseMemberCount++;
		// print(operChooseMemberCount);
	}

	public void clickClearTeam () {
		operChooseMemberCount = 0;
		operChooseTypeList.Clear();
		operChooseBtnIndexList.Clear();
		for (int i = 0; i < operCount; i++) {
			chooseOperMemberBtnArr[i].GetComponent<Button>().interactable = true;
			Image_operTeamMemberArr[i].sprite = Resources.Load<Sprite>("transparent") as Sprite;
		}
	}

	public void clickFightingStart () {
		// for (int i = 0; i < operChooseBtnIndexList.Count; i++)
		// 	print(operChooseBtnIndexList[i]);
		// for (int i = 0; i < operChooseTypeList.Count; i++)
		// 	print(operChooseTypeList[i]);
		if (operChooseMemberCount == 0) {
			warningPanel.SetActive(true);
			Text_warning.text = "你尚未指定運算符號攻擊順序";
		} else {
			if (operChooseMemberCount != operCount) {
				warningPanel.SetActive(true);
				Text_warning.text = "你指定的運算符號數量不到"+operCount+"個";
			} else {
				showTeamBeforeFighting.SetActive(true);
				for (int i = 0; i < operCount; i++)
					operTeamFieldImageArr[i].SetActive(false);
				StartCoroutine(setOperTeamPosition("beforeFight"));
				StartCoroutine(showFightingPanel(3.3f));
			}
		}
	}

	IEnumerator setOperTeamPosition (string state) {
		switch (state) {
			case "chooseOperMember":
				operTeamFieldImageArr[0].transform.position = new Vector3(chooseOperMemberBtnArr[0].transform.position.x, -30f, 0);
				operTeamFieldImageArr[1].transform.position = new Vector3(chooseOperMemberBtnArr[1].transform.position.x, -30f, 0);
				operTeamFieldImageArr[2].transform.position = new Vector3(chooseOperMemberBtnArr[2].transform.position.x, -30f, 0);
				yield return new WaitForSeconds(1f);
				break;
			case "beforeFight":
				operTeamFieldImageArr[0].transform.position = new Vector3(chooseOperMemberBtnArr[0].transform.position.x+580f, -20f, 0);
				operTeamFieldImageArr[1].transform.position = new Vector3(chooseOperMemberBtnArr[1].transform.position.x+530f, -20f, 0);
				operTeamFieldImageArr[2].transform.position = new Vector3(chooseOperMemberBtnArr[2].transform.position.x+480f, -20f, 0);
				yield return new WaitForSeconds(1.5f);
				for (int i = 0; i < operCount; i++)
					operTeamFieldImageArr[i].SetActive(true);
				break;
			case "fightPanel":
				operTeamFieldImageArr[0].transform.position = new Vector3(chooseOperMemberBtnArr[0].transform.position.x+685f, 110f, 0);
				operTeamFieldImageArr[1].transform.position = new Vector3(chooseOperMemberBtnArr[1].transform.position.x+505f, -10f, 0);
				operTeamFieldImageArr[2].transform.position = new Vector3(chooseOperMemberBtnArr[2].transform.position.x+325f, -130f, 0);
				yield return new WaitForSeconds(1f);
				break;
		}
	}

	IEnumerator showFightingPanel (float time) {
		yield return new WaitForSeconds(time);
		StartCoroutine(setOperTeamPosition("fightPanel"));
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
			Anim_operFightingResult.GetComponent<Image>().sprite = Image_nextFightOper.sprite;
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
				Image_fightResultHint.sprite = Resources.Load<Sprite>("fightingResult"+1) as Sprite;
				Anim_operFightingResult.Play("Operator Lose");
				StartCoroutine(showFightingFeedback("win"));
			} else {
				Image_fightResultHint.sprite = Resources.Load<Sprite>("fightingResult"+0) as Sprite;
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
				yield return new WaitForSeconds(0.3f);
				clickAnyPositionImage.SetActive(true);
				break;
		}
	}

	void showPartQuestion () {
		// print(operChooseBtnIndexList[0]);
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
		isInBracketA = false; isInBracketB = false;
		string tmp1 = "", tmp2 = "";
		if (quesNumTextArr[operChooseBtnIndexList[0]].GetComponent<Text>().text == "") {
			tmp1 = quesNumTextArr[operChooseBtnIndexList[0]-1].GetComponent<Text>().text;
			tmp2 = quesNumTextArr[operChooseBtnIndexList[0]+1].GetComponent<Text>().text;
		} else if (quesNumTextArr[operChooseBtnIndexList[0]+1].GetComponent<Text>().text == "") {
			tmp1 = quesNumTextArr[operChooseBtnIndexList[0]].GetComponent<Text>().text;
			tmp2 = quesNumTextArr[operChooseBtnIndexList[0]+2].GetComponent<Text>().text;
			isSpecialCalculate = true;
		} else {
			tmp1 = quesNumTextArr[operChooseBtnIndexList[0]].GetComponent<Text>().text;
			tmp2 = quesNumTextArr[operChooseBtnIndexList[0]+1].GetComponent<Text>().text;
		}
		// print(tmp1 + " " + tmp2);
		Text_partQues.text = removeLeftNumBracket(tmp1) + operTmpStr + removeRightNumBracket(tmp2);
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

	public void clickRechallengeGame () {
		calculatePanel.SetActive(false);
		fightingPanel.SetActive(false);
		if (operFailedCount != 0) {
			for (int i = 0; i < operFailedCount; i++)
				operTeamFailedImageArr[i].SetActive(false);
			showQuestion();
		}
		operFailedCount = 0;
		Text_userans.text = "ANS";
		isSpecialCalculate = false;
		userAnsList.Clear();
		clickClearTeam();
		// print("operCount: " + operCount);
		for (int i = 0; i < operCount; i++) {
			operTeamFieldImageArr[i].SetActive(true);
			chooseOperMemberBtnArr[i].SetActive(true);
		}
		StartCoroutine(setOperTeamPosition("chooseOperMember"));
	}

	public void clickFinishCalculate () {
		operFailedCount++;
		string tmpAns = ""; string bracketStr = "";

		// set user answer object and add to user answer list
		AnsObj userAnsObj = new AnsObj();
		userAnsObj.index = quesOperIndexList[operChooseBtnIndexList[0]];
		userAnsObj.operators = System.Convert.ToChar(operTmpStr);
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

		for (int i = 0; i < operFailedCount; i++)
			operTeamFailedImageArr[i].SetActive(true);
		for (int i = 0; i < operCount; i++)
			operTeamFieldImageArr[i].SetActive(true);
		clickClearAnsNum();
		Text_userans.text = "ANS";
		calculatePanel.SetActive(false);

		if (operFailedCount < operChooseMemberCount) {
			restartTypeModeFighting();
			Image_nextFightOper.sprite = Image_operTeamMemberArr[operFailedCount].sprite;
			if (operChooseTypeList.Count != 0) {
				operChooseTypeList.Remove(operChooseTypeList[0]);
				// print(operChooseTypeList[0]);
			}
			quesOperImageArr[operChooseBtnIndexList[0]].SetActive(false);
			if (operChooseBtnIndexList[0] == 2) {
				quesNumTextArr[operChooseBtnIndexList[0]+1].GetComponent<Text>().text = null;
				bracketStr = quesNumTextArr[operChooseBtnIndexList[0]-1].GetComponent<Text>().text;
				if (bracketStr.Contains("("))
					quesNumTextArr[operChooseBtnIndexList[0]].GetComponent<Text>().text = tmpAns + ")";
				else
					quesNumTextArr[operChooseBtnIndexList[0]].GetComponent<Text>().text = tmpAns;
			} else {
				quesNumTextArr[operChooseBtnIndexList[0]].GetComponent<Text>().text = null;
				bracketStr = quesNumTextArr[operChooseBtnIndexList[0]+2].GetComponent<Text>().text;
				if (bracketStr.Contains(")"))
					quesNumTextArr[operChooseBtnIndexList[0]+1].GetComponent<Text>().text = "(" + tmpAns;
				else
					quesNumTextArr[operChooseBtnIndexList[0]+1].GetComponent<Text>().text = tmpAns;
				
				if (isSpecialCalculate)
					quesNumTextArr[operChooseBtnIndexList[0]+2].GetComponent<Text>().text = null;
			}
			operChooseBtnIndexList.Remove(operChooseBtnIndexList[0]);
		} else {
			// print("計算完成！");
			StartCoroutine(checkUserAnswer());
		}
		// print(operFailedCount);
		// print(operMemberCount);
	}

	public IEnumerator checkUserAnswer () {
		misConceptions = MisIdent.getMisConception(quesObj.answer, userAnsList);

		if(quesObj.answer[quesObj.answer.Count-1].partAns == userAnsList[userAnsList.Count-1].partAns){
			stageEvents.showFeedBack(true, "");
			GameObject.Find("Datas").GetComponent<DatasControl>().getGameHistoryData(quesObj.question, quesObj.answer, userAnsList, true, misConceptions);
		}else{
			dynamicAssessment.setContents(quesObj, new List<AnsObj>(userAnsList), misConceptions[0]);
			stageEvents.showFeedBack(false , dynamicAssessment.getPrompt(misConceptions));
			GameObject.Find("Datas").GetComponent<DatasControl>().getGameHistoryData(quesObj.question, quesObj.answer, userAnsList, false, misConceptions);
		}
		yield return new WaitForSeconds(2f);
		GameObject.Find("Panel_" + npc + " Type").SetActive(false);
	}
}