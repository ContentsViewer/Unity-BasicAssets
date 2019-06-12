using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using System.Text;



namespace HUDMessageManagement
{

    [RequireComponent(typeof(Text))]
    public class SimpleHUDMessageArea : BaseHUDMessageArea
    {


        [Space(10)]
        public MessageEntranceAnimation entranceAnimation = MessageEntranceAnimation.Fade;
        public MessageExitAnimation exitAnimation = MessageExitAnimation.Fade;
        public MessageMode messageMode = MessageMode.Timer;
        public UpdateMode updateMode = UpdateMode.ScaledTime;

        [Space(10)]
        public float entranceTime = 1.0f;
        public float displayTime = 5.0f;
        public float exitTime = 1.0f;



        public enum MessageEntranceAnimation
        {
            Appear,
            Fade
        }

        public enum MessageExitAnimation
        {
            Disappear,
            Fade
        }

        public enum MessageMode
        {
            Normal,
            Timer
        }

        public enum UpdateMode
        {
            ScaledTime,
            UnscaledTime
        }

        enum MessageState
        {
            Ready,
            Start,
            Showing,
            ToEnd,
            End
        }

        class ControlBlock
        {
            public MessageState state = MessageState.Ready;

            public float transparency = 1.0f;
            public float messageStartTime;

            public LinkedListNode<Message> playingMessageNode;
        }

        LinkedList<Message> playingMessages = new LinkedList<Message>();
        Dictionary<Message, ControlBlock> controlBlockMap = new Dictionary<Message, ControlBlock>();
        Text text;

        protected override void Awake()
        {
            base.Awake();

            text = GetComponent<Text>();
        }


        protected override void Update()
        {
            base.Update();


            var messagesToRemove = new List<Message>();
            foreach (var message in playingMessages)
            {
                var controlBlock = controlBlockMap[message];

                switch (controlBlock.state)
                {
                    case MessageState.Start:

                        switch (entranceAnimation)
                        {
                            case MessageEntranceAnimation.Appear:

                                controlBlock.transparency = 1.0f;
                                controlBlock.state = MessageState.Showing;
                                break;

                            case MessageEntranceAnimation.Fade:
                                controlBlock.transparency += entranceTime * GetDeltaTime();
                                controlBlock.transparency = Mathf.Clamp01(controlBlock.transparency);

                                if (controlBlock.transparency >= 1.0f)
                                {
                                    controlBlock.state = MessageState.Showing;
                                }
                                break;

                        }

                        break;

                    case MessageState.Showing:

                        switch (messageMode)
                        {
                            case MessageMode.Normal:

                                break;


                            case MessageMode.Timer:

                                if (GetTime() - controlBlock.messageStartTime > displayTime)
                                {
                                    ExitMessage(message);
                                }

                                break;
                        }

                        break;

                    case MessageState.ToEnd:

                        switch (exitAnimation)
                        {
                            case MessageExitAnimation.Disappear:
                                controlBlock.transparency = 0.0f;
                                controlBlock.state = MessageState.End;
                                break;

                            case MessageExitAnimation.Fade:
                                controlBlock.transparency -= exitTime * GetDeltaTime();
                                controlBlock.transparency = Mathf.Clamp01(controlBlock.transparency);

                                if (controlBlock.transparency <= 0.0f)
                                {
                                    controlBlock.state = MessageState.End;
                                }

                                break;
                        }

                        break; // End MessageState.ToEnd

                        
                    case MessageState.End:
                        messagesToRemove.Add(message);
                        controlBlock.state = MessageState.Ready;
                        break;

                }
            }


            // 削除リストにあるMessageを削除
            foreach (var messageToRemove in messagesToRemove)
            {
                RemoveMessage(messageToRemove);
            }


            text.text = "";
            var textColor = text.color;
            var stringBuilder = new StringBuilder();

            foreach (var message in playingMessages)
            {
                var controlBlock = controlBlockMap[message];

                int transparencyInt = (int)(controlBlock.transparency * 255.0f);
                textColor.a = controlBlock.transparency;
                stringBuilder.Append("<color=#");
                stringBuilder.Append(ColorUtility.ToHtmlStringRGBA(textColor));
                stringBuilder.Append(">");
                stringBuilder.Append(message.text.Replace("{a}", transparencyInt.ToString("X2")));
                stringBuilder.Append("</color>");
                stringBuilder.Append("\n");

            }

            text.text = stringBuilder.ToString();

        }

        public override void SetMessage(Message messageToSet)
        {
            var controlBlock = new ControlBlock();
            controlBlockMap.Add(messageToSet, controlBlock);


            controlBlock.playingMessageNode = null;
            controlBlock.state = MessageState.Ready;
            controlBlock.messageStartTime = 0.0f;
            controlBlock.transparency = 0.0f;
        }

        public override void ShowMessage(Message messageToShow, bool dontOverride = false)
        {
            var controlBlock = controlBlockMap[messageToShow];

            if (controlBlock.playingMessageNode != null)
            {
                return;
            }


            controlBlock.playingMessageNode = playingMessages.AddLast(messageToShow);
            controlBlock.messageStartTime = GetTime();
            controlBlock.state = MessageState.Start;
            controlBlock.transparency = 0.0f;

        }

        public override void ExitMessage(Message messageToExit)
        {
            if (controlBlockMap.ContainsKey(messageToExit))
            {
                var controlBlock = controlBlockMap[messageToExit];

                if (controlBlock.playingMessageNode != null)
                {
                    controlBlock.state = MessageState.ToEnd;
                }
            }
        }

        public override void RemoveMessage(Message messageToRemove)
        {
            var controlBlock = controlBlockMap[messageToRemove];
            playingMessages.Remove(controlBlock.playingMessageNode);

            controlBlockMap.Remove(messageToRemove);

            HUDMessageManager.Instance.UnlinkMessage(messageToRemove);
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