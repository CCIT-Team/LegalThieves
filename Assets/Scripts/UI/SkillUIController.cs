using TMPro;
using UnityEngine;

namespace UI
{
    public class SkillUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text console;

        public void WriteToConsole(string message)
        {
            console.text += message;
            console.text += "\n";
        }
        
        public void TogleConsole()
        {
            console.gameObject.SetActive(!console.gameObject.activeSelf);
        }
    }
}