using UnityEngine;

public class DeathController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameProgressController controller = new GameProgressController();
            controller = transform.parent.parent.GetChild(0).GetComponent<GameProgressController>();
            controller.OpenDeathUI();
        } 
    }

    private void Update()
    {
        if (gameObject.name == "Saw")
        {
            Vector3 rotation = new Vector3(0, 0, 1f);
            transform.Rotate(rotation);
        }
    }
}
