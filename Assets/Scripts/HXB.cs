using System;
using System.IO;
using UnityEngine;

public static class HXB
{
    public static HaxbotData LoadHaxbot(GameObject bot, string who, string mainTexture = "_BaseMap")
    {
        string o;
        string[] grab = new string[2];

        // 
        using (StreamReader sR = new StreamReader(Path.Combine(Application.dataPath, "Bots", $"{who}", $"{who}.hxb")))
        {
            grab = sR.ReadToEnd().Split('\n');
            bot.name = grab[0];
            o = grab[1];
        }

        Texture2D t2d = Base64ToTexture2D(o);

        return new HaxbotData(grab[0], grab[1], t2d);
    }

    public static void SaveHXB(string loc, string filename, Texture2D txt)
    {
        string b64 = Texture2DToBase64(txt);

        using (StreamWriter sW = new StreamWriter(Path.Combine(loc, $"{filename}.hxb")))
        {
            sW.Write($"{filename}\n{b64}");
        }
    }

    public static string Texture2DToBase64(Texture2D texture)
    {
        byte[] imageData = texture.EncodeToPNG();
        return Convert.ToBase64String(imageData);
    }

    public static Texture2D Base64ToTexture2D(string encodedData)
    {
        byte[] imageData = Convert.FromBase64String(encodedData);

        int width, height;
        GetImageSize(imageData, out width, out height);

        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.filterMode = FilterMode.Point;
        texture.LoadImage(imageData);

        return texture;
    }

    static void GetImageSize(byte[] imageData, out int width, out int height)
    {
        width = ReadInt(imageData, 3 + 15);
        height = ReadInt(imageData, 3 + 15 + 2 + 2);
    }

    static int ReadInt(byte[] imageData, int offset)
    {
        return (imageData[offset] << 8) | imageData[offset + 1];
    }

    public static Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}

public struct HaxbotData
{
    string _name;
    public string name => _name;

    string _base64;
    public string base64 => _base64;

    Texture2D _txt2d;
    public Texture2D txt2d => _txt2d;

    public HaxbotData(string n, string b64, Texture2D txt2d)
    {
        _name = n;
        _base64 = b64;
        _txt2d = txt2d;
    }
}