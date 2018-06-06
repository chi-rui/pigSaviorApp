using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathDatasControl : MonoBehaviour {
	// Use this for initialization
	void Start () {
		// string t = "";
		// QuesObj quesObj = getQuestion( 1, 300, "A*(B-C)");
		// for(int i = 0; i < quesObj.question.Count; i++ ){
		// 	t += quesObj.question[i];
		// }
		// print(t + " = " + quesObj.answer[quesObj.answer.Count-1].partAns);
		// for(int i = 0; i < quesObj.answer.Count; i++ ){
		// 	print(quesObj.answer[i].numA + quesObj.answer[i].operators.ToString() + quesObj.answer[i].numB + " = " + quesObj.answer[i].partAns);
		// }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public QuesObj getQuestion( int miniNum, int maxNum, string template ){
		QuesObj quesObj = questionGenerator( miniNum, maxNum, template);
		while(quesObj.answer[quesObj.answer.Count-1].partAns < 0){
			quesObj = questionGenerator( 1, maxNum, template);
		}
		return quesObj;
	}


	private QuesObj questionGenerator( int miniNum, int maxNum, string template ){		
		QuesObj quesObj = new QuesObj();
		string[] question;
		List<char> numbers = new List<char>();
		char[] formula = template.ToCharArray();
		bool isInBracket = false, special = false;
		int i, j, num = 0, tempNum=0;
		List<AnsObj> answerList = new List<AnsObj>();
		List<AnsObj> answerTemp = new List<AnsObj>();
		AnsObj ansObj = new AnsObj();

		for(i = 0; i < formula.Length; i++){
			switch(formula[i]){
				case '+':case '-':
					ansObj = new AnsObj();
					ansObj.index = i;
					ansObj.operators = formula[i];
					if(isInBracket)
						ansObj.isInBracket = true;
					else
						ansObj.isInBracket = false;
					answerList.Add(ansObj);
					if(!quesObj.isPM)
						quesObj.isPM = true;
					break;
				case 'x':case '*':case '÷':case '/':
					ansObj = new AnsObj();
					ansObj.index = i;
					ansObj.operators = formula[i];
					if(isInBracket)
						ansObj.isInBracket = true;
					else
						ansObj.isInBracket = false;
					answerList.Add(ansObj);
					if(!quesObj.isMD)
						quesObj.isMD = true;
					break;
				case '(':
					isInBracket = true;
					if(!quesObj.isBracket)
						quesObj.isBracket = true;
					break;
				case ')':
					isInBracket = false;
					break;
				default:
					break;
			}
		}

		// sort the list by operator.
		for(i = 0; i < answerList.Count; i++){
			switch(answerList[i].operators){
				case 'x':case '*':
				case '/':case '÷':
					answerTemp.Add(answerList[i]);
					answerList.RemoveAt(i);
					i--;
					break;
				default:
					break;
			}
		}
		answerTemp = answerTemp.ToArray().Concat(answerList.ToArray()).ToList();
		answerList.Clear();

		// sort the list by bracket.
		for(i = 0; i < answerTemp.Count; i++){
			if(answerTemp[i].isInBracket){
				answerList.Add(answerTemp[i]);
				answerTemp.RemoveAt(i);
				i--;
			}
		}
		answerList = answerList.ToArray().Concat(answerTemp.ToArray()).ToList();	// string list for store answer.
		answerTemp.Clear();
		question = formula.Select(c => c.ToString()).ToArray();						// string array for store question.

		if(answerList.Count > 2)
			if(answerList[2].index < answerList[0].index && answerList[2].index > answerList[1].index)
				special = true;
			else if(answerList[2].index > answerList[0].index && answerList[2].index < answerList[1].index)
				special = true;
			else
				special = false;

		// if template have double brackets...
		if(special){
			int frontAns = 0, behindAns = 0; 
			// check the last operator.
			switch(answerList[2].operators){
			case '+':
				frontAns = UnityEngine.Random.Range(miniNum, (int)maxNum*4/5);
				behindAns = UnityEngine.Random.Range(miniNum, (int)maxNum*4/5);
				answerList[2].partAns = frontAns + behindAns;
				break;
			case 'x':case '*':
				frontAns = UnityEngine.Random.Range(miniNum, (int)maxNum/2);
				behindAns = UnityEngine.Random.Range(miniNum, (int)maxNum/2);
				answerList[2].partAns = frontAns * behindAns;
				break;
			case '-':
				frontAns = UnityEngine.Random.Range((int)maxNum/2, (int)maxNum*3/4);
				behindAns = UnityEngine.Random.Range(miniNum, frontAns-1);
				answerList[2].partAns = frontAns - behindAns;
				break;
			case '÷':case '/':
				while(checkPrime(frontAns)){
					frontAns = UnityEngine.Random.Range(miniNum+1, maxNum/2);
				}
				behindAns = getFactor(frontAns);
				answerList[2].partAns = frontAns / behindAns;
				break;
			default:
				print("operators not found.");
				break;
			}
			answerList[2].numA = frontAns;
			answerList[2].numB = behindAns;

			// set number to template question.
			for(i = 0; i < answerList.Count-1; i++ ){
				if(answerList[i].index < answerList[2].index)
					num = frontAns;
				else if (answerList[i].index > answerList[2].index)
					num = behindAns;

				// int tempNum = 0;
				
				switch(answerList[i].operators){
				case '+':
					tempNum = UnityEngine.Random.Range(miniNum, num);
					question[answerList[i].index-1] = tempNum.ToString();
					question[answerList[i].index+1] = (num - tempNum).ToString();
					break;
				case '-':
					tempNum = UnityEngine.Random.Range(num, maxNum);
					question[answerList[i].index-1] = tempNum.ToString();
					question[answerList[i].index+1] = (tempNum - num).ToString();
					break;
				case 'x':case '*':
					tempNum = getFactor(num);
					question[answerList[i].index-1] = tempNum.ToString();
					question[answerList[i].index+1] = (num / tempNum).ToString();
					break;
				case '÷':case '/':
					tempNum = UnityEngine.Random.Range(miniNum+1, maxNum/2);
					question[answerList[i].index-1] = (num * tempNum).ToString();
					question[answerList[i].index+1] = tempNum.ToString();
					break;
				default:
					break;
				}
				answerList[i].partAns = num;
				answerList[i].numA = int.Parse(question[answerList[i].index-1]);
				answerList[i].numB = int.Parse(question[answerList[i].index+1]);
			}


		// other templates.
		}else{
			// set numbers to question.
			List<char> counter = formula.ToList();					// char list to distinguish which is handled.
			List<int> stackNum = new List<int>();					// save the results of operations.
			bool isBefore = false, isAfter = false;					// shows whick is already finish operation.
			
			// check every operations in the formula by its order and set numbers into question.
			for( i=0; i < answerList.Count; i++){
				// check the numbers near the operator.
				for( j = answerList[i].index-1; j >= 0; j-- ){
					if(counter[j] == '(' || counter[j] == ')' ){
						continue;
					}else if(counter[j] == '@'){
						isBefore = true;
						break;
					}else{
						break;
					}
				}
				for( j = answerList[i].index+1; j < counter.Count; j++ ){
					if(counter[j] == '(' || counter[j] == ')' ){
						continue;
					}else if(counter[j] == '@'){
						isAfter = true;
						break;
					}else{
						break;
					}
				}

				// set numbers by four different situations.
				if(isAfter && isBefore){
					// if both finished operations besides the operator.
					switch(answerList[i].operators){
					case '+':
						answerList[i].partAns = stackNum[i-1] + stackNum[i-2];
						break;
					case '-':
						answerList[i].partAns = stackNum[i-1] - stackNum[i-2];
						break;					
					case 'x':case '*':
						answerList[i].partAns = stackNum[i-1] * stackNum[i-2];
						break;
					case '÷':case '/':
						answerList[i].partAns = stackNum[i-1] / stackNum[i-2];
						break;
					}
					answerList[i].numA = stackNum[i-1];
					answerList[i].numB = stackNum[i-2];
				}else if(isAfter){
					// if there's a finished operation after the operator.
					switch(answerList[i].operators){
					case '+':
						num = UnityEngine.Random.Range(miniNum,maxNum);
						answerList[i].partAns = stackNum[i-1] + num;
						break;
					case '-':
						answerList[i].partAns = UnityEngine.Random.Range(miniNum, (int)maxNum/3);
						num = answerList[i].partAns + stackNum[i-1];
						break;					
					case 'x':case '*':
						num = UnityEngine.Random.Range(miniNum,(int)Mathf.Sqrt(maxNum));
						answerList[i].partAns = stackNum[i-1] * num;
						break;
					case '÷':case '/':
						answerList[i].partAns = UnityEngine.Random.Range(miniNum, (int)Mathf.Sqrt(maxNum));
						num = stackNum[i-1] * answerList[i].partAns;
						break;
					}
					// put numbers into question.
					for( j = answerList[i].index-1; j >= 0; j-- ){
						if(question[j] == "(" || question[j] == ")" ){
							continue;
						}else{
							answerList[i].numA = num;
							answerList[i].numB = stackNum[i-1];
							question[j] = num.ToString();
							counter[j] = '@';
							break;
						}
					}
				}else if(isBefore){
					// if there's a finished operation before the operator.
					switch(answerList[i].operators){
					case '+':
						num = UnityEngine.Random.Range(miniNum,maxNum);
						answerList[i].partAns = stackNum[i-1] + num;
						break;
					case '-':
						num = UnityEngine.Random.Range(miniNum,stackNum[i-1]);
						answerList[i].partAns = stackNum[i-1] - num;
						break;					
					case 'x':case '*':
						num = UnityEngine.Random.Range(miniNum,(int)Mathf.Sqrt(maxNum));
						answerList[i].partAns = stackNum[i-1] * num;
						break;
					case '÷':case '/':
						num = getFactor(stackNum[i-1]);
						answerList[i].partAns = stackNum[i-1] / num;
						break;
					}
					// put numbers into question.
					for( j = answerList[i].index+1; j < question.Length; j++ ){
						if(question[j] == "(" || question[j] == ")" ){
							continue;
						}else{
							answerList[i].numA = stackNum[i-1];
							answerList[i].numB = num;
							question[j] = num.ToString();
							counter[j] = '@';
							break;
						}
					}
				}else{
					// if there's no finished operation besides the operator.
					switch(answerList[i].operators){
					case '+':
						tempNum = UnityEngine.Random.Range(miniNum,maxNum);
						num = UnityEngine.Random.Range(miniNum,maxNum);
						answerList[i].partAns = tempNum + num;
						break;
					case '-':
						num = UnityEngine.Random.Range(miniNum,maxNum/2);
						tempNum = UnityEngine.Random.Range(num,maxNum);
						answerList[i].partAns = tempNum - num;
						break;					
					case 'x':case '*':
						tempNum = UnityEngine.Random.Range(miniNum, (int)maxNum/2-1);
						num = UnityEngine.Random.Range(miniNum,(int)Mathf.Sqrt(maxNum));
						answerList[i].partAns = tempNum * num;;
						break;
					case '÷':case '/':
						while(checkPrime(tempNum)){
							tempNum = UnityEngine.Random.Range(miniNum, maxNum);						
						}
						num = getFactor(tempNum);
						answerList[i].partAns = tempNum / num;
						break;
					}
					// put numbers into question.
					for( j = answerList[i].index-1; j >= 0; j-- ){
						if(question[j] == "(" || question[j] == ")" ){
							continue;
						}else{
							answerList[i].numA = tempNum;
							question[j] = tempNum.ToString();
							counter[j] = '@';
							break;
						}
					}
					for( j = answerList[i].index+1; j < question.Length; j++ ){
						if(question[j] == "(" || question[j] == ")" ){
							continue;
						}else{
							answerList[i].numB = num;
							question[j] = num.ToString();
							counter[j] = '@';
							break;
						}
					}
				}
				// update the data of the operation result.
				stackNum.Add(answerList[i].partAns);
				isBefore = false;
				isAfter = false;
			}
		}
		quesObj.question = question.ToList();
		quesObj.answer = answerList;
		return quesObj;
	}

	private int getFactor( int number ){
		List<int> factors = new List<int>();
		for(int i = 2; i < number; i++ ){
			if(number % i == 0)
				factors.Add(i);
		}
		if(factors.Count == 0)
			return number;
		else
			return factors[UnityEngine.Random.Range(0, factors.Count)];
	}

	private bool checkPrime( int number ){
		for(int i = 2; i < number; i++ ){
			if(number % i == 0)
				return false;
		}
		return true;
	}

}