using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.Rendering;

public class ScreenshotForPoster : MonoBehaviour
{
    private RenderTexture captureTexture;

    [SerializeField] private Camera captureCamera;

    private void Start()
    {
        captureTexture = new RenderTexture(2560, 1440, 32, RenderTextureFormat.ARGB32);
        captureTexture.Create();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string folderPath = Application.dataPath + "/Screenshots/";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = string.Format("/capture_{0}.png", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"));

            captureCamera.backgroundColor = new Color(0, 0, 0, 0);
            captureCamera.clearFlags = CameraClearFlags.SolidColor;

            RenderTexture.active = captureTexture;
            captureCamera.targetTexture = captureTexture;
            captureCamera.Render();

            Texture2D texture2D = new Texture2D(captureTexture.width, captureTexture.height, TextureFormat.RGBA32, false);
            texture2D.ReadPixels(new Rect(0, 0, captureTexture.width, captureTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;

            byte[] bytes = texture2D.EncodeToPNG();

            if (bytes != null)
            {
                File.WriteAllBytes(folderPath + fileName, bytes);
            }
        }
    }
}
