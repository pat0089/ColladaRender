using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper
{
    /// <summary>
    /// Buffer to hold element attribute data
    /// </summary>
    /// <remarks>Extends OpenGL wrapper functionality</remarks>
    public class VertexBufferObject : BufferObject<float>
    {
        /// <summary>
        /// Reserves a Vertex Buffer Object in vram to hold attribute data
        /// </summary>
        public VertexBufferObject() : base(BufferTarget.ArrayBuffer)
        {
        }
        
        /// <summary>
        /// Fills the VertexBufferObject with Vector2 data
        /// </summary>
        /// <param name="data">Array of Vector2 data</param>
        public void Fill(Vector2[] data)
        {
            List<float> newData = new List<float>();
            for (int i = 0; i < data.Length; i++)
            {
                newData.Add(data[i].X);
                newData.Add(data[i].Y);
            }
            base.Fill(newData.ToArray());
        }
        
        /// <summary>
        /// Fills the VertexBufferObject with Vector3 data
        /// </summary>
        /// <param name="data">Array of Vector3 data</param>
        public void Fill(Vector3[] data)
        {
            List<float> newData = new List<float>();
            for (int i = 0; i < data.Length; i++)
            {
                newData.Add(data[i].X);
                newData.Add(data[i].Y);
                newData.Add(data[i].Z);
            }
            base.Fill(newData.ToArray());
        }


        /// <summary>
        /// Fills the VertexBufferObject with Vector4 data
        /// </summary>
        /// <param name="data">Array of Vector4 data</param>
        public void Fill(Vector4[] data)
        {
            List<float> newData = new List<float>();
            for (int i = 0; i < data.Length; i++)
            {
                newData.Add(data[i].X);
                newData.Add(data[i].Y);
                newData.Add(data[i].Z);
                newData.Add(data[i].W);
            }

            base.Fill(newData.ToArray());
        }
    }
}