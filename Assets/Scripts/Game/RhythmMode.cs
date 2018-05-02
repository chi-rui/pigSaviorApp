using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// , IPointerClickHandler
public class RhythmMode : MonoBehaviour {
	public GameObject pointer, characterAction, Panel_challengeFailed;
	public Image[] hitbarArr;
	public float speed;
	public Animator Anim_characterAction;
	public Sprite Sprite_characterGrab;
	public Image Image_characterAction;
	public Text Text_remainCounts, Text_perfectCounts, Text_missCounts, Text_userans;
	
	private Vector3 pos_L, pos_R;
	private int remainCounts, perfectCounts, missCounts;
	// private int[] hitBarsIndexArr = new int[7];
	private List<int> tmpSortingList = new List<int>();
	private List<int> hitBarsIndexList = new List<int>();
	private bool isPerfectHit, isChallengeFailed;

	// Use this for initialization
	void Start () {
		pos_L = new Vector3(-1860f, pointer.transform.position.y, 0);
		pos_R = new Vector3(-530f, pointer.transform.position.y, 0);
		speed = 0.5f;
		remainCounts = 10;
		isChallengeFailed = false;
		isPerfectHit = false;
		generateHitBars(1);
	}
	
	// Update is called once per frame
	// -530 -1860
	// Mathf.PingPong(speed * Time.time, 1.0f)
	void Update () {
		pointer.transform.position = Vector3.Lerp(pos_L, pos_R, Mathf.PingPong(speed * Time.time, 1.0f));

		if (Input.GetMouseButtonDown(0)) {
			if (isPerfectHit)
				clickPerfectHit();
			else
				clickMissHit();
			clickInRhythm();
		}

		if (isChallengeFailed)
			Panel_challengeFailed.SetActive(true);

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
				break;
			case 3:
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
			hitbarArr[hitBarsIndexList[i]].color = new Color32(255, 137, 30, 255);
			hitbarArr[hitBarsIndexList[i]].gameObject.tag = "hitbar";
		}
	}

	void clickInRhythm () {
		// StartCoroutine(waitForClick(1f));

		// set roles' action
		characterAction.transform.position = new Vector3(characterAction.transform.position.x, 30, characterAction.transform.position.z);
		Anim_characterAction.enabled = false;
		Image_characterAction.sprite = Sprite_characterGrab;

		// set hint text
		remainCounts--;
		Text_remainCounts.text = remainCounts.ToString();
		if (remainCounts == 0) {
			isChallengeFailed = true;
		}
	}

	// IEnumerator waitForClick (float time) {
	// 	yield return new WaitForSeconds(time);
	// }

	void clickPerfectHit() {
		// set hint text
		perfectCounts++;
		Text_perfectCounts.text = perfectCounts.ToString();
		print("isPerfectHit: " + isPerfectHit);
	}

	void clickMissHit() {
		// set hint text
		missCounts++;
		Text_missCounts.text = missCounts.ToString();
		print("isPerfectHit: " + isPerfectHit);
	}

	// Collision
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "hitbar") {
			isPerfectHit = true;
			// print("isPerfectHit: " + isPerfectHit);
		} else if (collider.gameObject.tag == "rhythmbar") {
			isPerfectHit = false;
			// print("isPerfectHit: " + isPerfectHit);
		} else {
			print("Error for collision");
		}
	}

	
}
