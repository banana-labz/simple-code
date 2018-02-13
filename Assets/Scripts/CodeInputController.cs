using UnityEngine;

public class CodeInputController : MonoBehaviour
{
    private GameObject AdditionalAdvice;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameProgressController.ableToSubmit = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameProgressController.ableToSubmit = false;
        }
    }

    private void Awake()
    {
        GameProgressController.ableToSubmit = false;
        AdditionalAdvice = GameProgressController.GetLastChild(GameProgressController.GetLastChild(transform.parent)).gameObject;
        AdditionalAdvice.SetActive(false);
    }
    private void Update()
    {
        if (GameProgressController.ableToSubmit)
        {
            AdditionalAdvice.SetActive(true);
        }
        else
        {
            AdditionalAdvice.SetActive(false);
        }
    }

}
