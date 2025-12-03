using UnityEngine;

public class Cidade : MonoBehaviour
{
    public double vidaAtual = 500;
    
    void Start()
    {
        Debug.Log($"🏙️ Cidade iniciada com {vidaAtual} de vida");
    }

    public void ReceberDano(double dano)
    {
        Debug.Log($"💥 Cidade recebeu {dano} de dano!");
        Debug.Log($"📉 Vida antes: {vidaAtual}");
        
        vidaAtual -= dano;
        
        Debug.Log($"📈 Vida depois: {vidaAtual}");
        
        if (vidaAtual <= 0)
        {
            Debug.Log("💀 CIDADE DESTRUÍDA!");
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
        
        // Feedback visual
        StartCoroutine(PiscarDano());
    }
    
    System.Collections.IEnumerator PiscarDano()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }
}