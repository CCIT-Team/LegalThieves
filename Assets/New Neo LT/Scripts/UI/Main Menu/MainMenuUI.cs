using LegalThieves.Menu;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI.Main_Menu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;
        
        public static MainMenuUI Instance;
        
        private void Start()
        {
            Instance = this;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        
        public void ShutDownServer()
        {
            mainMenu?.gameObject.SendMessage("OnDisconnectPressed");
        }
    }
}
