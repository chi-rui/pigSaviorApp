using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextQuesMode : MonoBehaviour {

	// set question
	public int minNum, maxNum;
	public List<string> quesTemplate;
	public string textQuesFilePath;
	private List<string> textQuesList = new List<string>();
	private QuesObj quesObj, temp;
	private MathDatasControl MathDatas;

// test
	private string testQues;

	void OnEnable () {
		MathDatas = GameObject.Find("EventSystem").GetComponent<MathDatasControl>();

		generateNewQuestion(minNum, maxNum, quesTemplate);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void generateNewQuestion (int min, int max, List<string> template) {
		// generate question and get operator counts
		quesObj = MathDatas.getQuestion(min, max, template[Random.Range(0, template.Count)]);
		temp = new QuesObj();
		temp.question = new List<string>(quesObj.question);
		temp.answer = new List<AnsObj>(quesObj.answer);

		testQues = "";
		for (int i = 0; i < temp.question.Count; i++)
			testQues += temp.question[i];
		print(testQues);

		// read text question file and set question in a list
		StreamReader textQuesFile = new StreamReader(textQuesFilePath);
		string textQuesContent = textQuesFile.ReadToEnd();
		// print(textQuesContent);
		// textQuesList.Add(textQuesContent.Split(new string[]{"@"}));
		textQuesFile.Close();

		print(textQuesList.Count);
	}

}
