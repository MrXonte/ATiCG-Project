using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    protected bool inUse = false;
    public void CardTrigger()
    {
        if (inUse)
            return;
        inUse = true;
        CardEffect();
    }

    virtual protected void CardEffect()
    {
        Debug.Log(gameObject.name + " used.");
        Destroy(gameObject);
        Destroy(this);
    }
}
