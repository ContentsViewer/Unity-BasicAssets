/*
 * キャラクタの身体コントロール.
 * 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct DamageHit
{
    public float damageAmount;
    public string attackName;
    public GameObject from;
    public float overrideDamageAmount;

    public DamageHit(float damageAmount, string attackName = "", GameObject from = null)
    {
        this.damageAmount = damageAmount;
        this.attackName = attackName;
        this.from = from;
        this.overrideDamageAmount = damageAmount;
    }
}


[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class BaseCharacterController : MonoBehaviour
{
    public Vector3 velocityMin = new Vector3(-100.0f, -100.0f, -100.0f);
    public Vector3 velocityMax = new Vector3(100.0f, 100.0f, 100.0f);

    public bool isActive = true;

    public float hpMax = 10.0f;
    public float initHp = 10.0f;
    

    public float Hp
    {
        get { return hp; }
        set
        {
            hp = Mathf.Clamp(value, 0.0f, hpMax);

            if(hp <= 0.0f && isActive)
            {
                deadFlagInternal = true;
                isActive = false;
                OnDead();
            }
        }
    }

    [System.NonSerialized]
    public Animator animator;

    [System.NonSerialized]
    public Rigidbody rigidbody;

    public bool DamageFlag { get; protected set; }
    public DamageHit DamageHit { get; protected set; }

    public bool DeadFlag { get; protected set; }

    protected bool deadFlagInternal;
    protected bool damageFlagInternal;

    protected float hp;

    protected virtual void UpdateCharacter() { }
    protected virtual void OnDead() { }
    protected virtual float OnDamage(DamageHit hit) { return hit.damageAmount;}

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        

        Hp = initHp;
    }

    protected virtual void Start()
    {

    }

    protected virtual void LateUpdate()
    {
        UpdateFlags();
    }

    protected virtual void OnDisable()
    {
        UpdateFlags();
    }

    void UpdateFlags()
    {
        if (deadFlagInternal)
        {
            DeadFlag = true;
            deadFlagInternal = false;
        }
        else
            DeadFlag = false;

        if (damageFlagInternal)
        {
            DamageFlag = true;
            damageFlagInternal = false;
        }
        else
            DamageFlag = false;
    }

    protected virtual void Update()
    {
        if (!isActive)
            return;



        UpdateCharacter();

        rigidbody.velocity = new Vector3(Mathf.Clamp(rigidbody.velocity.x, velocityMin.x, velocityMax.x),
                                           Mathf.Clamp(rigidbody.velocity.y, velocityMin.y, velocityMax.y),
                                           Mathf.Clamp(rigidbody.velocity.z, velocityMin.z, velocityMax.z));


    }
    
    
    public virtual void ReceiveDamage(DamageHit hit)
    {
        if (!isActive)
            return;

        damageFlagInternal = true;
        hit.overrideDamageAmount = OnDamage(hit);

        DamageHit = hit;

        Hp -= hit.overrideDamageAmount;

    }
    


}
