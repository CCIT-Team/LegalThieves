using UnityEngine;

public class PassageManager : MonoBehaviour
{
    public GameObject passageTrap;

    private void OnEnable()
    {
        passageTrap = Instantiate(passageTrap, transform, true);
    }
}