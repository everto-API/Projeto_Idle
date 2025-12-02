using UnityEngine;
using TMPro;

public class ClickVulcao : MonoBehaviour
{
    public TMP_Text dinheiroTexto;
    public InfoGeral info;

    public void Start()
    {
        info = new InfoGeral();
        info.Data();
    }
    public void Update()
    {
        dinheiroTexto.text = "Obsidian: " + info.obsidiana;
    }

    public void GeradorObsidian() {info.obsidiana += 1;}
}
