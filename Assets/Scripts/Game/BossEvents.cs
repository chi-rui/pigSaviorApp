using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossEvents : MonoBehaviour {
	private MathDatasControl MathDatas;
	private GameObject questionShield, OperatorPanel, CaculatePanel;
	// private Image oper;
	private bool isClicked;
	private QuesObj question;

	// question setting.
	public int maxNum;
	public List<string> templates;


	public Camera camera;

	// Use this for initialization
	void Start () {
		// set used variables.
		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();
		questionShield = GameObject.Find("Image_shield");
		questionShield.SetActive(false);
		OperatorPanel = GameObject.Find("Panel_operator");
		OperatorPanel.SetActive(false);
		CaculatePanel = GameObject.Find("Panel_caculate");
		// CaculatePanel.SetActive(false);

		isClicked = false;

		// set Boss and get a question.
		// StartCoroutine(Animation_BossAppear());
		StartCoroutine(caculate());

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

	private IEnumerator caculate(){
		yield return new WaitForSeconds(0f);
		Text t = GameObject.Find("Text_test").GetComponent<Text>();//.text;//.ToCharArray();
		Font font = t.font;
		CharacterInfo characterInfo = new CharacterInfo();
		string text = t.text;
		float position = 0;
		float seat = 0;
		GameObject b = GameObject.Find("Button_plus");
		b.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
		b.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);

		for( int i = 0; i < text.Length; i++ ){
			font.GetCharacterInfo(text[i], out characterInfo, t.fontSize);
			position += characterInfo.advance;
			switch(text[i]){
				case '＋':
					seat = position;
					print(position);
					break;
				default:
					break;
			}			
			b.transform.position = new Vector3((600 - seat)*-1-5, 200f, 0f);//camera.WorldToScreenPoint(new Vector3(318f, -200f, 0f));
		}
	}

	private IEnumerator waitAnimation( float time ){
		// ...
		

		// // break shield and initializations. (next question)
		yield return new WaitForSeconds(time);
		// questionShield.GetComponent<Image>().fillAmount = 0f;
		// questionShield.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = 0f;
		// questionShield.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
		// questionShield.SetActive(false);	
		// OperatorPanel.GetComponent<Image>().fillAmount = 0f;
		// // regeneration of question shield.
		// StartCoroutine(createQuestionShield(maxNum, templates));
	}
}
