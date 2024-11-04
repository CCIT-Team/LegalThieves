using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play
{
    public class ResourcesManager : MonoBehaviour
    {
        
        
        private readonly List<NetworkObject> _managedObjects = new ();

        public void Manage(NetworkObject obj)
        {
            _managedObjects.Add(obj);
        }

        public void Purge()
        {
            foreach (var obj in _managedObjects)
            {
                if (obj) NewGameManager.Instance.Runner.Despawn(obj);
            }
            _managedObjects.Clear();
        }
    }
}
