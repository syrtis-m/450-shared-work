using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mind : MonoBehaviour
{
    public GameObject[] characters;

    public GameObject current;
    
    void Start()
    {
        current = characters[0];
    }

    public void ChangePlayer(GameObject newCharacter)
    {
        current.GetComponent<PlayerCharMvmt>().enabled = false;
        current = newCharacter;
    }
}