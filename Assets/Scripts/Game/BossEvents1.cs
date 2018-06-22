using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class BossEvents1 : MonoBehaviour {
	private MathDatasControl MathDatas;
	private MisIdentify MisIdent;
	private DatasControl dataControl;
	public GameObject questionShield, OperatorPanel, CaculatePanel, AnswerPanel, BossLife, PlayerLife, teachPanel, Boss, Image_operator, GameReply;
	private bool isClicked;
	private QuesObj question;

	// question setting
	public int maxNum, miniNum;
	public List<string> templates;

	// game setting
	private float bossLife;
	private float playerLife;

	// user select for answer
	// private int operIndex;
	private int attackTime;
	private bool isTimeUp;

	// save user's answer to detect misConception.
	private List<AnsObj> userAns = new List<AnsObj>();
	private AnsObj userAnsObj;

	// Use this for initialization
	void Start () {
		// set used variables.
		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		dataControl = GameObject.Find("Datas").GetComponent<DatasControl>();
		// MisIdent = GameObject.Find("EventSystem").GetComponent<MisIdentify>();
		// questionShield = GameObject.Find("Image_shield");
		// questionShield.SetActive(false);
		// OperatorPanel = GameObject.Find("Panel_operator");
		// OperatorPanel.SetActive(false);
		// CaculatePanel = GameObject.Find("Panel_calculate");
		// CaculatePanel.SetActive(false);
		// AnswerPanel = GameObject.Find("Panel_enterAns");
		// AnswerPanel.SetActive(false);		
		// BossLife = GameObject.Find("Image_LifeBoss");
		// BossLife.SetActive(false);

		GameObject.Find("Image_main_character").GetComponent<Animator>().Play("Image_character_stand by");

		bossLife = 1f;
		playerLife = 1f;

		if(dataControl.progress <= dataControl.nowStage)
			GameObject.Find("Image_BackToChapter").SetActive(false);

		initial();

		// set Boss and get a question.
		StartCoroutine(teachBoss());
	}
	
	// Update is called once per frame
	void Update () {

	}

	private IEnumerator teachBoss(){
		if(GameObject.Find("Datas").GetComponent<DatasControl>().progress < 5){
			teachPanel.SetActive(true);
			while(teachPanel.activeInHierarchy){
				yield return new WaitForSeconds(0.1f);
			}
			teachPanel.SetActive(false);
		}
		Boss.SetActive(true);
		StartCoroutine(Animation_BossAppear());
	}

	private IEnumerator Animation_BossAppear(){
		// appear animation.
		yield return new WaitForSeconds(3f);

		// next state.
		StartCoroutine(createQuestionShield(miniNum, maxNum, templates));
	}

	private IEnumerator createQuestionShield(int mini, int max, List<string> t){
		yield return new WaitForSeconds(1f);
		// create shield for boss.
		questionShield.SetActive(true);
		GameObject.Find("Image_Boss").GetComponent<Animator>().Play("Boss02_createShield");
		questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		yield return new WaitForSeconds(2f);
		// get a question object for the shield.
		question = MathDatas.getQuestion(mini, max, t[UnityEngine.Random.Range(0, t.Count)]);
		for(int i = 0; i < question.question.Count; i++ ){
			questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text += question.question[i];
		}
		// initial user's AnsObj
		userAnsObj = new AnsObj();
		// next state.
		StartCoroutine(playerRound());
	}

	private IEnumerator playerRound(){
		yield return new WaitForSeconds(1f);
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
		userAnsObj.operators = selected;
		if(selected == question.answer[0].operators){
			StartCoroutine(calculate(selected));
			// initial.
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
		print(oper);
		yield return new WaitForSeconds(0f);
		CaculatePanel.SetActive(true);
		
		// set question selection.
		GameObject.Find("Text_A").GetComponent<Text>().text = question.question[0];
		switch(oper){
			case '+':
				GameObject.Find("Button_oper").GetComponent<Image>().sprite = Resources.Load<Sprite>("plusButton") as Sprite;
				break;
			case '-':
				GameObject.Find("Button_oper").GetComponent<Image>().sprite = Resources.Load<Sprite>("minusButton") as Sprite;
				break;
			case 'x':
				GameObject.Find("Button_oper").GetComponent<Image>().sprite = Resources.Load<Sprite>("multipledButton") as Sprite;
				break;
			case '÷':
				GameObject.Find("Button_oper").GetComponent<Image>().sprite = Resources.Load<Sprite>("dividedButton") as Sprite;
				break;
		}
		GameObject.Find("Text_B").GetComponent<Text>().text = question.question[2];
	}

	public void userAnswer( int quesIndex ){
		AnswerPanel.SetActive(true);
		GameObject.Find("Text_ques").GetComponent<Text>().text = question.question[quesIndex-1] + " " + question.question[quesIndex] + " " + question.question[quesIndex+1] + " =";
		int.TryParse(question.question[quesIndex-1] ,out userAnsObj.numA);
		int.TryParse(question.question[quesIndex+1] ,out userAnsObj.numB);
		userAnsObj.index = quesIndex;
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
		// List<string> misConception = MisIdent.getMisConception(question.answer, );

		// int ans = -1;
		bool b = int.TryParse(GameObject.Find("Text_userans").GetComponent<Text>().text, out userAnsObj.partAns);
		userAns.Clear();
		if(b){
			userAns.Add(userAnsObj);
			List<string> misConceptions = GameObject.Find("EventSystem").GetComponent<MisIdentify>().getMisConception(question.answer, userAns);
			if(userAnsObj.partAns == question.answer[0].partAns){
				// print("correct");
				GameObject.Find("Text_userans").GetComponent<Text>().text = "";
				GameObject.Find("Datas").GetComponent<DatasControl>().getGameHistoryData(question.question, question.answer, userAns, true, misConceptions);
				StartCoroutine(BreakShield());
			}else{
				// print("your ans is " + userAnsObj.partAns.ToString() + ", wrong.");
				// ... boss attack and new question
				GameObject.Find("Datas").GetComponent<DatasControl>().getGameHistoryData(question.question, question.answer, userAns, false, misConceptions);
				StartCoroutine(bossAttackPlayer());
			}
		}else{
			print("error : not a num.");

		}
	}

	private IEnumerator BreakShield(){
		AnswerPanel.SetActive(false);
		OperatorPanel.SetActive(false);
		CaculatePanel.SetActive(false);
		yield return new WaitForSeconds(1f);
		GameObject.Find("Image_Boss").GetComponent<Animator>().Play("Boss03_shieldBreak");
		yield return new WaitForSeconds(2.5f);
		// initial();
		StartCoroutine(bossAttackable());
	}

	private IEnumerator bossAttackable(){
		attackTime = 10;
		isTimeUp = false;
		InvokeRepeating("timer", 0f, 1f);
		BossLife.SetActive(true);
		// can be attack.
		while(attackTime > 0 && !isTimeUp){
			if(Input.GetMouseButtonDown(0)){
				// print("attacked");
				GameObject.Find("Image_main_character").GetComponent<Animator>().Play("Image_character attack", -1, 0f);
				GameObject.Find("Image_Boss").GetComponent<Animator>().Play("Boss04_beAttacked", -1, 0f);
				bossLife -= 0.01f;
				BossLife.transform.GetChild(0).GetComponent<Image>().fillAmount = bossLife;
			}
			yield return 0;
		}
		CancelInvoke();
		GameObject.Find("Image_main_character").GetComponent<Animator>().Play("Image_character stage");

		// check if game end.
		if(bossLife <= 0){
			print("finish game");
			GameObject.Find("Image_Boss").GetComponent<Animator>().Play("Boss05_playerWin");
			GameReply.SetActive(true);
			GameReply.transform.GetChild(0).GetComponent<Text>().text = "恭喜戰勝了\n傳說級豬兔兔";
			if(dataControl.progress == dataControl.nowStage)
				dataControl.progress += 1;
		}else{
			BossLife.SetActive(false);
			StartCoroutine(createQuestionShield(miniNum, maxNum, templates));
		}
	}

	private IEnumerator bossAttackPlayer(){
		GameObject.Find("Text_userans").GetComponent<Text>().text = "ANS";
		AnswerPanel.SetActive(false);
		OperatorPanel.SetActive(false);
		CaculatePanel.SetActive(false);
		GameObject.Find("Image_Boss").GetComponent<Animator>().Play("Boss06_attack");
		GameObject.Find("Image_main_character").GetComponent<Animator>().Play("Image_character_be attacked");
		playerLife -= 0.3f;
		yield return new WaitForSeconds(1.5f);
		while(GameObject.Find("Image_LifeDGC").GetComponent<Image>().fillAmount > playerLife){
			GameObject.Find("Image_LifeDGC").GetComponent<Image>().fillAmount -= 0.02f;
			yield return new WaitForSeconds(0.1f);
		}
		if(playerLife <= 0f){
			GameReply.SetActive(true);
			GameReply.transform.GetChild(0).GetComponent<Text>().text = "挑戰失敗了...\n整頓心情再挑戰看看吧！";
		}
		GameObject.Find("Image_main_character").GetComponent<Animator>().Play("Image_character_stand by");
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

	private void initial(){
		// questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		// isClicked = false;
		// operIndex = -1;
	}

	public void backToChapter(){
		SceneManager.LoadScene("Chapter"+dataControl.chapter.ToString());
	}

}
