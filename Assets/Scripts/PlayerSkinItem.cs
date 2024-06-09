using UnityEngine;
using UnityEngine.UI;

public class PlayerSkinItem : MonoBehaviour
{
   public void setColor(string color){
        if (ColorUtility.TryParseHtmlString(color,out Color newColor)) {
            GetComponent<Image>().color = newColor;
        }
   }
}
