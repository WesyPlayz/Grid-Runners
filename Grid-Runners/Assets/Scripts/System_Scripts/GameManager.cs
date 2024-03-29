using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float RoundTimer;
    public int players;
    public int players_Alive;
    public float map_size;
    public GameObject[] weapons;
    public GameObject[] basic_Weapons;
    public GameObject[] water_Weapons;
    public GameObject[] medieval_Weapons;

    // Start is called before the first frame update
    void Start()
    {
        RoundTimer = 45f * players * (map_size);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RoundTimer > 0)
            RoundTimer -= Time.deltaTime;
        else
            EndRound();
    }

    public void EndRound()
    {

    }
}
