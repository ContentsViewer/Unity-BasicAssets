using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HUDMessageManagement
{
    public class Message
    {
        public string AreaName { get; private set; }
        public string text;



        public Message(string text, string areaName)
        {
            this.text = text;
            this.AreaName = areaName;
        }
    }

    
}