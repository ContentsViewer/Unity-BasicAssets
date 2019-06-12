using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class StateMachine : MonoBehaviour
{

    public class State
    {
        public string Name { get; private set; }
        public Action<State> OnState { get; private set; }
        public StateMachine stateMachine;
        public State(string name, Action<State> onStateCallback)
        {
            Name = name;
            OnState = onStateCallback;
        }
    }

    public class StateLink
    {
        public StateLink(string stateName, float timeLength = float.PositiveInfinity, float delayTime = 0.0f)
        {
            this.stateName = stateName;
            this.timeLength = timeLength;
            this.delayTime = delayTime;
        }

        public string stateName;
        public float timeLength;
        public float delayTime;
    }

    public LinkedList<StateLink> StateLinkSeries { get; private set; }
    public State IdleState { get; private set; }
    public State CurrentState { get; private set; }
    public bool IsPlaying { get; private set; }

    public Func<StateMachine, bool> OnStateBegin { get; set; }
    public Action<StateMachine> OnStateEnd { get; set; }

    public float StateTimeLength { get; private set; }
    public float StateStartTime { get; private set; }
    public bool IsStateChange { get; private set; }

    State previousState;
    Dictionary<string, State> stateMap = new Dictionary<string, State>();

    bool ignoreSeriesRunningCheck;

    public StateMachine()
    {
        StateLinkSeries = new LinkedList<StateLink>();
    }

    public void AddState(State stateToAdd, bool isIdleState = false)
    {
        if (stateMap.ContainsKey(stateToAdd.Name))
            return;

        stateMap.Add(stateToAdd.Name, stateToAdd);
        stateToAdd.stateMachine = this;

        if (isIdleState)
        {
            IdleState = stateToAdd;
        }
    }


    public void RemoveState(string stateNameToRemove)
    {
        stateMap.Remove(stateNameToRemove);
    }


    void Goto(State state, float timeLength, float delayTime = 0.0f)
    {
        if (state == null)
        {
            return;
        }


        StateStartTime = Time.time + delayTime;
        StateTimeLength = timeLength;

        CurrentState = state;

    }

    public void Goto(string stateName, float timeLength = float.PositiveInfinity, float delayTime = 0.0f)
    {
        if (!stateMap.ContainsKey(stateName))
        {
            Debug.LogWarning(string.Format("{0}.StateMachine >> stateName '{1}' is not found.", gameObject.name, stateName));
            return;
        }

        if (!ignoreSeriesRunningCheck && StateLinkSeries.Count > 0)
        {
            Debug.LogWarning(string.Format("{0}.StateMachine >> stateName '{1}' is accepted," +
                " but StateLinkSerires(nextStateName'{2}') is already running. This serires will be stoped and clear.",
                gameObject.name, stateName, StateLinkSeries.First.Value.stateName));

            StateLinkSeries.Clear();
        }


        Goto(stateMap[stateName], timeLength, delayTime);
    }

    public void Goto(params StateLink[] serires)
    {
        if (serires.Length <= 0)
            return;


        if (!ignoreSeriesRunningCheck && StateLinkSeries.Count > 0)
        {
            Debug.LogWarning(string.Format("{0}.StateMachine >> New StateLinkSerires(firstStateName'{1}') is accepted," +
                " but Current StateLinkSerires(nextStateName'{2}') is already running. This serires will be stoped and clear.",
                gameObject.name, serires[0].stateName, StateLinkSeries.First.Value.stateName));

            StateLinkSeries.Clear();
        }

        foreach (var stateLink in serires)
        {
            StateLinkSeries.AddLast(stateLink);
        }


        ignoreSeriesRunningCheck = true;
        {
            Goto(StateLinkSeries.First.Value.stateName, StateLinkSeries.First.Value.timeLength, StateLinkSeries.First.Value.delayTime);
        }
        ignoreSeriesRunningCheck = false;

        StateLinkSeries.RemoveFirst();

    }



    public void Play()
    {
        Stop();

        IsPlaying = true;

        Goto(IdleState, 0.0f);
    }

    public void PlayDontOverride()
    {
        if (IsPlaying)
        {
            return;
        }

        Play();
    }

    public void Stop()
    {
        if (!IsPlaying)
        {
            return;
        }


        IsPlaying = false;
    }

    void Awake()
    {
    }


    void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        if (OnStateBegin == null || OnStateBegin(this))
        {
            if (CurrentState != null && CurrentState.OnState != null &&
                StateStartTime <= Time.time)
            {
                // Note:
                //  OnState()でCurrentStateの更新がある可能性があるので,
                //  previousStateの更新は, その前にやる.
                IsStateChange = (previousState != CurrentState);
                previousState = CurrentState;
                
                CurrentState.OnState(CurrentState);
            }

            if (StateStartTime + StateTimeLength < Time.time)
            {
                if (StateLinkSeries.Count > 0)
                {
                    ignoreSeriesRunningCheck = true;
                    {
                        Goto(StateLinkSeries.First.Value.stateName, StateLinkSeries.First.Value.timeLength, StateLinkSeries.First.Value.delayTime);
                    }
                    ignoreSeriesRunningCheck = false;
                        

                    StateLinkSeries.RemoveFirst();
                }
                else
                    Goto(IdleState, 0.0f);
            }

            OnStateEnd?.Invoke(this);
        }

    }
}
