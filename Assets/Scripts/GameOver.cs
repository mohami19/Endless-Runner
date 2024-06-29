using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using TMPro;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI leaderBoardScoreText;
    [SerializeField] TextMeshProUGUI leaderBoardNameText;

    private int score = 0;
    private string leaderBoardID = "22337";
    private int leaderBoardTopCount = 10;
    public void StopGame(int score){
        this.score = score;
        scoreText.text = score.ToString();
        GetLeaderBoard();
        AddXP(score);
    }
    
    public void SubmitScore(){
        StartCoroutine(SubmitScoreToLeaderBoard(score));
    }
    
    private IEnumerator SubmitScoreToLeaderBoard(int score){
        bool? nameSet = null;
        LootLockerSDKManager.SetPlayerName(inputField.text, (response) => {
            if (response.success) {
                Debug.Log("Successfully Set The Player name.");
                nameSet = true;
            } else {
                Debug.Log("Name Not Set");
                nameSet = false;
            }
        });
        yield return new WaitUntil(() => nameSet.HasValue);
        //if (!nameSet.Value) yield break;

        bool? scoreSubmitted = null;
        LootLockerSDKManager.SubmitScore("",score,leaderBoardID, (response) => {
            if (response.success) {
                Debug.Log("Successfully Set The Score");
                scoreSubmitted = true;
            } else {
                Debug.Log("Could not Set The Score");
                scoreSubmitted = false;
            }
        });
        yield return new WaitUntil(() => scoreSubmitted.HasValue);
        if (!scoreSubmitted.Value) yield break;
        GetLeaderBoard();
    }

    private void GetLeaderBoard() {
        LootLockerSDKManager.GetScoreList(leaderBoardID,leaderBoardTopCount, (response) => {
            if (response.success) {
                Debug.Log("Successfully retrieved The Scores");
                string leaderBoardName= "";
                string leaderBoardScore= "";
                LootLockerLeaderboardMember [] members = response.items;

                for (int i = 0; i < members.Length; i++) {
                    LootLockerPlayer player = members[i].player;
                    if (player == null) continue;
                    if (player.name != "") {
                        leaderBoardName += player.name +"\n";
                    } else {
                        leaderBoardName += player.id + "\n";
                    }
                    leaderBoardScore += members[i].score + "\n";
                }
                leaderBoardNameText.SetText(leaderBoardName);
                leaderBoardScoreText.SetText(leaderBoardScore);
            } else {
                Debug.Log("Failed to get scores from leaderBoard");
            }
        });
    }


    public void AddXP(int score){
        LootLockerSDKManager.SubmitXp(score, (response)=>{
            if (response.success){
                Debug.Log("Successfully Added XP");
            } else {
                Debug.Log("Failed To Add XP");
            }
        });
    }

    public void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}
