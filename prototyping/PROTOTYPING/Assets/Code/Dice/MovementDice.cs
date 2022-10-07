using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDice : MonoBehaviour
{
    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private int currentDiceSide;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("blueDice/");
        currentDiceSide = 0;
        rend.sprite = diceSides[currentDiceSide];
    }

    public int RollDice()
    {
        // Start up animation
        StartCoroutine(RollDiceAnimation());
        // Generate random number
        int randomDiceSide = Random.Range(1, 6);
        // Save it as current number
        currentDiceSide = randomDiceSide;
        // Send number over to mind
        return randomDiceSide;
    }

    private IEnumerator RollDiceAnimation()
    {
        // Animation will finish whenever but on proper side
        int currentSideCopy = currentDiceSide;
        for (int i = 0; i < 25; i++)
        {
            int randomDiceSide = Random.Range(1, 6);
            currentSideCopy = (randomDiceSide + currentSideCopy) % 6;
            rend.sprite = diceSides[currentSideCopy];
            yield return new WaitForSeconds(i * 0.01f);
        }
        // Roll was already sent over to mind
        rend.sprite = diceSides[currentDiceSide - 1];
    }

}
