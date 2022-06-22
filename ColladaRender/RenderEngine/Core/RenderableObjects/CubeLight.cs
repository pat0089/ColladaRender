using ColladaRender.RenderEngine.Core.EncapsulatedTypes;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.RenderableObjects
{
    
    /// <summary>
    /// Testing a combined cube and light object
    /// </summary>
    public class CubeLight: SceneObject
    {
        private Cube _cube;
        private Light _light;
        
        public CubeLight()
        {
            _cube = Cube.Generate(false, 0.1f, DefaultResources.LightShader);
            _light = new Light(Vector3.One * 3);
            _cube.SetPosition(_light.Position);
        }
        
        public CubeLight(Vector3 pos)
        {
            _cube = Cube.Generate(false, 0.1f, DefaultResources.LightShader);
            _light = new Light(pos);
            _cube.SetPosition(_light.Position);
        }

        public void Render(Matrix4 view, Matrix4 projection, Vector3 camPosition)
        {
            _cube.Render(view, projection, camPosition, _light);
        }

        public Light Light
        {
            get
            {
                return _light;
            }
        }
    }
}