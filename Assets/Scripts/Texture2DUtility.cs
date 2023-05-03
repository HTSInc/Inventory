using TMPro;
using UnityEngine;

public class StringTexturePair
{
    public string Text { get; set; }
    public Texture2D Icon { get; set; }

    public StringTexturePair(string text, Texture2D icon)
    {
        Text = text;
        Icon = icon;
    }
}

public static class Texture2DUtility
{

    public static Texture2D CombineTextures(Texture2D baseTexture, Texture2D overlayTexture, int overlayX, int overlayY)
    {
        Texture2D combinedTexture = new Texture2D(baseTexture.width, baseTexture.height);
        for (int y = 0; y < baseTexture.height; y++)
        {
            for (int x = 0; x < baseTexture.width; x++)
            {
                Color baseColor = baseTexture.GetPixel(x, y);
                Color overlayColor = Color.clear;

                int overlayPixelX = x - overlayX;
                int overlayPixelY = y - overlayY;

                if (overlayPixelX >= 0 && overlayPixelX < overlayTexture.width && overlayPixelY >= 0 && overlayPixelY < overlayTexture.height)
                {
                    overlayColor = overlayTexture.GetPixel(overlayPixelX, overlayPixelY);
                }

                Color finalColor = Color.Lerp(baseColor, overlayColor, overlayColor.a);
                combinedTexture.SetPixel(x, y, finalColor);
            }
        }

        combinedTexture.Apply();
        return combinedTexture;
    }

    public static void AddTexture(Texture2D baseTexture, Texture2D overlayTexture, Vector2 position)
    {
        int overlayX = (int)position.x;
        int overlayY = (int)position.y;
        Texture2D combinedTexture = CombineTextures(baseTexture, overlayTexture, overlayX, overlayY);
        baseTexture.SetPixels(combinedTexture.GetPixels());
        baseTexture.Apply();
    }

    public static Texture2D ResizeTexture(Texture2D texture, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;
        RenderTexture.active = rt;

        Graphics.Blit(texture, rt);

        Texture2D resizedTexture = new Texture2D(newWidth, newHeight);
        resizedTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resizedTexture.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return resizedTexture;
    }

    public static void RenderText(TextMeshPro text, Vector2Int size, out Texture2D texture)
    {
        Camera camera = Camera.main;
        RenderTexture renderTexture = RenderTexture.GetTemporary(size.x, size.y);
        camera.targetTexture = renderTexture;

        text.canvasRenderer.SetMaterial(text.fontSharedMaterial, null);
        text.canvasRenderer.SetTexture(text.fontSharedMaterial.mainTexture);
        text.canvasRenderer.SetMesh(text.mesh);

        camera.Render();

        RenderTexture.active = renderTexture;
        texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        camera.targetTexture = null;
        RenderTexture.ReleaseTemporary(renderTexture);
    }
}
