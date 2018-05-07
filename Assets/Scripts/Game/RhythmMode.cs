﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// , IPointerClickHandler
// only onion npc problem
public class RhythmMode : MonoBehaviour {
	public GameObject pointer, characterAction, challengeFailedPanel, chooseOperatorPanel, calculatePanel, perfectScenario, remainingText, hitResultText;
	public Animator Anim_characterAction, Anim_characterActionPerfect, Anim_onion, Anim_onionPerfect;
	public Sprite Sprite_characterGrab;
	public Image Image_characterAction;
	public Image[] hitbarArr;
	public Text Text_remainCounts, Text_hitResult;
	public float speed;
	
	private Vector3 pos_L, pos_R;
	private int remainCounts;
	private List<int> tmpSortingList = new List<int>();
	private List<int> hitBarsIndexList = new List<int>();
	private bool isRhythmStart, isPerfectHit, isChallengeFailed;

	// Use this for initialization
	void Start () {
		pos_L = new Vector3(-1860f, pointer.transform.position.y, 0);
		pos_R = new Vector3(-530f, pointer.transform.position.y, 0);
		speed = 0.5f;
		remainCounts = 10;
		isChallengeFailed = false;
		isPerfectHit = false;
		isRhythmStart = true;
		generateHitBars(1);
	}
	
	// Update is called once per frame
	// -530 -1860
	// Mathf.PingPong(speed * Time.time, 1.0f)
	void Update () {
		if (isRhythmStart) {
			pointer.transform.position = Vector3.Lerp(pos_L, pos_R, Mathf.PingPong(speed * Time.time, 1.0f));

			// set roles' action
			characterAction.transform.position = new Vector3(characterAction.transform.position.x, 60, characterAction.transform.position.z);
			Anim_characterAction.enabled = true;

			if (Input.GetMouseButtonDown(0)) {
				if (isPerfectHit)
					StartCoroutine(clickInRhythm(1.3f));
				else
					StartCoroutine(clickInRhythm(0.8f));
			}
		}

		if (isChallengeFailed)
			challengeFailedPanel.SetActive(true);

		// test
		if (Input.GetKeyDown(KeyCode.Space)) {

		}
	}

	void generateHitBars (int times) {
		switch (times) {
			case 1:
				ranNum(3);
				break;
			case 2:
				ranNum(2);
				break;
			case 3:
				ranNum(1);
				break;
			default:
				break;
		}
	}

	void ranNum (int counts) {
		for (int i = 0; i < 7; i++){
			tmpSortingList.Add(i);
			// print(tmpSortingList[i]);
		}
		// print(hitBarsIndexList.Count + " " + tmpSortingList.Count);
		int tmpListCount = tmpSortingList.Count;
		while (hitBarsIndexList.Count < tmpListCount) {
			int index = Random.Range(0, tmpSortingList.Count);
			if (!hitBarsIndexList.Contains(tmpSortingList[index])) {
				hitBarsIndexList.Add(tmpSortingList[index]);
				tmpSortingList.Remove(tmpSortingList[index]);
			}
		}
		// print(hitBarsIndexList.Count + " " + tmpSortingList.Count);
		for (int i = 0; i < counts; i++) {
			// print(hitBarsIndexList[i]);
			hitbarArr[hitBarsIndexList[i]].color = new Color32(41, 149, 255, 255);
			hitbarArr[hitBarsIndexList[i]].gameObject.tag = "hitbar";
		}
	}

	IEnumerator clickInRhythm (float time) {
		isRhythmStart = false;
		
		// set roles' action
		characterAction.transform.position = new Vector3(characterAction.transform.position.x, 40, characterAction.transform.position.z);
		Anim_characterAction.enabled = false;
		Image_characterAction.sprite = Sprite_characterGrab;

		// show hint result and Perfect hit scenario
		remainingText.SetActive(false);
		hitResultText.SetActive(true);

		if (isPerfectHit) {
			Text_hitResult.text = "Perfect";
			Anim_onion.enabled = false;
			perfectScenario.SetActive(true);
			Anim_characterActionPerfect.Play("character game action_grab");
			Anim_onionPerfect.Play("onion grabbed");
			// Image_onionM.rectTransform.sizeDelta = new Vector2(250, 300);
		}
		else
			Text_hitResult.text = "Miss";

		yield return new WaitForSeconds(time);

		// set remain counts text
		remainCounts--;
		Text_remainCounts.text = remainCounts.ToString();

		if (isPerfectHit)
			clickPerfectHit();
		else {
			isRhythmStart = true;
			// show remain counts text
			remainingText.SetActive(true);
			hitResultText.SetActive(false);
		}

		if (remainCounts == 0) {
			isRhythmStart = false;
			isChallengeFailed = true;
		}
	}

	void clickPerfectHit () {
		print("isPerfectHit: " + isPerfectHit);

		perfectScenario.SetActive(false);

		// show choose operator panel
		chooseOperatorPanel.SetActive(true);
	}

	// Collision
	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == "hitbar") {
			isPerfectHit = true;
			// print("isPerfectHit: " + isPerfectHit);
		} else if (collider.gameObject.tag == "rhythmbar") {
			isPerfectHit = false;
			// print("isPerfectHit: " + isPerfectHit);
		} else {
			isPerfectHit = false;
			// print("Error for collision");
		}
	}
}
