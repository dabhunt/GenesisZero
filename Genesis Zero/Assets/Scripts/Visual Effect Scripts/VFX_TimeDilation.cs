using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;
public class VFX_TimeDilation : MonoBehaviour
{
    public float distortionIntensity=.7f;
    public float vignetteIntensity=.5f;
    public void timeEffect(float duration)
    {
        //var postProcessLayer = gameObject.AddComponent<PostProcessLayer>();
        //var vignette = ScriptableObject.CreateInstance<Vignette>();
        //vignette.enabled.Override(true);
        //vignette.intensity.Override(vignetteIntensity);
        //var distortion = ScriptableObject.CreateInstance<LensDistortion>();
        //distortion.enabled.Override(true);
        //distortion.intensity.Override(distortionIntensity);
        ////pass in an array of chosen effects
        //print("vfx time effect running...");
        //PostProcessEffectSettings[] effects = {distortion, vignette};
        LensDistortion lens = ScriptableObject.CreateInstance<LensDistortion>();
        Vignette vig = ScriptableObject.CreateInstance<Vignette>();
        lens.intensity = new FloatParameter { value = 1 };
        vig.intensity = new FloatParameter { value = 1 };

        //DOTween.Sequence()
        //    .Append(DOTween.To(() => volume.weight, x => volume.weight = x, 1f, duration))
        //    .AppendInterval(1f)
        //    .Append(DOTween.To(() => volume.weight, x => volume.weight = x, 0f, duration))
        //    .OnComplete(() =>
        //    {
        //        RuntimeUtilities.DestroyVolume(volume, true, true);
        //        //Destroy(volume);
        //    });
    }
}
