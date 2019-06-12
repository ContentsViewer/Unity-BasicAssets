using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PIDOperatorFloat : PIDOperator<float>
{
    public override float Zero() { return 0.0f; }
    public override float Add(float a, float b) { return a + b; }
    public override float Multiply(float a, float b) { return a * b; }
    public override float Subtract(float a, float b) { return a - b; }
}

