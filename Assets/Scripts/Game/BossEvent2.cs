using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossEvent2 : MonoBehaviour {
	private MathDatasControl MathDatas;
	private MisIdentify MisIdent;
	private DatasControl dataControl;

	private bool isClicked;
	private QuesObj question;

	// question setting
	public int maxNum, miniNum;
	public List<string> templates;
	private int selectedIndex;

	// game setting
	public GameObject Player, Boss, questionShield, OperatorPanel, CaculatePanel, AnswerPanel, BossLife, PlayerLife, Image_operator, GameReply;
	private float bossLife;
	private float playerLife;
	private int attackTime;
	private bool isTimeUp;

	// save user's answer to detect misConception.
	private List<string> questionShowList = new List<string>();
	private List<int> operatorsAndNumbers = new List<int>();
	private List<AnsObj> userAns = new List<AnsObj>();
	private AnsObj userAnsObj;

	// Use this for initialization
	void Start () {
		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		dataControl = GameObject.Find("Datas").GetComponent<DatasControl>();

		GameObject.Find("Image_main character").GetComponent<Animator>().Play("Image_character stage");
		
		bossLife = 1f;
		playerLife = 1f;
		// initial();

		if(dataControl.progress <= dataControl.nowStage)
			GameObject.Find("Image_BackToChapter").SetActive(false);

		StartCoroutine(Animation_BossAppear());
	}
	
	// Update is called once per frame
	void Update () {

	}

	private IEnumerator Animation_BossAppear(){
		yield return new WaitForSeconds(3.5f);
		StartCoroutine(createQuestionShield(miniNum, maxNum, templates));
	}

	private IEnumerator createQuestionShield(int mini, int max, List<string> t){
		Boss.GetComponent<Animator>().Play("Boss03_Standby");
		yield return new WaitForSeconds(2.5f);

		questionShield.SetActive(true);
		Boss.GetComponent<Animator>().Play("Boss04_GenerateQuestion");
		questionShield.transform.GetChild(0).gameObject.SetActive(true);
		questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		yield return new WaitForSeconds(2f);

		question = MathDatas.getQuestion(mini, max, t[UnityEngine.Random.Range(0, t.Count)]);

		string temp = "";
		for(int i = 0; i < question.question.Count; i++ ){
			questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text += question.question[i];
			switch(question.question[i]){
				case "(":
					temp += question.question[i];
				break;
				case "+":case "-":case "x":case "*":case "÷":case "/":
					questionShowList.Add(question.question[i]);
					operatorsAndNumbers.Add(i);
				break;
				default:
					temp += question.question[i];
					operatorsAndNumbers.Add(i);
					if(i+1<question.question.Count && question.question[i+1] == ")"){
						i++;
						questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text += question.question[i];
						temp += question.question[i];
					}
					questionShowList.Add(temp);
					temp = "";
				break;
			}
		}

		// debug questionShowList
		// for(int i = 0; i < operatorsAndNumbers.Count; i++){
		// 	print(i.ToString() + " " + operatorsAndNumbers[i] + "\n");
		// }

		// initial user's AnsObj
		userAnsObj = new AnsObj();
		// next state.
		StartCoroutine(playerRound());
	}

	private IEnumerator playerRound(){
		yield return new WaitForSeconds(0.5f);
		OperatorPanel.SetActive(true);
		while(OperatorPanel.GetComponent<Image>().fillAmount < 1){
			OperatorPanel.GetComponent<Image>().fillAmount += 0.05f;
			yield return 0;
		}
		OperatorPanel.transform.GetChild(0).gameObject.SetActive(true);
		// next state.
		StartCoroutine(selectOperator());
	}

	private IEnumerator selectOperator(){
		Image oper = GameObject.Find("Image_operator").GetComponent<Image>();
		char selected = '+';
		isClicked = false;
		StartCoroutine(detectClick());
		while(!isClicked){
			yield return new WaitForSeconds(1f);
			if(isClicked)
				break;
			switch(oper.sprite.name){
				case "plus":
					oper.sprite = Resources.Load<Sprite>("minus") as Sprite;
					selected = '-';
					break;
				case "minus":
					oper.sprite = Resources.Load<Sprite>("multipled") as Sprite;
					selected = 'x';
					break;
				case "multipled":
					oper.sprite = Resources.Load<Sprite>("divided") as Sprite;
					selected = '÷';
					break;
				case "divided":
					oper.sprite = Resources.Load<Sprite>("plus") as Sprite;
					selected = '+';
					break;
			}
		}
		if(selected == question.answer[0].operators || selected == question.answer[1].operators){
			userAnsObj.operators = selected;
			StartCoroutine(calculate(selected));
			oper.sprite = Resources.Load<Sprite>("plus") as Sprite;
		}else{
			print("no selected operator");
			isClicked = false;
			// play false anim, and restart selectOperator.
			Image_operator.SetActive(false);
			GameObject.Find("Image_energyBall").GetComponent<Animator>().Play("miss match");
			yield return new WaitForSeconds(1f);
			Image_operator.SetActive(true);
			GameObject.Find("Image_energyBall").GetComponent<Animator>().Play("Boss_energy");
			StartCoroutine(selectOperator());
		}
	}

	private IEnumerator detectClick(){
		while(!isClicked){
			yield return 0;
			if(Input.GetMouseButtonDown(0)){
				isClicked = true;
			}
		}
	}

	private IEnumerator calculate( char oper ){
		// print(oper);
		yield return new WaitForSeconds(0f);
		CaculatePanel.SetActive(true);

		// set question selection.
		for(int i = 0; i < questionShowList.Count; i++){
			if(questionShowList[i] == "@"){
				if( i % 2 == 0 ){
					GameObject.Find("Text" + i.ToString()).GetComponent<Text>().text = "";
				}else{
					GameObject.Find("Button_oper" + i.ToString()).SetActive(false);
				}
				continue;
			}else{
				if( i % 2 != 0 ){
					// operator buttons
					switch(questionShowList[i]){
						case "+":
							GameObject.Find("Button_oper" + i.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("plusButton") as Sprite;
						break;
						case "-":
							GameObject.Find("Button_oper" + i.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("minusButton") as Sprite;
						break;
						case "x":case "*":
							GameObject.Find("Button_oper" + i.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("multipledButton") as Sprite;
						break;
						case "÷":case "/":
							GameObject.Find("Button_oper" + i.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("dividedButton") as Sprite;
						break;
					}
					if(questionShowList[i] != oper.ToString()){
						GameObject.Find("Button_oper" + i.ToString()).GetComponent<Button>().interactable = false;
					}else{
						GameObject.Find("Button_oper" + i.ToString()).GetComponent<Button>().interactable = true;
					}
				}else{
					// number text
					GameObject.Find("Text" + i.ToString()).GetComponent<Text>().text = questionShowList[i];
				}
			}
		}
	}

	public void userAnswer( int quesIndex ){
		selectedIndex = quesIndex;
		AnswerPanel.SetActive(true);
		
		if(userAns.Count == 0)
			GameObject.Find("Text_ques").GetComponent<Text>().text = questionShowList[quesIndex-1] + " " + questionShowList[quesIndex] + " " + questionShowList[quesIndex+1] + " =";
		else{
			string partQuestion = "";
			// for(int i = 0; i < questionShowList.Count; i++)
			// 	print(questionShowList[i]);
			for(int i = quesIndex-1; i >= 0; i--){
				if(questionShowList[i] != "@"){
					partQuestion += questionShowList[i];
					break;
				}
			}
			partQuestion += questionShowList[quesIndex];
			for(int i = quesIndex+1; i < questionShowList.Count; i++){
				if(questionShowList[i] != "@"){
					partQuestion += questionShowList[i];
					break;
				}
			}
			partQuestion += "= ";
			GameObject.Find("Text_ques").GetComponent<Text>().text = partQuestion;
		}

		int.TryParse(question.question[operatorsAndNumbers[quesIndex-1]] ,out userAnsObj.numA);
		int.TryParse(question.question[operatorsAndNumbers[quesIndex+1]] ,out userAnsObj.numB);
		userAnsObj.index = operatorsAndNumbers[quesIndex];

		CaculatePanel.SetActive(false);
		OperatorPanel.SetActive(false);
	}

	public void numberClick( string num ){
		Text t = GameObject.Find("Text_userans").GetComponent<Text>();
		if(t.text == "ANS")
			t.text = "";
		switch(num){
			case "1":
				t.text += "1";
				break;
			case "2":
				t.text += "2";
				break;
			case "3":
				t.text += "3";
				break;
			case "4":
				t.text += "4";
				break;
			case "5":
				t.text += "5";
				break;
			case "6":
				t.text += "6";
				break;
			case "7":
				t.text += "7";
				break;
			case "8":
				t.text += "8";
				break;
			case "9":
				t.text += "9";
				break;
			case "0":
				t.text += "0";
				break;
			case "c":
				t.text = "";
				break;
			default:
				print("other input...");
				break;
		}
	}

	public void checkAns(){
		bool b = int.TryParse(GameObject.Find("Text_userans").GetComponent<Text>().text, out userAnsObj.partAns);
		// userAns.Clear();
		if(b){
			userAns.Add(userAnsObj);

			if(userAns.Count == question.answer.Count){
				print("finish a question.");

				List<string> misConceptions = GameObject.Find("EventSystem").GetComponent<MisIdentify>().getMisConception(question.answer, userAns);
				// print(userAnsObj.partAns.ToString() + " / " + question.answer[1].partAns);
				if(userAnsObj.partAns == question.answer[1].partAns){
					print("correct!");

					GameObject.Find("Datas").GetComponent<DatasControl>().getGameHistoryData(question.question, question.answer, userAns, true, misConceptions);
					StartCoroutine(BreakShield());
				}else{
					print("wrong");

					GameObject.Find("Datas").GetComponent<DatasControl>().getGameHistoryData(question.question, question.answer, userAns, false, misConceptions);
					
					StartCoroutine(bossAttackPlayer());
				}
				// initial
				questionShowList.Clear();
				operatorsAndNumbers.Clear();
				CaculatePanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
				CaculatePanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
				userAns.Clear();
			}else{
				// print(userAnsObj.partAns.ToString());

				questionShowList[selectedIndex-1] = "@";
				questionShowList[selectedIndex] = "@";
				questionShowList[selectedIndex+1] = userAnsObj.partAns.ToString();
				
				questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
				for(int i = 0; i < questionShowList.Count; i++ ){
					if(questionShowList[i] != "@")
						questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text += questionShowList[i];
				}
				StartCoroutine(playerRound());
			}
			GameObject.Find("Text_userans").GetComponent<Text>().text = "ANS";
			
			userAnsObj = new AnsObj();
			AnswerPanel.SetActive(false);
		}else{
			print("error : not a num.");

		}
	}

// ...

	private IEnumerator BreakShield(){
		yield return new WaitForSeconds(0.2f);
		GameObject.Find("Boss Wolf").GetComponent<Animator>().Play("Boss06_BreakShield");
		yield return new WaitForSeconds(1.2f);

		StartCoroutine(bossAttackable());
	}

	private IEnumerator bossAttackable(){
		// Boss.GetComponent<Animator>().Play("Boss06_BeAttackable");

		attackTime = 10;
		isTimeUp = false;
		InvokeRepeating("timer", 0f, 1f);
		BossLife.SetActive(true);
		// can be attack.
		while(attackTime > 0 && !isTimeUp){
			if(Input.GetMouseButtonDown(0)){
				// print("attacked");
				GameObject.Find("Image_main character").GetComponent<Animator>().Play("Image_character attack", -1, 0f);
				Boss.GetComponent<Animator>().Play("Boss07_BeAttacked", -1, 0f);
				bossLife -= 0.01f;
				BossLife.transform.GetChild(0).GetComponent<Image>().fillAmount = bossLife;
			}
			yield return 0;
		}
		CancelInvoke();
		GameObject.Find("Image_main character").GetComponent<Animator>().Play("Image_character stage");

		// check if game end.
		if(bossLife <= 0){
			print("finish game");
			// GameObject.Find("IBoss Wolf").GetComponent<Animator>().Play("Boss05_playerWin");
			GameReply.SetActive(true);
			GameReply.transform.GetChild(0).GetComponent<Text>().text = "恭喜擊敗了\n大裝甲胖野狼";
			if(dataControl.progress == dataControl.nowStage)
				dataControl.progress += 1;
		}else{
			BossLife.SetActive(false);
			StartCoroutine(createQuestionShield(miniNum, maxNum, templates));
		}
	}

	private IEnumerator bossAttackPlayer(){
		Boss.GetComponent<Animator>().Play("Boss05_BossAttack");
		GameObject.Find("Image_main character").GetComponent<Animator>().Play("Image_character_be attacked");
		playerLife -= 0.3f;
		yield return new WaitForSeconds(1.2f);
		while(GameObject.Find("Image_LifeDGC").GetComponent<Image>().fillAmount > playerLife){
			GameObject.Find("Image_LifeDGC").GetComponent<Image>().fillAmount -= 0.02f;
			yield return new WaitForSeconds(0.1f);
		}
		if(playerLife <= 0f){
			GameReply.SetActive(true);
			GameReply.transform.GetChild(0).GetComponent<Text>().text = "挑戰失敗了...\n整頓心情再挑戰看看吧！";
		}
		GameObject.Find("Image_main character").GetComponent<Animator>().Play("Image_character_stand by");
		// if user life = 0?

		StartCoroutine(createQuestionShield(miniNum, maxNum, templates));
	}

	private void timer(){
		attackTime -= 1;
		// print(attackTime);
		if(attackTime == -1){
			isTimeUp = true;
		}
	}

	public void backToChapter(){
		SceneManager.LoadScene("Chapter"+dataControl.chapter.ToString());
	}


}
