using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper
{
    /// <summary>
    /// Buffer to hold element index data
    /// </summary>
    /// <remarks>Extends OpenGL wrapper functionality</remarks>
    public class ElementArrayBuffer : BufferObject<int>
    {
        /// <summary>
        /// Reserves a Element Buffer Object to hold the element indices in vram
        /// </summary>
        public ElementArrayBuffer() : base(BufferTarget.ElementArrayBuffer)
        {
        }
        
        /// <summary>
        /// Fills the ElementArrayBuffer with Vector2i data
        /// </summary>
        /// <param name="data">Array of Vector2i data</param>
        public void Fill(Vector2i[] data)
        {
            var newData = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                newData.Add(data[i].X);
                newData.Add(data[i].Y);
            }
            base.Fill(newData.ToArray());
        }
        
        /// <summary>
        /// Fills the ElementArrayBuffer with Vector3i data
        /// </summary>
        /// <param name="data">Array of Vector3i data</param>
        public void Fill(Vector3i[] data)
        {
            var newData = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                newData.Add(data[i].X);
                newData.Add(data[i].Y);
                newData.Add(data[i].Z);
            }
            base.Fill(newData.ToArray());
        }
        
        /// <summary>
        /// Fills the ElementArrayBuffer with Vector4i data
        /// </summary>
        /// <param name="data">Array of Vector4i data</param>
        public void Fill(Vector4i[] data)
        {
            var newData = new List<int>();
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