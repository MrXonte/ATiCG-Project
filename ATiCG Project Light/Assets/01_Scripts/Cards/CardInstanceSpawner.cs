using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstanceSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> cards;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(cards[Random.Range(0, cards.Count)], transform.position, Quaternion.Euler(Random.value*360f, Random.value * 360f, Random.value * 360f));
        Destroy(gameObject);
    }
}
