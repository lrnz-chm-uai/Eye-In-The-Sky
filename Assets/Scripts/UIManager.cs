using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private UIDocument uiDocument;
    private Button startButton;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();

        var root = uiDocument.rootVisualElement;
        startButton = root.Q<Button>("StartButton");
        if (startButton == null)
        {
            Debug.LogError("UIManager: no se encontró el botón 'StartButton' en el UXML.");
            return;
        }

        startButton.clicked += OnStartClicked;

        if (gameManager != null)
        {
            gameManager.OnGameEnded += HandleGameEnded;
        }

        // Estado inicial: botón visible y cursor libre para poder clickearlo
        ShowStartButton();
    }

    void OnDisable()
    {
        if (startButton != null)
        {
            startButton.clicked -= OnStartClicked;
        }

        if (gameManager != null)
        {
            gameManager.OnGameEnded -= HandleGameEnded;
        }
    }

    private void OnStartClicked()
    {
        switch (gameManager.CurrentState)
        {
            case GameManager.GameState.WaitingToStart:
                HideStartButton();
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                gameManager.StartGame();
                break;

            case GameManager.GameState.Won:
            case GameManager.GameState.Lost:
                // Reinicio: recargar la escena actual
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }

    private void HandleGameEnded()
    {
        // Victoria o derrota: liberar el cursor y re-mostrar el botón para reiniciar
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        ShowStartButton();
    }

    private void ShowStartButton()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        if (startButton != null)
        {
            startButton.style.display = DisplayStyle.Flex;
        }
    }

    private void HideStartButton()
    {
        if (startButton != null)
        {
            startButton.style.display = DisplayStyle.None;
        }
    }
}