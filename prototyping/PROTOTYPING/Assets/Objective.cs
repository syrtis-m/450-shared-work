using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    public static Objective instance;

    private Collider2D _collider;

    private void Awake()
    {
        instance = this;
        _collider = GetComponent<BoxCollider2D>();
    }

    public bool CheckObjectiveWin()
    {
        foreach (var character in Mind.instance.playerCharacters)
        {
            var point = character.transform.position;
            if (_collider.bounds.Contains(point))
            {
                return true;
            }
        }

        return false;
    }
}
