using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSound : MonoBehaviour
{
    private AudioSource source;

    public AudioClip[] StepSound;
    int stepnumber;

    public AudioClip[] SprintSound;
    int clipnumber;




    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    void Step()
    {
        stepnumber = Random.Range(0, StepSound.Length);
        source.PlayOneShot(StepSound[stepnumber]);

    }
    
    public void Sprint()
    {
        clipnumber = Random.Range(0, SprintSound.Length);
        source.PlayOneShot(SprintSound[clipnumber]);
    }


}
