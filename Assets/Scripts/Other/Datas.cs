﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// user data

[System.Serializable]
public class UserDatas
{
	public string account = "";
	public string passwd = "";
	public int gameProgress = 0;
	public Achievements achievements;
	public float timer;
}

[System.Serializable]
public class Achievements
{
	// public bool ...
}

// Chapter data

[System.Serializable]
public class StageInfo
{
	public Vector3 last;
	public Vector3 next;
	public bool isLastNeedTurn;
	public bool isLastHorizontalFirst;
	public bool isNextNeedTurn;
	public bool isNextHorizontalFirst;
	public int stageNum;		// progress of the stage.
	public int stageGoal;		// finish the stage while the progress achieve this number.
	public string sceneName;	// scenes of entering the stage. 
	public GameObject stageImg;	// initial image according to the stage.
	public GameObject stageFin;	// the image after finish the stage.
}

// Stage data
[System.Serializable]
public class NpcInfo
{
	public string npcName;			// npc 名稱
	public Sprite npcHeader;
	public List<string> trueContents;	// 劇情觸發時的回應內容
	public List<string> falseContents;	// 劇情未被觸發時的回應
	public List<Plot> plots;			// npc 所負責的劇情事件清單
}

[System.Serializable]
public class Plot
{
	public int sequence;		// 劇情事件的次序
	public int plotNumber;		// 劇情對應的正確回應索引
	public GameObject gamePanel;	// 劇情對應的遊戲名稱
	public GameObject changeGameObj;
	// public Sprite newSkin;
}

[System.Serializable]
public class QuesObj
{
	public List<string> question;
	public List<AnsObj> answer;
	public bool isBracket;
	public bool isPM;
	public bool isMD;
}

[System.Serializable]
public class AnsObj
{
	public int index;
	public char operators;
	public int partAns;
	public bool isInBracket;
	public int numA;
	public int numB;
}

[System.Serializable]
public class StagePrompts
{
	public List<Sprite> pictures;
	public List<string> words;
}

[System.Serializable]
public class page
{
	public Sprite image;
	public string test;
}
