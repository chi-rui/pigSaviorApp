using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NpcClicked : MonoBehaviour {

	public GameObject TalkWindow;
	public Text title;
	public List<string> correctPlot;
	public List<string> wrongPlot;
	public string npc;
	public List<RPGPlot> plots;
	// public int progress;
	public DataController dataController;
	// Use this for initialization
	void Start () {
		dataController = GameObject.Find("EventSystem").GetComponent<DataController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void npcClicked(){
		TalkWindow.SetActive(true);
		TalkWindow.transform.GetChild(1).GetComponentInChildren<Text>().text =  this.npc;
		for(int i = 0; i < plots.Count; i++){
			if(dataController.userProgress+1 > plots[i].sequence){
				print("this plot is finished.");
				TalkWindow.transform.GetChild(2).GetComponentInChildren<Text>().text = this.wrongPlot[0];
				break;
			}else if(dataController.userProgress+1 == plots[i].sequence){
				print("now plot.");
				TalkWindow.transform.GetChild(2).GetComponentInChildren<Text>().text = this.correctPlot[plots[i].plotNumber];
				dataController.nextPlot();
				break;
			}else{
				print("not finish");
				TalkWindow.transform.GetChild(2).GetComponentInChildren<Text>().text = this.wrongPlot[0];
			}
		}

	} 
}
