using UnityEngine;


//simple sprite swapping implementation of HMI base class
public class SpriteHMI : HMI
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite stop;
    [SerializeField] private Sprite walk;
    [SerializeField] private Sprite disabled;


    public override void Display(HMIState state)
    {
        base.Display(state);
        Sprite spr = null;

        switch (state)
        {
            case HMIState.STOP:
                spr = stop;

                break;
            case HMIState.WALK:
                spr = walk;

                break;
            default:
                spr = disabled;

                break;
        }

        _renderer.sprite = spr;
    }
}