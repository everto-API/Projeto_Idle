using UnityEngine;
using TMPro; // Necessário para mexer com textos
using UnityEngine.UI;

public class GerenciadorUI : MonoBehaviour
{
    [Header("Arraste os Textos Aqui")]
    public TMP_Text textoObsidiana;
    public TMP_Text textoPressao;
    public TMP_Text textoGeracao;

    void Update()
    {
        // Segurança: Se o InfoGeral ainda não carregou, não faz nada
        if (InfoGeral.Instance == null) return;

        // 1. Atualiza a Obsidiana (Dinheiro)
        if (textoObsidiana != null)
        {
            textoObsidiana.text = "Obsidiana: " + InfoGeral.Instance.obsidiana.ToString("F0");
        }

        // 2. Atualiza a Pressão (Atual / Máxima)
        if (textoPressao != null)
        {
            string atual = InfoGeral.Instance.pressao.ToString("F1");
            string maxima = InfoGeral.Instance.pressaoMaxima.ToString("F0");
            
            textoPressao.text = $"Pressão: {atual} / {maxima}";
            
            // Muda a cor do texto se estiver cheio
            if (InfoGeral.Instance.pressao >= InfoGeral.Instance.pressaoMaxima)
                textoPressao.color = Color.red; // Cheio
            else
                textoPressao.color = Color.white; // Enchendo
        }

        // 3. Atualiza a Geração (Quanto ganha por segundo)
        if (textoGeracao != null)
        {
            textoGeracao.text = "+" + InfoGeral.Instance.taxaGeracaoBase.ToString("F1") + " Pressão/s";
        }
    }
}