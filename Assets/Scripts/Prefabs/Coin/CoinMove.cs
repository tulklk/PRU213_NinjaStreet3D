using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMove : MonoBehaviour
{
    Coin coinScript;

    void Start()
    {
        coinScript = GetComponent<Coin>();
    }

    void Update()
    {
        if (coinScript == null || coinScript.playerTransform == null) return;

        transform.position = Vector3.MoveTowards(transform.position, coinScript.playerTransform.position, coinScript.moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBubble"))
        {
            coinScript.HandleCollect();
        }
    }
}

