using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

namespace HUDMessageManagement
{

    public class HUDMessageManager : MonoBehaviour
    {
        
        public static HUDMessageManager Instance { get; private set; }


        
        Dictionary<string, BaseHUDMessageArea> messageAreaMap;
        Dictionary<Message, BaseHUDMessageArea> messageToAreaMap;
        

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// MessageAreaを登録します.
        /// </summary>
        /// <param name="areaToAdd"></param>
        public void AddMessgaeArea(BaseHUDMessageArea areaToAdd)
        {
            if (messageAreaMap.ContainsKey(areaToAdd.areaName))
            {
                Debug.LogWarning("Such area name is already registed. areaName: " + areaToAdd.areaName);
                return;
            }

            messageAreaMap.Add(areaToAdd.areaName, areaToAdd);
        }
        
        
        void Awake()
        {
            messageAreaMap = new Dictionary<string, BaseHUDMessageArea>();
            messageToAreaMap = new Dictionary<Message, BaseHUDMessageArea>();


            Instance = this;
        }

        

        void Update()
        {

            //Debug.Log(messageMap.Count);
        }

        public BaseHUDMessageArea GetMessageArea(string areaName)
        {
            if (messageAreaMap.ContainsKey(areaName))
            {
                return messageAreaMap[areaName];
            }

            return null;
        }


        public void ExitMessage(Message messageToExit)
        {
            if (!messageToAreaMap.ContainsKey(messageToExit))
            {
                return;
            }

            messageToAreaMap[messageToExit].ExitMessage(messageToExit);
            
        }


        public void SetMessage(Message message)
        {
            if (messageToAreaMap.ContainsKey(message))
            {
                return;
            }


            if (!messageAreaMap.ContainsKey(message.AreaName))
            {
                Debug.LogWarning("Such area name doesn't exist. areaName: " + message.AreaName);
                return;
            }

            var messageArea = messageAreaMap[message.AreaName];

            messageToAreaMap.Add(message, messageArea);

            messageArea.SetMessage(message);



        }
        

        public void ShowMessage(Message message, bool dontOverride = false)
        {
            if (!messageToAreaMap.ContainsKey(message))
            {
                SetMessage(message);
            }

            messageToAreaMap[message].ShowMessage(message, dontOverride);


        }

        public void UnlinkMessage(Message message)
        {
            messageToAreaMap.Remove(message);
        }
    }
}