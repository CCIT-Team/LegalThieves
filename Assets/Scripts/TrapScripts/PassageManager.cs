using UnityEngine;

namespace TrapScripts
{
    

    public class PassageManager : MonoBehaviour
    {
        public GameObject trapPrefabs;
        private void OnEnable()
        {
            trapPrefabs = Instantiate(trapPrefabs, transform, false);
        }
    }
}