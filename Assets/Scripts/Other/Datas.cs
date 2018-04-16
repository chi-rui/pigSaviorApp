using System.Collections;
using UnityEngine;

// user data

[System.Serializable]
public class UserDatas
{
	public string account;
	public string passwd;
	public int gameProgress;
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
	public int stageNum;
	public string sceneName;
	public GameObject stageImg;
	public string[] spritesName;
}

// Stage data
[System.Serializable]
public class NpcInfo
{
	
}

[System.Serializable]
public class RPGPlot
{
	public int sequence;
	public int plotNumber;
}

[System.Serializable]
public class QuesObj
{
	public string question;
	public string answer;
}