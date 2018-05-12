using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeMode : MonoBehaviour {
	public GameObject warningPanel, showTeamBeforeFighting, fightingPanel, fightingResult, calculatePanel, clickAnyPositionImage;
	public GameObject[] operTeamMemberArr, chooseOperMemberArr, definiteOperTeamArr, operTeamInFightingArr, operTeamFailedArr;
	public Animator Anim_characterAction, Anim_npcFightingType, Anim_npcFightingResult, Anim_operFightingResult;
	public Sprite transparentSprite;
	public Sprite[] fightResultHintArr;
	public Image[] Image_operTeamMemberArr, Image_chooseOperMemberArr, Image_definiteOperTeamArr, Image_operTeamInFightingArr;
	public Image Image_firstOperMember, Image_fightResultHint, Image_operFightingResult;
	public Text Text_warning, Text_userans;
	public string npc;

	private int operCount, operMemberCount, operFailedCount;
	private bool isWin, isDraw, isAttackFailed;
	private string chooseNpcType;
	private List<string> operatorTypeList = new List<string>();

	// Use this for initialization
	void Start () {
		Anim_characterAction.Play("character game action_fighting");

// fake data
		operatorTypeList.Add("ground");
		operatorTypeList.Add("fire");
		operatorTypeList.Add("water");
		print(operatorTypeList[0]); 
		operCount = 3;

		// setting member fields position according to operator counts
		switch (operCount) {
			case 2:
				operTeamMemberArr[0].transform.position += new Vector3 (90f, 0, 0);
				chooseOperMemberArr[0].transform.position += new Vector3 (90f, 0, 0);
				operTeamMemberArr[1].transform.position += new Vector3 (140f, 0, 0);
				chooseOperMemberArr[1].transform.position += new Vector3 (140f, 0, 0);
			break;
			case 1:
				operTeamMemberArr[0].transform.position += new Vector3 (190f, 0, 0);
				chooseOperMemberArr[0].transform.position += new Vector3 (190f, 0, 0);
			break;
			default:
			break;
		}
		for (int i = 0; i < operCount; i++) {
			operTeamMemberArr[i].SetActive(true);
			chooseOperMemberArr[i].SetActive(true);
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

	// operatorTypeList.Add(type);
	public void chooseOperatorMember (int num) {
		if (operMemberCount < operCount) {
			for (int i = 0; i < operCount; i++)
				Image_operTeamMemberArr[operMemberCount].sprite = Image_chooseOperMemberArr[num].sprite;
		} else {
			warningPanel.SetActive(true);
			Text_warning.text = "你指定的運算符號數目已超過"+operCount+"個了喔！";
		}
		operMemberCount++;
	}

	public void clickClearTeam () {
		operMemberCount = 0;
		for (int i = 0; i < operCount; i++)
			Image_operTeamMemberArr[i].sprite = transparentSprite;
	}

	public void clickFightingStart () {
		switch (operMemberCount) {
			case 2:
				for (int i = 0; i < 2; i++)
					definiteOperTeamArr[i].transform.position += new Vector3 (80f, 0, 0);
			break;
			case 1:
				definiteOperTeamArr[0].transform.position += new Vector3 (140f, 0, 0);
			break;
		}
		for (int i = 0; i < operMemberCount; i++) {
			definiteOperTeamArr[i].SetActive(true);
			Image_definiteOperTeamArr[i].sprite = Image_operTeamMemberArr[i].sprite;
		}
		if (operMemberCount == 0) {
			warningPanel.SetActive(true);
			Text_warning.text = "你尚未指定運算符號攻擊順序";
		} else {
			showTeamBeforeFighting.SetActive(true);
			StartCoroutine(showFightingPanel(2.8f));
		}
	}

	IEnumerator showFightingPanel (float time) {
		yield return new WaitForSeconds(time);

		showTeamBeforeFighting.SetActive(false);
		fightingPanel.SetActive(true);

		for (int i = 0; i < operMemberCount; i++) {
			operTeamInFightingArr[i].SetActive(true);
			Image_operTeamInFightingArr[i].sprite = Image_operTeamMemberArr[i].sprite;
		}
		Image_firstOperMember.sprite = Image_operTeamMemberArr[0].sprite;
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
			default:
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
			Image_operFightingResult.sprite = Image_firstOperMember.sprite;
			switch (chooseNpcType) {
				case "wind":
					if (operatorTypeList[0] == "ground") {
						isWin = true;
						Anim_npcFightingResult.Play(npc + " Wind Win");
					} else {
						isWin = false;
						if (operatorTypeList[0] != "fire")
							isDraw = true;
					}
				break;
				case "fire": 
					if (operatorTypeList[0] == "wind") {
						isWin = true;
						Anim_npcFightingResult.Play(npc + " Fire Win");
					} else {
						isWin = false;
						if (operatorTypeList[0] != "water")
							isDraw = true;
					}
				break;
				case "water":
					if (operatorTypeList[0] == "fire") {
						isWin = true;
						Anim_npcFightingResult.Play(npc + " Water Win");
					} else {
						isWin = false;
						if (operatorTypeList[0] != "ground")
							isDraw = true;
					}
				break;
				case "ground":
					if (operatorTypeList[0] == "water") {
						isWin = true;
						Anim_npcFightingResult.Play(npc + " Ground Win");
					} else {
						isWin = false;
						if (operatorTypeList[0] != "wind")
							isDraw = true;
					}
				break;
				default:
				break;
			}
			if (isWin) {
				Image_fightResultHint.sprite = fightResultHintArr[0];
				Anim_operFightingResult.Play("Operator Lose");
				StartCoroutine(showCalculatePanel(1.5f));
			} else {
				Image_fightResultHint.sprite = fightResultHintArr[1];
				if (isDraw) {
					Anim_npcFightingResult.Play(npc + " Draw");
					Anim_operFightingResult.Play("Operator Draw");
				} else {
					Anim_npcFightingResult.Play(npc + " Lose Die");
					Anim_operFightingResult.Play("Operator Win");
				}
				StartCoroutine(showClickAnyPosition(0.6f));
				isAttackFailed = true;
			}
		}
	}

	IEnumerator showCalculatePanel (float time) {
		yield return new WaitForSeconds(time);

		fightingResult.SetActive(false);
		calculatePanel.SetActive(true);
	}

	IEnumerator showClickAnyPosition (float time) {
		yield return new WaitForSeconds(time);

		clickAnyPositionImage.SetActive(true);
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
		operTeamFailedArr[operFailedCount-1].SetActive(true);
		calculatePanel.SetActive(false);
		if (operFailedCount < operMemberCount) {
			restartTypeModeFighting();
			Image_firstOperMember.sprite = Image_operTeamMemberArr[operFailedCount].sprite;
			if (operatorTypeList.Count != 0) {
				operatorTypeList.Remove(operatorTypeList[0]);
				print(operatorTypeList[0]);
			}
		} else {
			print("攻擊完成！");
		}
		// print(operFailedCount);
		// print(operMemberCount);
	}
}
