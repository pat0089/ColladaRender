using System;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes
{
    /// <summary>
    /// A class that holds the attribute data for a vertex
    /// </summary>
    public class Vertex : IEquatable<Vertex>
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 TexCoord { get; set; }
        public Vector3 Color { get; set; }
        
        public Vertex()
        {
            Position = Vector3.Zero;
            Normal = Vector3.Zero;
            TexCoord = Vector2.Zero;
            Color = Vector3.Zero;
        }
        public bool Equals(Vertex vertex)
        {
            return vertex.Normal == Normal &&
                   vertex.TexCoord == TexCoord &&
                   vertex.Color == Color;
        }
        
        public override bool Equals(Object obj)
        {
            return Equals(obj as Vertex);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Normal.GetHashCode() ^ TexCoord.GetHashCode() ^ Color.GetHashCode();
        }
    }
}