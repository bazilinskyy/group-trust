using System.Collections;
using UnityEngine;


public class BlinkWithFrontLights : CustomBehaviour
{
    private PlayerAvatar PlayerAvatar;


    public override void Init(AICar aiCar)
    {
        PlayerAvatar = aiCar.playerAvatar;
    }


    public override void Trigger(CustomBehaviourData data)
    {
        var blinkPatternData = (BlinkPatternData) data;

        if (blinkPatternData != null)
        {
            StartCoroutine(Blink(blinkPatternData.blinkPattern));
        }
    }


    private IEnumerator Blink(AnimationCurve blinkPattern)
    {
        float timer = 0;
        var blinkPatternDuration = blinkPattern.keys[blinkPattern.length - 1].time;

        while (timer < blinkPatternDuration)
        {
            PlayerAvatar.frontLights.SetActive(blinkPattern.Evaluate(timer) > 0);
            timer += Time.deltaTime;

            yield return null;
        }

        PlayerAvatar.frontLights.SetActive(false);
    }
}