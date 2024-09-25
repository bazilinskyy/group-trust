using UnityEngine;


//allows controlling HMI state with keyboard buttons
public class ClientHMIController : MonoBehaviour
{
    [SerializeField] private bool m_useHMIS = false; // SOSXR

    private PlayerAvatar _avatar;
    private HMIManager _manager;


    private void Awake()
    {
        _avatar = GetComponent<PlayerAvatar>();
    }


    public void Init(HMIManager manager)
    {
        _manager = manager;
    }


    private void OnEnable()
    {
        ToggleHMIs();
    }


    /// <summary>
    ///     SOSXR: we only want to show the HMIs when the HMI is enabled, and we're not using the HMIs in this experiment
    /// </summary>
    private void ToggleHMIs()
    {
        if (_avatar.HMISlots.TopHMI == null || _avatar.HMISlots.HoodHMI == null || _avatar.HMISlots.WindshieldHMI == null)
        {
            enabled = m_useHMIS;
            return;
        }
        
        _avatar.HMISlots.TopHMI.gameObject.SetActive(m_useHMIS);
        _avatar.HMISlots.HoodHMI.gameObject.SetActive(m_useHMIS);
        _avatar.HMISlots.WindshieldHMI.gameObject.SetActive(m_useHMIS);
        enabled = m_useHMIS;
    }


    private void ChangeToState(HMIState state)
    {
        var hmis = _avatar.HMISlots;

        if (hmis.TopHMI != null)
        {
            _manager.RequestHMIChange(hmis.TopHMI, state);
        }

        if (hmis.HoodHMI != null)
        {
            _manager.RequestHMIChange(hmis.HoodHMI, state);
        }

        if (hmis.WindshieldHMI != null)
        {
            _manager.RequestHMIChange(hmis.WindshieldHMI, state);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeToState(HMIState.DISABLED);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeToState(HMIState.STOP);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeToState(HMIState.WALK);
        }
    }
}