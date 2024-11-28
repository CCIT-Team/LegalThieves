using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public class CompassRotate : MonoBehaviour
    {
        [SerializeField] private RectTransform compass;

        private void Start()
        {
            compass ??= transform.Find("Compass")?.GetComponent<RectTransform>(); 
        }

        public void RotateCompass(Transform localPosition)
        {
            if (compass == null)
                return;
            compass.eulerAngles = new Vector3(0, 0, localPosition.eulerAngles.y);
        }
        
        public void SetActive(bool active)
        {
            compass.gameObject.SetActive(active);
        }
    }
}