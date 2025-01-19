using System.Collections;
using ClassicUs.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace ClassicUs;

public static class ClassicEffects
{
    public static IEnumerator FlashCoroutine(Color color, float duration = 1f, float waitfor = 0f, float alpha = 0.3f)
    {
        yield return new WaitForSeconds(waitfor);

        color.a = alpha;
        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            fullscreen.enabled = true;
            fullscreen.gameObject.active = true;
            fullscreen.color = color;
        }

        yield return new WaitForSeconds(duration);

        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            if (fullscreen.color.Equals(color))
            {
                fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                fullscreen.enabled = false;
            }
        }
    }

    public static IEnumerator TextChangeCoroutine(TextMeshPro tmp, string text = null, Color? color = null, float waitfor = 1f)
    {
        var originalText = tmp.text; 
        var originalColor = tmp.color;
        if (text != null) tmp.text = text;
        tmp.color = color.HasValue ? color.Value : originalColor;

        yield return new WaitForSeconds(waitfor);

        tmp.text = originalText;
        tmp.color = originalColor;
    }

    public static IEnumerator ArrowCoroutine(Vector3? position = null, float duration = 1f, Color? color = null, float waitfor = 0f)
    {
        yield return new WaitForSeconds(waitfor);

        var gameObj = new GameObject();
        var arrow = gameObj.AddComponent<ArrowBehaviour>();
        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
        var renderer = gameObj.AddComponent<SpriteRenderer>();
        renderer.sprite = ResourceLoader.LoadSprite("ClassicUs.Assets.Arrow.png");
        arrow.image = renderer;
        gameObj.layer = 5;
        arrow.target = position.HasValue ? position.Value : Vector3.zero;

        yield return new WaitForSeconds(duration);
        gameObj.Destroy();
    }
}