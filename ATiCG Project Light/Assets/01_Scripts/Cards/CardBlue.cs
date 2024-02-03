using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBlue : Card
{
    [SerializeField] GameObject toSpawn;

    override
        protected void CardEffect()
    {
        Instantiate(toSpawn, new Vector3(transform.position.x, -0.499f, transform.position.z), Quaternion.identity);
        base.CardEffect();
    }
}
