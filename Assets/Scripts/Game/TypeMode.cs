using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeMode : MonoBehaviour {
	// public GameObject[] quesNumArr, quesOperArr, quesBracketArr;
	public GameObject warningPanel, showTeamBeforeFighting, fightingPanel, fightingResult, calculatePanel, clickAnyPositionImage;
	public GameObject[] operTeamFieldImageArr, chooseOperMemberBtnArr, operTeamFailedArr;
	public Animator Anim_characterAction, Anim_npcFightingType, Anim_npcFightingResult, Anim_operFightingResult;
	public Sprite transparentSprite;
	public Sprite[] fightResultHintArr, windOperArr, fireOperArr, waterOperArr, groundOperArr;
	// public Image[] Image_quesOperArr;
	public Image Image_nextFightOper, Image_fightResultHint, Image_operFightingResult;
	public Image[] Image_operTeamMemberArr, Image_chooseOperMemberArr;
	public Text Text_warning, Text_userans;
	// public Text[] Text_quesNumArr;
	public string npc;

	private int operCount, operChooseMemberCount, operFailedCount;
	private bool isWin, isDraw, isAttackFailed;
	private string chooseNpcType;
	private List<string> operChooseTypeList = new List<string>();
	// private List<string> tmpQuesNumList = new List<string>();
	// private List<int> quesOperList = new List<int>();
	// private List<string> typeList = new List<string> {"wind", "fire", "water", "ground"};
	// private List<string> typeRanList = new List<string>();

	// setting question
	// public int maxNum;
	// public List<string> quesTemplate;
	// private MathDatasControl MathDatas;
	// private QuesObj quesObj;

	// Use this for initialization
	void Start () {
		Anim_characterAction.Play("character game action_fighting");

		// MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		// generateQuesForTypeMode(maxNum, quesTemplate);

// fake data
		operCount = 3;
		operChooseTypeList.Add("wind");
		operChooseTypeList.Add("fire");
		operChooseTypeList.Add("water");
		print(operChooseTypeList[0]); 
		
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

	// operChooseTypeList.Add(type);
	public void chooseOperatorMember (int num) {
		if (operChooseMemberCount < operCount) {
			for (int i = 0; i < operCount; i++)
				Image_operTeamMemberArr[operChooseMemberCount].sprite = Image_chooseOperMemberArr[num].sprite;
		} else {
			warningPanel.SetActive(true);
			Text_warning.text = "你指定的運算符號數目已超過"+operCount+"個了喔！";
		}
		operChooseMemberCount++;
	}

	public void clickClearTeam () {
		operChooseMemberCount = 0;
		for (int i = 0; i < operCount; i++)
			Image_operTeamMemberArr[i].sprite = transparentSprite;
	}

	public void clickFightingStart () {
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
				operTeamFieldImageArr[0].transform.position = new Vector3(-1000f, -20f, 0f);
				operTeamFieldImageArr[1].transform.position = new Vector3(-830f, -20f, 0f);
				operTeamFieldImageArr[2].transform.position = new Vector3(-660f, -20f, 0f);
				yield return new WaitForSeconds(1.5f);
				for (int i = 0; i < operCount; i++)
					operTeamFieldImageArr[i].SetActive(true);
				break;
			case "fightpanel":
				operTeamFieldImageArr[0].transform.position = new Vector3(-810f, 110f, 0f);
				operTeamFieldImageArr[1].transform.position = new Vector3(-810f, -10f, 0f);
				operTeamFieldImageArr[2].transform.position = new Vector3(-810f, -130f, 0f);
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
		print(chooseNpcType);
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
						operTeamFailedArr[i].SetActive(false);
				}
				fightingResult.SetActive(false);
				calculatePanel.SetActive(true);
				break;
			case "lose":
				yield return new WaitForSeconds(0.6f);
				clickAnyPositionImage.SetActive(true);
				break;
		}
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
		calculatePanel.SetActive(false);
		operFailedCount++;
		for (int i = 0; i < operFailedCount; i++)
			operTeamFailedArr[i].SetActive(true);
		for (int i = 0; i < operCount; i++)
			operTeamFieldImageArr[i].SetActive(true);
		if (operFailedCount < operChooseMemberCount) {
			restartTypeModeFighting();
			Image_nextFightOper.sprite = Image_operTeamMemberArr[operFailedCount].sprite;
			if (operChooseTypeList.Count != 0) {
				operChooseTypeList.Remove(operChooseTypeList[0]);
				print(operChooseTypeList[0]);
			}
		} else {
			print("攻擊完成！");
		}
		// print(operFailedCount);
		// print(operMemberCount);
	}

}