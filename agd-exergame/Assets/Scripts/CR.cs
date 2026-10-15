using UnityEngine;


/// <summary>
/// CACHED REFERENCES Singleton
/// </summary>
public class CR : MonoBehaviour
{
    public static CR r;

    private void Awake()
    {
        if(r != null && r != this)
        {
            Destroy(gameObject);
            return;
        }

        r = this;
    }

    public int playerCount = 0;
}
