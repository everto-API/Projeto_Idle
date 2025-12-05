using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClickVulcao : MonoBehaviour
{
    [Header("Referências")]
    public TMP_Text dinheiroTexto;
    public GameObject indicadorPressaoCheia;
    public Transform alvoCidade; 
    
    [Header("Configuração do Projétil")]
    public GameObject projetilPrefab; 
    public float alturaArco = 4f;
    public float velocidadeVoo = 6f;
    public double danoBase = 1.0;

    [Header("Custo")]
    public float custoDoDisparo = 10f; // <--- NOVO: Quanto custa cada clique

    private double ultimaObsidiana = -1;

    void Update()
    {
        if (InfoGeral.Instance == null) return;

        // Atualiza texto de dinheiro
        if (InfoGeral.Instance.obsidiana != ultimaObsidiana && dinheiroTexto != null)
        {
            ultimaObsidiana = InfoGeral.Instance.obsidiana;
            dinheiroTexto.text = "Obsidian: " + ultimaObsidiana.ToString("F0");
        }
        
        // Atualiza indicador visual (Luzinha)
        if (indicadorPressaoCheia != null)
        {
            // MUDANÇA: Agora acende se tiver pressão para PELO MENOS UM TIRO
            bool temMunicao = InfoGeral.Instance.pressao >= custoDoDisparo;
            
            if (indicadorPressaoCheia.activeSelf != temMunicao) 
                indicadorPressaoCheia.SetActive(temMunicao);
        }
    }
    
    void OnMouseDown()
    {   
        DispararLava();
    }
    
    public void DispararLava()
    {
        if (InfoGeral.Instance == null || alvoCidade == null || projetilPrefab == null)
        {
            Debug.LogError("ERRO: Verifique configurações no Inspector!");
            return;
        }
        
        // 1. MUDANÇA: Verifica se tem o custo do disparo (10), não se está cheio
        if (InfoGeral.Instance.pressao < custoDoDisparo)
        {
            Debug.Log("Falta pressão para atirar! Precisa de: " + custoDoDisparo);
            return;
        }
        
        // Cria o projétil
        GameObject novoProjetil = Instantiate(projetilPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        
        // Configura
        ProjetilLavaSimples lavaScript = novoProjetil.GetComponent<ProjetilLavaSimples>();
        
        if (lavaScript != null)
        {
            double danoTotal = InfoGeral.Instance.dano * danoBase;
            lavaScript.Configurar(alvoCidade, danoTotal, velocidadeVoo, alturaArco);
        }
        
        // 2. MUDANÇA: Subtrai o custo (pagamento)
        InfoGeral.Instance.pressao -= custoDoDisparo;
        
        // Dica: Se quiser garantir que não fique negativo por erro de arredondamento:
        if (InfoGeral.Instance.pressao < 0) InfoGeral.Instance.pressao = 0;
    }
}