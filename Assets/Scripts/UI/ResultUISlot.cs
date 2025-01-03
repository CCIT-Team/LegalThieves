using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace New_Neo_LT.Scripts.UI
{
    public class ResultUISlot : MonoBehaviour
    {
        [Header("Slot References")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text upScoreText;
        [SerializeField] private TMP_Text downScoreText;
        [SerializeField] private RawImage playerRenderImage;
        [Space]
        [SerializeField] private Image upPointImage;
        [SerializeField] private Image downPointImage;
        
        private bool _isRenown;
        private bool IsRenown
        {
            get => _isRenown;
            set
            {
                if(_isRenown != value)
                {
                    SwapPointSprite();
                }
                _isRenown = value;
            }
        }
        
        public void SetSlot(string pName, bool isRenown, int upScore, int downScore, RenderTexture playerRender)
        {
            nameText.text = pName;
            IsRenown = isRenown;
            upScoreText.text = upScore.ToString();
            downScoreText.text = downScore.ToString();
            playerRenderImage.texture = playerRender;
        }
        
        public (float, float) GetRawImageSize()
        {
            return (playerRenderImage.rectTransform.rect.width, playerRenderImage.rectTransform.rect.height);
        }
        
        private void SwapPointSprite()
        {
            (upPointImage.transform.position, downPointImage.transform.position) = 
                (downPointImage.transform.position, upPointImage.transform.position);
        }
    }
}
