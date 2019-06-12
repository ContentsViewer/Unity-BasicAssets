using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;

namespace HUDMessageManagement
{

    public abstract class BaseHUDMessageArea : MonoBehaviour
    {
        
        public string areaName = "Alert";


        public abstract void SetMessage(Message messageToSet);
        public abstract void ShowMessage(Message messageToShow, bool dontOverride = false);
        public abstract void ExitMessage(Message messageToExit);
        public abstract void RemoveMessage(Message messageToRemove);

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            HUDMessageManager.Instance.AddMessgaeArea(this);
        }

        protected virtual void Update() { }
        
    }

}