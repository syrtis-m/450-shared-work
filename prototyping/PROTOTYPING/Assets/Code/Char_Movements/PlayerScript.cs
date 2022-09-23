using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerScript : MonoBehaviour
{
    public GameObject otherPlayer;
    
    //switch between which player's movement script is enabled at a time, enabling and disabling each other's movement script

    private void OnMouseDown()
    {
        //disable their movement script, enable ours.
        otherPlayer.GetComponent<PlayerCharMvmt>().enabled = false;
        GetComponent<PlayerCharMvmt>().enabled = true;
    }
}
