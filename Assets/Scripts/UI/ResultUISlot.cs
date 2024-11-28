using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace New_Neo_LT.Scripts.UI
{
    public class ResultUISlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text upScoreText;
        [SerializeField] private TMP_Text downScoreText;
        [SerializeField] private RawImage playerRenderImage;
        
        public void SetSlot(string pName, int upScore, int downScore, RenderTexture playerRender)
        {
            nameText.text = pName;
            upScoreText.text = upScore.ToString();
            downScoreText.text = downScore.ToString();
            playerRenderImage.texture = playerRender;
        }
        
        public (float, float) GetRawImageSize()
        {
            return (playerRenderImage.rectTransform.rect.width, playerRenderImage.rectTransform.rect.height);
        }
    }
}
