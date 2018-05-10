using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathDatasControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// quesObj = getQuestion( 200, "A+B+C+D");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public QuesObj getQuestion( int maxNum, string template ){
		QuesObj quesObj = questionGenerator( 1, maxNum, template);
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
		bool isInBracket = false;
		int i, j, num = 0;
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
					numbers.Add(formula[i]);
					break;
			}
		}

		// sort the list by operator.
		for(i = 0; i < answerList.Count; i++){
			switch(answerList[i].operators){
				case 'x':case '÷': case '*': case '/':
					answerTemp.Add(answerList[i]);
					answerList.RemoveAt(i);
					break;
				default:
					break;
			}
		}
		answerTemp = answerTemp.ToArray().Concat(answerList.ToArray()).ToList();
		answerList = new List<AnsObj>();

		// sort the list by bracket.
		for(i = 0; i < answerTemp.Count; i++){
			if(answerTemp[i].isInBracket){
				answerList.Add(answerTemp[i]);
				answerTemp.RemoveAt(i);
			}
		}
		answerList = answerList.ToArray().Concat(answerTemp.ToArray()).ToList();
		
		
		// set numbers to question.
		question = formula.Select(c => c.ToString()).ToArray();	// string array for store question.
		List<char> counter = formula.ToList();					// char list to distinguish which is handled.
		List<int> stackNum = new List<int>();					// save the results of operations.
		bool isBefore = false, isAfter = false;					// shows whick is already finish operation.
		
		// get a first num for operation.
		stackNum.Add(UnityEngine.Random.Range(miniNum, maxNum/3));

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
					answerList[i].partAns = stackNum[i] + stackNum[i-1];
					break;
				case '-':
					answerList[i].partAns = stackNum[i] - stackNum[i-1];
					break;					
				case 'x':case '*':
					answerList[i].partAns = stackNum[i] * stackNum[i-1];
					break;
				case '÷':case '/':
					answerList[i].partAns = stackNum[i] / stackNum[i-1];
					break;
				}
			}else if(isAfter){
				// if there's a finished operation after the operator.
				switch(answerList[i].operators){
				case '+':
					num = UnityEngine.Random.Range(miniNum,maxNum-stackNum[i]);
					answerList[i].partAns = stackNum[i] + num;
					break;
				case '-':
					answerList[i].partAns = UnityEngine.Random.Range(miniNum, (int)maxNum/3);
					num = answerList[i].partAns + stackNum[i];
					break;					
				case 'x':case '*':
					num = UnityEngine.Random.Range(miniNum,(int)Mathf.Sqrt(maxNum));
					answerList[i].partAns = stackNum[i] * num;
					break;
				case '÷':case '/':
					answerList[i].partAns = UnityEngine.Random.Range(miniNum, (int)Mathf.Sqrt(maxNum));
					num = stackNum[i] * answerList[i].partAns;
					break;
				}
				// put numbers into question.
				for( j = answerList[i].index-1; j >= 0; j-- ){
					if(question[j] == "(" || question[j] == ")" ){
						continue;
					}else{
						question[j] = num.ToString();
						counter[j] = '@';
						break;
					}
				}
			}else if(isBefore){
				// if there's a finished operation before the operator.
				switch(answerList[i].operators){
				case '+':
					num = UnityEngine.Random.Range(miniNum,maxNum-stackNum[i]);
					answerList[i].partAns = stackNum[i] + num;
					break;
				case '-':
					num = UnityEngine.Random.Range(miniNum,stackNum[i]);
					answerList[i].partAns = stackNum[i] - num;
					break;					
				case 'x':case '*':
					num = UnityEngine.Random.Range(miniNum,(int)Mathf.Sqrt(maxNum));
					answerList[i].partAns = stackNum[i] * num;
					break;
				case '÷':case '/':
					num = getFactor(stackNum[i]);
					answerList[i].partAns = stackNum[i] / num;
					break;
				}
				// put numbers into question.
				for( j = answerList[i].index+1; j < question.Length; j++ ){
					if(question[j] == "(" || question[j] == ")" ){
						continue;
					}else{
						question[j] = num.ToString();
						counter[j] = '@';
						break;
					}
				}
			}else{
				// if there's no finished operation besides the operator.
				switch(answerList[i].operators){
				case '+':
					num = UnityEngine.Random.Range(miniNum,maxNum-stackNum[i]);
					answerList[i].partAns = stackNum[i] + num;
					break;
				case '-':
					num = UnityEngine.Random.Range(miniNum,stackNum[i]);
					answerList[i].partAns = stackNum[i] - num;
					break;					
				case 'x':case '*':
					num = UnityEngine.Random.Range(miniNum,(int)Mathf.Sqrt(maxNum));
					answerList[i].partAns = stackNum[i] * num;;
					break;
				case '÷':case '/':
					num = UnityEngine.Random.Range(miniNum,(int)Mathf.Sqrt(maxNum));
					answerList[i].partAns = UnityEngine.Random.Range(miniNum, (int)Mathf.Sqrt(maxNum));
					stackNum[i] = answerList[i].partAns * num;
					break;
				}
				// put numbers into question.
				for( j = answerList[i].index+1; j < question.Length; j++ ){
					if(question[j] == "(" || question[j] == ")" ){
						continue;
					}else{
						question[j] = num.ToString();
						counter[j] = '@';
						break;
					}
				}
				for( j = answerList[i].index-1; j >= 0; j-- ){
					if(question[j] == "(" || question[j] == ")" ){
						continue;
					}else{
						question[j] = stackNum[i].ToString();
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
		// for(j = 0; j < question.Length; j++){
		// 	quesObj.questionList.Add(question[j]);
		// }
		quesObj.question = question.ToList();
		quesObj.answer = answerList;
		return quesObj;
	}

	int getFactor( int number ){
		List<int> factors = new List<int>();
		for(int i = 1; i <= (int)Mathf.Sqrt(number); i++ ){
			if(number % i == 0)
				factors.Add(i);
		}
		if(factors.Count == 1)
			return factors[0];
		else
			return factors[UnityEngine.Random.Range(1, factors.Count)];
	}

}