using System.Collections.Generic;
using ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper;
using OpenTK.Graphics.OpenGL4;

namespace ColladaRender.RenderEngine.Core
{
    public sealed class TextureManager
    {
        public static string Default;
        private static string CurrentTexture { get; set; }
        private static Dictionary<string, Texture> Textures { get; } = new();
        
        static TextureManager()
        {
            Default = "Default";
            LoadTexture(Default, "RenderEngine/res/img/pixel.bmp");
        }

        public static void LoadTexture(string name, string filepath)
        {
            Textures.Add(name, new Texture(filepath));
            SetTexture(name);
            UseTexture(TextureUnit.Texture0);
        }

        public static void UseTexture(TextureUnit textureUnit)
        {
            Textures[CurrentTexture].Use(textureUnit);
        }

        public static void SetTexture(string name)
        { 
            CurrentTexture = name;
        }
    }
    
    
}