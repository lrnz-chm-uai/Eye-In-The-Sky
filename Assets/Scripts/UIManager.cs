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
            gameManager.OnGameStarted += HandleGameStarted;
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
            gameManager.OnGameStarted -= HandleGameStarted;
        }
    }

    private void OnStartClicked()
    {
        switch (gameManager.CurrentState)
        {
            case GameManager.GameState.WaitingToStart:
                gameManager.StartGame();
                break;

            case GameManager.GameState.Won:
            case GameManager.GameState.Lost:
                // Pedir auto-inicio y recargar: la próxima escena arranca jugando
                GameManager.RequestAutoStartOnNextLoad();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }

    private void HandleGameStarted()
    {
        HideStartButton();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
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