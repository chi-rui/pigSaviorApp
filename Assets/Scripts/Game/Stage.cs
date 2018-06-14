using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour {
	
	public StageInfo stageInfo;
	private DatasControl datas;

	// Use this for initialization
	void Start () {
		datas = GameObject.Find("Datas").GetComponent<DatasControl>();
		if(stageInfo.stageNum < datas.progress){
			if(stageInfo.stageImg != null){
				stageInfo.stageImg.SetActive(false);
				stageInfo.stageFin.SetActive(true);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
