using OpenTK.Mathematics;
using System.Drawing;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes
{
    /// <summary>
    /// Describes a light in 3-D space
    /// </summary>
    public class Light
    {
        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; }
        public float Intensity { get; set; }

        public Light(Vector3 pos)
        {
            Position = pos;
            Color = Vector3.One;
            Intensity = 1.0f;
        }
    }
}