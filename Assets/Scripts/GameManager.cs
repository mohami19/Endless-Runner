using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    [SerializeField] private UnityEvent playerConnected;
    private IEnumerator Start() {
        bool connected = false;
        LootLockerSDKManager.StartGuestSession((response => {
            Debug.Log(response.text);
            if (!response.success) {
                Debug.Log("Error Starting LootLocker Session");
                return;
            }
            Debug.Log("Successfully Create LootLocker Session");
            connected = true;
        }));
        yield return new WaitUntil(()=> connected);
        playerConnected.Invoke();
    }
}
