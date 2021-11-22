using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper
{
    /// <summary>
    /// Wrapper for OpenGL BufferObjects
    /// </summary>
    /// <typeparam name="T">Basic data type to send to buffer</typeparam>
    /// <remarks>This buffer class is designed to be generic to allow future functionality to be written</remarks>
    public class BufferObject<T>  where T : struct
    {
        private int _id;
        private int _dataSize;
        private int _typeSize;
        private bool _isInstantiated;
        private BufferTarget _bufferType;

        /// <summary>
        /// Generates a BufferObject ID and sets the buffer type
        /// </summary>
        /// <param name="bufferType">Type of BufferObject</param>
        protected BufferObject(BufferTarget bufferType)
        {
            _id = GL.GenBuffer();
            _isInstantiated = false;
            _bufferType = bufferType;
        }

        /// <summary>
        /// Binds the BufferObject to the current context
        /// </summary>
        public void Bind()
        {
            GL.BindBuffer(_bufferType,_id);
        }

        /// <summary>
        /// Unbinds the BufferObject from the current context
        /// </summary>
        public void Unbind()
        {
            GL.BindBuffer(_bufferType, 0);
        }

        /// <summary>
        /// Deletes the BufferObject from the vram
        /// </summary>
        public void Delete()
        {
            if (_isInstantiated)
            {
                GL.DeleteBuffer(_id);
            }
        }
        
        /// <summary>
        /// Fills the BufferObject with named type data
        /// </summary>
        /// <param name="data">Array of named type data</param>
        public void Fill(T[] data)
        {
            _dataSize = data.Length;
            _typeSize = Marshal.SizeOf(typeof(T));
            GL.BufferData(_bufferType, new IntPtr(_dataSize * _typeSize), data, BufferUsageHint.StaticDraw);
            _isInstantiated = true;
        }
    }
}