using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class WriteOnManager : MonoBehaviour {

    private Dictionary<int, List<string>> Answers;

    public Text TheCrow;
    public Text QuestionOne;
    public Text QuestionTwo;

    private int CrowIndex;
    private int QuestionIndex;

    [TextArea(3, 10)]
    public string TheCrowText;

    [TextArea(3, 10)]
    public string QuestionsForOne;

    [TextArea(3, 10)]
    public string QuestionsForTwo;

    public char DelimiterCharacter;

	// Use this for initialization
	void Start () {
        Answers = new Dictionary<int, List<string>>();

        List<string> lines = new List<string>();
        lines.AddRange(QuestionsForOne.Split(DelimiterCharacter));

        Answers.Add(0, lines);

        lines = new List<string>();
        lines.AddRange(QuestionsForTwo.Split(DelimiterCharacter));

        Answers.Add(1, lines);

        lines = new List<string>();
        lines.AddRange(TheCrowText.Split(DelimiterCharacter));

        Answers.Add(2, lines);

        setAnswers();
        QuestionIndex++;
        CrowIndex++;
	}

    void setAnswers(bool crowOnly = false)
    {
        if (!crowOnly)
        {
            List<string> firstlines = new List<string>();
            Answers.TryGetValue(0, out firstlines);
            QuestionOne.text = firstlines[QuestionIndex];

            List<string> secondlines = new List<string>();
            Answers.TryGetValue(1, out secondlines);
            QuestionTwo.text = secondlines[QuestionIndex];
        }
       
        List<string> crowlines = new List<string>();
        Answers.TryGetValue(2, out crowlines);
        TheCrow.text = crowlines[CrowIndex];
       
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

    public void NextAnswer()
    {
        setAnswers();
        resetTextEntities();
    }
}
