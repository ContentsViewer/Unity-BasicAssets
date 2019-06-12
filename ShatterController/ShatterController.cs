using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShatterController : MonoBehaviour
{
    public float fragmentLifeTime = 5.0f;
    public int forceMin = -3;
    public int forceMax = 3;

    public void Shatter()
    {
        var random = new System.Random();

        gameObject.GetComponentsInChildren<Rigidbody>().ToList().ForEach(r =>
        {
            r.isKinematic = false;
            r.transform.SetParent(null);
            r.gameObject.SetActive(true);
            r.gameObject.AddComponent<AutoDestroy>().time = fragmentLifeTime;

            var vect = new Vector3(random.Next(forceMin, forceMax), random.Next(0, forceMax), random.Next(forceMin, forceMax));
            r.AddForce(vect, ForceMode.Impulse);
            r.AddTorque(vect, ForceMode.Impulse);
        });

        Destroy(gameObject);
    }
}
