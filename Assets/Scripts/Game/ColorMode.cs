using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorMode : MonoBehaviour {
	public GameObject warningPanel, waterGunImage, colorPainterImage, mixingColorResult, calculatePanel, clickAnyPositionImage;
	public Animator Anim_npcColored, Anim_painter, Anim_waterGun, Anim_npcMixingColor, Anim_operatorPairResult;
	public Sprite[] colorArr, mixColorResultHintArr;
	public Image[] colorChooseMemberArr;
	public Image Image_npc, Image_npcInResult, Image_mixColorResultHint;
	public Button[] Button_colorPaintArr;
	public Button Button_waterGun;
	public Text Text_warning;
	public string npc;

	private int colorMixCount;
	private bool isPair, isMixColorFailed;
	private string colorResult;
	private List<string> chooseColorList = new List<string>();
	private List<string> operatorColorList = new List<string>();

	// Use this for initialization
	void Start () {
		waterGunImage.SetActive(false);
		colorPainterImage.SetActive(false);
		isMixColorFailed = false;

// fake data
		operatorColorList.Add("orange");
		print(operatorColorList[0]);
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

	public void clickColorNpc (string color) {
		print(colorMixCount);
		colorPainterImage.SetActive(true);
		for (int i = 0; i < 3; i++)
			Button_colorPaintArr[i].interactable = false;
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
				default:
				break;
			}
			chooseColorList.Add(color);
			StartCoroutine(paintColorOnNpc(1.45f));
		} else {
			warningPanel.SetActive(true);
			Text_warning.text = "最多只能混合2種顏色喔！";
			colorPainterImage.SetActive(false);
			for (int i = 0; i < 3; i++)
				Button_colorPaintArr[i].interactable = true;
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

		for (int i = 0; i < 3; i++)
			Button_colorPaintArr[i].interactable = true;
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
			case "purple":
				Image_npc.color = new Color32(150, 50, 220, 255);
				Image_npcInResult.color = new Color32(150, 50, 220, 255);
			break;
			case "green":
				Image_npc.color = new Color32(50, 175, 55, 255);
				Image_npcInResult.color = new Color32(50, 175, 55, 255);
			break;
			default:
			break;
		}
	}

	public void clickClearColor () {
		waterGunImage.SetActive(true);
		Anim_waterGun.Play("Water gun");

		Button_waterGun.interactable = false;

		colorResult = null;
		print(colorResult);

		StartCoroutine(playWaterGunAnim(0.95f));
	}

	IEnumerator playWaterGunAnim (float time) {
		yield return new WaitForSeconds(time);

		waterGunImage.SetActive(false);
		restartColorMode();
		Button_waterGun.interactable = true;
	}

	public void clickFinishColor () {
		mixingColorResult.SetActive(true);
		print(colorResult);
		changeColor(colorResult);
		switch (colorResult) {
			case "orange":
				if (operatorColorList[0] == "orange")
					isPair = true;
				else
					isPair = false;
			break;
			case "purple":
				if (operatorColorList[0] == "purple")
					isPair = true;
				else
					isPair = false;
			break;
			case "green":
				if (operatorColorList[0] == "green")
					isPair = true;
				else
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
