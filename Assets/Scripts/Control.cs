using UnityEngine;
using UnityEngine.SceneManagement;

public class Control : MonoBehaviour
{
    // Start is called before the first frame update
    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
        //print("restart working");
    }

    public void CloseGame()
    {
        Application.Quit();
        //print("quit working");
    }
}
