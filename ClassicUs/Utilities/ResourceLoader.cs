using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClassicUs.Utilities;

public static class ResourceLoader
{
    private static DLoadImage _iCallLoadImage;

    public static Sprite LoadSprite(string path, float scale = 1f)
    {
        var pixelsPerUnit = 100f * scale;
        var pivot = new Vector2(0.5f, 0.5f);

        Texture2D tex = CreateEmptyTexture();
        var assembly = Assembly.GetExecutingAssembly();
        var imageStream = assembly.GetManifestResourceStream(path);
        var img = imageStream.ReadFully();
        LoadImage(tex, img, true);
        tex.DontDestroy();
        var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
        sprite.DontDestroy();
        sprite.name = Path.GetFileNameWithoutExtension(path);
        return sprite;
    }

    public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
    {
        _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
        var il2CPPArray = (Il2CppStructArray<byte>)data;
        _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
    }

    private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    public static Texture2D CreateEmptyTexture(int width = 0, int height = 0)
    {
        return new Texture2D(width, height, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero);
    }

    public static string LoadJson(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        using (var stream = assembly.GetManifestResourceStream(path))
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            string jsonString = reader.ReadToEnd();
            return jsonString;
        }
    }
}