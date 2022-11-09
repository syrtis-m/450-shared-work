using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovementDice : MonoBehaviour
{
    public Sprite[] diceSides = new Sprite[6];
    private SpriteRenderer _rend;
    private UIManager _uiManager;
    public int currentDiceSide;

    void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        _uiManager = GetComponentInParent<UIManager>();
        //diceSides = Resources.LoadAll<Sprite>("blueDice/");
        currentDiceSide = 1;
        
    }

    private void Start()
    {
        diceSides = _uiManager.movementDiceSides;
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
    
    private IEnumerator RollDiceAnimation()
    {
        // Animation will finish whenever but on proper side
        var currentSideCopy = currentDiceSide;
        for (var i = 0; i < 25; i++)
        {
            var randomDiceSide = Random.Range(1, 6);
            currentSideCopy = (randomDiceSide + currentSideCopy) % 6;
            _rend.sprite = diceSides[currentSideCopy];
            yield return new WaitForSeconds(i * 0.01f);
        }
        _rend.sprite = diceSides[currentDiceSide - 1];
    }

    private void RollDice()
    {//RollDice is triggered through Mind.RollDice event
        // Generate random number
        // Save it as current number
        currentDiceSide = Random.Range(1, 7);
        Debug.Log("mvmt:" + currentDiceSide);
        // Start up animation
        StartCoroutine(RollDiceAnimation());
    }

    

}
