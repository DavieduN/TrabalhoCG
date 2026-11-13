using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        // Trava o cursor no centro da tela e o deixa invisível
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Lê os movimentos do mouse. Time.deltaTime garante que a velocidade
        // independe da taxa de quadros (FPS) do jogo.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calcula a rotação vertical e limita para não virar o pescoço 360 graus
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Aplica a rotação vertical apenas na Câmera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Aplica a rotação horizontal no corpo inteiro do Player
        playerBody.Rotate(Vector3.up * mouseX);
    }
}