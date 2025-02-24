using UnityEngine;
using UnityEngine.UI;


// Initially this script was ment to obtain data from the participants performance (i.e., successful crossings without any crashes etc.) 
// This script was never used in the end.
public class PlayerController : MonoBehaviour
{
    public float speed;
    public Text countText;
    public Text winText;
    public GameObject HalfwayCrossing;
    public GameObject Marker;
    private float count;
    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";
    }


    private void FixedUpdate()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");

        var movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HalfwayCrossing"))
        {
            count = count + 1f;
            SetCountText();
        }
        else if (other.gameObject.CompareTag("Car"))
        {
            count = 0;
            SetCountText();
        }
    }


    private void SetCountText()
    {
        countText.text = "Count: " + count;
    }
}