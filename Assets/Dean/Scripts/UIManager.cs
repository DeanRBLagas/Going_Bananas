using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerHealth;
    [SerializeField] private TextMeshProUGUI playerMoney;
    private Player player;

    private void Start()
    {
        Time.timeScale = 1;
        player = FindFirstObjectByType<Player>();
        player.pauseUI.SetActive(false);
    }

    private void Update()
    {
        playerHealth.text = "Health: " + player.currentHealth;
        playerMoney.text = "Money: " + player.money;
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        player.pauseUI.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
