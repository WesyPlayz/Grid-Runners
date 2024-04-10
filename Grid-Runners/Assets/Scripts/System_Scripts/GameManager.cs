using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float RoundTimer;
    public int players;
    public int players_Alive;
    public float map_size;


    

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

    

    //main menu options
    


    public void quit()
    {
        Application.Quit();
    }
}
