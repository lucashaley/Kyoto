using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityAtoms.BaseAtoms;

public class Fadeable : MonoBehaviour
{
    public enum RenderMode { Opaque, Cutout, Fade, Transparent };

    public Renderer[] childrenRenderers;
    public bool isFaded;

    public FloatVariable fadeValue;

    void Awake()
    {
        childrenRenderers = GetComponentsInChildren<Renderer>();
    }

    void ToggleFade()
    {
        if (isFaded)
        {
            FadeIn();
        } else {
            FadeOut();
        }

        isFaded = !isFaded;
    }

    public void FadeOut(float duration = -1)
    {
        // fade out global shadows
        // Tween.Value(gameController.shadowDistance, gameController.shadowDistanceFaded, HandleShadowFade, gameController.shadowFadeTime, 0f);

        duration = duration<0 ? fadeValue.Value : duration;

        // go through each child renderer, and start the tween
        foreach (Renderer rend in childrenRenderers)
        {
            // then go through each material, as there may be more than one
            for (int i = 0; i < rend.materials.Length; i++)
            {
                // check what kind of material it is
                // we need to do different things to outlines and fills
                switch (rend.materials[i].shader.name)
                {
                    case string s when s.Contains("Standard"):
                        // switch to fade render mode
                        // we can do this here, as we loop, to save cycles
                        FlipRendermode(rend.materials[i]);

                        Color tempColor = rend.materials[i].GetColor("_Color");
                        tempColor.a = 0.0f;
                        Tween.Color (rend.materials[i], tempColor, duration, 0.0f);
                        break;

                    // case string s when s.Contains("Outline Fill"):
                    //     Tween.ShaderFloat (rend.materials[i], "_OutlineWidth", gameController.fadeableOutlineWidth, gameController.stateChangeTime, gameController.shadowFadeTime);
                    //     break;
                    //
                    // case string s when s.Contains("Universal"):
                    //     Debug.LogError("URP Material!", rend.materials[i]);
                    //     break;
                }
            }
        }
    }
    public void FadeIn(float duration = -1, float delay = 0f)
    {
        duration = duration<0 ? fadeValue.Value : duration;

        //go through each child renderer, and start the tween
        foreach (Renderer rend in childrenRenderers)
        {
            // then go through each material, as there may be more than one
            for (int i = 0; i < rend.materials.Length; i++)
            {
                // check what kind of material it is
                // we need to do different things to outlines and fills
                switch (rend.materials[i].shader.name)
                {
                    case string s when s.Contains("Standard"):
                        Color tempColor = rend.materials[i].GetColor("_Color");
                        tempColor.a = 1f;
                        Tween.Color (rend.materials[i],
                                     tempColor,
                                     duration,
                                     delay,
                                     null,
                                     Tween.LoopType.None,
                                     null,
                                     FlipRendermodeAll);
                        break;

                    // case string s when s.Contains("Outline Fill"):
                    //     Tween.ShaderFloat (rend.materials[i], "_OutlineWidth", 0f, gameController.stateChangeTime, 0f);
                    //     break;
                    //
                    // case string s when s.Contains("Universal"):
                    //     Debug.LogError("URP Material!", rend.materials[i]);
                    //     break;
                }
            }
        }

        // fade in global shadows
        // Tween.Value(gameController.shadowDistanceFaded, gameController.shadowDistance, HandleShadowFade, gameController.shadowFadeTime, gameController.stateChangeTime);
    }

    void FlipRendermodeAll()
    {
        foreach (Renderer rend in childrenRenderers)
        {
            for (int i = 0; i < rend.materials.Length; i++)
            {
                FlipRendermode(rend.materials[i]);
            }
        }
    }

    void FlipRendermode(Material mat)
    {
        if (mat.shader.name.Contains("Standard"))
        {
            int currentMode = (int)mat.GetFloat("_Mode");

            switch (currentMode)
            {
                case (int)RenderMode.Opaque:
                    // from https://forum.unity.com/threads/access-rendering-mode-var-on-standard-shader-via-scripting.287002/
                    mat.SetFloat("_Mode", (float)RenderMode.Fade);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;

                    break;

                case (int)RenderMode.Fade:
                    // from https://forum.unity.com/threads/access-rendering-mode-var-on-standard-shader-via-scripting.287002/
                    mat.SetFloat("_Mode", (float)RenderMode.Opaque);
                    // mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    // mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 1);
                    mat.EnableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = -1;

                    break;
            }
        }
    }

    void HandleShadowFade(float value)
    {
        QualitySettings.shadowDistance = value;
    }
}
