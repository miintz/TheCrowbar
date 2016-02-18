using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using Assets.Resources.Scripts;
using System.IO;

public class WriteOnManager : MonoBehaviour {

    
    public Text TheCrow;
    public Text QuestionOne;
    public Text QuestionTwo;

    public bool QuestionsFirst = false; //eerst vragen of eerst het gedicht?

    [TextArea(3, 10)]
    public string TheCrowText;

    [TextArea(3, 10)]
    public string QuestionsForOne;

    [TextArea(3, 10)]
    public string QuestionsForTwo;
    
    public string DisableClickEventsAt;
    public string DisableSwipeAt;

    public char DelimiterCharacter;

    private Dictionary<int, List<string>> Input;
    private int CrowIndex;
    private int QuestionIndex;

    private List<int> Answers;
    private Dictionary<int, string> PoemsList;    

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

        /*
        PoemsList.AddRange(Poems.Split(DelimiterCharacter));
       
        string[] nu = SwapWords.Split(DelimiterCharacter);
        for (int i = 0; i < nu.Length; i++)
        {
            List<string> l = new List<string>();
            l.AddRange(nu[i].Split(','));
            SwapWordsList.Add(i, l);
        }       
        */

        populatePoems();

        if (QuestionsFirst)
            setAnswersToGameObjects();
        else
            setPoemToGameObjects();
	}

    private void setPoemToGameObjects()
    {
        TheCrow.text = PoemsList[1];
    }

    private void populatePoems()
    {
        string[] poemfiles = Directory.GetFiles("Assets/Resources/Poems", "*.txt");
        int i = 0;
        foreach (string path in poemfiles)
        {           
            string text = File.ReadAllText(path);
            PoemsList.Add(i, text);
            i++;
        }
    }

    void setAnswersToGameObjects(bool crowOnly = false)
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
        //for (int i = 0; i < Answers.Count; i+)+   
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
        setAnswersToGameObjects();        
        Answers.Add(index);
    }
}
