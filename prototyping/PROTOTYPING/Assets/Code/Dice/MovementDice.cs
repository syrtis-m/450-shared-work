using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDice : MonoBehaviour
{
    private Sprite[] _diceSides;
    private SpriteRenderer _rend;
    public int currentDiceSide;
    private IEnumerator _enumerator;

    void Awake()
    {
        _enumerator = RollDiceAnimation();
        _rend = GetComponent<SpriteRenderer>();
        _diceSides = Resources.LoadAll<Sprite>("blueDice/");
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
    
    public IEnumerator RollDiceAnimation()
    {
        Debug.Log("mvmt_cortn");
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
        Debug.Log("RollMVMT");
        // Generate random number
        // Save it as current number
        currentDiceSide = Random.Range(1, 6);
        // Start up animation
        StartCoroutine(_enumerator);
    }

    

}
