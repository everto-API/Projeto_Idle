using UnityEngine;
using System;
using System.Globalization;

public class ProgressoOffline : MonoBehaviour
{
    public static ProgressoOffline Instance;

    public const string LastQuitKey = "Ultima Saida";
    private const double TempoLimiteReconstrucaoSegundos = 1800.0; // 30 minutos offline → cidade reconstrói 

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
    void Start()
    {
        CalcularProgressoOffline();
    }

    void OnApplicationQuit()
    {
        SalvarHoraSaida();
    }

    void OnApplicationPause(bool pausou)
    {
        if (pausou)
            SalvarHoraSaida();
    }
    void SalvarHoraSaida()
    {
        string horaAtualDoSave = DateTime.Now.ToString("o", CultureInfo.InvariantCulture);
        PlayerPrefs.SetString(LastQuitKey, horaAtualDoSave);
        PlayerPrefs.Save();

        Debug.Log("💾 Hora de saída salva: " + horaAtualDoSave);
    }

    void CalcularProgressoOffline()
    {
        if (!PlayerPrefs.HasKey(LastQuitKey))
        {
            Debug.Log("Jogando pela primeira vez sem progresso offline.");
            return;
        }

        string ultimoString = PlayerPrefs.GetString(LastQuitKey);

        DateTime ultimaDataSalva = DateTime.Parse(
            ultimoString,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind
        );

        DateTime agora = DateTime.Now;

        double segundosOffline = (agora - ultimaDataSalva).TotalSeconds;

        Debug.Log($"⏱ Tempo offline: {segundosOffline:F0} segundos");

        AplicarEfeitosOffline(segundosOffline);
    }
    void AplicarEfeitosOffline(double segundos)
    {
        InfoGeral info = InfoGeral.Instance;

        if (info == null)
        {
            Debug.LogError("InfoGeral não encontrado na cena!");
            return;
        }

        Cidade cidade = FindAnyObjectByType<Cidade>();
        if (cidade == null)
        {
            Debug.LogError("Cidade não encontrada na cena!");
            return;
        }
    
        const double VidaTotalCidade = 500.0;

        if (segundos > TempoLimiteReconstrucaoSegundos)
        {
            cidade.vidaAtual = VidaTotalCidade; 
            cidade.GetComponent<SpriteRenderer>().color = Color.white;

            Debug.Log("Cidade se reconstruiu enquanto você estava fora! Vida restaurada.");
        }
        else
        {
            double minutosOffline = segundos / 60.0;
            double danoOffline = minutosOffline * info.dano;

            cidade.ReceberDano(danoOffline);
        }

        Debug.Log($"❤️ Vida final da cidade após cálculo offline: {cidade.vidaAtual:F1}");
    }
}