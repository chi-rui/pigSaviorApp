using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeMode : MonoBehaviour {
	public GameObject warningPanel, showTeamBeforeFighting, fightingPanel, fightingResult, calculatePanel, clickAnyPositionImage;
	public Animator Anim_characterAction, Anim_npcType, Anim_npcTypeinFighting, Anim_operatorFighting;
	public Sprite[] typeArr, fightResultHintArr;
	public Image[] teamMemberArr, playerDefiniteTeamArr, playerTeamMemberArr;
	public Image Image_fightResultHint;
	public Text Text_warning;
	public string npc;

	private int memberNum;
	private bool isWin, isDraw, isAttackFailed;
	private string animationPlay;
	private List<string> chooseTypeList = new List<string>();
	private List<string> operatorTypeList = new List<string>();

	// Use this for initialization
	void Start () {
		Anim_characterAction.Play("character game action_fighting");
		isWin = false;
		isDraw = false;
		isAttackFailed = false;

// fake data
		operatorTypeList.Add("water");
		print(operatorTypeList[0]);
	}
	
	// Update is called once per frame
	void Update () {
		if (isAttackFailed) {
			if (Input.GetMouseButtonDown(0)) {
				restartTypeMode();
				clickAnyPositionImage.SetActive(false);
				isAttackFailed = false;
			}
		}
	}

	public void clickTypeBtns (string type) {
		if (memberNum < 3) {
			switch (type) {
				case "wind":
					print("click wind type");
					teamMemberArr[memberNum].sprite = typeArr[0];
					chooseTypeList.Add("wind");
					break;
				case "fire":
					print("click fire type");
					teamMemberArr[memberNum].sprite = typeArr[1];
					chooseTypeList.Add("fire");
					break;
				case "water":
					print("click water type");
					teamMemberArr[memberNum].sprite = typeArr[2];
					chooseTypeList.Add("water");
					break;
				case "ground":
					print("click ground type");
					teamMemberArr[memberNum].sprite = typeArr[3];
					chooseTypeList.Add("ground");
					break;
				default:
					break;
			}
			memberNum++;
		} else {
			print("超過隊友數目");
			warningPanel.SetActive(true);
			Text_warning.text = "你派出的隊友數已經超過3個了喔！";
		}
	}

	public void clickClearTeam () {
		restartTypeMode();
	}

	public void clickFightingStart () {
		for (int i = 0; i < chooseTypeList.Count; i++)
			print(chooseTypeList[i]);

		showTeamBeforeFighting.SetActive(true);
		for (int i = 0; i < chooseTypeList.Count; i++)
			playerDefiniteTeamArr[i].sprite = teamMemberArr[i].sprite;

		StartCoroutine(showFightingPanel(2.8f));
	}

	IEnumerator showFightingPanel (float time) {
		yield return new WaitForSeconds(time);

		showTeamBeforeFighting.SetActive(false);
		fightingPanel.SetActive(true);

		for (int i = 0; i < chooseTypeList.Count; i++)
			playerTeamMemberArr[i].sprite = teamMemberArr[i].sprite;

		changeNpcTypeAnim (chooseTypeList[0]);
	}

	void changeNpcTypeAnim (string type) {
		// if finish attacking, remove chooseTypeList[0]
		switch (type) {
			case "wind":
				Anim_npcType.Play("Cauliflower Wind");
				break;
			case "fire":
				Anim_npcType.Play("Cauliflower Fire");
				break;
			case "water":
				Anim_npcType.Play("Cauliflower Water");
				break;
			case "ground":
				Anim_npcType.Play("Cauliflower Ground");
				break;
			default:
				break;
		}
	}

	// need to change operator sprite with type and symbol(+-x÷)
	public void clickAttack () {
		fightingResult.SetActive(true);
		switch (chooseTypeList[0]) {
			case "wind":
				if (operatorTypeList[0] == "ground") {
					isWin = true;
					animationPlay = npc + " Wind Win";
				} else if (operatorTypeList[0] == "fire") {
					isWin = false;
				} else {
					isWin = false;
					isDraw = true;
				}
				changeNpcFightingAnim(animationPlay);
			break;
			case "fire": 
				if (operatorTypeList[0] == "wind") {
					isWin = true;
					animationPlay = npc + " Fire Win";
				} else if (operatorTypeList[0] == "water") {
					isWin = false;
				} else {
					isWin = false;
					isDraw = true;
				}
				changeNpcFightingAnim(animationPlay);
			break;
			case "water":
				if (operatorTypeList[0] == "fire") {
					isWin = true;
					animationPlay = npc + " Water Win";
				} else if (operatorTypeList[0] == "ground") {
					isWin = false;
				} else {
					isWin = false;
					isDraw = true;
				}
				changeNpcFightingAnim(animationPlay);
			break;
			case "ground":
				if (operatorTypeList[0] == "water") {
					isWin = true;
					animationPlay = npc + " Ground Win";
				} else if (operatorTypeList[0] == "wind") {
					isWin = false;
				} else {
					isWin = false;
					isDraw = true;
				}
			break;
			default:
			break;
		}

		if (isWin) {
			Image_fightResultHint.sprite = fightResultHintArr[0];
			Anim_operatorFighting.Play("Operator Lose");
		} else {
			Image_fightResultHint.sprite = fightResultHintArr[1];
			if (isDraw) {
				animationPlay = npc + " Draw";
				Anim_operatorFighting.Play("Operator Draw");
			} else {
				animationPlay = npc + " Lose Die";
				Anim_operatorFighting.Play("Operator Win");
			}
			isAttackFailed = true;
		}
		StartCoroutine(showTypeModeFeedback(1.5f));
		changeNpcFightingAnim(animationPlay);
	}

	void changeNpcFightingAnim (string animation) {
		print(animation);
		Anim_npcTypeinFighting.Play(animation);
	}

	IEnumerator showTypeModeFeedback (float time) {
		yield return new WaitForSeconds(time);

		if (isWin) {
			fightingResult.SetActive(false);
			calculatePanel.SetActive(true);
		} else
			clickAnyPositionImage.SetActive(true);
	}

	void restartTypeMode () {
		fightingResult.SetActive(false);
		fightingPanel.SetActive(false);
		isWin = false;
		isDraw = false;
		memberNum = 0;
		chooseTypeList.Clear();
		for (int i = 0; i < 3; i++)
			teamMemberArr[i].sprite = typeArr[4];
	}
}
