using System;
using System.Collections;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(CollectPowerUp(collision.gameObject));
        }
    }

    private IEnumerator CollectPowerUp(GameObject player)
    {
        _audioSource.Play();
        player.GetComponent<PlayerController>().TakePowerUp(1);
        gameObject.GetComponent<SpriteRenderer>().sprite = null;

        yield return new WaitForSeconds(0.15f);
        
        Destroy(gameObject);
    }
}
