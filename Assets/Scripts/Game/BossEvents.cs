using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEvents : MonoBehaviour {
	private MathDatasControl MathDatas;
	private GameObject CaculatePanel, questionShield;
	// private Image oper;
	private bool isClicked;
	private QuesObj question;

	// question setting.
	public int maxNum;
	public List<string> templates;

	// Use this for initialization
	void Start () {
		// set used variables.
		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		CaculatePanel = GameObject.Find("Panel_caculate");
		CaculatePanel.SetActive(false);
		questionShield = GameObject.Find("Image_shield");
		questionShield.SetActive(false);
		isClicked = false;

		// set Boss and get a question.
		StartCoroutine(Animation_BossAppear());

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
		while(questionShield.GetComponent<Image>().fillAmount < 1){
			questionShield.GetComponent<Image>().fillAmount += 0.01f;
			questionShield.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value += 0.01f;
			yield return 0;
		}
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
		CaculatePanel.SetActive(true);
		while(CaculatePanel.GetComponent<Image>().fillAmount < 1){
			CaculatePanel.GetComponent<Image>().fillAmount += 0.05f;
			yield return 0;
		}
		CaculatePanel.transform.GetChild(0).gameObject.SetActive(true);
		// next state.
		StartCoroutine(AnimateOperator());
	}

	private IEnumerator AnimateOperator(){
		Image oper = GameObject.Find("Image_operator").GetComponent<Image>();
		// print("in");
		while(!isClicked){
			yield return new WaitForSeconds(1f);
			if(isClicked)
				break;
			switch(oper.sprite.name){
				case "plus":
					oper.sprite = Resources.Load<Sprite>("minus") as Sprite;
					break;
				case "minus":
					oper.sprite = Resources.Load<Sprite>("multipled") as Sprite;
					break;
				case "multipled":
					oper.sprite = Resources.Load<Sprite>("divided") as Sprite;
					break;
				case "divided":
					oper.sprite = Resources.Load<Sprite>("plus") as Sprite;
					break;
			}
		}


		// ...
	}

	// public void SelectOperator( int oper ){
	// 	GameObject energy = GameObject.Find("Image_energyBall");
	// 	switch(oper){
	// 		case 1:
	// 			energy.GetComponent<Image>().color = Color.Lerp(energy.GetComponent<Image>().color ,Color.red, 5f);
	// 			break;
	// 		case 2:
	// 			energy.GetComponent<Image>().color = Color.Lerp(energy.GetComponent<Image>().color ,Color.yellow, 5f);
	// 			break;
	// 		case 3:
	// 			energy.GetComponent<Image>().color = Color.Lerp(energy.GetComponent<Image>().color ,Color.blue, 5f);
	// 			break;
	// 		case 4:
	// 			energy.GetComponent<Image>().color = Color.Lerp(energy.GetComponent<Image>().color ,Color.green, 5f);
	// 			break;
	// 		default:
	// 			energy.GetComponent<Image>().color = Color.Lerp(energy.GetComponent<Image>().color ,Color.white, 5f);
	// 			break;
	// 	}
	// }

	private IEnumerator waitAnimation( float time ){
		// ...
		

		// // break shield and initializations. (next question)
		yield return new WaitForSeconds(time);
		// questionShield.GetComponent<Image>().fillAmount = 0f;
		// questionShield.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = 0f;
		// questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		// questionShield.SetActive(false);	
		// CaculatePanel.GetComponent<Image>().fillAmount = 0f;
		// // regeneration of question shield.
		// StartCoroutine(createQuestionShield(maxNum, templates));
	}
}
