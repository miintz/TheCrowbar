using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace Assets.Resources.Scripts
{
	public class SoundManager : MonoBehaviour
	{
		//string absolutePath = "./"; // relative path to where the app is running

	 	AudioSource src;
		List<AudioClip> singles = new List<AudioClip>();
		List<AudioClip> crowds = new List<AudioClip>();

		int soundIndex = 0;

		//compatible file extensions
		//string[] fileTypes = { "ogg", "wav" };

		//FileInfo[] files;

		void Start()
		{
			//being able to test in unity
			//if (Application.isEditor) absolutePath = "Assets/Resources/Clips";
			if (src == null) src = gameObject.AddComponent<AudioSource>();
			reloadSounds();
		}

		AudioClip[] clips;

		void reloadSounds()
		{
			singles = UnityEngine.Resources.LoadAll<AudioClip>("Clips/Single").ToList();
			crowds = UnityEngine.Resources.LoadAll<AudioClip>("Clips/Crowd").ToList();
				
			/*
			absolutePath = "Assets/Resources/Clips/Single";

			DirectoryInfo info = new DirectoryInfo(absolutePath);
			files = info.GetFiles();

			//check if the file is valid and load it
			foreach (FileInfo f in files)
			{
				if (validFileType(f.FullName))
				{
					StartCoroutine(loadFile(f.FullName, true));
				}
			}

			absolutePath = "Assets/Resources/Clips/Crowd";

			info = new DirectoryInfo(absolutePath);
			files = info.GetFiles();

			//check if the file is valid and load it
			foreach (FileInfo f in files)
			{

				if (validFileType(f.FullName))
				{
					StartCoroutine(loadFile(f.FullName, false));
				}
			}
			*/
		}

		/*
		bool validFileType(string filename)
		{
			foreach (string ext in fileTypes)
			{
				if (!filename.Contains(".meta"))
				{
					if (filename.IndexOf(ext) > -1) return true;
				}
			}
			return false;
		}
		*/

		/*
		IEnumerator loadFile(string path, bool crowd = false)
		{
			WWW www = new WWW("file://" + path);

			AudioClip myAudioClip = www.audioClip;
			while (!myAudioClip.isReadyToPlay)
				yield return www;

			AudioClip clip = www.GetAudioClip(false);
			string[] parts = path.Split('\\');
			clip.name = parts[parts.Length - 1];

			if (!crowd)
			{                
				crowds.Add(clip);
			}
			else
				singles.Add(clip);
		}
		*/

		public void PlaySingleRandom()
		{	
			//if(!GameObject.Find("EventSystem").GetComponent<WriteOnManager>().AnswerDisabled)
				src.PlayOneShot(singles[UnityEngine.Random.Range(0, singles.Count)]);
		}

		public void PlayCrowdRandom()
		{
			if(!GameObject.Find("EventSystem").GetComponent<WriteOnManager>().AnswerDisabled)
				src.PlayOneShot(crowds[UnityEngine.Random.Range(0, singles.Count)]);
		}
	}
}