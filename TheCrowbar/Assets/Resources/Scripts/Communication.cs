using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace Assets.Resources.Scripts
{
    public static class CommunicationState
    {

        private static List<Exception> Errors;
        private static SerialPort Port;

        //deze draait aan beide kanten, en communiceert door COM.
        public static void StartNewCommunicationState(string Com = "COM1")
        {
            Port = new SerialPort(Com);

            try
            {
                Port.Open();
            }
            catch (Exception E)
            {
                LogError(E);
            }

        }

        private void ListenToTablet()
        {
            //dit word mss een beetje lastig met EOLs 
        }

        public void ToComputer() //moet een com selectie inbouwen, als dat uberhaupt kan 
        {
            //send (tablet kant)
        }

        public void FromTablet()
        {
            //lees de state uit (computer kant)
        }

        private static void LogError(Exception E)
        {
            Errors.Add(E);
        }

        public void DumpLogErrors()
        {
            foreach (Exception e in Errors)
            {
                Debug.Log(e.InnerException.Message);
            }
        }
    }
}