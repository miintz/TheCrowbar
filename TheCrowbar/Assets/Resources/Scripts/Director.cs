using UnityEngine;
using System.Collections;

namespace Assets.Resources.Scripts
{
    public class Director : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            CommunicationState.StartNewCommunicationState("COM1");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}