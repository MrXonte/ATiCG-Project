using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    Transform player;
    Transform t;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        t = transform;
    }

    private void Update()
    {
        if ((t.position - player.position).magnitude < 1.5f)
            Win();
    }

    void Win()
    {
        GameState.mazeSize += 5;
        SceneManager.LoadScene(0);
    }
}
