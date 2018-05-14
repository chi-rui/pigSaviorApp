using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class BossEvents1 : MonoBehaviour {
	private MathDatasControl MathDatas;
	private GameObject questionShield, OperatorPanel, CaculatePanel, AnswerPanel, BossLife, PlayerLife;
	private bool isClicked;
	private QuesObj question;

	// question setting
	public int maxNum;
	public List<string> templates;

	// game setting
	private float bossLife;
	private float playerLife;

	// user select for answer
	private int operIndex;
	private int attackTime;
	private bool isTimeUp;

	// Use this for initialization
	void Start () {
		// set used variables.
		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		questionShield = GameObject.Find("Image_shield");
		questionShield.SetActive(false);
		OperatorPanel = GameObject.Find("Panel_operator");
		OperatorPanel.SetActive(false);
		CaculatePanel = GameObject.Find("Panel_calculate");
		CaculatePanel.SetActive(false);
		AnswerPanel = GameObject.Find("Panel_enterAns");
		AnswerPanel.SetActive(false);		
		BossLife = GameObject.Find("Image_LifeBoss");
		BossLife.SetActive(false);


		bossLife = 1f;
		playerLife = 1f;

		initial();

		// set Boss and get a question.
		StartCoroutine(Animation_BossAppear());
		// StartCoroutine(caculate());

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0) && !isClicked){
			isClicked = true;
		}
	}

	private IEnumerator Animation_BossAppear(){
		// appear animation.
		yield return new WaitForSeconds(3f);

		// next state.
		StartCoroutine(createQuestionShield(maxNum, templates));
	}

	private IEnumerator createQuestionShield(int max, List<string> t){
		yield return new WaitForSeconds(1f);
		// create shield for boss.
		questionShield.SetActive(true);
		// while(questionShield.GetComponent<Image>().fillAmount < 1){
		// 	questionShield.GetComponent<Image>().fillAmount += 0.01f;
		// 	questionShield.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value =1;//+= 0.01f;
		// 	yield return 0;
		// }
		GameObject.Find("Image_Boss").GetComponent<Animator>().Play("Boss02_createShield");
		yield return new WaitForSeconds(2f);
		// get a question object for the shield.
		question = MathDatas.getQuestion(max, t[UnityEngine.Random.Range(0, t.Count)]);
		for(int i = 0; i < question.question.Count; i++ ){
			questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text += question.question[i];
		}

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
		if(selected == question.answer[0].operators){
			StartCoroutine(calculate(selected));
			// initial.
			GameObject.Find("Image_operator").GetComponent<Image>().sprite = Resources.Load<Sprite>("plus") as Sprite;
		}else{
			print("no selected operator");
			isClicked = false;
			// play false anim, and restart selectOperator. 
			StartCoroutine(selectOperator());			
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
		operIndex = quesIndex;
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
		int ans = -1;
		bool b = int.TryParse(GameObject.Find("Text_userans").GetComponent<Text>().text, out ans);
		if(b){
			if(ans == question.answer[0].partAns){
				print("correct");
				GameObject.Find("Text_userans").GetComponent<Text>().text = "";
				StartCoroutine(BreakShield());
			}else{
				print("your ans is " + ans.ToString() + ", wrong.");
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
		initial();
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
				print("attacked");
				GameObject.Find("Image_main_character").GetComponent<Animator>().Play("Image_character attack", -1, 0f);
				GameObject.Find("Image_Boss").GetComponent<Animator>().Play("Boss04_beAttacked", -1, 0f);
				bossLife -= 0.01f;
				BossLife.GetComponent<Image>().fillAmount = bossLife;
			}
			yield return 0;
		}
		CancelInvoke();
		GameObject.Find("Image_main_character").GetComponent<Animator>().Play("Image_character stage");

		// check if game end.
		if(bossLife <= 0){
			print("finish game");
			GameObject.Find("Image_Boss").GetComponent<Animator>().Play("Boss05_playerWin");
		}else{
			BossLife.SetActive(false);
			StartCoroutine(createQuestionShield(maxNum, templates));
		}
	}

	private void timer(){
		attackTime -= 1;
		print(attackTime);
		if(attackTime == -1){
			isTimeUp = true;
		}
	}

	private void initial(){
		questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		isClicked = false;
		operIndex = -1;
	}

}
