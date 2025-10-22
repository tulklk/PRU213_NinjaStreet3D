using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Transform playerTransform;
    public float moveSpeed = 17f;
    public float roatationSpeed = 100f;
    public GameObject collectEffect;

    private CoinMove coinMoveScript;
    private bool isCollected = false;

    void Start()
    {
        coinMoveScript = GetComponent<CoinMove>();
        coinMoveScript.enabled = false;
        playerTransform = PlayerControllerSmooth.PlayerTransform;
    }

    void Update()
    {
        transform.Rotate(0, roatationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("CoinDetector"))
        {
            coinMoveScript.enabled = true;
            return;
        }

        if (other.CompareTag("Player") || other.CompareTag("PlayerBubble"))
        {
            AudioManager.instance.sfxSource.PlayOneShot(AudioManager.instance.coinCollect);
            HandleCollect();
        }
    }

    public void HandleCollect()
    {
        isCollected = true;

        if (collectEffect != null)
        {
            GameObject effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        GameManager.instance.scores++;
        GameManager.instance.AddCoin();

        Destroy(gameObject);
    }
}

