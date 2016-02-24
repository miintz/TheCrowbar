using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using Assets.Resources.Scripts;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.Assertions;

public class WriteOnManager : MonoBehaviour
{
    public Text TheCrow;
    public Text QuestionOne;
    public Text QuestionTwo;

    private GameObject TheCrowGO;
    private GameObject QuestionOneGO;
    private GameObject QuestionTwoGO;

    public bool QuestionsFirst = false; //eerst vragen of eerst het gedicht?

	
	[TextArea(3, 10)]
	public string TheCrowIntro;

	[TextArea(3, 10)]
	public string TheCrowIntroIntro;

	[TextArea(3, 10)]
	public string TheCrowMiddle;
	
	[TextArea(3, 10)]
	public string TheCrowOutro;

	[TextArea(3, 10)]
	public string TheCrowOutroOutro;
	
	[TextArea(3, 10)]
    public string TheCrowText;

    [TextArea(3, 10)]
    public string QuestionsForOne;

    [TextArea(3, 10)]
    public string QuestionsForTwo;

    public string BackgroundOptionsOne;
    public string BackgroundOptionsTwo;

    public string DisableClickEventsAt;
    public string DisableSwipeAt;

    public char DelimiterCharacter;

    public bool NoClick = true;

	public bool AnswerDisabled = true;

    private Dictionary<int, List<string>> Input;
    private int CrowIndex;
    private int QuestionIndex;

    private List<int> Answers;
    private SortedList<int, string> PoemsList;
    private List<string> CurrentPoems;
    private int CurrentQuestion = 0;
    private int CurrentPoem = 0;

    public bool PoemMode;

	private bool CrowIntro;
	private bool CrowIntroIntro;
	private bool CrowMiddle;
	private bool CrowOutro;
	private bool CrowOutroOutro;

    public GameObject SwipeGUILeft;
    public GameObject SwipeGUIRight;

    private List<string[]> ActiveOptions = new List<string[]>();

	private GameObject logoGO;

    // Use this for initialization
    void Start()
    {
		logoGO = GameObject.Find("Logo");
        QuestionOneGO = GameObject.Find("Question 1");
        QuestionTwoGO = GameObject.Find("Question 2");
        TheCrowGO = GameObject.Find("Crow");

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

        string[] optionsone = BackgroundOptionsOne.Split(DelimiterCharacter);
        string[] optionstwo = BackgroundOptionsTwo.Split(DelimiterCharacter);

        ActiveOptions.Add(new string[2] { optionsone[0], optionstwo[0] });
        ActiveOptions.Add(new string[2] { optionsone[1], optionstwo[1] });
        ActiveOptions.Add(new string[2] { optionsone[2], optionstwo[2] });
        
        SwipeGUILeft = GameObject.FindGameObjectWithTag("GUIleft");
        SwipeGUIRight = GameObject.FindGameObjectWithTag("GUIright");        

        //vul lijst met poems
        populatePoems();        
        System.Threading.Thread.Sleep(100); //sometimes reading takes too long

        if (QuestionsFirst) {
			ShowCrowIntroIntro ();
		}
        else
            switchToPoemMode();
      
        SwipeGUIRight.SetActive(false);
    }

    private void setPoemToGameObjects()
    {
        TheCrow.verticalOverflow = VerticalWrapMode.Overflow;
        //TheCrow.text = PoemsList.get

        IList<int> k = PoemsList.Keys;
        IList<string> v = PoemsList.Values;
        
        int max = k.Max();
        System.Random rand = new System.Random();

        int poemInt = rand.Next(1, max);        

        CurrentPoems = new List<string>();
        //pak alle texts met deze key
        for (int i = 0; i < v.Count; i++)
        {
            if (k[i] == poemInt)
            {           
                CurrentPoems.Add(v[i]);
            }
        }

        try
        {
            //soms is deze lijst leeg
            string t = CurrentPoems[0].Replace("%EACUTE%", "é");
            //gebruik de eerste uit de currentpoems lijst        
            TheCrow.text = t;
        }
        catch (Exception e)
        {
            Debug.Log(CurrentPoems.Count + " " + PoemsList.Count + " " + poemInt);
            TheCrow.text = "something went wrong: " + e.InnerException.ToString();
        }

    }

    private void populatePoems()
    {
        PoemsList = new SortedList<int, string>(new DuplicateKeyComparer<int>()); //lijst met niet-unique keys
        TextAsset[] poems = Resources.LoadAll<TextAsset>("Poems");

        int i = 0;
        foreach (TextAsset path in poems)
        {            
            //pak eerste stukje van de .txt naam, dat is de key
            int n = Int32.Parse(path.name.ToCharArray()[0].ToString());

            string text = path.text;

            PoemsList.Add(n, text);
            i++;
        }
    }

    void setAnswersToGameObjects(bool crowOnly = false)
    {
        TheCrow.verticalOverflow = VerticalWrapMode.Truncate;

        List<string> firstlines = new List<string>();
        Input.TryGetValue(0, out firstlines);

        List<string> secondlines = new List<string>();
        Input.TryGetValue(1, out secondlines);

        List<string> crowlines = new List<string>();
        Input.TryGetValue(2, out crowlines);

        //als de laatste vraag er al is moeten we naar poemmode	
		if (CurrentQuestion == crowlines.Count - 1)
        {
           	ShowCrowMiddle();
            return; //niet de rest uitvoeren
        }

        if (!crowOnly)
        {
			QuestionOne.text = firstlines[CurrentQuestion];
			QuestionOne.GetComponent<WriteOn>().Init();

			QuestionTwo.text = secondlines[CurrentQuestion];
			QuestionTwo.GetComponent<WriteOn>().Init();

            QuestionIndex++;

        }

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
    void Update()
    {        
		if (PoemMode)
        	Swipe();
    }

    //http://forum.unity3d.com/threads/swipe-in-all-directions-touch-and-mouse.165416/
    //inside class
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    public void Swipe()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            firstPressPos = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
        }
        if (UnityEngine.Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();

            ////swipe upwards
            //if(currentSwipe.y > 0 && currentSwipe.x > -0.5f  && currentSwipe.x < 0.5f)
            //{
            //    Debug.Log("up swipe");
            //}
            ////swipe down
            //if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            //{
            //    Debug.Log("down swipe");
            //}

            //swipe left
            if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                //laad vorige 
                if (CurrentPoem < CurrentPoems.Count - 1)
                {
                    CurrentPoem++;
                    this.GetComponent<UDPSend>().sendString("disablefx");
                    SwipeGUIRight.SetActive(true);
                }
                else
                {
                    //switchToQuestionMode();
					ShowCrowOutro();
					return; //moeten niet verder 
                }

                string t = CurrentPoems[CurrentPoem].Replace("%EACUTE%", "é");
                TheCrow.text = t;
            }
            //swipe right
            if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                //laad volgende gedicht
                if (CurrentPoem > 0)
                {
                    CurrentPoem--;
                    if (CurrentPoem == 0)
                        SwipeGUIRight.SetActive(false);
                }

                //%EACUTE%
                string t = CurrentPoems[CurrentPoem].Replace("%EACUTE%", "é");
                TheCrow.text = t;
            }
        }
    }
    
    private void switchToQuestionMode()
    {
        PoemMode = false;
        QuestionIndex = 0;
        CurrentQuestion = 0;

        Answers = new List<int>();

        CrowIndex = 0;
        TheCrow.GetComponent<WriteOn>().NoClick = false;
        QuestionOne.GetComponent<WriteOn>().NoClick = false;
        QuestionTwo.GetComponent<WriteOn>().NoClick = false;

        QuestionOneGO.SetActive(true);
        QuestionTwoGO.SetActive(true);
       // TheCrowGO.SetActive(true);

        SwipeGUIRight.SetActive(false);
        SwipeGUILeft.SetActive(false);

        this.NoClick = false;

        setAnswersToGameObjects();
    }
    private void switchToPoemMode()
    {
        PoemMode = true;
        QuestionOne.text = "";
        QuestionTwo.text = "";

        CurrentPoems = new List<string>();
        CurrentPoem = 0;

        //TheCrow.GetComponent<WriteOn>().NoClick = true;
        QuestionOne.GetComponent<WriteOn>().NoClick = true;
        QuestionTwo.GetComponent<WriteOn>().NoClick = true;

        QuestionOneGO.SetActive(false);
        QuestionTwoGO.SetActive(false);
        //TheCrowGO.SetActive(true);

        this.NoClick = true;        

        SwipeGUILeft.SetActive(true);
        //SwipeGUIRight.SetActive(true);

        setPoemToGameObjects();
    }

    public void NextAnswer(int index)
    {    
		if (CrowIntroIntro) {
			AnswerDisabled = true;
			ShowCrowIntro();
			//CrowIntro = false;
			CrowIntroIntro = false;
		}
		else if (CrowIntro) {
			AnswerDisabled = true;
			switchToQuestionMode ();
			CrowIntro = false;
		} else if (CrowMiddle) {
			AnswerDisabled = true;
			switchToPoemMode ();
			CrowMiddle = false;
		} else if (CrowOutro) {
			AnswerDisabled = true;
			ShowCrowOutroOutro();
			CrowOutro = false;
		}
		else if (CrowOutroOutro) {
			AnswerDisabled = true;
			ShowCrowIntroIntro();
			CrowOutroOutro = false;
		}
		else if (!NoClick && !PoemMode && !AnswerDisabled)
		{   
			GameObject.Find("CrowSound").GetComponent<SoundManager>().PlayCrowdRandom();
            this.GetComponent<UDPSend>().sendString(ActiveOptions[CurrentQuestion][index]);
			AnswerDisabled = true;
            setAnswersToGameObjects();

            Answers.Add(index);

            CurrentQuestion++;
        }

    }

	void ShowCrowOutro()
	{
		this.GetComponent<UDPSend>().sendString("startfx");

		string CrowT = TheCrowOutro;
		List<string[]> replacements = new List<string[]> ();

		replacements.Add (new string[]{"isn't", "is"});
		replacements.Add (new string[]{"wasn't", "was"});
		replacements.Add (new string[]{"didn't need", "needed"});

		for (int a = 0; a < replacements.Count; a++) {
			CrowT = CrowT.Replace(replacements[a][System.Math.Abs(Answers[a]-1)], replacements[a][Answers[a]]); 
		}

		TheCrow.text = CrowT;
		SwipeGUILeft.SetActive (false);
		SwipeGUIRight.SetActive (false);
		QuestionTwo.text = "";
		QuestionOneGO.SetActive (true);

		Vector3 pos = GameObject.Find("Text 1").transform.position;
		pos.y -= 24;
		GameObject.Find("Text 1").transform.position = pos;
		QuestionOne.text = "Continue...";
		CrowOutro = true;
		PoemMode = false;
	}

	void ShowCrowOutroOutro()
	{
		TheCrow.text = TheCrowOutroOutro;
		QuestionTwo.text = "";
		Vector3 pos = GameObject.Find("Text 1").transform.position;
		pos.y += 24;
		GameObject.Find("Text 1").transform.position = pos;
		QuestionOne.text = "OK";
		CrowOutroOutro = true;
	}

	void ShowCrowMiddle()
	{
		TheCrow.text = TheCrowMiddle;
		QuestionTwo.text = "";
		QuestionOne.text = "OK";
		CrowMiddle = true;
	}

	private void ShowCrowIntroIntro()
	{
		logoGO.SetActive (true);
		SwipeGUILeft.SetActive (false);
		TheCrow.text = "";
		QuestionOne.text = "";
		QuestionTwo.text = "";
		CrowIntroIntro = true;
	}

	private void ShowCrowIntro()
	{
		logoGO.SetActive (false);

		SwipeGUILeft.SetActive (false);
		TheCrow.text = TheCrowIntro;
		QuestionTwo.text = "";
		//QuestionTwoGO.SetActive (false);
		QuestionOne.text = "OK!";

		CrowIntro = true;
	}

	void DisableQuestions()
	{
		QuestionOneGO.SetActive (false);
		QuestionTwoGO.SetActive (false);
	}

	void EnableQuestions()
	{
		QuestionOneGO.SetActive (false);
		QuestionTwoGO.SetActive (false);
	}
}
public class DuplicateKeyComparer<TKey>
                :
             IComparer<TKey> where TKey : IComparable
{
    #region IComparer<TKey> Members

    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1;   // Handle equality as beeing greater
        else
            return result;
    }

    #endregion
}