using UnityEngine;


//Animator based HMI implementation
public class AnimatorHMI : HMI
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Animator animator;

    //Animator triggers
    [SerializeField] private string stopTrigger = "stop";
    [SerializeField] private string walkTrigger = "walk";
    [SerializeField] private string disabledTrigger = "disabled";

    //texture to be set on certain state changes
    [SerializeField] private Texture2D stop;
    [SerializeField] private Texture2D walk;
    [SerializeField] private Texture2D disabled;
    private Material material;


    private void Awake()
    {
        material = meshRenderer.material;
    }


    public override void Display(HMIState state)
    {
        base.Display(state);

        switch (state)
        {
            case HMIState.STOP:
                material.mainTexture = stop;
                animator.SetTrigger(stopTrigger);

                break;
            case HMIState.WALK:
                material.mainTexture = walk;
                animator.SetTrigger(walkTrigger);

                break;
            default:
                material.mainTexture = disabled;
                animator.SetTrigger(disabledTrigger);

                break;
        }
    }
}