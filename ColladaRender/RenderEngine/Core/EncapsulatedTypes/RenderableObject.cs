using ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes
{
    /// <summary>
    /// Contains the basic sub-objects and data needed to be a renderable object, as well as a couple of dynamic built in rendering functions
    /// </summary>
    public class RenderableObject
    {
        protected VertexArrayObject _vao;
        protected Shader _shader = DefaultResources.DefaultShader;
        public string TextureName = TextureManager.Default;
        protected Matrix4 _modelTransform = Matrix4.CreateTranslation(0,0,0);
        protected int _numIndices;

        /// <summary>
        /// The base rendering function for a 3-D object
        /// </summary>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        public void Render(Matrix4 view, Matrix4 projection)
        {
            _vao.BindAllAttribs();
            _shader.Activate();

            _shader.SetMat4("uModel", _modelTransform);
            _shader.SetMat4("uView", view);
            _shader.SetMat4("uProjection", projection);

            GL.DrawElements(PrimitiveType.Triangles, _numIndices, DrawElementsType.UnsignedInt, 0);
            _vao.UnbindAllAttribs();
        }
        
        /// <summary>
        /// Advanced rendering function that does Phong shading based on the Light and camera position parameters
        /// </summary>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        /// <param name="camPosition">Camera position</param>
        /// <param name="light">Light object</param>
        public void Render(Matrix4 view, Matrix4 projection, Vector3 camPosition, Light light)
        {
            TextureManager.SetTexture(TextureName);
            TextureManager.UseTexture(TextureUnit.Texture0);

            _vao.BindAllAttribs();
            _shader.Activate();

            _shader.SetMat4("uModel", _modelTransform);
            _shader.SetMat4("uView", view);
            _shader.SetMat4("uProjection",projection);

            if (_shader == DefaultResources.ModelShader)
            {
                _shader.SetVec3("uLightColor", light.Color);
                _shader.SetVec3("uLightPosition", light.Position);
                _shader.SetFloat("uLightIntensity", light.Intensity);
                _shader.SetVec3("uViewPos", camPosition);
            }

            GL.DrawElements(PrimitiveType.Triangles, _numIndices, DrawElementsType.UnsignedInt, 0);
            _vao.UnbindAllAttribs();
        }

        /// <summary>
        /// Sets the position of an object in 3-D space
        /// </summary>
        /// <param name="toSet">Position to set to</param>
        public void SetPosition(Vector3 toSet)
        {
            _modelTransform = Matrix4.CreateTranslation(toSet);
        }
        
        /// <summary>
        /// Translate the position of an object in 3-D space
        /// </summary>
        /// <param name="toTranslateBy">Relative position to translate by</param>
        public void Translate(Vector3 toTranslateBy)
        {
            _modelTransform += Matrix4.CreateTranslation(toTranslateBy);
        }
    }
}