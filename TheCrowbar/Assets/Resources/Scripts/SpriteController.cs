using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SpriteController : MonoBehaviour {

    public bool ShufflePositionOnStart = true;

    private Dictionary<string, List<GameObject>> Sprites;
    private Dictionary<string, List<string>> SpritesVarietyOriginal;
    private Dictionary<string, Dictionary<string, Transform>> SpritesVarietyTransforms;

    private int CurrentVarietyStepper = 0;

    private Sprite[] BeerSprites;
    private Sprite[] WineSprites;
    private Sprite[] ChampagneSprites;

	// Use this for initialization
	void Start () {
        BeerSprites = Resources.LoadAll<Sprite>("Sprites/Bottles/Beer");
        WineSprites = Resources.LoadAll<Sprite>("Sprites/Bottles/Wine");
        ChampagneSprites = Resources.LoadAll<Sprite>("Sprites/Bottles/Champagne");
        
	    //we hebben een lijst met objecten...
        PopulateSpriteList();

        if (ShufflePositionOnStart)
            ShufflePosition();
	}
	
	// Update is called once per frame
	void Update () {
      
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetVariety(10);
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
	}

    void SetOversee(bool active = false)
    { 
        //hide een aantal flessen?
        string[] Keys = new string[Sprites.Count];
        Sprites.Keys.CopyTo(Keys, 0);

        foreach (string Key in Keys)
        {
            for (int i = 0; i < Sprites[Key].Count; i++)
            {
                Sprites[Key][i].SetActive(active);
            }
        }
    }

    void SetVariety(int stepper)
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
}