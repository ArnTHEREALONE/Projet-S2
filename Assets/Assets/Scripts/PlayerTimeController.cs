using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTimeController : MonoBehaviour
{
    [Header("Player Prefabs")]
    public GameObject[] playerPrefabs;

    [Header("Input Keys (AZERTY)")]
    public KeyCode previousKey = KeyCode.Alpha1;
    public KeyCode nextKey = KeyCode.Alpha2;

    [Header("Camera")]
    public ThirdPersonCamera cameraController;

    private int currentIndex = 0;
    private GameObject currentPlayerInstance;

    void Start()
    {
        if (playerPrefabs == null || playerPrefabs.Length == 0)
        {
            Debug.LogError("Aucun prefab assigné.");
            return;
        }

        SpawnCurrentPlayer();
    }

    void Update()
    {
        if (Gamepad.current != null)
        {

        }
        bool lb = Gamepad.current.leftShoulder.isPressed;
        bool rb = Gamepad.current.rightShoulder.isPressed;

        if (Input.GetKeyDown(nextKey) || rb)
            CyclePlayer(true);

        if (Input.GetKeyDown(previousKey) || lb)
            CyclePlayer(false);
    }

    private void CyclePlayer(bool forward)
    {
        Vector3 savedPosition = currentPlayerInstance.transform.position;
        Quaternion savedRotation = currentPlayerInstance.transform.rotation;

        Destroy(currentPlayerInstance);

        if (forward)
            currentIndex = (currentIndex + 1) % playerPrefabs.Length;
        else
            currentIndex = (currentIndex - 1 + playerPrefabs.Length) % playerPrefabs.Length;

        currentPlayerInstance = Instantiate(
            playerPrefabs[currentIndex],
            savedPosition,
            savedRotation
        );

        AssignCameraTarget();
    }

    private void SpawnCurrentPlayer()
    {
        currentPlayerInstance = Instantiate(
            playerPrefabs[currentIndex],
            transform.position,
            transform.rotation
        );

        AssignCameraTarget();
    }

    private void AssignCameraTarget()
    {
        if (cameraController != null)
            cameraController.target = currentPlayerInstance.transform;
    }
}
