using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class WriteOn : MonoBehaviour
{
    public int WriteOnRate = 150; //woord per zoveel ms
    public Text Target; //text script target
    public String PauseChar;
    public bool Interactable;
    public bool NoTouch = true;
    public int Delay;

    private String[] Words;
    private float WordTimer;
    private int CurrentIndex;
    private bool Delaying = false;
    private bool Delayed = false;

    private bool ReadyForLaunch = false;

    public void Start()
    {
       //niks
    }
    public void Init()
    {
        //hide text
        String TargetText = Target.text;
        Target.text = "";
        Words = TargetText.Split(' '); //dit mag blijkbaar tegenwoordig
        WordTimer = 0.0f;

        CurrentIndex = 0;

        if (Delay > 0)
        {
            Delaying = true;
            Delayed = Delaying;
        }

        ReadyForLaunch = true;
    }

    public void Update()
    {
        if (ReadyForLaunch)
        {
            WordTimer += Time.deltaTime * 1000;

            //eerst moet de delay voorbij zijn
            if (Delaying && WordTimer >= Delay)
            {
                Delaying = false;
                WordTimer = 0.0f;
            }

            if (WordTimer >= WriteOnRate && CurrentIndex < Words.Length && !Delaying)
            {
                Target.text = "";
                for (int i = 0; i < CurrentIndex + 1; i++)
                {
                    if (i != 0)
                        Target.text += " " + Words[i];
                    else
                        Target.text += Words[i];
                }

                CurrentIndex++;
                WordTimer = 0.0f;
            }

            if (Interactable)
            {
                int fingerCount = 0;
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                        fingerCount++;
                }

                if (NoTouch && Input.GetMouseButtonDown(0))
                {
                    Interact();
                }
                else if (fingerCount > 0)
                {
                    Interact();
                }
            }
        }
    }

    public void Reset()
    {
        if (Delayed)
            Delaying = Delayed;

        String TargetText = Target.text;
        Target.text = "";

        Words = TargetText.Split(' '); //dit mag blijkbaar tegenwoordig
        WordTimer = 0.0f;

        CurrentIndex = 0;
    }

    private void Interact()
    {        
        //omzeil eventsystem    
        //WriteOn[] o = (WriteOn[])Resources.FindObjectsOfTypeAll(typeof(WriteOn));       
        
        //foreach (WriteOn writer in o)
        //{
        //    if (writer.Interactable)
        //    {
        //        Vector3 p = writer.gameObject.transform.position;
        //        Vector3 m = Input.mousePosition;

        //        float distance = Vector3.Distance(p, m);
        //        Debug.Log(writer.gameObject.name + " " + distance);
        //    }
        //}
    }
}