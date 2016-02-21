using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using Assets.Resources.Scripts;
using System.IO;
using System;
using System.Linq;

public class WriteOnManager : MonoBehaviour
{


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

    public bool NoClick = true;

    private Dictionary<int, List<string>> Input;
    private int CrowIndex;
    private int QuestionIndex;

    private List<int> Answers;
    private SortedList<int, string> PoemsList;
    private List<string> CurrentPoems;
    private int CurrentPoem = 0;

    public bool PoemMode;

    // Use this for initialization
    void Start()
    {
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

        //vul lijst met poems
        populatePoems();

        if (QuestionsFirst)
            switchToQuestionMode();
        else
            switchToPoemMode();
    }

    private void setPoemToGameObjects()
    {
        TheCrow.verticalOverflow = VerticalWrapMode.Overflow;
        //TheCrow.text = PoemsList.get

        IList<int> k = PoemsList.Keys;
        IList<string> v = PoemsList.Values;

        int max = k.Max();
        System.Random rand = new System.Random();

        int poemInt = rand.Next(0, max);
        
        CurrentPoems = new List<string>();
        //pak alle texts met deze key
        for (int i = 0; i < v.Count; i++)
        {
            if (k[i] == poemInt)
                CurrentPoems.Add(v[i]);
        }
        //Debug.Log(CurrentPoems.Count + " peomInt: " + poemInt);
        //gebruik de eerste uit de currentpoems lijst
        TheCrow.text = CurrentPoems[0];
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
        if (QuestionIndex == crowlines.Count)
        {
            Debug.Log("switching to poemmode");
            switchToPoemMode();
            return; //niet de rest uitvoeren
        }

        if (!crowOnly)
        {
            QuestionOne.text = firstlines[QuestionIndex];
            QuestionOne.GetComponent<WriteOn>().Init();

            QuestionTwo.text = secondlines[QuestionIndex];
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
        //for (int i = 0; i < Answers.Count; i+)+   
        //{
        //    List<string> list = new List<string>();
        //    Answers.TryGetValue(i, out list);

        //    foreach (string answer in list)
        //    {
        //        //Debug.Log(answer + " " + i);
        //    }
        //}

        //detect swipe

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
                Debug.Log("left swipe");
                //laad vorige 
                if (CurrentPoem < CurrentPoems.Count - 1)
                    CurrentPoem++;
                else
                {
                    Debug.Log("final swipe! switching to question mode");
                    switchToQuestionMode();
                }


                TheCrow.text = CurrentPoems[CurrentPoem];
            }
            //swipe right
            if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                Debug.Log("right swipe");
                //laad volgende gedicht
                if (CurrentPoem > 0)
                    CurrentPoem--;

                TheCrow.text = CurrentPoems[CurrentPoem];
            }
        }
    }

    private void switchToQuestionMode()
    {
        PoemMode = false;
        QuestionIndex = 0;
        CrowIndex = 0;
        TheCrow.GetComponent<WriteOn>().NoClick = false;
        QuestionOne.GetComponent<WriteOn>().NoClick = false;
        QuestionTwo.GetComponent<WriteOn>().NoClick = false;

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

        TheCrow.GetComponent<WriteOn>().NoClick = true;
        QuestionOne.GetComponent<WriteOn>().NoClick = true;
        QuestionTwo.GetComponent<WriteOn>().NoClick = true;

        this.NoClick = true;

        setPoemToGameObjects();
    }

    public void NextAnswer(int index)
    {
     //   Debug.Log("peommode " +  PoemMode);
        if (!NoClick && !PoemMode)
        {
            setAnswersToGameObjects();
            Answers.Add(index);
        }

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