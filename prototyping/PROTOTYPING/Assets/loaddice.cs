using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loaddice : MonoBehaviour
{
    private void OnMouseDown()
    {
        String active_name = SceneManager.GetActiveScene().name;
        Debug.Log(active_name);

        if ("MAIN" != active_name)
        {
            SceneManager.LoadScene("Scenes/MAIN");
        }
        else
        {
            SceneManager.LoadScene("Scenes/Char_Movements");
        }
        
    }
}
