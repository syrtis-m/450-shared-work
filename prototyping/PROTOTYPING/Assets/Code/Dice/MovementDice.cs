using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDice : MonoBehaviour
{
    private Sprite[] _diceSides;
    private SpriteRenderer _rend;
    private int _currentDiceSide;

    void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
        _diceSides = Resources.LoadAll<Sprite>("blueDice/");
        _currentDiceSide = 0;
        _rend.sprite = _diceSides[_currentDiceSide];
    }

    public int RollDice()
    {
        // Generate random number
        var randomDiceSide = Random.Range(1, 6);
        // Save it as current number
        _currentDiceSide = randomDiceSide;
        // Start up animation
        StartCoroutine(RollDiceAnimation());
        // Send number over to mind
        return randomDiceSide;
    }

    private IEnumerator RollDiceAnimation()
    {
        // Animation will finish whenever but on proper side
        var currentSideCopy = _currentDiceSide;
        for (var i = 0; i < 25; i++)
        {
            var randomDiceSide = Random.Range(1, 6);
            currentSideCopy = (randomDiceSide + currentSideCopy) % 6;
            _rend.sprite = _diceSides[currentSideCopy];
            yield return new WaitForSeconds(i * 0.01f);
        }
        // Roll was already sent over to mind
        _rend.sprite = _diceSides[_currentDiceSide - 1];
    }

}
