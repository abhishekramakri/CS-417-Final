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
        if (_menuAction.WasPressedThisFrame() || Keyboard.current.mKey.wasPressedThisFrame)
            ToggleMenu();
    }

    void ToggleMenu()
    {
        _menuOpen = !_menuOpen;

        if (_menuOpen)
        {
            Vector3 flatForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            menuPanel.transform.position = cameraTransform.position + flatForward * distanceFromCamera;
            menuPanel.transform.rotation = Quaternion.LookRotation(flatForward);
        }

        menuPanel.SetActive(_menuOpen);
    }

    // Wipe saved state and reload from scratch
    public void Restart()
    {
        SaveManager.Instance?.ClearSave();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Save current state (destroyed objects + player position) then exit
    public void SaveAndExit()
    {
        // OnApplicationQuit in SaveManager handles the actual write
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Wipe saved state then exit — next launch starts fresh
    public void Quit()
    {
        SaveManager.Instance?.ClearSave();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
