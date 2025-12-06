using UnityEngine;

public class ControleVulcao : MonoBehaviour
{
    // A imagem que o vulcão terá quando estiver cheio.
    public Sprite spriteVulcaoCheio; 
    
    // Guardamos o sprite original para poder voltar ao normal
    private Sprite spriteVulcaoOriginal; 
    private SpriteRenderer spriteRenderer; 
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 1. Guarda o sprite original que está no objeto ANTES de tudo
        if (spriteRenderer != null)
        {
            spriteVulcaoOriginal = spriteRenderer.sprite;
        }

        if (spriteVulcaoCheio == null)
        {
            Debug.LogError("O spriteVulcaoCheio não foi definido no Inspector! O script não vai funcionar.");
        }
    }

    void Update()
    {
        if (InfoGeral.Instance == null || spriteRenderer == null) return;

        // 2. Chama a função que verifica a pressão a cada frame.
        VerificarPressao(
            (float)InfoGeral.Instance.pressao, 
            (float)InfoGeral.Instance.pressaoMaxima
        );
    }
    
    void VerificarPressao(float pressaoAtual, float pressaoMaxima)
    {
        // Se a pressão atual for IGUAL ou MAIOR que a pressão máxima...
        if (pressaoAtual >= pressaoMaxima)
        {
            // Se o sprite de "cheio" for válido E ele for diferente do atual...
            if (spriteVulcaoCheio != null && spriteRenderer.sprite != spriteVulcaoCheio)
            {
                // 3. TROCA para o sprite de vulcão cheio.
                spriteRenderer.sprite = spriteVulcaoCheio;
                Debug.Log("Vulcão CHEIO! O sprite mudou.");
            }
        }
        // CASO CONTRÁRIO (se a pressão não estiver no máximo)...
        else
        {
            // 4. TROCA de volta para o sprite original.
            if (spriteVulcaoOriginal != null && spriteRenderer.sprite != spriteVulcaoOriginal)
            {
                spriteRenderer.sprite = spriteVulcaoOriginal;
                // Opcional: Debug.Log("Vulcão voltando ao normal.");
            }
        }
    }
}