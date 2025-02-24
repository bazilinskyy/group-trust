using UnityEngine;


// Initially this script was ment to be used to reset the position of the avatar in Unity to the marker location.
// However, I failed to get it working. To reset the participants, I would recalibrate the Xsens Link using MVN Analyze and after a successful calibration reset the position of the participant in space.


public class ParticipantReset : MonoBehaviour
{
    public GameObject Participant;
    public GameObject Marker;
    private float CorrectY;
    private float WrongY;


    // Use this for initialization
    private void Start()
    {
    }


    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            WrongY = Marker.transform.position.y;
            CorrectY = Participant.transform.position.y;
            Participant.transform.position = Marker.transform.position + new Vector3(0, -WrongY + CorrectY, 0);
        }
    }
}