using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDOperatorVector2 : PIDOperator<Vector2>
{
    public override Vector2 Zero() { return new Vector2(0, 0); }
    public override Vector2 Add(Vector2 a, Vector2 b) { return a + b; }
    public override Vector2 Multiply(float a, Vector2 b) { return a * b; }
    public override Vector2 Subtract(Vector2 a, Vector2 b) {return a - b;}
}

public class PIDOperatorVector3 : PIDOperator<Vector3>
{
    public override Vector3 Zero() { return new Vector3(0, 0, 0); }
    public override Vector3 Add(Vector3 a, Vector3 b) { return a + b; }
    public override Vector3 Multiply(float a, Vector3 b) { return a * b; }
    public override Vector3 Subtract(Vector3 a, Vector3 b) { return a - b; }
}
