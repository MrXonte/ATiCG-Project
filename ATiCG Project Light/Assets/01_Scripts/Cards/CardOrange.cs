using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardOrange : Card
{
    override
        protected void CardEffect()
    {
        TeleportPlayer();
        base.CardEffect();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.tag == "Floor")
            CardTrigger();
    }

    void TeleportPlayer()
    {

        GameObject.FindGameObjectWithTag("Player").transform.position = transform.position;
    }
}
