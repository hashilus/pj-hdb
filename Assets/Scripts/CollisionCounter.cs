using UnityEngine;

public class CollisionCounter : MonoBehaviour
{
    public TitleController titleController;

    private void OnCollisionEnter(Collision collision)
    {
        var bullet = collision.gameObject.GetComponent<Bullet>();
        if (!bullet) return;

        if (bullet.Shooter == PlayerID.Player1)
        {
            titleController.player1StartCount++;
        }
        else if (bullet.Shooter == PlayerID.Player2)
        {
            titleController.player2StartCount++;
        }
    }
}
