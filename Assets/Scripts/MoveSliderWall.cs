using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSliderWall : MonoBehaviour
{
    public Transform Wall;
    public ActivateTrigger Pedestal;
    public Transform StartPos;
    public Transform EndPos;
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
            Wall.transform.position = Vector3.MoveTowards(Wall.transform.position, EndPos.position, Speed * Time.deltaTime);
        }
        else
        {
            Wall.transform.position = Vector3.MoveTowards(Wall.transform.position, StartPos.position, Speed * Time.deltaTime);
        }
    }
    private void Activated()
    {
        Open = !Open;
    }
}