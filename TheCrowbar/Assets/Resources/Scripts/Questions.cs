using UnityEngine;
using System.Collections;

namespace Assets.Resources.Scripts
{
    public class Questions : MonoBehaviour
    { //deze draait op de tablet

        // Use this for initialization
        void Start()
        {
            CommunicationState.StartNewCommunicationState("COM1");
        }

        // Update is called once per frame
        void Update()
        {
            //deze draait op de pad 
        }
    }
}