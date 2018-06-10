using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Npc : MonoBehaviour {

	public NpcInfo npcInfo;
	private StageEvents stageEvents;
	private bool isTalked = false;

	// Use this for initialization
	void Start () {
		stageEvents = GameObject.Find("EventSystem").GetComponent<StageEvents>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void npcClicked(){
		if(!Collision.isCollision){
			stageEvents.setGameInfo(null, null, null, Resources.Load<Sprite>("DGC") as Sprite, "地瓜豬 :", "距離太遠了...", false);
		}else{
			for(int i = 0; i < this.npcInfo.plots.Count; i++){
				if(stageEvents.userProgress+1 == this.npcInfo.plots[i].sequence){
					stageEvents.setGameInfo(this.gameObject, this.npcInfo.plots[i].changeGameObj, this.npcInfo.plots[i].gamePanel, this.npcInfo.npcHeader, this.npcInfo.npcName + " :", this.npcInfo.trueContents[this.npcInfo.plots[i].plotNumber], true);
					break;
				}else{
					stageEvents.setGameInfo(null, null, null, this.npcInfo.npcHeader, this.npcInfo.npcName + " :", this.npcInfo.falseContents[0], false);
				}
			}
			if(this.npcInfo.plots.Count == 0)
				stageEvents.setGameInfo(null, null, null, this.npcInfo.npcHeader, this.npcInfo.npcName + " :", this.npcInfo.falseContents[0], false);
		}
	} 

	public void specialTalk(){
		if(!this.isTalked){
			stageEvents.setGameInfo( null, null, null, this.npcInfo.npcHeader, this.npcInfo.npcName + " :", this.npcInfo.trueContents[0], false);
			this.isTalked = true;
		}else{
			stageEvents.setGameInfo( null, null, null, this.npcInfo.npcHeader, this.npcInfo.npcName + " :", this.npcInfo.falseContents[0], false);
		}
	}
	
}
