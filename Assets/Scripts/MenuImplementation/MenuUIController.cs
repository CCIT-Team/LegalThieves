using Fusion.Menu;
using UnityEngine;

namespace LegalThieves.Menu
{
    public class MenuUIController : FusionMenuUIController<FusionMenuConnectArgs>
    {
        private static MenuUIController _instance;
        public static MenuUIController Instance
        {
            get
            {
                if(_instance == null)
                    _instance = FindObjectOfType<MenuUIController>();
                return _instance;
            }
            set
            {
                if(_instance == null)
                    _instance = value;
                if(_instance != value)
                    Destroy(value);
            }
        }
        
        // [Space]
        // [SerializeField] 

        public void ShutDownServer()
        {
            _connection?.DisconnectAsync(ConnectFailReason.UserRequest);
            var main = Get<FusionMenuUIMain>();
            var gameplay = Get<FusionMenuUIGameplay>();
            
            if (!main.IsShowing && main != null) main.Show();
            
            if (gameplay.IsShowing && gameplay != null) gameplay.Hide();
        }
        
        
    }
}
