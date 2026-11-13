using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;

    public bool isClockRunning = false;
    public float globalTime = 0f;
    public int currentSegment = 0; // Vai de 0 a 7

    // Isso permite que o Cristal seja avisado na exata hora que a batida muda
    public delegate void OnBeat();
    public event OnBeat OnSegmentChanged;

    private int lastSegment = -1;

    [Header("Áudio Global do Ritmo")]
    public AudioClip somTick;
    public float pitchAtual = 1.0f; // Permite que o cristal mude o tom do relógio
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void StartClock()
    {
        isClockRunning = true;
        globalTime = 0f;
        currentSegment = 0;
        lastSegment = -1;
    }

    public void StopClock()
    {
        isClockRunning = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isClockRunning)
            {
                StopClock();
                Debug.Log("[CHEAT] Relógio Parado!");
            }
            else
            {
                StartClock();
                Debug.Log("[CHEAT] Relógio Iniciado (Modo de Teste)!");
            }
        }
        
        if (!isClockRunning) return;

        globalTime += Time.deltaTime;
        currentSegment = Mathf.FloorToInt((globalTime % 4.0f) / 0.5f);

        if (currentSegment != lastSegment)
        {
            lastSegment = currentSegment;

            // Toca o TICK centralizado!
            if (audioSource != null && somTick != null)
            {
                audioSource.pitch = pitchAtual;
                audioSource.PlayOneShot(somTick);
            }

            OnSegmentChanged?.Invoke(); 
        }
    }
}