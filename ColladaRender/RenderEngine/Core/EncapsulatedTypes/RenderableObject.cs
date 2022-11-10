using ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes
{
    /// <summary>
    /// Contains the basic sub-objects and data needed to be a renderable object, as well as a couple of dynamic built in rendering functions
    /// </summary>
    public class RenderableObject: SceneObject
    {
        protected VertexArrayObject _vao;
        protected Shader _shader = DefaultResources.DefaultShader;
        public string TextureName = TextureManager.Default;
        protected int _numIndices;

        public void Render()
        {
            _vao.BindAllAttribs();
        }


        /// <summary>
        /// The base rendering function for a 3-D object
        /// </summary>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        public void Render(Matrix4 view, Matrix4 projection)
        {
            _vao.BindAllAttribs();
            _shader.Activate();

            _shader.SetMat4("uModel", Matrix4.CreateTranslation(_position) * Matrix4.CreateScale(_scale) * Matrix4.CreateFromQuaternion(_rotation));
            _shader.SetMat4("uView", view);
            _shader.SetMat4("uProjection", projection);

            GL.DrawElements(PrimitiveType.Triangles, _numIndices, DrawElementsType.UnsignedInt, 0);
            _vao.UnbindAllAttribs();
        }
        
        /// <summary>
        /// Advanced rendering function that does Phong shading based on the Light and camera _position parameters
        /// </summary>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        /// <param name="camPosition">Camera _position</param>
        /// <param name="light">Light object</param>
        public void Render(Matrix4 view, Matrix4 projection, Vector3 camPosition, Light light)
        {
            TextureManager.SetTexture(TextureName);
            TextureManager.UseTexture(TextureUnit.Texture0);

            _vao.BindAllAttribs();
            _shader.Activate();

            _shader.SetMat4("uModel", Matrix4.CreateTranslation(_position) * Matrix4.CreateScale(_scale) * Matrix4.CreateFromQuaternion(_rotation));
            _shader.SetMat4("uView", view);
            _shader.SetMat4("uProjection",projection);

            if (_shader == DefaultResources.ModelShader || _shader == DefaultResources.PBRShader)
            {
                _shader.SetVec3("uLightColor", light.Color);
                _shader.SetVec3("uLightPosition", light.Position);
                if (_shader == DefaultResources.ModelShader) _shader.SetFloat("uLightIntensity", light.Intensity);
                _shader.SetVec3("uViewPos", camPosition);
            }

            GL.DrawElements(PrimitiveType.Triangles, _numIndices, DrawElementsType.UnsignedInt, 0);
            _vao.UnbindAllAttribs();
        }
    }
}