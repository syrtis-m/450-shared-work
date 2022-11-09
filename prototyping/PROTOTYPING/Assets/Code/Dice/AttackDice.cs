using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackDice : MonoBehaviour
{
    public Sprite[] diceSides = new Sprite[6];
    private SpriteRenderer _rend;
    private UIManager _uiManager;
    public int currentDiceSide;

    void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        _uiManager = GetComponentInParent<UIManager>();
        //diceSides = Resources.LoadAll<Sprite>("redDice/");
        currentDiceSide = 1;
    }

    private void Start()
    {
        diceSides = _uiManager.attackDiceSides;
        _rend.sprite = diceSides[currentDiceSide];
    }

    private void OnEnable()
    {
        Mind.BeginPlayerTurnEvent += RollDice;
    }

    private void OnDisable()
    {
        Mind.BeginPlayerTurnEvent -= RollDice;
    }
    
    

    private void RollDice()
    {
        // Generate random number
        // Save it as current number
        currentDiceSide = Random.Range(1, 7);;
        Debug.Log("atk:" + currentDiceSide);
        // Start up animation
        StartCoroutine(RollDiceAnimation());
    }

    private IEnumerator RollDiceAnimation()
    {
        // Animation will finish whenever but on proper side
        int currentSideCopy = currentDiceSide;
        for (int i = 0; i < 25; i++)
        {
            int randomDiceSide = Random.Range(1, 6);
            currentSideCopy = (randomDiceSide + currentSideCopy) % 6;
            _rend.sprite = diceSides[currentSideCopy];
            yield return new WaitForSeconds(i * 0.01f);
        }
        _rend.sprite = diceSides[currentDiceSide - 1];
    }

}