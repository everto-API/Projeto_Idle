using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClickVulcao : MonoBehaviour
{
    // ======================================================
    // REFERÊNCIAS DE COMPONENTES (Arraste do Inspector)
    // ======================================================
    [Header("═ REFERÊNCIAS OBRIGATÓRIAS ═")]
    [Tooltip("Texto UI que mostra a Obsidiana")]
    public TMP_Text dinheiroTexto;
    
    [Tooltip("Transform da cidade (alvo do ataque)")]
    public Transform alvoCidade;
    
    // ======================================================
    // CONFIGURAÇÕES DA TRAJETÓRIA (Ajuste no Inspector!)
    // ======================================================
    [Header("═ CONFIGURAÇÕES DA TRAJETÓRIA ═")]
    [Tooltip("Dano base multiplicado pelo dano do InfoGeral")]
    public double danoBase = 1.0;
    
    [Space(10)]
    [Tooltip("QUÃO ALTO o projétil sobe (em unidades Unity)")]
    [Range(3f, 25f)]
    public float alturaArco = 10f;
    
    [Tooltip("Velocidade do voo - mais lento = mais tempo no ar")]
    [Range(1f, 10f)]
    public float velocidadeVoo = 3f;
    
    [Tooltip("Tamanho visual do projétil")]
    [Range(0.5f, 5f)]
    public float tamanhoProjetil = 1.5f;
    
    // ======================================================
    // EFEITOS VISUAIS (Opcionais)
    // ======================================================
    [Header("═ EFEITOS VISUAIS (Opcionais) ═")]
    [Tooltip("Objeto que aparece quando o vulcão pode atirar")]
    public GameObject indicadorPressaoCheia;
    
    [Tooltip("Botão UI para testes (encher pressão rapidamente)")]
    public Button botaoTesteEncherPressao;
    
    // ======================================================
    // VARIÁVEIS PRIVADAS
    // ======================================================
    private float tempoUltimoLogPressao = 0f;
    
    // ======================================================
    // MÉTODOS UNITY
    // ======================================================
    
    void Start()
    {
        // Configura botão de teste se existir
        if (botaoTesteEncherPressao != null)
        {
            botaoTesteEncherPressao.onClick.AddListener(EncherPressaoParaTeste);
        }
        
        Debug.Log("🌋 Vulcão inicializado!");
        Debug.Log($"🎯 Alvo: {(alvoCidade != null ? alvoCidade.name : "NÃO CONFIGURADO")}");
        Debug.Log($"📏 Configuração: Altura={alturaArco}, Velocidade={velocidadeVoo}, Tamanho={tamanhoProjetil}");
    }
    
    void Update()
    {
        // 1. Atualiza texto da Obsidiana
        if (InfoGeral.Instance != null && dinheiroTexto != null)
        {
            dinheiroTexto.text = "Obsidian: " + InfoGeral.Instance.obsidiana.ToString("F0");
        }
        
        // 2. Controla indicador visual de pressão cheia
        if (InfoGeral.Instance != null && indicadorPressaoCheia != null)
        {
            bool podeAtirar = InfoGeral.Instance.pressao >= InfoGeral.Instance.pressaoMaxima;
            indicadorPressaoCheia.SetActive(podeAtirar);
        }
        
        // 3. Log da pressão a cada 3 segundos
        if (InfoGeral.Instance != null && Time.time - tempoUltimoLogPressao > 3f)
        {
            tempoUltimoLogPressao = Time.time;
            Debug.Log($"🔋 Pressão do vulcão: {InfoGeral.Instance.pressao:F1}/{InfoGeral.Instance.pressaoMaxima}");
        }
    }
    
    // ======================================================
    // MÉTODOS PÚBLICOS (Chamados por UI ou testes)
    // ======================================================
    
    /// <summary>
    /// Enche a pressão instantaneamente (para testes)
    /// </summary>
    public void EncherPressaoParaTeste()
    {
        if (InfoGeral.Instance != null)
        {
            InfoGeral.Instance.pressao = InfoGeral.Instance.pressaoMaxima;
            Debug.Log($"⚡ Pressão máxima forçada: {InfoGeral.Instance.pressao}/{InfoGeral.Instance.pressaoMaxima}");
        }
    }
    
    /// <summary>
    /// Gera Obsidiana (para testes de economia)
    /// </summary>
    public void GeradorObsidian() 
    {
        if (InfoGeral.Instance != null)
        {
            InfoGeral.Instance.obsidiana += 1;
            Debug.Log($"💰 +1 Obsidiana! Total: {InfoGeral.Instance.obsidiana}");
        }
    }
    
    /// <summary>
    /// Força disparo ignorando pressão (para testes)
    /// </summary>
    public void DispararForcado()
    {
        if (InfoGeral.Instance != null)
        {
            InfoGeral.Instance.pressao = InfoGeral.Instance.pressaoMaxima;
            DispararLava();
        }
    }
    
    // ======================================================
    // MÉTODOS DE CLIQUE/INPUT
    // ======================================================
    
    /// <summary>
    /// Chamado quando clica no vulcão (requer Collider2D)
    /// </summary>
    void OnMouseDown()
    {   
        DispararLava();
    }
    
    /// <summary>
    /// Método principal de disparo do vulcão
    /// </summary>
    public void DispararLava()
    {
        Debug.Log("=== TENTATIVA DE DISPARO DO VULCÃO ===");
        
        // 1. VERIFICAÇÕES DE SEGURANÇA
        if (InfoGeral.Instance == null)
        {
            Debug.LogError("❌ ERRO: InfoGeral.Instance é null!");
            return;
        }
        
        if (alvoCidade == null)
        {
            Debug.LogError("❌ ERRO: AlvoCidade não configurado! Arraste a cidade para o campo 'Alvo Cidade' no Inspector.");
            return;
        }
        
        // 2. VERIFICA SE A PRESSÃO ESTÁ MÁXIMA
        bool pressaoSuficiente = InfoGeral.Instance.pressao >= InfoGeral.Instance.pressaoMaxima;
        
        if (!pressaoSuficiente)
        {
            double falta = InfoGeral.Instance.pressaoMaxima - InfoGeral.Instance.pressao;
            double tempoFalta = falta / InfoGeral.Instance.taxaGeracaoBase;
            
            Debug.Log($"⏳ Pressão insuficiente!");
            Debug.Log($"   Atual: {InfoGeral.Instance.pressao:F1}/{InfoGeral.Instance.pressaoMaxima}");
            Debug.Log($"   Faltam: {falta:F1} unidades");
            Debug.Log($"   Aguarde: {tempoFalta:F1} segundos");
            return;
        }
        
        Debug.Log("✅ CONDIÇÕES ATENDIDAS! Iniciando lançamento...");
        
        // 3. CRIA O PROJÉTIL
        GameObject projetil = CriarProjetilRedondo();
        
        // 4. CONFIGURA O PROJÉTIL
        ProjetilLavaSimples lavaScript = projetil.GetComponent<ProjetilLavaSimples>();
        if (lavaScript != null)
        {
            double danoTotal = InfoGeral.Instance.dano * danoBase;
            
            lavaScript.Configurar(
                novoAlvo: alvoCidade,
                novoDano: danoTotal,
                novaVelocidade: velocidadeVoo,
                novaAlturaMaxima: alturaArco
            );
            
            Debug.Log($"💣 Projétil configurado!");
            Debug.Log($"   • Dano: {danoTotal}");
            Debug.Log($"   • Velocidade: {velocidadeVoo}");
            Debug.Log($"   • Altura do arco: {alturaArco}");
            Debug.Log($"   • Tamanho: {tamanhoProjetil}");
        }
        else
        {
            Debug.LogError("❌ Falha ao obter script ProjetilLavaSimples do projétil!");
            return;
        }
        
        // 5. RESETA A PRESSÃO (zera após disparo)
        InfoGeral.Instance.pressao = 0;
        
        Debug.Log("🎇 LANÇAMENTO REALIZADO COM SUCESSO!");
        Debug.Log("   Trajetória: Vulcão → Céu (" + alturaArco + "m) → Cidade");
    }
    
    // ======================================================
    // CRIAÇÃO DO PROJÉTIL
    // ======================================================
    
    /// <summary>
    /// Cria um projétil visualmente redondo com todos os componentes necessários
    /// </summary>
    GameObject CriarProjetilRedondo()
    {
        Debug.Log("🛠️ Criando projétil com arco de " + alturaArco + "m...");
        
        // 1. CRIA O GAMEOBJECT
        GameObject projetil = new GameObject("ProjetilLava_ArcoAlto");
        projetil.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        projetil.tag = "Projetil";
        
        // 2. ADICIONA VISUAL (SPRITE REDONDO)
        SpriteRenderer sr = projetil.AddComponent<SpriteRenderer>();
        
        // Cria uma textura para círculo suave
        int texSize = 64;
        Texture2D tex = new Texture2D(texSize, texSize, TextureFormat.RGBA32, false);
        
        float centro = texSize / 2f;
        float raio = centro - 4;
        
        // Preenche com transparência
        Color32[] coresTransparentes = new Color32[texSize * texSize];
        for (int i = 0; i < coresTransparentes.Length; i++)
            coresTransparentes[i] = Color.clear;
        tex.SetPixels32(coresTransparentes);
        
        // Desenha círculo PERFEITO com gradiente
        for (int x = 0; x < texSize; x++)
        {
            for (int y = 0; y < texSize; y++)
            {
                float dx = x - centro;
                float dy = y - centro;
                float distancia = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (distancia <= raio)
                {
                    float alpha = 1f;
                    
                    // Borda suave (anti-aliasing)
                    if (distancia > raio - 4)
                        alpha = (raio - distancia) / 4f;
                    
                    // Gradiente: centro (amarelo) -> borda (vermelho)
                    float gradiente = 1 - (distancia / raio);
                    Color cor = Color.Lerp(
                        new Color(1, 0.3f, 0, alpha),     // Vermelho alaranjado (borda)
                        new Color(1, 0.9f, 0.3f, alpha),   // Amarelo (centro)
                        gradiente * 0.8f
                    );
                    
                    tex.SetPixel(x, y, cor);
                }
            }
        }
        tex.Apply();
        
        // Cria sprite da textura
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, texSize, texSize), 
                                 new Vector2(0.5f, 0.5f), 100);
        
        // Configurações visuais
        sr.color = new Color(1, 0.5f, 0.1f, 1); // Laranja inicial
        sr.sortingOrder = 100; // Fica na frente da maioria dos objetos
        
        // 3. APLICA TAMANHO CONFIGURADO
        projetil.transform.localScale = new Vector3(tamanhoProjetil, tamanhoProjetil, 1f);
        
        // 4. ADICIONA COMPONENTES FÍSICOS
        Rigidbody2D rb = projetil.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        CircleCollider2D collider = projetil.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.25f * tamanhoProjetil;
        
        // 5. ADICIONA SCRIPT DE COMPORTAMENTO
        projetil.AddComponent<ProjetilLavaSimples>();
        
        // 6. DESTRÓI APÓS 15 SEGUNDOS (segurança)
        Destroy(projetil, 15f);
        
        Debug.Log("✅ Projétil criado com sucesso!");
        Debug.Log($"   • Posição inicial: {projetil.transform.position}");
        Debug.Log($"   • Escala: {tamanhoProjetil}x");
        Debug.Log($"   • Cor: {sr.color}");
        
        return projetil;
    }
    
    // ======================================================
    // MÉTODOS AUXILIARES
    // ======================================================
    
    /// <summary>
    /// Restaura a cor normal do vulcão (para efeitos de piscar)
    /// </summary>
    void VoltarCorNormal()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.red;
    }
    
    /// <summary>
    /// Exibe informações atuais do vulcão no console
    /// </summary>
    public void ExibirStatus()
    {
        Debug.Log("=== STATUS DO VULCÃO ===");
        Debug.Log($"• Altura configurada: {alturaArco}m");
        Debug.Log($"• Velocidade: {velocidadeVoo}");
        Debug.Log($"• Tamanho projétil: {tamanhoProjetil}");
        Debug.Log($"• Alvo: {(alvoCidade != null ? alvoCidade.name : "NÃO CONFIGURADO")}");
        
        if (InfoGeral.Instance != null)
        {
            Debug.Log($"• Pressão: {InfoGeral.Instance.pressao:F1}/{InfoGeral.Instance.pressaoMaxima}");
            Debug.Log($"• Pode atirar: {InfoGeral.Instance.pressao >= InfoGeral.Instance.pressaoMaxima}");
        }
    }
}