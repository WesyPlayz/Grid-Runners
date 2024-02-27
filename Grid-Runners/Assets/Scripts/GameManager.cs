using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float RoundTimer;
    public int players;


    // Start is called before the first frame update
    void Start()
    {
        RoundTimer = 45f * players;
    }

    // Update is called once per frame
    void Update()
    {
        if (RoundTimer > 0)
            RoundTimer -= Time.deltaTime;
        else
            EndRound();
    }

    void EndRound()
    {

    }
}
