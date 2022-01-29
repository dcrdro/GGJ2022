using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    public Transform Wall;
    public Transform ToPosition;
    public float Speed;
    public int NeedsToOpen;
    public bool Open = false;
    private AudioSource AudioS;

    void Start()
    {
        AudioS = GetComponent<AudioSource> ();
    }

    void Update()
    {
        if (BladePedestal.ActivatedCount >= NeedsToOpen && !Open)
        {
            Activated();
        }
        if (Open)
        {
            Wall.transform.position = Vector3.MoveTowards(Wall.transform.position, ToPosition.position, Speed * Time.deltaTime);
        }
    }
    private void Activated()
    {
        Open = true;
        AudioS.Play();
    }

   
}
