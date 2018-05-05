using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeMode : MonoBehaviour {
	public GameObject warningPanel, showTeamBeforeFighting, fightingPanel, fightingResult, calculatePanel;
	public Image[] teamMemberArr, playerDefiniteTeamArr, playerTeamMemberArr;
	public Image fightResultHint;
	public Animator Anim_characterAction, Anim_npcType, Anim_npcTypeinFighting, Anim_operatorFighting;
	public Sprite[] typeArr, fightResultHintArr;

	private int memberNum;
	private bool isWin, isDraw;
	private string animationPlay;
	private List<string> chooseTypeList = new List<string>();
	private List<string> enemyTypeList = new List<string>();

	// Use this for initialization
	void Start () {
		Anim_characterAction.Play("character game action_fighting");
		isWin = false;
		isDraw = false;

		enemyTypeList.Add("water");
		enemyTypeList.Add("wind");
		enemyTypeList.Add("fire");
		for (int i = 0; i < enemyTypeList.Count; i++)
			print(enemyTypeList[i]);
	}
	
	// Update is called once per frame
	void Update () {
		
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
			print("超過組員數目");
			warningPanel.SetActive(true);
		}
	}

	// warnings confirm btn
	public void clickConfirmBtn () {
		warningPanel.SetActive(false);
	}

	public void clickClearTeam () {
		for (int i = 0; i < 3; i++) {
			memberNum = 0;
			teamMemberArr[i].sprite = typeArr[4];
		}
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

	// only cauliflower now, and need to change operator sprite with type and symbol(+-x÷)
	public void clickAttack (string npc) {
		// npc = "Cauliflower";
		fightingResult.SetActive(true);
		switch (chooseTypeList[0]) {
			case "wind":
				if (enemyTypeList[0] == "ground") {
					isWin = true;
					animationPlay = npc + " Wind Win";
				} else if (enemyTypeList[0] == "fire") {
					isWin = false;
				} else {
					isWin = false;
					isDraw = true;
					animationPlay = npc + " Wind";
				}
				changeNpcFightingAnim(animationPlay);
			break;
			case "fire": 
				if (enemyTypeList[0] == "wind") {
					isWin = true;
					animationPlay = npc + " Fire Win";
				} else if (enemyTypeList[0] == "water") {
					isWin = false;
				} else {
					isWin = false;
					isDraw = true;
					animationPlay = npc + " Fire";
				}
				changeNpcFightingAnim(animationPlay);
			break;
			case "water":
				if (enemyTypeList[0] == "fire") {
					isWin = true;
					animationPlay = npc + " Water Win";
				} else if (enemyTypeList[0] == "ground") {
					isWin = false;
				} else {
					isWin = false;
					isDraw = true;
					animationPlay = npc + " Water";
				}
				changeNpcFightingAnim(animationPlay);
			break;
			case "ground":
				if (enemyTypeList[0] == "water") {
					isWin = true;
					animationPlay = npc + " Ground Win";
				} else if (enemyTypeList[0] == "wind") {
					isWin = false;
				} else {
					isWin = false;
					isDraw = true;
					animationPlay = npc + " Ground";
				}
			break;
			default:
			break;
		}

		if (isWin) {
			fightResultHint.sprite = fightResultHintArr[0];
			Anim_operatorFighting.Play("Operator Lose");
			StartCoroutine(showCalculatePanel(1.5f));
		} else {
			fightResultHint.sprite = fightResultHintArr[1];
			if (isDraw)
				Anim_operatorFighting.Play("Operator Original");
			else {
				animationPlay = npc + " Lose Die";
				Anim_operatorFighting.Play("Operator Win");
			}
		}

		changeNpcFightingAnim(animationPlay);
	}

	void changeNpcFightingAnim (string animation) {
		print(animation);
		Anim_npcTypeinFighting.Play(animation);
	}

	IEnumerator showCalculatePanel (float time) {
		yield return new WaitForSeconds(time);

		fightingResult.SetActive(false);
		calculatePanel.SetActive(true);
	}
}
