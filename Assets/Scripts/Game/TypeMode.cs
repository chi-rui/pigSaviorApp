using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeMode : MonoBehaviour {
	public Animator Anim_characterAction, Anim_cauliflowerType;
	public Image[] teamMemberArr, playerDefiniteTeamArr, playerTeamMemberArr;
	public Sprite[] typeArr;
	public GameObject warningPanel, showTeamBeforeFighting, fightingPanel;

	private int memberNum;
	private List<string> chooseTypeList = new List<string>();

	// Use this for initialization
	void Start () {
		Anim_characterAction.Play("character game action_fighting");
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

		StartCoroutine(showFightingPanel(3.3f));
	}

	IEnumerator showFightingPanel (float time) {
		yield return new WaitForSeconds(time);

		showTeamBeforeFighting.SetActive(false);
		fightingPanel.SetActive(true);

		for (int i = 0; i < chooseTypeList.Count; i++)
			playerTeamMemberArr[i].sprite = teamMemberArr[i].sprite;

		// if finish attacking, remove chooseTypeList[0]
		switch (chooseTypeList[0]) {
			case "wind":
				Anim_cauliflowerType.Play("Cauliflower Wind");
				break;
			case "fire":
				Anim_cauliflowerType.Play("Cauliflower Fire");
				break;
			case "water":
				Anim_cauliflowerType.Play("Cauliflower Water");
				break;
			case "ground":
				Anim_cauliflowerType.Play("Cauliflower Ground");
				break;
			default:
				break;
		}
	}

}
