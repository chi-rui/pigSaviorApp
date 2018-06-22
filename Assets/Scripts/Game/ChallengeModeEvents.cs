using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeModeEvents : MonoBehaviour {
	public GameObject character, warningPanel;
	public GameObject[] gamePanel;
	public Vector2[] pointsPos, changeLayerPos;
	public Button[] workspaceBtns;

	private Color[] bgColors;
	private int layerNum, pointsNum;
	private float speed, fspeed;
	private	List<int> ranGameModeList = new List<int>();

	// Use this for initialization
	void Start () {
		layerNum = 1;
		pointsNum = 0;

		// yellow, green, red, blue, purple
		bgColors = new Color[] {
			new Color32(255, 255, 195, 255),
			new Color32(205, 255, 195, 255),
			new Color32(255, 205, 195, 255),
			new Color32(200, 250, 255, 255),
			new Color32(210, 200, 255, 255),
		};

		ranGameMode();
	}
	
	// Update is called once per frame
	void Update () {
		// print((layerNum-1) % 5);
		GameObject.Find("Panel_background").GetComponent<Image>().color = bgColors[(layerNum-1) % 5];
	}

	// 0: rhythm 1: color 2: type
	void ranGameMode () {
		List<int> gameModeList = new List<int> {0, 1, 2};
		while (ranGameModeList.Count < 5) {
			int index = Random.Range(0, gameModeList.Count);
			ranGameModeList.Add(gameModeList[index]);
		}
		print("ranGameModeList.Count: " + ranGameModeList.Count);
		for (int i = 0; i < ranGameModeList.Count; i++)
			print(ranGameModeList[i]);
	}

	public void clickStartGame () {
		gamePanel[ranGameModeList[pointsNum]].SetActive(true);
	}

	public void nextClicked () {
		for (int i = 0; i < 3; i++)
			workspaceBtns[i].interactable = false;

		pointsNum++;
		// print(pointsNum);
		if (pointsNum == 5) {
			ranGameModeList.Clear();
			character.GetComponent<Animator>().Play("character small back");
			StartCoroutine(move(changeLayerPos[0]));
			StartCoroutine(changeToNewLayer());
		} else {
			StartCoroutine(move(pointsPos[pointsNum]));
		}
	}

	public void lastClicked () {
		for (int i = 0; i < 3; i++)
			workspaceBtns[i].interactable = false;

		if (pointsNum == 0) {
			warningPanel.SetActive(true);
			if (layerNum == 1)
				GameObject.Find("Text_warning").GetComponent<Text>().text = "此處為起點";
			else
				GameObject.Find("Text_warning").GetComponent<Text>().text = "無法返回上一層";
		} else
			pointsNum--;
		// print(pointsNum);
		StartCoroutine(move(pointsPos[pointsNum]));
	}

	IEnumerator move (Vector2 position) {
		speed = 0.03f;
		fspeed = Vector2.Distance(character.transform.position, position) * speed;
		while(speed != 0){
			character.transform.position = Vector2.Lerp(character.transform.position, position, speed);
			speed = calculateNewSpeed(position);
			yield return 0;
		}
		for (int i = 0; i < 3; i++) {
			if (pointsNum == 5)
				workspaceBtns[i].interactable = false;
			else
				workspaceBtns[i].interactable = true;
		}
	}

	IEnumerator changeToNewLayer () {
		yield return new WaitForSeconds(1.5f);
		character.transform.position = changeLayerPos[1];
		pointsNum = 0;
		StartCoroutine(move(pointsPos[pointsNum]));
		layerNum++;
		GameObject.Find("Text_layer name").GetComponent<Text>().text = "第"+layerNum.ToString()+"層";
		ranGameMode();
		StartCoroutine(changeCharacterAni());
	}

	IEnumerator changeCharacterAni () {
		yield return new WaitForSeconds(0.6f);
		character.GetComponent<Animator>().Play("character small");
		for (int i = 0; i < 3; i++)
			workspaceBtns[i].interactable = true;
	}

	private float calculateNewSpeed (Vector2 target) {
		float tmp = Vector2.Distance(character.transform.position, target);
		if (tmp == 0)
			return tmp;
		else
			return (fspeed / tmp);
	}
}
