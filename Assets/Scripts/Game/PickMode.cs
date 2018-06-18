using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickMode : MonoBehaviour {

	public GameObject item, GamePanel;
	public string itemName;
	public bool isFirst;
	private int sumofitems = 0, itemsPicked = 0, answer;
	private StageEvents stageEvents;
	private bool wait = false;

	// Use this for initialization
	void Start () {
		stageEvents = GameObject.Find("EventSystem").GetComponent<StageEvents>();
		DatasControl.GameMode = "PICK";
	}

	// execute when gameobject is Active. 
	void OnEnable() {
		answer = UnityEngine.Random.Range(1,9);
		this.transform.GetChild(1).GetComponentInChildren<Text>().text = "請採摘 " + answer + " 個" + this.itemName +  "！" ;
		GameObject[] clones = GameObject.FindGameObjectsWithTag("clone");
		for(int i = 0; i < clones.Length; i++)
			Destroy(clones[i]);
		for(int i = 0; i < 10; i++)
			setItems();
		initial();
	}

	// Update is called once per frame
	void Update () {
		if(sumofitems < 10 && !wait){
			float t = Random.Range(0.5f, 2.5f);
			StartCoroutine(itemGrow(t));
			wait = true;
		}
	}

	void setItems(){
		item.SetActive(true);
		float x = UnityEngine.Random.Range(this.transform.GetChild(2).GetChild(0).position.x-400,this.transform.GetChild(2).GetChild(0).position.x+400);
		// float x = UnityEngine.Random.Range(-600f,500f);
		float y = UnityEngine.Random.Range(this.transform.GetChild(2).GetChild(0).position.y-200, this.transform.GetChild(2).GetChild(0).position.y+200);
		// float y = UnityEngine.Random.Range(400f, -400f);
		var b = Instantiate(this.transform.GetChild(2).GetChild(1),new Vector3(x, y, 0), Quaternion.identity, this.transform.GetChild(2));
		b.gameObject.transform.position = new Vector3(x, y, 0);
		// print(b.gameObject.transform.position);
		b.gameObject.tag = "clone";
		// b.gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
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
		this.GamePanel.SetActive(false);
	}

	public void checkAns(){
		StartCoroutine(gameFinish(2f));
		if(this.itemsPicked == this.answer){
			stageEvents.showFeedBack(true, "");
		}else{
			stageEvents.showFeedBack(false, "數量好像不太正確喔！");
			initial();
		}
	}

	public void initial(){
		this.itemsPicked = 0;
		this.transform.GetChild(3).GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = "x " + itemsPicked.ToString();
	}
}
