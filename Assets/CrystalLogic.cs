using UnityEngine;

public class CrystalLogic : MonoBehaviour
{
    public bool isActive = false;
    
    private int crystalPhase = 0; // Vai de 0 a 8
    private bool rechargeBuffered = false;

    private float lastXRotation, lastYRotation;
    private float accumulatedRotation;
    private Renderer rend;

    [Header("Áudio")]
    public AudioClip somQuebrar;

    void Start()
    {
        rend = GetComponent<Renderer>();
        lastXRotation = transform.eulerAngles.x;
        lastYRotation = transform.eulerAngles.y;
    }

    // Quando o cristal é ativado, ele se inscreve no relógio
    public void AtivarCristal()
    {
        isActive = true;
        crystalPhase = 0;
        rechargeBuffered = false;
        
        // Garante que o som comece normal
        RhythmManager.Instance.pitchAtual = 1.0f; 
        
        RhythmManager.Instance.StartClock();
        RhythmManager.Instance.OnSegmentChanged += ProcessarTick; 
    }

    // Se o cristal for destruído (caiu ou quebrou), desinscreve do relógio para não dar erro
    void OnDestroy()
    {
        if (RhythmManager.Instance != null)
        {
            RhythmManager.Instance.OnSegmentChanged -= ProcessarTick;
            RhythmManager.Instance.StopClock();
        }
    }

    void Update()
    {
        if (!isActive) return;

        // Lógica de agitar para recarregar
        float deltaX = Mathf.DeltaAngle(lastXRotation, transform.eulerAngles.x);
        float deltaY = Mathf.DeltaAngle(lastYRotation, transform.eulerAngles.y);
        
        accumulatedRotation += Mathf.Abs(deltaX) + Mathf.Abs(deltaY);
        
        lastXRotation = transform.eulerAngles.x;
        lastYRotation = transform.eulerAngles.y;

        if (accumulatedRotation >= 240f)
        {
            rechargeBuffered = true; // Salva no Buffer!
            accumulatedRotation = 0f;
            Debug.Log("Chacoalhado! Recarga no próximo beat...");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;

        // Se bater no alvo correto da Fase 2...
        if (collision.gameObject.CompareTag("StrangeRock"))
        {
            if (somQuebrar != null) 
                AudioSource.PlayClipAtPoint(somQuebrar, transform.position, 1.0f);

            Destroy(collision.gameObject); 
            GameManager.Instance.AvancarParaFase3(); 
            Destroy(gameObject);
        }
        // Se bater forte no chão ou nas paredes da caverna...
        else if (collision.gameObject.name != "Plane")
        {
            if (somQuebrar != null) 
                AudioSource.PlayClipAtPoint(somQuebrar, transform.position, 1.0f);

            GameManager.Instance.Morrer();
            Destroy(gameObject);
        }
    }

    // Esta função roda EXATAMENTE a cada 0.5s junto com as plataformas
    private void ProcessarTick()
    {
        if (rechargeBuffered)
        {
            crystalPhase = 0;
            rechargeBuffered = false;
            RhythmManager.Instance.pitchAtual = 1.0f;
            // Tocar som de RECARGA aqui!
        }
        else
        {
            crystalPhase++;
            
            if (crystalPhase >= 8)
            {
                Debug.Log("O Cristal estilhaçou!");
                if (somQuebrar != null) 
                    AudioSource.PlayClipAtPoint(somQuebrar, transform.position, 1.0f);
                GameManager.Instance.Morrer();
                Destroy(gameObject);
                return; // Morreu
            }
            
            // Aumenta o tom e muda a cor a cada fase que passa!
            // Ex: audioSource.pitch = 1.0f + (crystalPhase * 0.1f);
            // audioSource.PlayOneShot(somBeep);
            RhythmManager.Instance.pitchAtual = 1.0f + (crystalPhase * 0.2f);
        }

        // Atualiza a cor (Azul para Vermelho)
        float pct = (float)crystalPhase / 8f;
        rend.material.color = Color.Lerp(Color.blue, Color.red, pct);
    }
}