using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper
{
    /// <summary>
    /// Wrapper for GLSL shader functionality
    /// </summary>
    public class Shader
    {
        private int _id;

        private Dictionary<string, int> _uniformsDictionary;

        /// <summary>
        /// Vertex and Fragment shader program creator
        /// </summary>
        /// <param name="vertShaderPath">Path to the vertex shader</param>
        /// <param name="fragShaderPath">Path to the fragment shader</param>
        public Shader(string vertShaderPath, string fragShaderPath)
        {
            //read in both shader files and compile them, then create a shader program
            var shaderSource = File.ReadAllText(vertShaderPath);
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSource);
            CompileShader(vertexShader);

            shaderSource = File.ReadAllText(fragShaderPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);

            _id = GL.CreateProgram();
            
            //attach & link the shaders to the shader program 
            GL.AttachShader(_id, vertexShader);
            GL.AttachShader(_id, fragmentShader);
            LinkProgram(_id);

            //cache the uniform locations in the dictionary
            GL.GetProgram(_id, GetProgramParameterName.ActiveUniforms, out var _numUniforms);
            _uniformsDictionary = new Dictionary<string, int>();
            for (var i = 0; i < _numUniforms; i++)
            {
                var _key = GL.GetActiveUniform(_id, i, out _, out _);
                var _location = GL.GetUniformLocation(_id, _key);
                _uniformsDictionary.Add(_key, _location);
            }

        }
        
        /// <summary>
        /// Link the shader program
        /// </summary>
        /// <param name="program">Program ID to link</param>
        /// <exception cref="Exception">Throws the error code from linking</exception>
        private void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int) All.True)
            {
                throw new Exception($"Error occurred while linking ({program}): code {code}");
            }
        }

        /// <summary>
        /// Compiles the shader program
        /// </summary>
        /// <param name="shader">Program ID to compile</param>
        /// <exception cref="Exception">Throws the error from compiling</exception>
        private void CompileShader(int shader)
        {
            //attempt to compile shader
            GL.CompileShader(shader);
            
            //check for errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int) All.True)
            {
                //output errors
                var info = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occured while compiling shader (ID: {shader})\n\nLog: {info}");
            }
        }
        
        /// <summary>
        /// Gets the attribute number of a named attribute
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <returns>The attribute number</returns>
        public int GetAttributeLocation(string name)
        {
            return GL.GetAttribLocation(_id, name);
        }

        /// <summary>
        /// Activates the shader program
        /// </summary>
        public void Activate()
        {
            GL.UseProgram(_id);
        }

        /// <summary>
        /// Sets a named uniform integer value
        /// </summary>
        /// <param name="name">Name of the uniform</param>
        /// <param name="data">Value to set</param>
        public void SetInt(string name, int data)
        {
            GL.UseProgram(_id);
            GL.Uniform1(_uniformsDictionary[name], data);
        }
        
        /// <summary>
        /// Sets a named uniform floating-point value
        /// </summary>
        /// <param name="name">Name of the uniform</param>
        /// <param name="data">Value to set</param>
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(_id);
            GL.Uniform1(_uniformsDictionary[name], data);
        }
        
        /// <summary>
        /// Sets a named uniform Vector3 value
        /// </summary>
        /// <param name="name">Name of the uniform</param>
        /// <param name="data">Value to set</param>
        public void SetVec3(string name, Vector3 data)
        {
            GL.UseProgram(_id);
            GL.Uniform3(_uniformsDictionary[name], data);
        }
        
        /// <summary>
        /// Sets a named uniform Matrix4 value
        /// </summary>
        /// <param name="name">Name of the uniform</param>
        /// <param name="data">Value to set</param>
        public void SetMat4(string name, Matrix4 data)
        {
            GL.UseProgram(_id);
            GL.UniformMatrix4(_uniformsDictionary[name], true, ref data);
        }
    }
}