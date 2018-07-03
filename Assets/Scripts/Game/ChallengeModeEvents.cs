using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChallengeModeEvents : MonoBehaviour {
	public GameObject warningPanel, correctPanel, wrongPanel;
	public GameObject[] gamePanel, topText;

	private GameObject character;
	private Vector2[] pointsPos, changeLayerPos;
	private Color[] bgColors;
	private int layerNum, pointsNum, userLife, finishPointCount;
	private float speed, fspeed;
	private bool[] isFinished;
	private string challengeProgress;
	private	List<int> ranGameModeList = new List<int>();

	// Use this for initialization
	void Start () {
		DatasControl.GameMode = "CHALLENGE";
		// GameObject.Find("Datas").GetComponent<DatasControl>().nowStage = 999;

		userLife = 3;
		layerNum = 1;
		character = GameObject.Find("Image_character");
		isFinished = new bool[5];

		// yellow, green, red, blue, purple
		bgColors = new Color[] {
			new Color32(230, 230, 230, 255),
			new Color32(205, 255, 195, 255),
			new Color32(255, 205, 195, 255),
			new Color32(200, 250, 255, 255),
			new Color32(210, 200, 255, 255)
		};

		// set points position
		pointsPos = new Vector2[] {
			new Vector2(-400f, 90f),
			new Vector2(-200f, 70f),
			new Vector2(0, 90f),
			new Vector2(200f, 70f),
			new Vector2(400f, 90f)
		};
		// change to new layer position
		changeLayerPos = new Vector2[] {
			new Vector2(400f, 450f),
			new Vector2(-400f, -300f)
		};

		// set best score
		if (Login.user_challProgress != "" && Login.user_challProgress != null) {
			string[] bestScore = Login.user_challProgress.Split('-');
			GameObject.Find("Text_best score").GetComponent<Text>().text = "Best：第" + bestScore[0] + "層第" + bestScore[1] + "階";
		}

		StartCoroutine(getRank());
		StartCoroutine(showCover());
		ranGameMode();
	}
	
	// Update is called once per frame
	void Update () {
		// print((layerNum-1) % 5);
		GameObject.Find("Panel_background").GetComponent<Image>().color = bgColors[(layerNum-1) % 5];
	}

	IEnumerator getRank () {
		string URL = "http://163.21.245.192/PigSaviorAPP/challengeRank.php";
		WWWForm form = new WWWForm();
		Dictionary<string, string> data = new Dictionary<string, string>();
		data.Add("download", "1");
		foreach (KeyValuePair<string, string> post in data) {
			form.AddField(post.Key, post.Value);
		}
		WWW www = new WWW(URL, form);
		yield return www;
		// print(www.text);
		string[] bestData = www.text.Split('#');
		for (int i = 0; i < bestData.Length; i++) {
			// print(bestData[i]);
			string[] rank = bestData[i].Split('-');
			topText[i].GetComponent<Text>().text = "第"+ rank[0] +"層第"+ rank[1] +"階";
		}
	}

	IEnumerator showCover () {
		yield return new WaitForSeconds(3f);
		GameObject.Find("Panel_cover").SetActive(false);
	}

	// 0: rhythm 1: color 2: type
	void ranGameMode () {
		List<int> gameModeList = new List<int> {0, 1, 2};
		while (ranGameModeList.Count < 5) {
			int index = Random.Range(0, gameModeList.Count);
			ranGameModeList.Add(gameModeList[index]);
		}
		print("ranGameModeList.Count: " + ranGameModeList.Count);
		// for (int i = 0; i < ranGameModeList.Count; i++)
		// 	print(ranGameModeList[i]);
	}

	public void clickStartGame () {
		if (!isFinished[pointsNum])
			gamePanel[ranGameModeList[pointsNum]].SetActive(true);
		else {
			warningPanel.SetActive(true);
			GameObject.Find("Text_warning").GetComponent<Text>().text = "不能再進去了...";
		}
	}

	void setBtnsState (bool isBeClicked) {
		Button[] workspaceBtns = new Button[] {
			GameObject.Find("Button_left").GetComponent<Button>(),
			GameObject.Find("Button_right").GetComponent<Button>(),
			GameObject.Find("Button_Go").GetComponent<Button>()
		};
		if (isBeClicked) {
			for (int i = 0; i < 3; i++)
				workspaceBtns[i].interactable = true;
		} else {
			for (int i = 0; i < 3; i++)
				workspaceBtns[i].interactable = false;
		}
	}

	public void nextClicked () {

		setBtnsState(false);
		pointsNum++;
		// print("pointsNum: " + pointsNum + " / finishPointCount: " + finishPointCount);
		if (pointsNum == 5) {
			// change to new layer
			if (finishPointCount == 5) {
				ranGameModeList.Clear();
				finishPointCount = 0;
				character.GetComponent<Animator>().Play("character small back");
				StartCoroutine(move(changeLayerPos[0]));
				StartCoroutine(changeToNewLayer());
			} else {
				pointsNum--;
				warningPanel.SetActive(true);
				GameObject.Find("Text_warning").GetComponent<Text>().text = "這一層還沒有完成喔";
				setBtnsState(true);
			}
		} else {
			StartCoroutine(move(pointsPos[pointsNum]));
		}
	}

	public void lastClicked () {
		setBtnsState(false);
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
		if (pointsNum == 5)
			setBtnsState(false);
		else
			setBtnsState(true);
	}

	IEnumerator changeToNewLayer () {
		yield return new WaitForSeconds(1.5f);
		character.transform.position = changeLayerPos[1];
		pointsNum = 0;
		StartCoroutine(move(pointsPos[pointsNum]));
		for (int i = 0; i < 5; i++) {
			GameObject.Find("Image_points"+i).GetComponent<Image>().sprite = Resources.Load<Sprite>("point") as Sprite;
			isFinished[i] = false;
		}
		layerNum++;
		GameObject.Find("Text_layer name").GetComponent<Text>().text = "第"+layerNum.ToString()+"層";
		GameObject.Find("Image_road start").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
		GameObject.Find("Image_road end").GetComponent<Image>().color = new Color32(255, 255, 255, 0);
		ranGameMode();
		StartCoroutine(changeCharacterAni());
	}

	IEnumerator changeCharacterAni () {
		yield return new WaitForSeconds(0.6f);
		character.GetComponent<Animator>().Play("character small");
		setBtnsState(true);
	}

	private float calculateNewSpeed (Vector2 target) {
		float tmp = Vector2.Distance(character.transform.position, target);
		if (tmp == 0)
			return tmp;
		else
			return (fspeed / tmp);
	}

	public void showFeedBack (bool ans, string prompts) {
		if (ans) {
			StartCoroutine(Feedback(correctPanel));
			pointFinished();
		} else {
			GameObject.Find("Feedbacks").transform.GetChild(1).GetChild(2).GetComponent<Text>().text = prompts;
			userLife--;
			if(userLife == 0){
				GameObject.Find("player life").transform.GetChild(1).gameObject.SetActive(false); //GetComponent<Image>().sprite = null;
				GameObject.Find("player life").transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("gg") as Sprite;
			} else {
				string life = "Life" + userLife.ToString();
				GameObject.Find("player life").transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(life) as Sprite;
			}
			StartCoroutine(Feedback(wrongPanel));
		}
	}

	public IEnumerator Feedback (GameObject imageFeedBack){
		imageFeedBack.SetActive(true);
		yield return new WaitForSeconds(2f);
		imageFeedBack.SetActive(false);
		if (userLife == 0) {
			StartCoroutine(upload_USER(challengeProgress, Login.account));
			GameObject.Find("EventSystem").GetComponent<DynamicAssessment>().teachNum(-1);
		}
		if (finishPointCount == 5)
			GameObject.Find("Image_road end").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
	}

	void pointFinished () {
		finishPointCount++;
		// print("finishPointCount: " + finishPointCount);
		isFinished[pointsNum] = true;
		GameObject.Find("Image_points"+pointsNum).GetComponent<Image>().sprite = Resources.Load<Sprite>("blingPoint") as Sprite;
		string[] bestScore = new string[2];
		if (Login.user_challProgress != "" && Login.user_challProgress != null)
			bestScore = Login.user_challProgress.Split('-');
		else {
			bestScore[0] = "1";
			bestScore[1] = "0";
		}
		if (layerNum > int.Parse(bestScore[0]) || (layerNum == int.Parse(bestScore[0]) && finishPointCount > int.Parse(bestScore[1]))) {
			GameObject.Find("Text_best score").GetComponent<Text>().text = "Best：第" + layerNum + "層第" + finishPointCount + "階";
			challengeProgress = layerNum.ToString() + "-" + finishPointCount.ToString();
		} else
			challengeProgress = bestScore[0] + "-" + bestScore[1];
	}

	IEnumerator upload_USER (string challProgress, string account) {
		string URL = "http://163.21.245.192/PigSaviorAPP/userDataUpdate.php";
		WWWForm form = new WWWForm();
		Dictionary<string, string> data = new Dictionary<string, string>();
		data.Add("challProgress", challProgress);
		data.Add("account", account);
		foreach (KeyValuePair<string, string> post in data) {
			form.AddField(post.Key, post.Value);
		}
		WWW www = new WWW(URL, form);
		yield return www;
		print(www.text);
	}

	public void backToMain(){
		DatasControl.GameMode = "";
		SceneManager.LoadScene("MainPage");
	}
}
