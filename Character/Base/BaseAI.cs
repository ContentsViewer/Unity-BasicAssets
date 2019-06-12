/*
 * AI
 * 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseCharacterController))]
public class BaseAI : MonoBehaviour
{
    public int debugRandomState = -1;
    

    protected StateMachine stateMachine;
    protected BaseCharacterController characterController;

    protected virtual void Awake()
    {
        stateMachine = GetComponent<StateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<StateMachine>();
        }

        characterController = GetComponent<BaseCharacterController>();

        stateMachine.OnStateBegin = OnStateBegin;
        stateMachine.OnStateEnd = OnStateEnd;
        
    }
    

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void OnDisable()
    {
        stateMachine.enabled = false;
    }

    protected virtual void OnEnable()
    {
        stateMachine.enabled = true;
    }

    protected virtual bool OnStateBegin(StateMachine stateMachine)
    {
        return true;
    }

    protected virtual void OnStateEnd(StateMachine stateMachine)
    {
    }

    protected virtual int SelectRandomState(params int[] stateRates)
    {
#if UNITY_EDITOR
        if (debugRandomState != -1)
        {
            return debugRandomState;
        }
#endif
        var n = Random.Range(0, 101);
        var sum = 0;
        for(int i = 0; i < stateRates.Length; i++)
        {
            sum += stateRates[i];
            if(n < sum)
            {
                return i;
            }
        }

        return stateRates.Length - 1;
    }
}
