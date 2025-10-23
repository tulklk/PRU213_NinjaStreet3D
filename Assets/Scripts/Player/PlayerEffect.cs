using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    public static PlayerEffect Instance { get; private set; }
    [Header("Spark Effect")]
    public GameObject sparkEffectLeft;
    public GameObject sparkEffectRight;
    public GameObject driftSmoke;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

}
