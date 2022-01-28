using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public List<GameObject> Objects;
    public int Count;

    public Transform From;
    public Transform To;

    public KeyBlade Trigger;
    void Start()
    {
        Trigger.PickedUp += Spawn;
        Spawn();
    }

    void Spawn()
    {
        for (int i = 0; i < Count; i++)
        {
            Instantiate(RandomGameobject, RandomPosFromTo, RandomRotation);
        }
    }
    GameObject RandomGameobject => Objects[Random.Range(0, Objects.Count)];
    Vector3 RandomPosFromTo => new Vector3(Random.Range(From.position.x, To.position.x),
            Random.Range(From.position.y, To.position.y),
            Random.Range(From.position.z, To.position.z));
    Quaternion RandomRotation => Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
}
