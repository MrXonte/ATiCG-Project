using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDispenser : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    GameObject card;

    private void Update()
    {
        if(card == null)
            CreateCard();
    }

    void CreateCard()
    {
        card = Instantiate(cardPrefab, transform);
        card.transform.rotation = Quaternion.Euler(90, 0, 0);
        card.transform.localPosition = Vector3.zero;
    }
}
