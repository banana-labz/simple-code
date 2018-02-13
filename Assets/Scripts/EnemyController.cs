using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject AdditionalAdvice;
    public GameObject enemy;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && enemy != null && enemy.activeInHierarchy)
        {
            GameProgressController.ableToBattle = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameProgressController.ableToBattle = false;
        }
    }

    private void Update()
    {
        if (GameProgressController.ableToBattle && enemy != null && enemy.activeInHierarchy)
        {
            AdditionalAdvice.SetActive(true);
        }
        else
        {
            AdditionalAdvice.SetActive(false);
        }
    }

}
