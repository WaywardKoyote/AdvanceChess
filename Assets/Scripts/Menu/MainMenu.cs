using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameLevel;

    public Camera cam;
    public float rotSpeed = -10f;
    private Vector3 rotPoint = new Vector3(-0.5f, 0, -0.5f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BGMusicPlayer.Instance.PlayBGMusic();
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.RotateAround(rotPoint, Vector3.up, rotSpeed * Time.deltaTime);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameLevel);
    }

    public void OpenOptions()
    {

    }

    public void CloseOptions()
    {
        
    }

    public void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
