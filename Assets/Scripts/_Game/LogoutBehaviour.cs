using Boom.Patterns.Broadcasts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutBehaviour : MonoBehaviour
{
    [SerializeField, Header("Optional Scene")] private string sceneToGoToOnLogout;
    public void Logout()
    {
        Broadcast.Invoke<UserLogout>();

        if (!string.IsNullOrEmpty(sceneToGoToOnLogout))
        {
            Debug.Log("Load scene: "+ sceneToGoToOnLogout);
            SceneManager.LoadScene(sceneToGoToOnLogout);
        }
    }
}
