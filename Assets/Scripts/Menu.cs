using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject WinMenu;
    public ActivateTrigger Trigger;
    private bool Active;
    private bool Win = false;

    private void Start()
    {
        Trigger.Activated += YouWin;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Win)
            {
                if (!Active)
                {
                    PauseMenu.SetActive(true);
                    Active = true;
                    Time.timeScale = 0;
                }
                else
                {
                    Continue();
                }
            }
            else
            {
                Quit();
            }
        }
    }
    public void YouWin()
    {
        WinMenu.SetActive(true);
        Win = true;
        Time.timeScale = 0;
    }
    public void Continue()
    {
        PauseMenu.SetActive(false);
        Active = false;
        Time.timeScale = 1;
    }
    public void Quit()
    {
        Application.Quit(0);
        Debug.Log("Quiting");
    }
    
    //public 
}
