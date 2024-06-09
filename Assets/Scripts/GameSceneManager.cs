using UnityEngine.SceneManagement;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
}
