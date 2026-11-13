using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private Vector3 velocity;
    private bool isGrounded;

    void Update()
    {
        isGrounded = controller.isGrounded;

        // Reseta a gravidade se estiver no chão
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Cria o vetor de movimento baseado na direção que o player está olhando
        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1f) move.Normalize();

        // Aplica o movimento horizontal
        controller.Move(move * speed * Time.deltaTime);

        // Sistema de Pulo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Fórmula da física para pulo: raiz(altura * -2 * gravidade)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplica a gravidade continuamente
        velocity.y += gravity * Time.deltaTime;

        // Move o personagem no eixo Y (caindo ou pulando)
        controller.Move(velocity * Time.deltaTime);
    }
}