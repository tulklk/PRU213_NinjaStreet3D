using UnityEngine;

public class NitroSelected : MonoBehaviour
{
    public static NitroSelected Instance { get; private set; }

    public int currentNitroIndex;
    public GameObject[] nitros;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (GameObject nitro in nitros)
        {
            if (nitro != null)
                nitro.SetActive(false);
        }
    }

    void Start()
    {
        currentNitroIndex = PlayerPrefs.GetInt("SelectedNitro", 0);
        HideAllNitro();
    }

    public void HideAllNitro()
    {
        foreach (GameObject nitro in nitros)
        {
            if (nitro != null)
                nitro.SetActive(false);
        }
    }

    public void ActivateCurrentNitro()
    {
        if (currentNitroIndex < nitros.Length && nitros[currentNitroIndex] != null)
        {
            nitros[currentNitroIndex].SetActive(true);
        }
    }

    public void DeactivateCurrentNitro()
    {
        if (currentNitroIndex < nitros.Length && nitros[currentNitroIndex] != null)
        {
            nitros[currentNitroIndex].SetActive(false);
        }
    }

    public GameObject GetCurrentNitro()
    {
        return (currentNitroIndex < nitros.Length) ? nitros[currentNitroIndex] : null;
    }
}
