using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTest : MonoBehaviour
{
    public int numCards;
    public List<int> cardNums;
    public int generations;
    // Start is called before the first frame update
    void Awake()
    {
        cardNums = new List<int>(numCards);
        for (int i = 0; i < numCards; i++)
        {
            // Human-friendly 1-based numbers, the value is symbolic anyways, won't be used later in the programming.
            cardNums[i] = i + 1;
        }

        for (int i = 0; i < generations; i++)
        {

        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
