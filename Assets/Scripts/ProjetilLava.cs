using UnityEngine;
using System.Collections;

public class ProjetilLavaSimples : MonoBehaviour
{
    private Transform alvo;
    private double dano;
    private float velocidade;
    private float alturaMaxima;
    private Vector3 startPos;
    private float progresso = 0f;
    private bool configurado = false;
    private TrailRenderer trail;
    private bool atingiu = false;
    private float tempoVoo = 0f;
    private float duracaoVoo = 4f;

    // Configura o projétil quando ele nasce
    public void Configurar(Transform novoAlvo, double novoDano, float novaVelocidade, float novaAlturaMaxima)
    {
        alvo = novoAlvo;
        dano = novoDano;
        velocidade = novaVelocidade;
        alturaMaxima = novaAlturaMaxima;
        startPos = transform.position;
        configurado = true;
        
        float distancia = Vector3.Distance(startPos, alvo.position);
        duracaoVoo = Mathf.Clamp(distancia / velocidade * 2f, 3f, 6f);

        trail = GetComponent<TrailRenderer>(); 
        if (trail == null) trail = gameObject.AddComponent<TrailRenderer>();
        
        transform.position = startPos;
    }

    void Update()
    {
        if (!configurado || alvo == null || atingiu) return;
        
        tempoVoo += Time.deltaTime;
        progresso = tempoVoo / duracaoVoo;
        
        if (progresso <= 1f)
        {
            Vector3 posHorizontal = Vector3.Lerp(startPos, alvo.position, progresso);
            float altura = alturaMaxima * Mathf.Sin(Mathf.PI * progresso);
            
            Vector3 posFinal = new Vector3(
                posHorizontal.x,
                startPos.y + altura,
                posHorizontal.z
            );
            
            transform.position = posFinal;
            
            // Rotação visual
            if (progresso < 0.99f)
            {
                float nextProgress = Mathf.Min(progresso + 0.01f, 1f);
                Vector3 nextPos = CalcularPosicao(nextProgress);
                Vector3 direcao = (nextPos - transform.position).normalized;
                
                if (direcao != Vector3.zero)
                {
                    float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
                    angulo -= 90; 
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angulo), Time.deltaTime * 10f);
                }
            }
            
            // Giro extra "caótico"
            transform.Rotate(0, 0, 200 * Time.deltaTime);
            
            // Controle de Cor (Lava esfriando/esquentando)
            float normalizadoAltura = altura / alturaMaxima;
            Color corLava;
            if (normalizadoAltura > 0.7f)
                corLava = Color.Lerp(new Color(1, 0.6f, 0.2f, 1), new Color(1, 0.9f, 0.3f, 1), (normalizadoAltura - 0.7f) / 0.3f);
            else if (normalizadoAltura > 0.3f)
                corLava = new Color(1, 0.5f, 0.1f, 1);
            else
                corLava = Color.Lerp(new Color(0.8f, 0.2f, 0, 1), new Color(1, 0.3f, 0, 1), normalizadoAltura / 0.3f);
            
            GetComponent<SpriteRenderer>().color = corLava;
            
            // Se estiver muito perto do final e baixo, força o impacto
            if (progresso > 0.9f && altura < 0.5f)
            {
                AtingirAlvo();
            }
        }
        else
        {
            AtingirAlvo();
        }
    }
    
    Vector3 CalcularPosicao(float p)
    {
        Vector3 posHorizontal = Vector3.Lerp(startPos, alvo.position, p);
        float altura = alturaMaxima * Mathf.Sin(Mathf.PI * p);
        return new Vector3(posHorizontal.x, startPos.y + altura, posHorizontal.z);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!configurado || atingiu) return;
        
        // Verifica colisão com a cidade
        if (other.GetComponent<Cidade>() != null && progresso > 0.5f)
        {
            AtingirAlvo();
        }
    }
    
    void AtingirAlvo()
    {
        if (atingiu) return;
        atingiu = true;
        
        if (alvo != null)
        {
            Cidade cidade = alvo.GetComponent<Cidade>();
            if (cidade != null)
            {
                // Aplica o dano na cidade
                cidade.ReceberDano(dano);
                
                // BLOCO if para adicionar a quantidade de moedas :D
                if (InfoGeral.Instance != null)
                {
                    InfoGeral.Instance.obsidiana += dano;
                }

                CriarOndaDeChoque();
            }
        }
        if (trail != null)
        {
            trail.time = 0.5f;
            trail.autodestruct = true;
            trail.transform.parent = null; 
        }
        
        CriarExplosaoLava();
        
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.1f);
    }
    
    void CriarOndaDeChoque()
    {
        GameObject onda = new GameObject("OndaDeChoque");
        onda.transform.position = transform.position;
        
        SpriteRenderer sr = onda.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
        sr.color = new Color(1, 0.7f, 0.2f, 0.7f);
        sr.sortingOrder = 103;
        
        onda.transform.localScale = Vector3.one * 0.5f;
        
        StartCoroutine(AnimarOndaDeChoque(onda));
        
        Destroy(onda, 0.8f);
    }
    
    void CriarExplosaoLava()
    {
        for (int i = 0; i < 6; i++)
        {
            StartCoroutine(CriarParticulaLava(i * 0.05f));
        }
    }

    IEnumerator AnimarOndaDeChoque(GameObject onda)
    {
        float tempo = 0f;
        float duracao = 0.7f;
        Vector3 escalaInicial = onda.transform.localScale;
        Vector3 escalaFinal = escalaInicial * 4f;
        
        SpriteRenderer sr = onda.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color corInicial = sr.color;
        Color corFinal = new Color(corInicial.r, corInicial.g, corInicial.b, 0);
        
        while (tempo < duracao && onda != null)
        {
            float t = tempo / duracao;
            onda.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
            if (sr != null) sr.color = Color.Lerp(corInicial, corFinal, t);
            
            tempo += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator CriarParticulaLava(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        GameObject particula = new GameObject("LavaParticula");
        particula.transform.position = transform.position + 
            new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        
        // Destruição garantida da partícula
        Destroy(particula, 1.0f); 

        SpriteRenderer sr = particula.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
        sr.color = new Color(1, Random.Range(0.3f, 0.6f), 0, 1);
        sr.sortingOrder = 104;
        
        particula.transform.localScale = Vector3.one * Random.Range(0.2f, 0.5f);
        
        float tempo = 0f;
        float duracao = 0.8f;
        Vector3 direcao = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), 0).normalized;
        
        while (tempo < duracao && particula != null)
        {
            particula.transform.position += direcao * Time.deltaTime * 3f;
            particula.transform.localScale = Vector3.Lerp(particula.transform.localScale, Vector3.zero, Time.deltaTime * 3f);
            tempo += Time.deltaTime;
            yield return null;
        }
    }
}