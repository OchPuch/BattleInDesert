using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Texture2DHelper
    {
        public static Texture2D ChangeFormat(Texture2D oldTexture, TextureFormat newFormat)
        {
            //Create new empty Texture
            Texture2D newTex = new Texture2D(oldTexture.width, oldTexture.height, newFormat, false);
            //Copy old texture pixels into new one
            newTex.SetPixels(oldTexture.GetPixels());
            //Apply
            newTex.Apply();

            return newTex;
        }


        public static Texture2D CropTexture(Texture2D oldTexture, int startX, int startY, int endX, int endY)
        {
            //Create new empty Texture
            Texture2D newTex = new Texture2D(endX - startX, endY - startY, oldTexture.format, false);
            //Copy old texture pixels into new one
            newTex.SetPixels(oldTexture.GetPixels(startX, startY, endX - startX, endY - startY));
            //Apply
            newTex.Apply();

            return newTex;
        }

        public static Texture2D MakeTextureFitGrid(Texture2D oldTexture, int gridSizeX, int gridSizeY)
        {
            var gridWidth = oldTexture.width / gridSizeX;
            var gridHeight = oldTexture.height / gridSizeY;
            var newWidth = gridWidth * gridSizeX;
            var newHeight = gridHeight * gridSizeY;

            return CropTexture(oldTexture, 0, 0, newWidth, newHeight);
        }


        public static Texture2D CompressToFitBounds(Texture2D oldTexture, int boundX, int boundY, int gridSizeX,
            int gridSizeY)
        {
            var gridWidth = oldTexture.width / gridSizeX;
            var gridHeight = oldTexture.height / gridSizeY;
            //if grid size is bigger than bounds then we need to compress texture to fit bounds
            if (gridWidth > boundX || gridHeight > boundY)
            {
                //if grid width is bigger than grid height then we need to compress texture by width
                if (gridWidth > gridHeight)
                {
                    var newWidth = boundX * gridSizeX;
                    var newHeight = (int)(oldTexture.height * ((float)newWidth / oldTexture.width));
                    //Create new empty Texture
                    Texture2D newTex = new Texture2D(newWidth, newHeight, oldTexture.format, false);
                    //Copy avg color of old texture pixels into new one
                    for (int i = 0; i < newWidth; i++)
                    {
                        for (int j = 0; j < newHeight; j++)
                        {
                            var avgColor = Color.black;
                            var pixelsCount = 0;
                            for (int k = 0; k < oldTexture.width / newWidth; k++)
                            {
                                for (int l = 0; l < oldTexture.height / newHeight; l++)
                                {
                                    avgColor += oldTexture.GetPixel(i * oldTexture.width / newWidth + k,
                                        j * oldTexture.height / newHeight + l);
                                    pixelsCount++;
                                }
                            }

                            avgColor /= pixelsCount;
                            newTex.SetPixel(i, j, avgColor);
                        }
                    }

                    //Apply
                    newTex.Apply();

                    return newTex;
                }
                //if grid height is bigger than grid width then we need to compress texture by height
                else
                {
                    var newHeight = boundY * gridSizeY;
                    var newWidth = (int)(oldTexture.width * ((float)newHeight / oldTexture.height));
                    //Create new empty Texture
                    Texture2D newTex = new Texture2D(newWidth, newHeight, oldTexture.format, false);
                    //Copy avg color of old texture pixels into new one
                    for (int i = 0; i < newWidth; i++)
                    {
                        for (int j = 0; j < newHeight; j++)
                        {
                            var avgColor = Color.black;
                            var pixelsCount = 0;
                            for (int k = 0; k < oldTexture.width / newWidth; k++)
                            {
                                for (int l = 0; l < oldTexture.height / newHeight; l++)
                                {
                                    avgColor += oldTexture.GetPixel(i * oldTexture.width / newWidth + k,
                                        j * oldTexture.height / newHeight + l);
                                    pixelsCount++;
                                }
                            }

                            avgColor /= pixelsCount;
                            newTex.SetPixel(i, j, avgColor);
                        }
                    }

                    //Apply
                    newTex.Apply();

                    return newTex;
                }
            }
            else
            {
                return oldTexture;
            }
        }

        public static Texture2D MakeTextureAppropriate(Texture2D oldTexture, int boundX, int boundY, int gridSizeX,
            int gridSizeY, TextureFormat format)
        {
            var newTexture = ChangeFormat(oldTexture, format);
            newTexture = CompressToFitBounds(newTexture, boundX, boundY, gridSizeX, gridSizeY);
            newTexture = MakeTextureFitGrid(newTexture, gridSizeX, gridSizeY);
            return newTexture;
        }


        public static bool CompareLandScapeTexturesData(byte[] tex1, byte[] tex2)
        {
            if (tex1.Length != tex2.Length)
                return false;

            for (int i = 0; i < tex1.Length; i++)
            {
                if (tex1[i] != tex2[i])
                    return false;
            }

            return true;
        }

        public static int AverageDifferenceBetweenTextures(byte[] tex1, byte[] tex2)
        {
            if (tex1.Length != tex2.Length)
            {
                //Errorlog
                Debug.LogError("Textures are not the same size");

                return -1;
            }


            Color color1 = AverageColorWithoutOutliers(tex1);
            Color color2 = AverageColorWithoutOutliers(tex2);

            return AverageDifferenceBetweenColors(color1, color2);
        }

        public static int AverageDifferenceBetweenColors(Color color1, Color color2)
        {
            return (int)(Mathf.Abs(color1.r - color2.r) + Mathf.Abs(color1.g - color2.g) +
                         Mathf.Abs(color1.b - color2.b));
        }

        public static Color AverageColor(byte[] tex)
        {
            Color color = Color.black;

            int r, g, b;
            r = g = b = 0;
            for (int i = 0; i < tex.Length; i += 3)
            {
                r += tex[i];
                g += tex[i + 1];
                b += tex[i + 2];
            }

            color.r = r / (tex.Length / 3);
            color.g = g / (tex.Length / 3);
            color.b = b / (tex.Length / 3);

            return color;
        }

        public static Color AverageColorWithoutOutliers(byte[] tex)
        {
            const float k = 1.5f;

            Color color = Color.black;

            List<byte> r, g, b;
            r = new List<byte>();
            g = new List<byte>();
            b = new List<byte>();

            for (int i = 0; i < tex.Length - 2; i += 3)
            {
                r.Add(tex[i]);
                g.Add(tex[i + 1]);
                b.Add(tex[i + 2]);
            }

            r.Sort();
            g.Sort();
            b.Sort();
            
            //Get Q1 and Q3
            float Q1R, Q3R, Q1G, Q3G, Q1B, Q3B;
            Q1R = GetQ1(r);
            Q3R = GetQ3(r);
            Q1G = GetQ1(g);
            Q3G = GetQ3(g);
            Q1B = GetQ1(b);
            Q3B = GetQ3(b);

            //Get IQR
            float IQR, IQRG, IQRB;
            IQR = Q3R - Q1R;
            IQRG = Q3G - Q1G;
            IQRB = Q3B - Q1B;

            //Get lower and upper bounds
            float lowerBoundR, upperBoundR, lowerBoundG, upperBoundG, lowerBoundB, upperBoundB;
            lowerBoundR = Q1R - k * IQR;
            upperBoundR = Q3R + k * IQR;
            lowerBoundG = Q1G - k * IQRG;
            upperBoundG = Q3G + k * IQRG;
            lowerBoundB = Q1B - k * IQRB;
            upperBoundB = Q3B + k * IQRB;

            //Remove outliers
            for (int i = 0; i < r.Count; i++)
            {
                if (r[i] < lowerBoundR || r[i] > upperBoundR)
                {
                    r.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < g.Count; i++)
            {
                if (g[i] < lowerBoundG || g[i] > upperBoundG)
                {
                    g.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < b.Count; i++)
            {
                if (b[i] < lowerBoundB || b[i] > upperBoundB)
                {
                    b.RemoveAt(i);
                    i--;
                }
            }


            //Get average
            float avgR, avgG, avgB;
            avgR = avgG = avgB = 0;

            for (int i = 0; i < r.Count; i++)
            {
                avgR += r[i];
            }

            for (int i = 0; i < g.Count; i++)
            {
                avgG += g[i];
            }

            for (int i = 0; i < b.Count; i++)
            {
                avgB += b[i];
            }

            avgR /= r.Count;
            avgG /= g.Count;
            avgB /= b.Count;

            color.r = avgR;
            color.g = avgG;
            color.b = avgB;

            return color;
        }

        private static float GetQ1(List<byte> bytes)
        {
            return bytes[bytes.Count / 4];
        }

        private static float GetQ3(List<byte> bytes)
        {
            return bytes[bytes.Count * 3 / 4];
        }
        
        public static Texture2D LoadImage(string path)
        {
            //How do i load image from path and import it to unity?
            var texture = new Texture2D(2, 2);
            texture.LoadImage(System.IO.File.ReadAllBytes(path));
            
            //Make filter mode point to avoid blurring
            texture.filterMode = FilterMode.Point;
            //Make Wrap mode clamp to avoid texture bleeding
            texture.wrapMode = TextureWrapMode.Clamp;
            //Make alphha source none to avoid alpha bleeding
            return texture;

        }
        
    }
    
    
}