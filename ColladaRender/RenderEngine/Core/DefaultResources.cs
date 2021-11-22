using ColladaRender.RenderEngine.Core.EncapsulatedTypes;
using ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper;

namespace ColladaRender.RenderEngine.Core
{
    /// <summary>
    /// The default resources of the rendering engine
    /// </summary>
    public static class DefaultResources
    { 
        public static Shader ModelShader { get; }
        public static Shader LightShader { get; }
        public static Shader DefaultShader { get; }

        static DefaultResources()
        {
            DefaultShader = new Shader("RenderEngine/res/GLSLShaders/defaultShader.vert",
                "RenderEngine/res/GLSLShaders/defaultShader.frag");
            
            LightShader = new Shader("RenderEngine/res/GLSLShaders/lightShader.vert",
                "RenderEngine/res/GLSLShaders/lightShader.frag");
            
            ModelShader =  new Shader("RenderEngine/res/GLSLShaders/modelShader.vert",
                "RenderEngine/res/GLSLShaders/modelShader.frag");
        }
    }
}