using UnityEngine;
using System.Collections;

public class Cidade : MonoBehaviour
{
    // Variável para a CONEXÃO com o texto de derrota
    [Header("UI de Derrota")]
    public GameObject textoDerrota; // <--- NOVO

    // Variáveis que você vai configurar no Inspector
    [Header("Sprites de Dano")]
    public Sprite spriteDanoQuebrado;
    public Sprite spriteDanoFogo;    

    [Header("Dados da Vida")]
    public double vidaAtual; 
    private double vidaInicial;
    
    private SpriteRenderer sr;
    private Sprite spriteOriginal; 

    void Start()
    {
        if (InfoGeral.Instance != null)
        {
            vidaAtual = InfoGeral.Instance.vida_da_ilha;
            vidaInicial = InfoGeral.Instance.vida_da_ilha;
        }
        
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            spriteOriginal = sr.sprite;
        }
        
        // NOVO: Garante que o texto de derrota comece invisível
        if (textoDerrota != null)
        {
            textoDerrota.SetActive(false); 
        }
    }

    void Update()
    {
        // ... Lógica de troca de sprite (VerificarStatusVisual) ...
        // (Deixei a função VerificarStatusVisual completa da resposta anterior)
        
        if (sr == null || InfoGeral.Instance == null) return;
        
        float porcentagemVida = (float)(vidaAtual / vidaInicial); 
        
        if (porcentagemVida < 0.6f)
        {
            if (spriteDanoFogo != null)
            {
                sr.sprite = spriteDanoFogo;
                sr.color = Color.Lerp(Color.red, Color.yellow, 0.5f); 
            }
        }
        else if (porcentagemVida < 1.0f)
        {
            if (spriteDanoQuebrado != null)
            {
                sr.sprite = spriteDanoQuebrado;
                sr.color = Color.white; 
            }
        }
        else
        {
            sr.sprite = spriteOriginal;
            sr.color = Color.white;
        }
    }
    
    // --- Recebimento de Dano ---
    
    public void ReceberDano(double danoSofrido)
    {
        Debug.Log($"💥 Cidade recebeu {danoSofrido} de dano!");
        
        vidaAtual -= danoSofrido;
        
        if (InfoGeral.Instance != null)
        {
             InfoGeral.Instance.vida_da_ilha = vidaAtual;
        }
        
        // 🚨 AQUI É ONDE O TEXTO É ATIVADO!
        if (vidaAtual <= 0)
        {
            Destroy(gameObject); // Destroi a cidade visualmente
            Debug.Log("💀 CIDADE DESTRUÍDA!");
            
            // NOVO: Liga o objeto de texto de derrota
            if (textoDerrota != null) 
            {
                textoDerrota.SetActive(true);
            }
        }
        
        StartCoroutine(PiscarDano());
    }
    System.Collections.IEnumerator PiscarDano()
    {
        // ... (O resto da sua função PiscarDano)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color original = sr.color;

        for(int i = 0; i < 5; i++) // Reduzido para 2 piscadas para ser mais rápido
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f); 
            // Usa a cor determinada pelo VerificarStatusVisual() como "original"
            sr.color = original; 
            yield return new WaitForSeconds(0.05f);
        }
    }
}