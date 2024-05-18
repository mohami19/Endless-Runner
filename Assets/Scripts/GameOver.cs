using System.Collections;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;

    public void StopGame(int score){
        gameOverCanvas.SetActive(true);
    }
    
    public void RestartLevel(int score){

    }
    
    public void SubmitScore(int score){

    }
    
    public void AddXP(int score){

    }
    
    
}
