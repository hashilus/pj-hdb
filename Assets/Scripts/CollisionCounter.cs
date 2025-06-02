using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCounter : MonoBehaviour
{
    public TitleController titleController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.IndexOf("1P") > 0)
        {
            titleController.player1StartCount++;
        }
        else if(collision.gameObject.name.IndexOf("2P") > 0)
        {
            titleController.player2StartCount++;
        }
    }


}
