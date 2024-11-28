using Fusion.Menu;
using LegalThieves.Menu;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI.Main_Menu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private FusionMenuUIController<FusionMenuConnectArgs> menuUIController;
        
        public static MainMenuUI Instance;
        
        private void Awake()
        {
            Instance = this;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }

        public void ShowGamePlayUI()
        {
            menuUIController?.Show<FusionMenuUIGameplay>();
        }
        
        public void ShutDownServer()
        {
            menuUIController?.gameObject.SendMessage("OnDisconnectPressed");
            var gameplayUI = menuUIController?.Get<FusionMenuUIGameplay>();
            gameplayUI?.gameObject.SendMessage("OnDisconnectPressed");
            // mainMenu?.gameObject.SendMessage("OnDisconnectPressed");
        }
    }
}
