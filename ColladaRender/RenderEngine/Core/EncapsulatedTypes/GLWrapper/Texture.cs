using ImageMagick;
using Image = ImageMagick.MagickImage;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper
{
    /// <summary>
    /// Wrapper for OpenGL Texture functionality
    /// </summary>
    public class Texture
    {
        private int _id;

        /// <summary>
        /// Loads a texture into vram given its file path
        /// </summary>
        /// <param name="filepath">File path to load from</param>
        /// <param name="unit">Texture unit to create the texture on</param>
        public Texture(string filepath)
        {
            //Generate the internal id and bind the Texture2D context to it
            _id = GL.GenTexture();
            Use(TextureUnit.Texture0);

            //use a Bitmap image object that is loaded from the filepath; is disposed automatically
            using (var image = new Image(filepath))
            {
                //The image in OpenGL context has to be in UV space not SR, which is loaded from file
                image.Flip();
                //Rip actual image the data away from the image object
                var data = image.GetPixels();
                
                //Load the read image data into vram
                GL.TexImage2D(TextureTarget.Texture2D,
                    0, //number of mipmap levels in the image currently
                    PixelInternalFormat.Rgba, //number of color components
                    image.Width, //width
                    image.Height, //height
                    0, //border size
                    PixelFormat.Bgra, //format of the data loaded from the file
                    PixelType.UnsignedByte, //data type of pixel data
                    data.ToByteArray(0, 0, image.Width, image.Height, PixelMapping.RGBA) //pointer to the pixel data
                    );
            }
            
            //Set up filters for mipmapping
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            
            //Wrap the textures both ways for UV values beyond 1.0 if ever needed in this renderer
            //Now needed for viewing certain models that make use of UVs from beyond 0.0 <-> 1.0
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            
            //Generate the mipmap for this image, then unbind the Texture2D context
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            Unuse();
        }

        /// <summary>
        /// Binds the texture to the given texture unit 
        /// </summary>
        /// <param name="unit">TextureUnit to bind the texture to</param>
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            Bind();
        }
        
        /// <summary>
        /// Unbinds the texture from its texture unit
        /// </summary>
        public void Unuse()
        {
            Unbind();
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        /// <summary>
        /// Binds the texture to the current context
        /// </summary>
        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }

        /// <summary>
        /// Unbinds the texture from the current context
        /// </summary>
        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}