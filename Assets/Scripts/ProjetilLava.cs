using UnityEngine;
using System.Collections; // ← ADICIONE ESTA LINHA!
using System.Collections.Generic; // ← (opcional)

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
    private float escalaOriginal;
    
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
        
        escalaOriginal = transform.localScale.x;
        
        Debug.Log($"🌋 LANÇAMENTO! Altura máxima: {alturaMaxima}m");
        
        AdicionarRastroLava();
        
        transform.position = startPos;
    }
    
    void AdicionarRastroLava()
    {
        trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 1.2f;
        trail.startWidth = 0.5f;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.minVertexDistance = 0.05f;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1, 0.9f, 0.3f, 1), 0f),
                new GradientColorKey(new Color(1, 0.5f, 0, 1), 0.4f),
                new GradientColorKey(new Color(0.8f, 0.2f, 0, 1), 0.8f),
                new GradientColorKey(new Color(0.4f, 0.1f, 0, 1), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.9f, 0.3f),
                new GradientAlphaKey(0.6f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = gradient;
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
            
            if (progresso < 0.99f)
            {
                float nextProgress = Mathf.Min(progresso + 0.01f, 1f);
                Vector3 nextPos = CalcularPosicao(nextProgress);
                
                Vector3 direcao = (nextPos - transform.position).normalized;
                if (direcao != Vector3.zero)
                {
                    float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
                    
                    if (progresso < 0.5f)
                        angulo -= 75;
                    else
                        angulo -= 105;
                    
                    transform.rotation = Quaternion.Lerp(
                        transform.rotation,
                        Quaternion.Euler(0, 0, angulo),
                        Time.deltaTime * 10f
                    );
                }
            }
            
            transform.Rotate(0, 0, 180 * Time.deltaTime);
            
            float normalizadoAltura = altura / alturaMaxima;
            Color corLava;
            
            if (normalizadoAltura > 0.7f)
                corLava = Color.Lerp(new Color(1, 0.6f, 0.2f, 1), new Color(1, 0.9f, 0.3f, 1), (normalizadoAltura - 0.7f) / 0.3f);
            else if (normalizadoAltura > 0.3f)
                corLava = new Color(1, 0.5f, 0.1f, 1);
            else
                corLava = Color.Lerp(new Color(0.8f, 0.2f, 0, 1), new Color(1, 0.3f, 0, 1), normalizadoAltura / 0.3f);
            
            GetComponent<SpriteRenderer>().color = corLava;
            
            if (progresso > 0.6f)
            {
                float quedaProgresso = (progresso - 0.6f) / 0.4f;
                float escala = escalaOriginal * (1 + quedaProgresso * 0.3f);
                transform.localScale = Vector3.one * escala;
            }
            
            if (progresso > 0.7f && altura < 1.5f)
            {
                Debug.Log("💥 IMPACTO IMINENTE!");
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
        
        if (other.GetComponent<Cidade>() != null && progresso > 0.6f)
        {
            Debug.Log($"🌋 IMPACTO NA CIDADE!");
            AtingirAlvo();
        }
    }
    
    void AtingirAlvo()
    {
        if (atingiu) return;
        atingiu = true;
        
        Debug.Log($"💣 IMPACTO! Altura máxima: {alturaMaxima}m");
        
        if (alvo != null)
        {
            Cidade cidade = alvo.GetComponent<Cidade>();
            if (cidade != null)
            {
                Debug.Log($"🏙️ Dano: {dano}");
                cidade.ReceberDano(dano);
                CriarOndaDeChoque();
            }
        }
        
        if (trail != null)
        {
            trail.time = 0.1f;
            trail.autodestruct = true;
        }
        
        CriarExplosaoLava();
        
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        
        Destroy(gameObject, 1f);
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
            StartCoroutine(CriarParticulaLava(i * 0.08f));
        }
        
        Camera.main.transform.position += new Vector3(
            Random.Range(-0.2f, 0.2f),
            Random.Range(-0.1f, 0.1f),
            0
        );
        Invoke("ResetarCamera", 0.08f);
    }
    
    void ResetarCamera()
    {
        if (Camera.main != null)
            Camera.main.transform.position = new Vector3(0, 0, -10);
    }
    
    // CORRIGIDO: Removido "System.Collections." do início
    IEnumerator AnimarOndaDeChoque(GameObject onda)
    {
        float tempo = 0f;
        float duracao = 0.7f;
        Vector3 escalaInicial = onda.transform.localScale;
        Vector3 escalaFinal = escalaInicial * 4f;
        Color corInicial = onda.GetComponent<SpriteRenderer>().color;
        Color corFinal = new Color(corInicial.r, corInicial.g, corInicial.b, 0);
        
        while (tempo < duracao)
        {
            float t = tempo / duracao;
            onda.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
            onda.GetComponent<SpriteRenderer>().color = Color.Lerp(corInicial, corFinal, t);
            
            tempo += Time.deltaTime;
            yield return null;
        }
    }
    
    // CORRIGIDO: Removido "System.Collections." do início
    IEnumerator CriarParticulaLava(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        GameObject particula = new GameObject("LavaParticula");
        particula.transform.position = transform.position + 
            new Vector3(Random.Range(-1.2f, 1.2f), Random.Range(-0.8f, 0.8f), 0);
        
        SpriteRenderer sr = particula.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
        
        sr.color = new Color(
            1,
            Random.Range(0.3f, 0.6f),
            Random.Range(0f, 0.2f),
            Random.Range(0.7f, 0.9f)
        );
        
        sr.sortingOrder = 104;
        
        float tamanho = Random.Range(0.3f, 1f);
        particula.transform.localScale = Vector3.one * tamanho;
        
        float tempo = 0f;
        float duracao = Random.Range(0.4f, 0.8f);
        Vector3 direcao = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.5f, 1f),
            0
        ).normalized;
        
        Vector3 escalaFinal = particula.transform.localScale * 0.3f;
        Color corFinal = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
        
        while (tempo < duracao)
        {
            float t = tempo / duracao;
            
            particula.transform.position += direcao * Time.deltaTime * 2f;
            
            particula.transform.localScale = Vector3.Lerp(
                particula.transform.localScale, 
                escalaFinal, 
                t * 0.5f
            );
            sr.color = Color.Lerp(sr.color, corFinal, t);
            
            tempo += Time.deltaTime;
            yield return null;
        }
        
        Destroy(particula);
    }
    
    void OnDestroy()
    {
        if (configurado)
            Debug.Log("🌋 Trajetória completa: Vulcão → Céu → Cidade");
    }
    
    void OnDrawGizmosSelected()
    {
        if (!configurado || alvo == null) return;
        
        Gizmos.color = new Color(1, 0.4f, 0, 0.5f);
        int segmentos = 40;
        
        for (int i = 0; i < segmentos; i++)
        {
            float t1 = i / (float)segmentos;
            float t2 = (i + 1) / (float)segmentos;
            
            Vector3 pos1 = CalcularPosicao(t1);
            Vector3 pos2 = CalcularPosicao(t2);
            
            Gizmos.DrawLine(pos1, pos2);
            
            if (Mathf.Abs(t1 - 0.5f) < 0.05f)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(pos1, 0.2f);
                Gizmos.color = new Color(1, 0.4f, 0, 0.5f);
            }
        }
        
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(startPos, alvo.position);
    }
}