using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    public float minDistance = 2.5f;
    public float maxDistance = 6f;
    public float moveSpeed = 15f; // Velocidade em que o objeto persegue o alvo
    public float rotationSpeed = 200f;

    private Rigidbody grabbedObject;
    private float currentDistance;
    private LineRenderer lineRenderer;
    private MouseLook mouseLook; // Referência para travar a câmera do player

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mouseLook = GetComponent<MouseLook>();
        lineRenderer.enabled = false; // Esconde a linha no começo
    }

    void Update()
    {
        if (grabbedObject == null && lineRenderer.enabled)
        {
            Release();
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            // Atira o raio
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                // Tenta pegar o componente de Botão da UI no objeto que o raio bateu
                UnityEngine.UI.Button btn = hit.collider.GetComponent<UnityEngine.UI.Button>();
                
                if (btn != null)
                {
                    // Se achou um botão, invoca o "OnClick()" exatamente como se o mouse tivesse clicado
                    btn.onClick.Invoke();
                }
            }
        }

        // Agarrar / Soltar com E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (grabbedObject == null) TryGrab();
            else Release();
        }

        if (grabbedObject != null)
        {
            // Aproximar / Afastar com o Scroll do Mouse
            float scroll = Input.mouseScrollDelta.y;
            currentDistance += scroll * 0.5f; // Multiplicador de sensibilidade do scroll
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            // Atualiza os pontos visuais da linha
            // Ponto 0: Um pouco abaixo e a frente da câmera (para não sair do meio do seu "olho")
            lineRenderer.SetPosition(0, transform.position + transform.forward * 0.3f - transform.up * 0.2f); 
            // Ponto 1: Centro do objeto agarrado
            lineRenderer.SetPosition(1, grabbedObject.position);

            // Lógica de Rotação (Segurando R)
            if (Input.GetKey(KeyCode.R))
            {
                mouseLook.enabled = false; // Trava o script da câmera
                
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                
                // Rotaciona o objeto em relação ao mundo e à câmera
                grabbedObject.transform.Rotate(Vector3.up, -mouseX * rotationSpeed * Time.deltaTime, Space.World);
                grabbedObject.transform.Rotate(transform.right, mouseY * rotationSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                mouseLook.enabled = true; // Libera a câmera se não estiver segurando R
            }
        }
    }

    // A física real deve ser calculada aqui para evitar tremedeiras ("jitter")
    void FixedUpdate()
    {
        if (grabbedObject == null) return;

        // O objeto deve SEMPRE perseguir o alvo, mesmo enquanto você segura 'R'.
        // Assim, se você andar (WASD) enquanto rotaciona, o objeto acompanha o player normalmente
        // e a física de colisão da bomba se mantém ativa.
        // Calcula o ponto alvo exato na frente do jogador
        Vector3 targetPoint = transform.position + transform.forward * currentDistance;
        // Calcula o vetor de direção do objeto até o ponto
        Vector3 direction = targetPoint - grabbedObject.position;
        // Movemos o objeto aplicando velocidade (ele vai respeitar colisões naturais)
        grabbedObject.linearVelocity = direction * moveSpeed;

        // Se estiver rotacionando, nós apenas zeramos a velocidade ANGULAR (o giro da física),
        // para que a nossa rotação manual via código não brigue com a simulação física.
        if (Input.GetKey(KeyCode.R))
        {
            grabbedObject.angularVelocity = Vector3.zero;
        }
    }

    void TryGrab()
    {
        // Cria um raio partindo do centro da câmera para frente
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Se o raio bater em algo dentro da distância máxima...
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // ...e esse algo tiver a tag "Interactable"
            if (hit.collider.CompareTag("Interactable"))
            {
                grabbedObject = hit.collider.GetComponent<Rigidbody>();
                
                // Pega a distância atual do objeto para não puxá-lo bruscamente
                currentDistance = Vector3.Distance(transform.position, grabbedObject.position);
                currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
                
                // Desliga a gravidade e aumenta o atrito do ar para ele não quicar como mola
                grabbedObject.useGravity = false;
                grabbedObject.linearDamping = 10f; 
                
                lineRenderer.enabled = true;

                FragileItem fragile = grabbedObject.GetComponent<FragileItem>();
                if (fragile != null)
                {
                    fragile.isActive = true;
                }
                CrystalLogic crystal = grabbedObject.GetComponent<CrystalLogic>();
                if (crystal != null && !crystal.isActive)
                {
                    crystal.AtivarCristal(); // Começa a música e o relógio global!
                }
                TreasureLogic tesouro = grabbedObject.GetComponent<TreasureLogic>();
                if (tesouro != null && !tesouro.isActive)
                {
                    tesouro.AtivarTesouro();
                }
            }
        }
    }

    void Release()
    {
        if (grabbedObject != null)
        {
            // --- NOVO: ANTI-CHEAT DO TESOURO ---
            TreasureLogic tesouro = grabbedObject.GetComponent<TreasureLogic>();
            if (tesouro != null)
            {
                tesouro.DesativarTesouro();
            }

            grabbedObject.useGravity = true;
            grabbedObject.linearDamping = 0f;
            grabbedObject = null;
        }

        lineRenderer.enabled = false;
        mouseLook.enabled = true;
    }
}