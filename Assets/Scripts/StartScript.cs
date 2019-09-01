using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    //Load the game scene
    public void StartGame() {
        SceneManager.LoadScene("GameScene");
    }
}
