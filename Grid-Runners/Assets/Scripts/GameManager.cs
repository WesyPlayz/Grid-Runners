using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float RoundTimer = 90f;



    // Start is called before the first frame update
    void Start()
    {
        
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
