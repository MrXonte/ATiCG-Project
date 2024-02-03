using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class CardTriggerField : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interactable" && other.GetComponent<Card>() != null)
        {
            if (other.GetComponent<CardOrange>() != null)
                SceneManager.LoadScene(0);
            else
                other.GetComponent<Card>().CardTrigger();
        }
    }
}
