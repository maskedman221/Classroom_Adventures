using System.IO;
using UnityEngine;

public static class ImageSaver
{
    public static void SaveSpriteToFile(Sprite sprite, string fileName)
    {
        if (sprite == null) return;

        Texture2D tex = sprite.texture;
        byte[] bytes = tex.EncodeToPNG();  // convert texture to PNG

        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, bytes);

        Debug.Log("Saved image to: " + path);
    }
    public static Sprite LoadSpriteFromFile(string fileName)
    {
    string path = Path.Combine(Application.persistentDataPath, fileName);

    if (!File.Exists(path)) return null;

    byte[] bytes = File.ReadAllBytes(path);
    Texture2D tex = new Texture2D(2, 2);
    tex.LoadImage(bytes);

    return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}