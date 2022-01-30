using System.Collections;
using System.Collections.Generic;
using Cutscene;
using UnityEngine;

public class MoveSliderWall : MonoBehaviour
{
    public Transform Wall;
    public ActivateTrigger Pedestal;
    public Transform StartPos;
    public Transform EndPos;
    public float Speed;
    public bool Open = false;
    public int NeedsToOpen;
    public string CutseneKey;
    private AudioSource AudioS;

    private void Start()
    {
        Pedestal.Activated += Activated;
        AudioS = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (BladePedestal.ActivatedCount >= NeedsToOpen && !Open)
        {
            Open = true;
            CutsceneManager.Instance.Show(CutseneKey);
        }
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
        Open = false;
        AudioS.Play();
    }
}
