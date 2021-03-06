﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SpriteController : MonoBehaviour {

    public bool ShufflePositionOnStart = true;
    public bool HideFlamesOnStart = true;
    public bool SingleEffect = true;
    private List<int> ActiveEffects = new List<int>();

    private Dictionary<string, List<GameObject>> Sprites;
    private Dictionary<string, List<string>> SpritesVarietyOriginal;
    private Dictionary<string, Dictionary<string, Transform>> SpritesVarietyTransforms;

    private int CurrentVarietyStepper = 0;

    public bool KeyMode = false;

    private Sprite[] BeerSprites;
    private Sprite[] WineSprites;
    private Sprite[] ChampagneSprites;
    private GameObject[] Fires;
    
    public string BackgroundOptionsOne;
    public string BackgroundOptionsTwo;

    private List<string[]> ActiveOptions = new List<string[]>();

    private string ReceivedMessage;

	// Use this for initialization
	void Start () {
        BeerSprites = Resources.LoadAll<Sprite>("Sprites/Bottles/Beer");
        WineSprites = Resources.LoadAll<Sprite>("Sprites/Bottles/Wine");
        ChampagneSprites = Resources.LoadAll<Sprite>("Sprites/Bottles/Champagne");
        
        Fires = GameObject.FindGameObjectsWithTag("flames"); //1tje gaat niet?
       
	    //we hebben een lijst met objecten...
        PopulateSpriteList();

        if (ShufflePositionOnStart)
            ShufflePosition();

        if (HideFlamesOnStart)
            SetInflammable(false);

        string[] optionsone = BackgroundOptionsOne.Split(';');
        string[] optionstwo = BackgroundOptionsTwo.Split(';');

        ActiveOptions.Add(new string[2] { optionsone[0], optionstwo[0] });
        ActiveOptions.Add(new string[2] { optionsone[1], optionstwo[1] });
        ActiveOptions.Add(new string[2] { optionsone[2], optionstwo[2] });
	}
	
	// Update is called once per frame
	void Update () {

        if (!KeyMode)
        {
            string udpr = this.GetComponent<UDPReceive>().lastReceivedUDPPacket;
            if (ReceivedMessage != udpr)
            {
                ReceivedMessage = udpr;
                handleMessage();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                SetVariety(12); //blijft altijd 1 unique fles
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                SetVariety(1);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                SetOversee(false);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                SetOversee(true);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                SetInflammable(true);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                SetInflammable(false);
            }
        }
	}   

    void SetInflammable(bool IN_FLAMES = false, bool active = true)
    {        
        for (int i = 0; i < Fires.Length; i++) //stupid!! de -1 zorgde voor de bug...
        {            
            Fires[i].SetActive(IN_FLAMES);
        }

        if(active)
            ActiveEffects.Add(0);
    }

    void SetOversee(bool visible = false, bool active = true)
    { 
        //hide een aantal flessen?
        string[] Keys = new string[Sprites.Count];
        Sprites.Keys.CopyTo(Keys, 0);

        foreach (string Key in Keys)
        {
            for (int i = 0; i < Sprites[Key].Count; i++)
            {
                Sprites[Key][i].SetActive(visible);
            }
        }

        if(active)
            ActiveEffects.Add(2);
    }

    void SetVariety(int stepper, bool active = true)
    {
        CurrentVarietyStepper = stepper;        
        
        //op basis van de shelf-by-shelf soorten
        string[] Keys = new string[Sprites.Count];
        Sprites.Keys.CopyTo(Keys, 0);

        foreach (string Key in Keys)
        {
            string current = null;
            List<string> Renderers = SpritesVarietyOriginal[Key];

            int Step = 0;
            for (int i = 0; i < Renderers.Count - 1; i++)
            {
                if (current == null)
                    current = Renderers[i];
                else
                {   
                    Step++;

                    if (Step == stepper)
                    {
                        Step = 0;
                        if(i != Renderers.Count - 1)
                            current = Renderers[i + 1];
                    }

                    //set sprite, doe dit direct want sprites zijn reference types. geeft gelul enzo
                    Sprites[Key][i].GetComponent<SpriteRenderer>().sprite = BeerSprites.First(t => t.name == current);
                    string n = Sprites[Key][i].GetComponent<SpriteRenderer>().sprite.name;
                    
                    //Transform tr = SpritesVarietyTransforms[Key][current];
                        
                    //Vector3 localScale = tr.localScale;
                    //Vector3 localPosition = Sprites[Key][i].transform.position;
                    //localPosition.y = tr.position.y;

                    //Sprites[Key][i].transform.localScale = localScale;
                    //Sprites[Key][i].transform.position = localPosition;

                }
            }            
        }

        if(active)
            ActiveEffects.Add(1);
    }

    void ShufflePosition()
    {
        if (Sprites.Count == 0)
            Debug.LogError("Populate sprites list first by calling populateSpriteList");

        string[] Keys = new string[Sprites.Count];
        Sprites.Keys.CopyTo(Keys, 0);

        foreach (string Key in Keys)
        {          
            List<GameObject> SpriteList = Sprites[Key];            

            float leftBound = 0.0f;
            float rightBound = 0.0f;

            int i = 0;            

            //hopelijk is dit op volgorde...
            foreach (GameObject Sprite in SpriteList) //get bounds
            {
                if (i == 0)               
                    leftBound = Sprite.transform.position.x;                

                if (i == SpriteList.Count - 1)                 
                    rightBound = Sprite.transform.position.x;
                
                i++;
            }

            float difference = Mathf.Abs(leftBound - rightBound);
            float distance = difference / (SpriteList.Count - 5); //????
            
            List<float> newPositions = new List<float>();
            //generate positions
            for (int z = 0; z < SpriteList.Count; z++)
            {
                newPositions.Add(leftBound + (z * distance));
            }

            System.Random random = new System.Random();
            
            //randomize sort (fisher yates)
            List<int> newkeys = new List<int>();
            
            System.Threading.Thread.Sleep(100); //dit moet want anders geeft random gvd dezelfde random getallen, want de clock is niet verder gegaan.

            for (int r = newPositions.Count - 1; r > 0; r--)
            {
                int swapIndex = random.Next(r + 1);
                newkeys.Add(swapIndex);
                float tmp = newPositions[r];

                newPositions[r] = newPositions[swapIndex];
                newPositions[swapIndex] = tmp;
            }
     
            //now set the position
            int o = 0;
            foreach (GameObject Sprite in SpriteList)
            {
                Vector3 p = Sprite.transform.position;
                p.x = newPositions[o];
                Sprite.transform.position = p;                
                o++;
            }
        }        
    }
    
    void PopulateSpriteList()
    {
        Sprites = new Dictionary<string, List<GameObject>>();

        GameObject[] bottles = GameObject.FindGameObjectsWithTag("bottle");

        //get unique parent names and make number of list
        List<string> uniqueParentNames = new List<string>();

        foreach (GameObject o in bottles)
        {
            if (!uniqueParentNames.Contains(o.transform.parent.name))
                uniqueParentNames.Add(o.transform.parent.name);
        }
        
        //nu voor elke naam, lijst maken
        foreach (string name in uniqueParentNames)
        {
           
            Sprites.Add(name, new List<GameObject>());
        }

        foreach (GameObject bottle in bottles)
        {          
            string name = bottle.transform.parent.name;
            Sprites[name].Add(bottle);
        }

        //zo.  now sort
        string[] Keys = new string[Sprites.Count];
        Sprites.Keys.CopyTo(Keys, 0);
        
        for (int k = 0; k < Keys.Length; k++)
        {         
            Sprites[Keys[k]] = Sprites[Keys[k]].OrderBy(t => t.transform.name).ToList(); //sort by name ASC
        }

        PopulateSpriteVarietyList();
    }

    void PopulateSpriteVarietyList()
    {        
        SpritesVarietyOriginal = new Dictionary<string, List<string>>();
        SpritesVarietyTransforms = new Dictionary<string, Dictionary<string, Transform>>();

        string[] Keys = new string[Sprites.Count];
        Sprites.Keys.CopyTo(Keys, 0);

        foreach (string Key in Keys)
        {            
            SpritesVarietyOriginal.Add(Key, new List<string>());
            SpritesVarietyTransforms.Add(Key, new Dictionary<string, Transform>());
        }

        foreach (string Key in Keys)
        {                        
            //namen van de textures en de scales.
            foreach (GameObject obj in Sprites[Key])
            {
                string name = obj.GetComponent<SpriteRenderer>().sprite.name;
                
                SpritesVarietyOriginal[Key].Add(name);

                if (!SpritesVarietyTransforms[Key].ContainsKey(name))
                {
                    SpritesVarietyTransforms[Key].Add(name, obj.transform);
                }
            }
        }
    }

    internal void handleMessage()
    {     
        foreach (string[] items in ActiveOptions)
        {         
            foreach (string i in items)
            {
                if (ReceivedMessage == i)
                {
                    //Debug.Log("hebbasases " + i + " " + ReceivedMessage);
                    //if (i.Contains("fl"))
                    //    Debug.Log("waarom vuurt dit niet?");

                    //if (i.Length != ReceivedMessage.Length)
                    //    Debug.Log("huh");

                    //if (i.Contains("fl")) //ik heb werkelijk waar geen flets idee waarom de case niet werkt. 
                    //{                  
                    //    SetInflammable(true);                            
                    //}   

                    switch (i)
                    {                     
                        case "flame":
                            if (SingleEffect)
                                DisableEffects();

                            SetInflammable(true);
                            
                            break;
                        case "noflame":
                            if (SingleEffect)
                                DisableEffects();

                            SetInflammable(true);
                            
                            break;
                        case "variety":
                            //SetVariety(1);
                            if (SingleEffect)
                                DisableEffects();

                            SetVariety(13);
                            
                            break;
                        case "novariety":
                            if (SingleEffect)
                                DisableEffects();

                            SetVariety(13);
                            
                            break;
                        case "oversee":
                            if (SingleEffect)
                                DisableEffects();

                            SetOversee(false);
                            
                            break;
                        case "nooversee":
                            //SetOversee(false);
                            if (SingleEffect)
                                DisableEffects();

                            SetOversee(false);
                            
                            break;                        
                    }
                }
            }
        }
    }

    private void DisableEffects()
    {
        List<int> remover = new List<int>();
        foreach (int a in ActiveEffects)
        {         
            switch (a)
            {
                case 0:
                    Debug.Log("disabling flames");
                    SetInflammable(false, false);
                    remover.Add(a);
                    break;
                case 1:
                    Debug.Log("disabling variety");
                    SetVariety(1, false);
                    remover.Add(a);
                    break;
                case 2:
                    Debug.Log("disabling oversee");
                    SetOversee(true, false);
                    remover.Add(a);
                    break;
            }
        }

        foreach (int oo in remover)
        {
            ActiveEffects.Remove(oo);
        }
    }
}