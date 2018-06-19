using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameWorkTeach : MonoBehaviour {

	public List<page> TeachPages = new List<page>();

	public GameObject Image_TeachWork, Text_TeackWork, Image_selections;
	// public int effectiveProgress;


	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnEnable(){
		if(GameObject.Find("Datas").GetComponent<DatasControl>().progress <= GameObject.Find("Datas").GetComponent<DatasControl>().nowStage){
			if(TeachPages.Count != 0){
				StartCoroutine(Teach());
			}else{
				print("error! no teach infomations here.");
			}
		}else{
			this.gameObject.SetActive(false);
		}
	}

	public IEnumerator Teach(){
		Image_selections.SetActive(false);
		for(int i = 0; i < TeachPages.Count; i++){
			Image_TeachWork.GetComponent<Image>().sprite = TeachPages[i].image;
			Text_TeackWork.GetComponent<Text>().text = TeachPages[i].test;
			yield return new WaitForSeconds(4.5f);
		}
		Image_selections.SetActive(true);
		Text_TeackWork.GetComponent<Text>().text = "請選擇接下來要做什麼。";
	}

	public void again(){
		StartCoroutine(Teach());
	}


}
