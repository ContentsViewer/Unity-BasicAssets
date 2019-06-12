using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace HUDMessageManagement
{



    public class SlotHUDMessageArea : BaseHUDMessageArea
    {
        public GameObject textPrefab;

        [Space(10)]
        public float displayTime = 2.0f;
        public float updateTime = 0.3f;
        public UpdateMode updateMode;

        public enum UpdateMode
        {
            ScaledTime,
            UnscaledTime
        }
        LinkedList<Message> readyList = new LinkedList<Message>();
        LinkedList<Message> playingList = new LinkedList<Message>();

        class ControlBlock
        {
            public LinkedListNode<Message> listNode = null;
            public Text text;
            public float startTime;
            public Vector2 anchorMin;
            public Vector2 anchorMax;
        }
        Dictionary<Message, ControlBlock> controlBlockMap = new Dictionary<Message, ControlBlock>();

        float lastUpdateTime;
        bool slideUp;
        bool slideIn;
        float slideUpAmount;
        RectTransform rectTrfm;

        protected override void Awake()
        {
            base.Awake();

            rectTrfm = GetComponent<RectTransform>();
        }

        protected override void Update()
        {
            base.Update();

            if (GetTime() > lastUpdateTime + updateTime && readyList.Count > 0)
            {
                var message = readyList.First.Value;
                var controlBlock = controlBlockMap[message];

                readyList.RemoveFirst();

                if (playingList.Count > 0) slideUp = true;


                controlBlock.listNode = playingList.AddLast(message);
                controlBlock.startTime = GetTime();

                var newTextObject = Instantiate(textPrefab);
                controlBlock.text = newTextObject.GetComponent<Text>();
                newTextObject.transform.SetParent(transform, false);

                slideUpAmount = controlBlock.text.rectTransform.rect.height / rectTrfm.rect.height;
                //Debug.Log(controlBlock.text.rectTransform.rect.height + " : " + rectTrfm.rect.height);
                controlBlock.anchorMin = new Vector2(-1.0f, 0.0f);
                controlBlock.anchorMax = new Vector2(0.0f, 0.0f);

                slideIn = true;

                lastUpdateTime = GetTime();
            }


            if (slideIn)
            {
                float slideValue = Mathf.Lerp(0.0f, 1.0f, (GetTime() - lastUpdateTime) / updateTime * 2.0f);
                var message = playingList.Last.Value;
                var controlBlock = controlBlockMap[message];

                controlBlock.text.rectTransform.anchorMin = controlBlock.anchorMin + new Vector2(slideValue, 0.0f);
                controlBlock.text.rectTransform.anchorMax = controlBlock.anchorMax + new Vector2(slideValue, 0.0f);

                if (GetTime() - lastUpdateTime > updateTime / 2.0f)
                {
                    slideIn = false;

                    controlBlock.anchorMin = new Vector2(0.0f, 0.0f);
                    controlBlock.anchorMax = new Vector2(1.0f, 0.0f);
                }
            }

            if (slideUp)
            {

                float upValue = Mathf.Lerp(0.0f, slideUpAmount, (GetTime() - lastUpdateTime) / updateTime * 2.0f);

                foreach (var message in playingList)
                {
                    var controlBlock = controlBlockMap[message];
                    if (controlBlock.listNode == playingList.Last) continue;

                    controlBlock.text.rectTransform.anchorMin = controlBlock.anchorMin + new Vector2(0.0f, upValue);
                    controlBlock.text.rectTransform.anchorMax = controlBlock.anchorMax + new Vector2(0.0f, upValue);

                }

                if (GetTime() - lastUpdateTime > updateTime / 2.0f)
                {
                    slideUp = false;

                    foreach (var message in playingList)
                    {
                        var controlBlock = controlBlockMap[message];
                        if (controlBlock.listNode == playingList.Last) continue;

                        controlBlock.anchorMin = controlBlock.text.rectTransform.anchorMin;
                        controlBlock.anchorMax = controlBlock.text.rectTransform.anchorMax;
                    }

                }
            }

            var removeList = new List<Message>();
            foreach (var message in playingList)
            {
                var controlBlock = controlBlockMap[message];

                var color = controlBlock.text.color;
                var t = (GetTime() - controlBlock.startTime) / displayTime;
                t *= t;
                color.a = Mathf.Lerp(1.0f, 0.0f, t);
                controlBlock.text.color = color;

                controlBlock.text.text = message.text.Replace("{a}", ((int)(color.a * 255)).ToString("X2"));
                if (GetTime() - controlBlock.startTime > displayTime)
                    removeList.Add(message);
            }

            foreach (var message in removeList)
            {
                RemoveMessage(message);
            }


        }

        public override void SetMessage(Message messageToSet)
        {
            controlBlockMap.Add(messageToSet, new ControlBlock());
        }

        public override void ShowMessage(Message messageToShow, bool dontOverride = false)
        {
            var controlBlock = controlBlockMap[messageToShow];

            if (dontOverride)
            {
                if (controlBlock.listNode != null) return;
            }

            controlBlock.listNode = readyList.AddLast(messageToShow);
        }

        public override void ExitMessage(Message messageToExit)
        {
            // 出たものは戻らない.
            return;
        }

        public override void RemoveMessage(Message messageToRemove)
        {
            var controlBlock = controlBlockMap[messageToRemove];
            controlBlock.listNode.List.Remove(controlBlock.listNode);
            controlBlockMap.Remove(messageToRemove);
            if (controlBlock.text != null)
            {
                Destroy(controlBlock.text.gameObject);
            }
            HUDMessageManager.Instance.UnlinkMessage(messageToRemove);
        }

        public void ClearReadyMessages()
        {
            foreach(var message in readyList)
            {
                controlBlockMap.Remove(message);
                HUDMessageManager.Instance.UnlinkMessage(message);
            }

            readyList.Clear();
        }

        float GetDeltaTime()
        {
            if (updateMode == UpdateMode.UnscaledTime)
            {
                return Time.unscaledDeltaTime;
            }
            else if (updateMode == UpdateMode.ScaledTime)
            {
                return Time.deltaTime;
            }

            return Time.deltaTime;
        }

        float GetTime()
        {
            if (updateMode == UpdateMode.UnscaledTime)
            {
                return Time.unscaledTime;
            }
            else if (updateMode == UpdateMode.ScaledTime)
            {
                return Time.time;
            }

            return Time.time;
        }

    }
}