using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Npc : MonoBehaviour {

	public NpcInfo npcInfo;
	public GameObject TalkWindow;
	public Text title;
	private StageEvents stageEvents;
	public Button enterGame;

	// Use this for initialization
	void Start () {
		stageEvents = GameObject.Find("EventSystem").GetComponent<StageEvents>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void npcClicked(){
		// need to set npc.interactable = false;
		TalkWindow.SetActive(true);
		TalkWindow.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>().sprite = this.npcInfo.npcHeader;
		TalkWindow.transform.GetChild(1).GetComponentInChildren<Text>().text =  this.npcInfo.npcName + " :";
		for(int i = 0; i < this.npcInfo.plots.Count; i++){
			if(this.npcInfo.plots[i].isFinished)
				continue;
			if(stageEvents.userProgress+1 == this.npcInfo.plots[i].sequence){
				print("now plot.");
				TalkWindow.transform.GetChild(2).GetComponentInChildren<Text>().text = this.npcInfo.trueContents[this.npcInfo.plots[i].plotNumber];
				// events here.
				
				if(this.npcInfo.plots[i].gamePanel != null){
					stageEvents.setGamePanel(this.npcInfo.plots[i].gamePanel);
				}else{
					stageEvents.setGamePanel(null);
				}
				// this.npcInfo.plots[i].isFinished = true;
				break;
			}else{
				print("not finish");
				TalkWindow.transform.GetChild(2).GetComponentInChildren<Text>().text = this.npcInfo.falseContents[0];
			}
		}
	} 


}
