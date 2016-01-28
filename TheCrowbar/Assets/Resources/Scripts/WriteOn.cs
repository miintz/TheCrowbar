using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
public class WriteOn : MonoBehaviour {

    public int WriteOnRate; //frames?
    public Text Target; //text script target

    private String[] Words;
    private float WordTimer;
    private int CurrentIndex;

	void Start () {
	    //hide text
        String TargetText = Target.text;
        Target.text = "";

        Words = TargetText.Split(' '); //dit mag blijkbaar tegenwoordig
        WordTimer = 0.0f;
        
        CurrentIndex = 0;
	}
	
	void Update () {
        WordTimer += Time.deltaTime * 1000;

        if (WordTimer >= WriteOnRate && CurrentIndex < Words.Length)
        {
            Target.text = "";

            for (int i = 0; i < CurrentIndex; i++)
            {                
                if (i != 0)
                    Target.text += " " + Words[i];
                else
                    Target.text += Words[i];
            }

            CurrentIndex++;
            WordTimer = 0.0f;
        }    
    }
}
