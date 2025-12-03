using UnityEngine;

public class InfoGeral : MonoBehaviour
{
    public static InfoGeral Instance; 
    
    // === DADOS DO JOGO ===
    public double obsidiana = 0;
    public double pressao = 0;
    public double dano = 1.0;
    public double vida_da_ilha = 100;
    
    // === CONFIGURAÇÃO DA PRESSÃO ===
    public double pressaoMaxima = 10f; // Reduzido para encher rápido
    public float taxaGeracaoBase = 10f; // Aumentado para 2 por segundo  
    public float multiplicadorBuff = 1f;
    
    void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        float taxaReal = taxaGeracaoBase * multiplicadorBuff;

        if (pressao < pressaoMaxima)
        {
            pressao += taxaReal * Time.deltaTime; 
            
            if (pressao > pressaoMaxima)
            {
                pressao = pressaoMaxima;
            }
            
            // Log a cada segundo aproximadamente
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"🔋 Pressão: {pressao:F1}/{pressaoMaxima} (+{taxaReal}/s)");
            }
        }
        else if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"✅ PRESSÃO MÁXIMA ({pressaoMaxima}) ALCANÇADA! Pode disparar!");
        }
    }

    public void ComprarUpgradeTaxaPressao(float aumento)
    {
        multiplicadorBuff += aumento;
        Debug.Log("Buff comprado! Novo Multiplicador: " + multiplicadorBuff);
    }
    
    public void ResetData()
    {
        obsidiana = 0;
        pressao = 0; 
        multiplicadorBuff = 1f;
    }
}