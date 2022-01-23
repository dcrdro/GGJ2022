using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    public Transform Wall;
    public ActivateTrigger Pedestal;
    public Transform ToPosition;
    public float Speed;
    public bool Open = false;

    private void Start()
    {
        Pedestal.Activated += Activated;
    }
    void Update()
    {
        if (Open)
        {
            Wall.transform.position = Vector3.MoveTowards(Wall.transform.position, ToPosition.position, Speed * Time.deltaTime);
        }
    }
    private void Activated()
    {
        Open = true;
    }
}
