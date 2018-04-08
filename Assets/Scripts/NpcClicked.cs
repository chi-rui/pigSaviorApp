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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void npcClicked(){
		TalkWindow.SetActive(true);
		TalkWindow.transform.GetChild(1).GetComponentInChildren<Text>().text =  this.npc;
		TalkWindow.transform.GetChild(2).GetComponentInChildren<Text>().text =  this.correctPlot[0];
	} 
}
