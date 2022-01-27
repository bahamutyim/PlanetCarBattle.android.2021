using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace com.ahyim.planet
{
    public class Tool
    {
        public static string ListValueToString<T>(List<T> list)
        {
            string returnStr = "";
            if (list != null) { 
                if (list.Count == 0)
                {
                    return "[empty List]";
                }
                else
                {
                    foreach (T obj in list)
                    {
                        returnStr += obj.ToString() + ",";
                    }
                    return "[" + returnStr.Substring(0, returnStr.Length - 1) + "]";
                }

                
            }
            else
            {
                return "[null List]";
            }


        }
        public static Texture2D LoadImage(string textureName)
        {
            string texturePath = string.Format("{0}/tempImage/{1}.png", Application.persistentDataPath, textureName);
            Debug.LogFormat("texturePath:{0}", texturePath);
            Texture2D image = Resources.Load<Texture2D>(texturePath);
            return image;
        }

        public static Texture2D RTImage(Camera cam, string textureName)
        {
            string texturePath = string.Format("{0}/tempImage/{1}.png", Application.persistentDataPath, textureName);

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = cam.targetTexture;
            cam.Render();
            Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
            image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            image.Apply();
            byte[] bytes = image.EncodeToPNG();
            Directory.CreateDirectory(Path.GetDirectoryName(texturePath));

            File.WriteAllBytes(texturePath, bytes);
            
            RenderTexture.active = currentRT;
            return image;
        }
    }
}
