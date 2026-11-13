using UnityEngine;
using TMPro; 

public class GameManager : MonoBehaviour
{
    // O Singleton: permite que a Bomba, a Lava e o Cristal achem o GameManager facilmente
    public static GameManager Instance;

    [Header("Menu e Interface")]
    public GameObject canvasMenu;
    public TextMeshProUGUI instrucaoText; // Opcional: para atualizar o texto se quiser

    [Header("Jogador")]
    public CharacterController playerController;

    [Header("Checkpoints (Onde o Player nasce)")]
    public Transform playerSpawnA; // Início do Jogo
    public Transform playerSpawnB; // Após quebrar a parede
    public Transform playerSpawnC; // Após ativar o tesouro

    [Header("Pedestais (Onde os itens nascem)")]
    public Transform pedestalA;
    public Transform pedestalB;
    public Transform pedestalC;

    [Header("Prefabs dos Objetos")]
    public GameObject bombPrefab;
    public GameObject cristalPrefab;
    public GameObject tesouroPrefab;

    [Header("Estado do Jogo")]
    public int faseAtual = 1;
    private GameObject currentItem; // Substituiu o 'currentBomb' para servir para todos os itens

    [Header("Level Design")]
    public GameObject primeiraPlataformaFase2;

    [Header("Vitória")]
    public GameObject canvasVitoria;
    public TextMeshProUGUI textoTempoFinal;
    public AudioSource somVitoria;
    
    private float tempoTotalJogo = 0f;
    private bool cronometroAtivo = false;

    [Header("Trilha Sonora")]
    public AudioSource musicaIda;
    public AudioSource musicaVolta;

    void Update()
    {
        // Roda o tempo apenas enquanto o jogo estiver valendo
        if (cronometroAtivo)
        {
            tempoTotalJogo += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Jogador forçou o respawn!");
            Morrer(); // Reaproveita toda a lógica que já criamos!
        }
    }

    // Chamado pelo Tesouro quando ele chega na Zona Final
    public void VencerJogo()
    {
        cronometroAtivo = false; // Para o relógio

        if (musicaIda != null) musicaIda.Stop();
        if (somVitoria != null) somVitoria.Play();
        
        // Formata o tempo para "MM:SS"
        int minutos = Mathf.FloorToInt(tempoTotalJogo / 60F);
        int segundos = Mathf.FloorToInt(tempoTotalJogo - minutos * 60);
        string tempoFormatado = string.Format("{0:00}:{1:00}", minutos, segundos);

        // Mostra a tela de Vitória
        if (canvasVitoria != null) canvasVitoria.SetActive(true);
        if (textoTempoFinal != null) textoTempoFinal.text = tempoFormatado;

        // Trava o jogador para ele parar de andar
        playerController.enabled = false;
        
        // Destrava o mouse para o jogador poder fechar o jogo ou clicar em "Reiniciar"
        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("Vitória! Tempo: " + tempoFormatado);
    }

    void Awake()
    {
        // Configuração padrão do Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Essa é a função chamada pelo seu Botão no Canvas
    public void IniciarJogo()
    {
        canvasMenu.SetActive(false); // Esconde o menu
        faseAtual = 1;

        // --- AQUI ESTÁ A CORREÇÃO DO RELÓGIO ---
        cronometroAtivo = true;      // Liga o cronômetro
        tempoTotalJogo = 0f;         // Zera o tempo (importante para quando o jogador der "Restart")
        if (canvasVitoria != null) canvasVitoria.SetActive(false); // Esconde a tela de vitória
        // ---------------------------------------

        // Garante que o player comece no lugar exato
        playerController.enabled = false;
        playerController.transform.position = playerSpawnA.position;
        playerController.enabled = true;

        SpawnarItem();
    }

    // Chamado pela Bomba, Cristal ou pela Lava quando algo dá errado
    public void Morrer()
    {
        Debug.Log("Falhou! Voltando para o checkpoint da Fase " + faseAtual);

        // 1. Desliga o controle do jogador para a física não anular o teleporte
        playerController.enabled = false;

        // 2. Verifica a fase e teleporta para o lugar certo
        if (faseAtual == 1)
        {
            playerController.transform.position = playerSpawnA.position;
        }
        else if (faseAtual == 2)
        {
            playerController.transform.position = playerSpawnB.position;
        }
        else if (faseAtual == 3)
        {
            playerController.transform.position = playerSpawnC.position;
        }

        // 3. Religa o controle e recria o objeto daquela fase
        playerController.enabled = true;
        SpawnarItem();
    }

    // Chamado pela Bomba quando ela acerta a parede destrutível
    public void AvancarParaFase2()
    {
        faseAtual = 2;
        SpawnarItem();
        
        // Habilita a plataforma para o jogador conseguir pular
        if (primeiraPlataformaFase2 != null)
            primeiraPlataformaFase2.SetActive(true); 
            
        Debug.Log("Parede destruída! Caminho para a Fase 2 liberado.");
    }

    public void AtualizarCheckpointFase2(Transform novoSpawnPlayer, Transform novoPedestalItem)
    {
        playerSpawnB = novoSpawnPlayer;
        pedestalB = novoPedestalItem;
        Debug.Log("Checkpoint IDA (Fase 2) salvo!");
    }

    // Atualiza o checkpoint da Volta
    public void AtualizarCheckpointFase3(Transform novoSpawnPlayer, Transform novoPedestalItem)
    {
        playerSpawnC = novoSpawnPlayer;
        pedestalC = novoPedestalItem;
        Debug.Log("Checkpoint VOLTA (Fase 3) salvo!");
    }

    // Chamado pelo pedestal do Cristal quando ele chegar no fim do parkour
    public void AvancarParaFase3()
    {
        faseAtual = 3;
        SpawnarItem(); // Cria o tesouro no Ponto C
        Debug.Log("Cristal entregue! Tesouro liberado. Fuga iniciada!");
    }

    // Função interna para limpar o item velho e criar o novo no lugar certo
    private void SpawnarItem()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        if (faseAtual == 1)
            currentItem = Instantiate(bombPrefab, pedestalA.position, pedestalA.rotation);
        else if (faseAtual == 2)
            currentItem = Instantiate(cristalPrefab, pedestalB.position, pedestalB.rotation);
        else if (faseAtual == 3)
            currentItem = Instantiate(tesouroPrefab, pedestalC.position, pedestalC.rotation);
    }
}