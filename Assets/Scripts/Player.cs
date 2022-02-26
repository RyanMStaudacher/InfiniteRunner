using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float jumpHeight = 4.0f;

    private float hMove;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();

        if(Input.GetButtonDown("Jump"))
        {
            PlayerJump();
        }
    }

    private void MovePlayer()
    {
        hMove = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        this.transform.Translate(hMove, 0, 0, Space.World);
    }

    private void PlayerJump()
    {
        this.transform.Translate(0, jumpHeight, 0, Space.World);
    }
}
