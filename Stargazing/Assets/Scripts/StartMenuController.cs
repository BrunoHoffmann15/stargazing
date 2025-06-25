using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartARScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void AccessHelpScreen()
    {
        SceneManager.LoadScene("HelpScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
