using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject PauseMenu;
    private bool Active;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
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
}
