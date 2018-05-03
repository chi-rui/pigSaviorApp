using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickMode : MonoBehaviour {

	public GameObject item, GamePanel;
	private int sumofitems = 0;
	private int itemsPicked = 0;
	private int answer;
	public string itemName;
	private StageEvents stageEvents;
	bool wait = false;

	// Use this for initialization
	void Start () {
		stageEvents = GameObject.Find("EventSystem").GetComponent<StageEvents>();
	}

	// execute when gameobject is Active. 
	void OnEnable() {
		answer = UnityEngine.Random.Range(1,9);
		this.transform.GetChild(1).GetComponentInChildren<Text>().text = "請採摘 " + answer + " 個" + this.itemName +  "！" ;
		GameObject[] clones = GameObject.FindGameObjectsWithTag("clone");
		for(int i = 0; i < clones.Length; i++)
			Destroy(clones[i]);
		for(int i = 0; i < 5; i++)
			setItems();
		initial();
	}

	// Update is called once per frame
	void Update () {
		if(sumofitems < 15 && !wait){
			float t = Random.Range(0.5f, 2.5f);
			StartCoroutine(itemGrow(t));
			wait = true;
		}
	}

	void setItems(){
		item.SetActive(true);
		float x = UnityEngine.Random.Range(this.transform.GetChild(2).GetChild(0).position.x-400,this.transform.GetChild(2).GetChild(0).position.x+400);
		float y = UnityEngine.Random.Range(this.transform.GetChild(2).position.y-200, this.transform.GetChild(2).position.y+200);
		var b = Instantiate(this.transform.GetChild(2).GetChild(1),new Vector3(x, y, 0), Quaternion.identity, this.transform.GetChild(2));
		b.gameObject.tag = "clone";
		this.sumofitems++;
		item.SetActive(false);
	}

	public void pickitem( GameObject item ){
		this.itemsPicked++;
		this.sumofitems--;
		Destroy(item);
		this.transform.GetChild(3).GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = "x " + itemsPicked.ToString();
	}

	IEnumerator itemGrow( float time ){
		yield return new WaitForSeconds(time);
		setItems();
		wait = false;
	}

	IEnumerator gameFinish( float time ){
		yield return new WaitForSeconds(time);
		stageEvents.nextProgress();
		GamePanel.SetActive(false);
	}

	public void checkAns(){
		if(this.itemsPicked == this.answer){
			stageEvents.showFeedBack(true);
			StartCoroutine(gameFinish(2f));
			
		}else{
			stageEvents.showFeedBack(false);
			initial();
		}
	}

	public void initial(){
		this.itemsPicked = 0;
		this.transform.GetChild(3).GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = "x " + itemsPicked.ToString();
	}
}
