using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject menuPanel;
    public Transform cameraTransform;

    [Header("Menu Position")]
    public float distanceFromCamera = 1.5f;

    InputAction _menuAction;
    bool _menuOpen;

    void Awake()
    {
        _menuAction = new InputAction();
        _menuAction.AddBinding("<XRController>{LeftHand}/menuButton");
        _menuAction.AddBinding("<Keyboard>/escape");
        _menuAction.Enable();
        menuPanel.SetActive(false);
    }

    void OnDestroy()
    {
        _menuAction.Disable();
    }

    void Update()
    {
        if (_menuAction.WasPressedThisFrame())
            ToggleMenu();
    }

    void ToggleMenu()
    {
        _menuOpen = !_menuOpen;

        if (_menuOpen)
        {
            menuPanel.transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
            menuPanel.transform.rotation = Quaternion.LookRotation(cameraTransform.forward);
        }

        menuPanel.SetActive(_menuOpen);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
