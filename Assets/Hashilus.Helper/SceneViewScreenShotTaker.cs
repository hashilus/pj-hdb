#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hashilus
{
    public static class SceneViewScreenShotTaker
    {
        const string FolderName = "ScreenShots";
        const int Scale = 3;

        [MenuItem("Hashilus/Take ScreenShot %#&s")]
        static void Take()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
            {
                Debug.LogError("シーンビューがありません");
                return;
            }

            var sceneCamera = UnityEngine.Object.Instantiate(sceneView.camera);
            var originalClearFlag = sceneCamera.clearFlags;
            var originalTargetTexture = sceneCamera.targetTexture;

            var rt = new RenderTexture(sceneView.camera.pixelWidth * Scale, sceneView.camera.pixelHeight * Scale, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default)
            {
                antiAliasing = 8
            };

            var tex = new Texture2D(rt.width, rt.height);

            try
            {
                RenderTexture.active = rt;
                sceneCamera.clearFlags = CameraClearFlags.Nothing;
                sceneCamera.targetTexture = rt;
                sceneCamera.Render();
                tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                tex.Apply();

                if (!Directory.Exists(FolderName)) Directory.CreateDirectory(FolderName);
                var path = Path.Combine(FolderName, DateTime.Now.ToString("yyyyMMdd-HHmmss-ff") + ".png");
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Debug.Log(new FileInfo(path).FullName + "に書き出しました");
            }
            finally
            {
                sceneCamera.clearFlags = originalClearFlag;
                sceneCamera.targetTexture = originalTargetTexture;
                RenderTexture.active = null;
                if (sceneCamera != null) UnityEngine.Object.DestroyImmediate(sceneCamera.gameObject);
                if (rt != null) UnityEngine.Object.DestroyImmediate(rt);
                if (tex != null) UnityEngine.Object.DestroyImmediate(tex);
            }
        }
    }
}
#endif