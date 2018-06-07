using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisIdentify : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public List<string> getMisConception( List<AnsObj> trueAns, List<AnsObj> userAns ){
		
		/**** MisConception number ****/
		//	運算規則系列：
		//	01	括號先做 V
		//	02	先乘除後加減 V
		//	03	由左而右
		//	運算過程系列：
		//	04	忽略計算過程(過程錯，答案對)
		//	05	計算錯誤
		//	運算符號系列：
		//	06	加減互換
		//	07	加乘互換
		//	08	乘除互換
		//	09	減除互換
		bool mis01 = false, mis02 = false, mis03 = false, mis04 = false, mis05 = false,
			 mis06 = false, mis07 = false, mis08 = false, mis09 = false;
		/**** MisConception number ****/

		List<string> misConception = new List<string>();
		int bracketSep = -1, i = 0, answer = 0;
		string questionOper = "";
		bool operSwitch = false;

		for(i = 0; i < trueAns.Count; i++){
			if(!trueAns[i].isInBracket && bracketSep == -1){
				bracketSep = i;
			}
			questionOper += trueAns[i].operators.ToString();
		}

		for(i = 0; i < userAns.Count; i++){
			
			// check rule type misConception
			if(i < bracketSep){
				if(!userAns[i].isInBracket)
					mis01 = true;
			}

			switch(userAns[i].operators){
				case '*':case 'x':case '÷':case '/':
					if(operSwitch){
						mis02 = true;
					}
					break;
				case '+':case '-':
					if(!operSwitch)
						operSwitch = true;
					break;
				default:
					print("no oper");
					break;
			}

			if(i == bracketSep-1)
				operSwitch = false;


			switch(questionOper){
				case "++":case "+++":
				case "**":case "***":
				case "xx":case "xxx":
					break;
				default:
					if(i > 0){
						// print(userAns[i].index + " " + userAns[i-1].index);
						if(userAns[i].index < userAns[i-1].index)
							if(!(userAns[i].isInBracket ^ userAns[i-1].isInBracket))
								if(isSameLevelOper(userAns[i].operators, userAns[i-1].operators))
									mis03 = true;
					}
					break;
			}

			// check process type and operator type misConception
			switch(userAns[i].operators){
				case '+':
					answer = userAns[i].numA + userAns[i].numB;
					break;
				case '-':
					answer = userAns[i].numA - userAns[i].numB;
					break;
				case '*':case 'x':
					answer = userAns[i].numA * userAns[i].numB;
					break;
				case '/':case '÷':
					answer = (int)userAns[i].numA / userAns[i].numB;
					break;
				default:
					print("error : unknow operators.");
					break;
			}

			if(userAns[i].partAns == trueAns[i].partAns){
				if(i == trueAns.Count-1)
					if(mis05 || mis06 || mis07 || mis08 || mis09)
						mis04 = true;
			}else if(userAns[i].partAns == answer){
				// nothing : caculate is fine but rules have some problems.
			}else{
				switch(userAns[i].operators){
					case '+':
						if(userAns[i].partAns == userAns[i].numA - userAns[i].numB)
							mis06 = true;
						else if(userAns[i].partAns == userAns[i].numA * userAns[i].numB)
							mis07 = true;
						else
							mis05 = true;
						break;
					case '-':
						if(userAns[i].partAns == userAns[i].numA + userAns[i].numB)
							mis06 = true;
						else if(userAns[i].partAns == (int)userAns[i].numA / userAns[i].numB)
							mis09 = true;
						else
							mis05 = true;
						break;
					case '*':case 'x':
						if(userAns[i].partAns == userAns[i].numA + userAns[i].numB)
							mis07 = true;
						else if(userAns[i].partAns == (int)userAns[i].numA / userAns[i].numB)
							mis08 = true;
						else
							mis05 = true;
						break;
					case '/':case '÷':
						if(userAns[i].partAns == userAns[i].numA - userAns[i].numB)
							mis09 = true;
						else if(userAns[i].partAns == userAns[i].numA * userAns[i].numB)
							mis08 = true;
						else
							mis05 = true;
						break;
					default:
						print("error : unknow operators.");
						break;
				}
			}
		}

		if(mis01)
			misConception.Add("mis01");
		if(mis02)
			misConception.Add("mis02");
		if(mis03)
			misConception.Add("mis03");
		if(mis04)
			misConception.Add("mis04");
		if(mis05)
			misConception.Add("mis05");
		if(mis06)
			misConception.Add("mis06");
		if(mis07)
			misConception.Add("mis07");
		if(mis08)
			misConception.Add("mis08");
		if(mis09)
			misConception.Add("mis09");

		return misConception;
	}

	private bool isSameLevelOper( char operA, char operB ){
		switch(operA){
			case '+':
				if(operB == '-')
					return true;
				else
					break;
			case '-':
				if(operB == '+' || operB == '-')
					return true;
				else
					break;
			case '*':case 'x':
				if(operB == '/' || operB == '÷')
					return true;
				else
					break;
			case '/':case '÷':
				if(operB == '*' || operB == 'x' || operB == '/' || operB == '÷')
					return true;
				else
					break;
		}
		return false;
	}
}
