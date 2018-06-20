using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class SpecialLogic1 : MonoBehaviour {

	private DatasControl dataControl;
	private StageEvents stageEvents;
	private MathDatasControl mathDatas;

	public GameObject SelectionPanel, CorrectPanel, WrongPanel, stageClear;
	private QuesObj question;
	private List<int> numbers = new List<int>();
	private string ansIndex;
	private Color[] colors;
	private string[] contents;

	// Use this for initialization
	void Start () {
		DatasControl.GameMode = "SPECIAL";
		dataControl = GameObject.Find("Datas").GetComponent<DatasControl>();
		stageEvents = GameObject.Find("EventSystem").GetComponent<StageEvents>();
		// mathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		
		// get question object to set numbers.
		// question = mathDatas.getQuestion(1,500,"A-B-C-D");
		
		// set talk contents.
		// StreamReader logicQuestion = new StreamReader("Assets/Resources/LogicQuestionFile/stage12.txt");
		// string[] talkTemplate = logicQuestion.ReadToEnd().Split('\n');
		// string[] contents = talkTemplate[Random.Range(0, talkTemplate.Length-1)].Split('@');
		// logicQuestion.Close();
		StartCoroutine(getQuesContent());
		

		// set colors
		colors = new Color[]{
			new Color32(255, 0, 0, 255),
			new Color32(255, 135, 35, 255),
			new Color32(255, 255, 0, 255),
			new Color32(0, 255, 0, 255),
			new Color32(0, 0, 255, 255),
			new Color32(150, 50, 220, 255),
			new Color32(108, 108, 108, 255),
		};

		// set board npc
		for(int i = 1; i < 8; i++){
			string board = "Button_Board" + i.ToString();
			GameObject.Find(board).GetComponent<Image>().color = colors[i-1];
			// GameObject.Find(board).GetComponent<Npc>().npcInfo.falseContents[0] = contents[i-1];
		}





	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator getQuesContent () {
		string URL = "http://163.21.245.192/PigSaviorAPP/files/stage13.txt";
		WWWForm form = new WWWForm();
		Dictionary<string, string> data = new Dictionary<string, string>();
		data.Add("download", "1");
		foreach (KeyValuePair<string, string> post in data) {
			form.AddField(post.Key, post.Value);
		}
		WWW www = new WWW(URL, form);
		yield return www;
		print(www.text);

		string[] talkTemplate = www.text.Split('\n');
		string[] contents = talkTemplate[Random.Range(0, talkTemplate.Length-1)].Split('@');
		ansIndex = contents[contents.Length-1];
		print(ansIndex);

		for(int i = 0; i < 7; i++){
			// int j = Random.Range(0,7);
			int k = Random.Range(0,6);
			// Color c = colors[i];
			string temp = contents[i];
			// colors[i] = colors[j];
			contents[i] = contents[k];
			// colors[j] = c;
			contents[k] = temp;
		}

		for(int i = 1; i < 8; i++){
			string board = "Button_Board" + i.ToString();
			GameObject.Find(board).GetComponent<Image>().color = colors[i-1];
			GameObject.Find(board).GetComponent<Npc>().npcInfo.falseContents[0] = contents[i-1];
		}
	}

	public void showSelectionPanel(){
		if(stageEvents.userProgress >= 1){
			StartCoroutine(selectAns());
		}
	}

	public IEnumerator selectAns(){
		while(stageEvents.TalkWindow.activeInHierarchy){
			yield return new WaitForSeconds(0.1f);
		}
		SelectionPanel.SetActive(true);
	}

	public void clickAns( string selections ){
		if(ansIndex == selections){
			stageClear.SetActive(true);
		}else{
			WrongPanel.SetActive(true);
		}
		StartCoroutine(back());
	}

	public IEnumerator back(){
		// while(WrongPanel.activeInHierarchy || stageClear.activeInHierarchy ){
		yield return new WaitForSeconds(2f);
		// }
		SceneManager.LoadScene("Chapter"+dataControl.chapter.ToString());
	}
}
