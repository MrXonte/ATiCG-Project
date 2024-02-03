using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardWhite : Card
{
    Transform player;
    Transform t;
    bool armed = false;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        t = transform;
    }


    override
        protected void CardEffect()
    {
        TeleportPlayer();
        base.CardEffect();
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Floor")
    //        CardTrigger();
    //}

    void TeleportPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = transform.position;
    }

    void Update()
    {
        if (!armed)
        {
            if ((t.position - player.position).magnitude < 3)
                armed = true;
            return;
        }

        if ((t.position - player.position).magnitude > 5)
            CardTrigger();
    }
}
