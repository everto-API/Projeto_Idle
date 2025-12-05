using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BotaoUpgrade : MonoBehaviour
{
    public enum TipoUpgrade
    {
        AumentarDano,
        AumentarTanquePressao,
        AumentarVelocidadeRecarga
    }

    [Header("Configuração")]
    public TipoUpgrade tipoDeMelhoria;
    public string nomeDoUpgrade = "Nome do Upgrade";
    
    [Header("Economia")]
    public double custoAtual = 10;
    public float multiplicadorCusto = 1.5f; // O preço aumenta 50% a cada compra
    public double poderDoUpgrade = 1.0;

    [Header("Referências de UI")]
    public TMP_Text textoNome;
    public TMP_Text textoCusto;
    public TMP_Text textoNivel;
    public Button botao;

    private int nivelAtual = 0;

    void Start()
    {
        AtualizarTextoUI();
    }

    void Update()
    {
        if (InfoGeral.Instance == null || botao == null) return;

        // Se não tem dinheiro, o botão fica cinza
        bool temDinheiro = InfoGeral.Instance.obsidiana >= custoAtual;
        botao.interactable = temDinheiro;
    }

    public void Comprar()
    {
        if (InfoGeral.Instance == null) return;
        
        // 1. Verifica se tem dinheiro
        if (InfoGeral.Instance.obsidiana < custoAtual) return;

        // 2. Paga o custo
        InfoGeral.Instance.obsidiana -= custoAtual;

        // 3. Aplica o efeito dependendo do tipo que você escolheu no Inspector
        switch (tipoDeMelhoria)
        {
            case TipoUpgrade.AumentarDano:
                InfoGeral.Instance.dano += poderDoUpgrade;
                Debug.Log($"🔥 Dano subiu! Agora é: {InfoGeral.Instance.dano}");
                break;

            case TipoUpgrade.AumentarTanquePressao:
                InfoGeral.Instance.pressaoMaxima += poderDoUpgrade;
                Debug.Log($"🔋 Tanque aumentou! Max: {InfoGeral.Instance.pressaoMaxima}");
                break;

            case TipoUpgrade.AumentarVelocidadeRecarga:
                InfoGeral.Instance.taxaGeracaoBase += (float)poderDoUpgrade;
                Debug.Log($"⚡ Recarga acelerou! Taxa: {InfoGeral.Instance.taxaGeracaoBase}");
                break;
        }

        // 4. Aumenta o preço e o nível
        custoAtual = custoAtual * multiplicadorCusto;
        nivelAtual++;

        // 5. Atualiza o texto do botão
        AtualizarTextoUI();
    }

    void AtualizarTextoUI()
    {
        if (textoNome) textoNome.text = nomeDoUpgrade;
        if (textoCusto) textoCusto.text = "Custo: " + custoAtual.ToString("F0");
        if (textoNivel) textoNivel.text = "Lv. " + nivelAtual;
    }
}