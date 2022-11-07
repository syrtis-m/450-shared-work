using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDice : MonoBehaviour
{
    public Sprite[] _diceSides;
    private SpriteRenderer _rend;
    public int currentDiceSide;

    void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        _diceSides = Resources.LoadAll<Sprite>("blueDice/");
        currentDiceSide = 1;
        _rend.sprite = _diceSides[currentDiceSide];
    }
    
    private void OnEnable()
    {
        Mind.BeginPlayerTurnEvent += RollDice;
    }

    private void OnDisable()
    {
        Mind.BeginPlayerTurnEvent -= RollDice;
    }
    
    public IEnumerator RollDiceAnimation()
    {
        // Animation will finish whenever but on proper side
        var currentSideCopy = currentDiceSide;
        for (var i = 0; i < 25; i++)
        {
            var randomDiceSide = Random.Range(1, 6);
            currentSideCopy = (randomDiceSide + currentSideCopy) % 6;
            _rend.sprite = _diceSides[currentSideCopy];
            yield return new WaitForSeconds(i * 0.01f);
        }
        _rend.sprite = _diceSides[currentDiceSide - 1];
    }

    private void RollDice()
    {//RollDice is triggered through Mind.RollDice event
        // Generate random number
        // Save it as current number
        currentDiceSide = Random.Range(1, 6);
        Debug.Log("mvmt:" + currentDiceSide);
        // Start up animation
        StartCoroutine(RollDiceAnimation());
    }

    

}
