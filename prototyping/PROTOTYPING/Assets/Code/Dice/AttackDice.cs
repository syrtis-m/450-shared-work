using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDice : MonoBehaviour
{
    private Sprite[] diceSides;
    private SpriteRenderer rend;
    public int currentDiceSide;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("redDice/");
        currentDiceSide = 0;
        rend.sprite = diceSides[currentDiceSide];
    }

    public void RollDice()
    {
        // Start up animation
        StartCoroutine(RollDiceAnimation());
        // Generate random number
        int randomDiceSide = Random.Range(1, 6);
        // Save it as current number
        currentDiceSide = randomDiceSide;
        // Send number over to mind
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