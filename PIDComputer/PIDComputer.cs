using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PIDOperator<T>
{
    abstract public T Zero();
    abstract public T Add(T a, T b);
    abstract public T Subtract(T a, T b);
    abstract public T Multiply(float a, T b);
}

public class PIDComputer<T>
{
    public float kp;
    public float ki;
    public float kd;

    T[] diff = new T[2];
    T integral;

    PIDOperator<T> pidOperator;
    

    public PIDComputer(float kp, float ki, float kd, PIDOperator<T> pidOperator)
    {
        this.pidOperator = pidOperator;

        diff[0] = pidOperator.Zero();
        diff[1] = pidOperator.Zero();
        integral = pidOperator.Zero();

        this.kp = kp;
        this.ki = ki;
        this.kd = kd;
    }

    public T Compute(T current, T target, float deltaT)
    {
        

        diff[0] = diff[1];
        diff[1] = pidOperator.Subtract(current, target);
        integral = pidOperator.Add(integral, pidOperator.Multiply(deltaT / 2.0f, pidOperator.Add(diff[0], diff[1])));

        var p = pidOperator.Multiply(-kp, diff[1]);
        var i = pidOperator.Multiply(-ki, integral);
        var d = pidOperator.Multiply(-kd, pidOperator.Multiply(1 / deltaT, pidOperator.Subtract(diff[1], diff[0])));

        return pidOperator.Add(pidOperator.Add(p, i), d);
    }
}
