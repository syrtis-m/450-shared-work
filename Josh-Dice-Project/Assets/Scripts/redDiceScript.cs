using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redDiceScript : MonoBehaviour
{

    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private int currentDiceSide;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("redDice/");
        currentDiceSide = 5;
        rend.sprite = diceSides[currentDiceSide];
    }

    private void OnMouseDown()
    {
        StartCoroutine("RollDice");
    }

    private IEnumerator RollDice()
    {
        for (int i = 0; i < 25; i++)
        {
            int randomDiceSide = Random.Range(1, 6);
            currentDiceSide = (randomDiceSide + currentDiceSide) % 6;
            rend.sprite = diceSides[currentDiceSide];
            yield return new WaitForSeconds(i * 0.01f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

