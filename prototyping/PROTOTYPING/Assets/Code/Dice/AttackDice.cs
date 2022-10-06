using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AttackDice : MonoBehaviour
{
    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private int currentDiceSide;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("redDice/");
        currentDiceSide = 0;
        rend.sprite = diceSides[currentDiceSide];
    }

    public async Task<int> RollDice()
    {
        for (int i = 0; i < 25; i++)
        {
            int randomDiceSide = Random.Range(1, 6);
            currentDiceSide = (randomDiceSide + currentDiceSide) % 6;
            rend.sprite = diceSides[currentDiceSide];
            await Task.Delay(i * 10);
        }
        return currentDiceSide + 1;
    }

}
