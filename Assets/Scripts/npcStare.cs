using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcStare : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    float PlayerPositionX;
    GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPositionX = Player.transform.position.x;
        if (PlayerPositionX < this.transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else { spriteRenderer.flipX = false; }
    }
}
