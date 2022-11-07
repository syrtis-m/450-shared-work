using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackDice : MonoBehaviour
{
    private Sprite[] _diceSides;
    private SpriteRenderer _rend;
    public int currentDiceSide;
    private IEnumerator _enumerator;

    void Awake()
    {
        _enumerator = RollDiceAnimation();
        _rend = GetComponent<SpriteRenderer>();
        _diceSides = Resources.LoadAll<Sprite>("redDice/");
        currentDiceSide = 1;
        _rend.sprite = _diceSides[currentDiceSide];
    }

    private void OnEnable()
    {
        Mind.RollDice += RollDice;
    }

    private void OnDisable()
    {
        Mind.RollDice -= RollDice;
    }
    
    

    private void RollDice()
    {
        StartCoroutine(_enumerator);
        Debug.Log("RollAtk");
        // Generate random number
        // Save it as current number
        currentDiceSide = Random.Range(1, 6);;
        // Start up animation
    }
    
    public IEnumerator RollDiceAnimation()
    {
        Debug.Log("atk_cortn");
        // Animation will finish whenever but on proper side
        int currentSideCopy = currentDiceSide;
        for (int i = 0; i < 25; i++)
        {
            int randomDiceSide = Random.Range(1, 6);
            currentSideCopy = (randomDiceSide + currentSideCopy) % 6;
            _rend.sprite = _diceSides[currentSideCopy];
            yield return new WaitForSeconds(i * 0.01f);
        }
        _rend.sprite = _diceSides[currentDiceSide - 1];
    }

}