using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LootLocker.Requests;

public class PlayerSkinItem : MonoBehaviour ,IPointerClickHandler
{
    private string colorName;
    public void OnPointerClick(PointerEventData eventData)
    {
        LootLockerSDKManager.UpdateOrCreateKeyValue("skin",colorName, (response) =>{
            if (response.success) {
                Debug.Log("Set PLayer Skin color key Successfully");
            } else {
                Debug.Log("Set PLayer Skin color key Unsuccessfully");
            }
        });
    }

    public void setColor(string color){
        if (ColorUtility.TryParseHtmlString(color,out Color newColor)) {
            GetComponent<Image>().color = newColor;
        }
        colorName = color;
   }

   
}
