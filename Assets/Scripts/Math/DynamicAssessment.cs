using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class DynamicAssessment : MonoBehaviour {

	public int countMistake;
	private bool RuleType, ProcessType, OperType, isTeaching;

	public GameObject teachingPanel, teachPage, selectionFunc, selectionQues, promptQues;

	public List<QuesObj> quesList = new List<QuesObj>();
	public List<List<AnsObj>> userList = new List<List<AnsObj>>();
	public List<string> misConceptions = new List<string>();


	// for test
	// private MathDatasControl MathDatas;
	public Text TrueAnsBefore, TrueAnsAfter, UserAnsBefore, UserAnsAfter;

	// Use this for initialization
	void Start () {
		// countMistake = 0;

		RuleType = false;
		ProcessType = false;
		OperType = false;

		// MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		// for(int i = 0; i < 3; i++){
		// 	QuesObj test = MathDatas.getQuestion(1, 200, "(A÷B)÷(C÷D)");
		// 	setContents(test, test.answer, "迷思概念測試"+i.ToString());
		// }

		// teachNum(-1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string getPrompt( List<string> misConception ){
		switch(misConception[0]){
			case "mis01":
				if(!RuleType){
					RuleType = true;
					return "注意一下運算規則再試試看吧";
				}else{
					return "括號裡面的運算要比誰都先做喔";
				}
			case "mis02":
				if(!RuleType){
					RuleType = true;
					return "注意一下運算規則再試試看吧";
				}else{
					return "運算符號是有計算的先後順序的\n想一想再試一次吧";
				}
			case "mis03":
				if(!RuleType){
					RuleType = true;
					return "注意一下運算規則再試試看吧";
				}else{
					return "運算要由左而右進行\n才不會導致結果錯誤喔";
				}
			case "mis04":
				if(!ProcessType){
					ProcessType = true;
					return "計算的過程中好像有點狀況\n小心一些再試試看吧";
				}else{
					return "運算的過程也是很重要的喔\n再嘗試一次吧";	
				}
			case "mis05":
				if(!ProcessType){
					ProcessType = true;
					return "計算的過程中好像有點狀況\n小心一些再試試看吧";
				}else{
					return "好像有些小地方沒有注意到喔\n算完了不要忘記再檢查一下喔";	
				}
			case "mis06":
				if(!OperType){
					OperType = true;
					return "不同的運算符號有不同的意義\n想一想再試試看吧";
				}else{
					return "一直一橫是加號\n一橫結束的是減號\n想想看要怎麼使用他們呢";
				}
			case "mis07":
				if(!OperType){
					OperType = true;
					return "不同的運算符號有不同的意義\n想一想再試試看吧";
				}else{
					return "一直一橫是加號\n長相叉叉是乘號\n他們是不一樣的東西喔";
				}
			case "mis08":
				if(!OperType){
					OperType = true;
					return "不同的運算符號有不同的意義\n想一想再試試看吧";
				}else{
					return "長相叉叉是乘號\n上下點點是除號\n他們概念上是相反的喔";
				}
			case "mis09":
				if(!OperType){
					OperType = true;
					return "不同的運算符號有不同的意義\n想一想再試試看吧";
				}else{
					return "一橫結束是減號\n上下點點是除號\n多了點點概念就不一樣囉";
				}
			default:
				return "哎呀！好像出了些小問題！";
		}
	}

	public void setContents( QuesObj question, List<AnsObj> user, string misConception ){
		quesList.Add(question);
		userList.Add(user);
		misConceptions.Add(misConception);

		// for(int i = 0; i < userList.Count; i++)
		// 	print(userList[i][0].partAns +"\n");

		if(quesList.Count > 3)
			print("question too much.");
	}

	public void teachNum( int index ){

		StartCoroutine(teaching(index));
		if(index == -1){
			selectionQues.SetActive(true);
			string question = "";
			for(int i = 0; i < 3; i++ ){
				for(int j = 0; j < quesList[i].question.Count; j++)
					question += quesList[i].question[j];
				selectionQues.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text = question;
				question = "";
			}
			selectionQues.SetActive(false);
		}
	}

	// public void teaching( List<QuesObj> questions, List<AnsObj> userAns, List<string> misConceptions ){
	public IEnumerator teaching( int index ){
		teachingPanel.SetActive(true);
		// teachPage.SetActive(true);
		if(index == -1){
			for(int i = 0; i < 3; i++){
				promptQues.SetActive(true);
				promptQues.transform.GetChild(2).GetComponent<Text>().text = "第 "+(i+1).ToString()+" 題";
				yield return new WaitForSeconds(4f);
				promptQues.SetActive(false);
				teachPage.SetActive(true);
				StartCoroutine(teachingProcess(quesList[i].question, quesList[i].answer, userList[i], misConceptions[i]));
				while(isTeaching)
					yield return new WaitForSeconds(0.1f);
				teachPage.SetActive(false);
			}
		}else{
			teachPage.SetActive(true);
			StartCoroutine(teachingProcess(quesList[index].question, quesList[index].answer, userList[index], misConceptions[index]));
			while(isTeaching)
				yield return new WaitForSeconds(0.1f);
			teachPage.SetActive(false);
		}
		// open panel and gave selection.
		selectionFunc.SetActive(true);
	}

	private IEnumerator teachingProcess( List<string> question, List<AnsObj> trueAns, List<AnsObj> userAns, string misConception ){
		isTeaching = true;
		List<string> left = new List<string>(question);
		List<string> right = new List<string>(question);
		string temp = "";
		// set mis field to prompt words
		switch(misConception){
			case "mis01":
				teachPage.transform.GetChild(9).GetComponent<Text>().text = "本次重點：括號裡面的運算要優先計算！";
				break;
			case "mis02":
				teachPage.transform.GetChild(9).GetComponent<Text>().text = "本次重點：先乘除，後加減！";
				break;
			case "mis03":
				teachPage.transform.GetChild(9).GetComponent<Text>().text = "本次重點：由左而右進行運算！";
				break;
			case "mis04":
				teachPage.transform.GetChild(9).GetComponent<Text>().text = "本次重點：除了答案也要注意計算的過程！";
				break;
			case "mis05":
				teachPage.transform.GetChild(9).GetComponent<Text>().text = "本次重點：填寫答案前記得檢查，確保沒有計算錯誤！";
				break;
			case "mis06":
				teachPage.transform.GetChild(9).GetComponent<Text>().text = "本次重點：注意加法和減法的運算概念不同！";
				break;
			case "mis07":
				teachPage.transform.GetChild(9).GetComponent<Text>().text = "本次重點：注意加法和乘法的運算概念不同！";
				break;
			case "mis08":
				teachPage.transform.GetChild(9).GetComponent<Text>().text = "本次重點：注意乘法和除法的運算概念不同！";
				break;
			case "mis09":
				teachPage.transform.GetChild(9).GetComponent<Text>().text = "本次重點：注意減法和除法的運算概念不同！";
				break;
		}

		// initial test question
		for(int i = 0; i < question.Count; i++){
			temp += question[i];
		}
		TrueAnsBefore.text = UserAnsBefore.text = temp;
		TrueAnsAfter.text = UserAnsAfter.text = "";
		yield return new WaitForSeconds(0f);

		// Count the loop by answer list length.
		for(int i = 0; i < trueAns.Count; i++){
			yield return new WaitForSeconds(2f);
			// a step of true answer.
			left[trueAns[i].index] = trueAns[i].partAns.ToString();
			left = caculatedNumProcess( left, trueAns[i].index );
			left = bracketsProcess(left, trueAns[i].index);
			temp = "";
			for(int j = 0; j < left.Count; j++){
				if(left[j] != "@")
					temp += left[j];
			}
			TrueAnsAfter.text = temp;

			yield return new WaitForSeconds(2f);
			// a step of user answer.
			right[userAns[i].index] = userAns[i].partAns.ToString();
			right = caculatedNumProcess( right, userAns[i].index );
			right = bracketsProcess(right, userAns[i].index);
			temp = "";
			for(int j = 0; j < right.Count; j++){
				if(right[j] != "@")
					if(right[j] == userAns[i].partAns.ToString())
						if(trueAns[i].partAns != userAns[i].partAns)
							temp += "<color=red>"+right[j]+"</color>";
						else
							temp += right[j];
					else
						temp += right[j];
			}

			if(TrueAnsAfter.text == temp)
				UserAnsAfter.text = temp;
			else
				UserAnsAfter.text = temp;//"<color=red>"+temp+"</color>";
				// Debug.Log("<color=red>"+temp+"</color>");

			yield return new WaitForSeconds(2f);
			if( i == trueAns.Count -1 ){
				// ...the last answer.
				yield return new WaitForSeconds(2f);

			}else{
				TrueAnsBefore.text = TrueAnsAfter.text;
				UserAnsBefore.text = UserAnsAfter.text;
				TrueAnsAfter.text = UserAnsAfter.text = "";
			}
		}
		isTeaching = false;
	}

	public void finishTeaching(){
		quesList.Clear();
		userList.Clear();
		misConceptions.Clear();
		teachingPanel.SetActive(false);
		SceneManager.LoadScene("Chapter"+GameObject.Find("Datas").GetComponent<DatasControl>().chapter.ToString());
	}

	private List<string> caculatedNumProcess( List<string> q, int operIndex ){
		List<string> question = new List<string>(q);
		int i = 0, j = 0;
		for(i = operIndex-1; i >= 0; i-- ){
			if(int.TryParse(question[i], out j)){
				question[i] = "@";
				break;
			}
		}
		for(i = operIndex+1; i < question.Count; i++ ){
			if(int.TryParse(question[i], out j)){
				question[i] = "@";
				break;
			}
		}
		return question;
	}

	private List<string> bracketsProcess( List<string> q, int operIndex ){
		List<string> question = new List<string>(q);
		int upBracket = -1, downBracket = -1, counter = 0, result;
		string temp = "";

		for(int i = 0; i < question.Count; i++){
			switch(question[i]){
				case "(":
					upBracket = i;
					counter = 0;
					break;
				case ")":
					downBracket = i;
					if(counter == 1){
						question[i] = "@";
						for(int j = i-1; j >= 0; j--){
							if(question[j] == "("){
								question[j] = "@";
								break;
							}
						}
					}else if(counter % 2 == 0){
						for(int j = i-1; j >= 0; j--){
							if(question[j] != "@"){
								if(int.TryParse(question[j], out result)){
									for(int k = upBracket-1; k >= 0; k--){
										if( question[k] != "@"){
											if(question[k] == ")"){
												question[k] = "@";
												question[upBracket] = "@";
												upBracket = -1;
											}else{
												temp = question[upBracket];
												question[upBracket] = question[k];
												question[k] = temp;
												upBracket = k;
											}
											break;
										}
									}
								}else{
									for(int k = i+1; k < question.Count; k++){
										if(question[k] != "@"){
											temp = question[i];
											question[i] = question[k];
											question[k] = temp;
											downBracket = k;
											i++;
										}
										break;
									}
								}
							break;
							}
						}
					}
					break;
				case "@":
					break;
				default:
					counter++;
					break;
			}
		}
		return question;
	}

}
