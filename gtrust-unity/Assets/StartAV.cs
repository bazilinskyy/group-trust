using UnityEngine;


public class StartAV : MonoBehaviour
{
    public bool InitiateAV = false;
    private float Timer1;


    // Start is called before the first frame update
    private void Start()
    {
    }


    // Update is called once per frame
    private void Update()
    {
        if (InitiateAV && Timer1 < 5f)
        {
            Timer1 += Time.deltaTime;
        }
        else if (InitiateAV && Timer1 > 5f)
        {
            InitiateAV = false;
            Timer1 = 0f;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Do nothing if trigger isn't enabled
        if (enabled == false)
        {
            return;
        }

        if (other.gameObject.CompareTag("ManualCar"))
        {
            InitiateAV = true;
        }
    }
}