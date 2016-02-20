using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SpriteRandomizer : MonoBehaviour {

    public bool ShufflePositionOnStart = true;

    private Dictionary<string, List<GameObject>> Sprites;

	// Use this for initialization
	void Start () {
	    //we hebben een lijst met objecten...
        PopulateSpriteList();

        if (ShufflePositionOnStart)
            ShufflePosition();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SetVariety(float percentage)
    { 
        
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
            float distance = difference / (SpriteList.Count - 6); //????
            
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
    }
}
