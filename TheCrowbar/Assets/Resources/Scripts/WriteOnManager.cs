using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class WriteOnManager : MonoBehaviour {

    
    public Text TheCrow;
    public Text QuestionOne;
    public Text QuestionTwo;

    [TextArea(3, 10)]
    public string TheCrowText;

    [TextArea(3, 10)]
    public string QuestionsForOne;

    [TextArea(3, 10)]
    public string QuestionsForTwo;

    public char DelimiterCharacter;

    private Dictionary<int, List<string>> Input;
    private int CrowIndex;
    private int QuestionIndex;

    private List<int> Answers;

	// Use this for initialization
	void Start () {
        Answers = new List<int>();
        Input = new Dictionary<int, List<string>>();
        
        List<string> lines = new List<string>();
        lines.AddRange(QuestionsForOne.Split(DelimiterCharacter));

        Input.Add(0, lines);

        lines = new List<string>();
        lines.AddRange(QuestionsForTwo.Split(DelimiterCharacter));

        Input.Add(1, lines);

        lines = new List<string>();
        lines.AddRange(TheCrowText.Split(DelimiterCharacter));

        Input.Add(2, lines);
        
        setAnswers();        
	}

    void setAnswers(bool crowOnly = false)
    {        
        if (!crowOnly)
        {            
            List<string> firstlines = new List<string>();
            Input.TryGetValue(0, out firstlines);
            QuestionOne.text = firstlines[QuestionIndex];
            QuestionOne.GetComponent<WriteOn>().Init();

            List<string> secondlines = new List<string>();
            Input.TryGetValue(1, out secondlines);
            QuestionTwo.text = secondlines[QuestionIndex];
            QuestionTwo.GetComponent<WriteOn>().Init();

            QuestionIndex++;
        }
       
        List<string> crowlines = new List<string>();
        Input.TryGetValue(2, out crowlines);
        TheCrow.text = crowlines[CrowIndex];
        TheCrow.GetComponent<WriteOn>().Init();

        CrowIndex++;        
    }

    void resetTextEntities()
    {
        QuestionOne.GetComponent<WriteOn>().Reset();
        QuestionTwo.GetComponent<WriteOn>().Reset();
        TheCrow.GetComponent<WriteOn>().Reset();
    }

	// Update is called once per frame
	void Update () {
        //for (int i = 0; i < Answers.Count; i++)
        //{
        //    List<string> list = new List<string>();
        //    Answers.TryGetValue(i, out list);

        //    foreach (string answer in list)
        //    {
        //        //Debug.Log(answer + " " + i);
        //    }
        //}
	}

    public void NextAnswer(int index)
    {
        setAnswers();
        //resetTextEntities(); dit werkt niet

        Answers.Add(index);
    }
}
