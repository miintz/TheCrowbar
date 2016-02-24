using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BottleController : MonoBehaviour {

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
    private GameObject[] Firelights;
    
    public string BackgroundOptionsOne;
    public string BackgroundOptionsTwo;

    private List<string[]> ActiveOptions = new List<string[]>();

    private string ReceivedMessage;
    private float MainLightIntensity = 4;
    private float DirLightIntensity = 1;
    private float ShelfAlbedeo = 1f;

    Color[] colors = new Color[] { Color.red, Color.blue, Color.yellow, Color.green, Color.cyan, new Color(213f / 255f, 165f / 255f, 0f), new Color(135f / 255f, 0f, 213f / 255f) };

	// Use this for initialization
	void Start () {
        //BeerSprites = Resources.LoadAll<Sprite>("Sprites/Bottles/Beer");
        //WineSprites = Resources.LoadAll<Sprite>("Sprites/Bottles/Wine");
        //ChampagneSprites = Resources.LoadAll<Sprite>("Sprites/Bottles/Champagne");
        
        Fires = GameObject.FindGameObjectsWithTag("flames"); //1tje gaat niet?
        Firelights = GameObject.FindGameObjectsWithTag("Firelight"); //1tje gaat niet?
        GameObject.FindGameObjectsWithTag("Firelight").ToList().ForEach(t => t.GetComponent<Light>().range = 9);

        //GameObject[] aa = GameObject.FindGameObjectsWithTag("bottle");
        //foreach (GameObject a in aa)
        //{
        //    a.GetComponent<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //}
        //GameObject.Find("BrickWall1").GetComponent<SpriteRenderer>().receiveShadows = true;
        

	    //we hebben een lijst met objecten...
        //PopulateSpriteList();

        if (ShufflePositionOnStart)
            ShuffleColors();
            //ShufflePosition();


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

        if (MainLightIntensity < GameObject.Find("MainLight").GetComponent<Light>().intensity)
            GameObject.Find("MainLight").GetComponent<Light>().intensity -= 0.1f;
        else
            GameObject.Find("MainLight").GetComponent<Light>().intensity += 0.1f;

        if (GameObject.Find("MainLight").GetComponent<Light>().intensity == MainLightIntensity)
            MainLightIntensity = GameObject.Find("MainLight").GetComponent<Light>().intensity;

        if (ShelfAlbedeoDec && ShelfAlbedeo < GameObject.FindGameObjectWithTag("Shelf").GetComponent<MeshRenderer>().material.color.r)
            GameObject.FindGameObjectsWithTag("Shelf").ToList().ForEach(t => t.GetComponent<MeshRenderer>().material.color = new Color(t.GetComponent<MeshRenderer>().material.color.r - 0.05f,t.GetComponent<MeshRenderer>().material.color.g - 0.05f ,t.GetComponent<MeshRenderer>().material.color.b - 0.05f));
        else if(!ShelfAlbedeoDec && ShelfAlbedeo > GameObject.FindGameObjectWithTag("Shelf").GetComponent<MeshRenderer>().material.color.r)
            GameObject.FindGameObjectsWithTag("Shelf").ToList().ForEach(t => t.GetComponent<MeshRenderer>().material.color = new Color(t.GetComponent<MeshRenderer>().material.color.r + 0.05f, t.GetComponent<MeshRenderer>().material.color.g + 0.05f, t.GetComponent<MeshRenderer>().material.color.b + 0.05f));

        //BUGGY
        //Debug.Log(GameObject.Find("DirLight").GetComponent<Light>().intensity + " " + DirLightIntensity);
        //if (DirLightIntensity != System.Math.Round(GameObject.Find("DirLight").GetComponent<Light>().intensity, 1))
        //{
        //    if (DirLightIntensity < System.Math.Round(GameObject.Find("DirLight").GetComponent<Light>().intensity, 1))
        //        GameObject.Find("DirLight").GetComponent<Light>().intensity -= 0.1f;
        //    else
        //        GameObject.Find("DirLight").GetComponent<Light>().intensity += 0.1f;

        //    if (System.Math.Round(GameObject.Find("DirLight").GetComponent<Light>().intensity, 1) == DirLightIntensity)
        //        DirLightIntensity = (float)System.Math.Round(GameObject.Find("DirLight").GetComponent<Light>().intensity, 1);
        //}

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
                if(SingleEffect)
                    DisableEffects();

				SetReplace(true);
                //SetVariety(true); //blijft altijd 1 unique fles
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (SingleEffect)
                    DisableEffects();

				SetReplace(false);
				//SetVariety(false);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                if (SingleEffect)
                    DisableEffects();

                SetOversee(false);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                if (SingleEffect)
                    DisableEffects();

                SetOversee(true);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                if (SingleEffect)
                    DisableEffects();

                SetInflammable(true);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (SingleEffect)
                    DisableEffects();

                SetInflammable(false);
            }
        }
	}   

    void SetInflammable(bool IN_FLAMES = false, bool active = true)
    {        
        for (int i = 0; i < Fires.Length; i++) //stupid!! de - 1 zorgde voor de bug...
        {            
            Fires[i].SetActive(IN_FLAMES);
         
        }
        for (int i = 0; i < Firelights.Length; i++) //stupid!! de - 1 zorgde voor de bug...
        {
            Firelights[i].SetActive(IN_FLAMES);
        }

        if(active)
            ActiveEffects.Add(0);
    }

    void SetOversee(bool visible = false, bool active = true)
    { 
        //hide een aantal flessen?
        if (!visible)
        {
			List<GameObject> GOs = GameObject.FindGameObjectsWithTag("bottle").ToList();
			foreach(GameObject a in GOs)
			{
				if(UnityEngine.Random.Range(0,2) == 1)
            		a.GetComponent<MeshRenderer>().enabled = false;
			}

            MainLightIntensity = 1;
            DirLightIntensity = 0.6f;
            ShelfAlbedeo = 0.5f;
            ShelfAlbedeoDec = true;
            GameObject.FindGameObjectsWithTag("Shelf").ToList().ForEach(t => t.GetComponent<MeshRenderer>().material.color = Color.gray);
        }
        else
        {
            GameObject.FindGameObjectsWithTag("bottle").ToList().ForEach(t => t.GetComponent<MeshRenderer>().enabled = true);
            MainLightIntensity = 5;
            DirLightIntensity = 1;
            ShelfAlbedeo = 1;
            ShelfAlbedeoDec = false;

        }

        if(active)
            ActiveEffects.Add(2);
    }

	void SetReplace(bool wine = false, bool active = true)
	{
		if(wine)
			GameObject.FindGameObjectsWithTag ("bottle").ToList ().ForEach (t => t.transform.localScale = new Vector3(1.0f, 0.6f, 1.0f));
		else
			GameObject.FindGameObjectsWithTag ("bottle").ToList ().ForEach (t => t.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f));

		if (active)
			ActiveEffects.Add(0);
	}

    void SetVariety(bool single = false, bool active = true)
    {
        //kies kleur
        if (single)
        {
            Color chosen = colors[UnityEngine.Random.Range(0, colors.Length - 1)];
            GameObject.FindGameObjectsWithTag("bottle").ToList().ForEach(t => t.GetComponent<MeshRenderer>().material.color = chosen);
            GameObject.Find("MainLight").GetComponent<Light>().color = chosen;
        }
        else
        {
            ShuffleColors();
            GameObject.Find("MainLight").GetComponent<Light>().color = Color.white;

        }

        
        if (active)
                ActiveEffects.Add(1);
    }

    void ShuffleColors()
    {         
        GameObject.FindGameObjectsWithTag("bottle").ToList().ForEach(t => t.GetComponent<MeshRenderer>().material.color = colors[UnityEngine.Random.Range(0, colors.Length - 1)]);
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
        //Debug.Log("handling message:L " + ReceivedMessage);
        //foreach (string[] items in ActiveOptions)
        //{         
        //    foreach (string i in items)
        //    {
        //        if (ReceivedMessage == i)
        //        {

                    //if (i.Contains("fl"))
                        //Debug.Log("waarom vuurt dit niet?");

                    //if (i.Length != ReceivedMessage.Length)
                    //    Debug.Log("huh");

                   

                    switch (ReceivedMessage)
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

                            //SetVariety(true);
							SetReplace(true);
                            
                            break;
                        case "novariety":
                            if (SingleEffect)
                                DisableEffects();

                            //SetVariety(true);
							SetReplace(true);
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

                        case "disablefx":
                            DisableEffects();
                            break;
						case "startfx":
							SetOversee(false);
							SetReplace(true);
							SetInflammable(true);
							break;
                    }
                //}
        //    }
        //}
    }

    private void DisableEffects()
    {
        List<int> remover = new List<int>();
        foreach (int a in ActiveEffects)
        {         
            switch (a)
            {
                case 0:
                    SetInflammable(false, false);
                    remover.Add(a);
                    break;
                case 1:
                    //SetVariety(false, false);
					SetReplace(false,false);
                    remover.Add(a);
                    break;
                case 2:
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

    public bool ShelfAlbedeoDec { get; set; }
}