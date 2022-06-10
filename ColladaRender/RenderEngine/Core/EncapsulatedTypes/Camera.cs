using System;
using System.Numerics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Matrix4 = OpenTK.Mathematics.Matrix4;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes
{
    /// <summary>
    /// Describes a camera in 3-D space
    /// </summary>
    public class Camera
    {
        private Quaternion _direction = Quaternion.Identity;

        private Vector2 _clickedMousePos = Vector2.Zero;

        private float _fieldOfView = MathHelper.PiOver3;

        private bool _leftWasDown;
        
        /// <summary>
        /// FOV angle in degrees
        /// </summary>
        public float FOV
        {
            get
            {
                return MathHelper.RadiansToDegrees(_fieldOfView);
            }
            set
            {
                var degs = MathHelper.Clamp(value, 1.0f, 90.0f);
                _fieldOfView = MathHelper.DegreesToRadians(degs);
            }
        }

        private Vector3 _position;
        
        /// <summary>
        /// Position of the camera
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return _position;
            } 
            
        }
        private float _aspectRatio;

        private float _sensitivity = 0.2f;
        
        private float _cameraSpeed = 1.5f;

        private float _lengthFromCenter = 100.0f;

        private float _zoomPercentage = 1.0f;
        
        /// <summary>
        /// Movement speed of the camera
        /// </summary>
        public float Speed
        {
            get
            {
                return _cameraSpeed;
            }
            set
            {
                _cameraSpeed = value;
            }
        }
        
        /// <summary>
        /// View matrix of the camera
        /// </summary>
        /// <returns>Matrix representation of the eye direction</returns>
        public Matrix4 ViewMatrix {
            get
            {
                return Matrix4.LookAt(_position * (Vector3.One * (1.0f - _zoomPercentage) * _lengthFromCenter), Vector3.Zero, Vector3.UnitY);
            }
        }

        /// <summary>
        /// Projection matrix of the camera
        /// </summary>
        /// <returns>Matrix representation of the perspective FOV of the camera</returns>
        public Matrix4 ProjectionMatrix
        { 
            get {
                return Matrix4.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, 0.01f, 100.0f);
            }
        }

        /// <summary>
        /// Creates a camera view
        /// </summary>
        /// <param name="position">Starting position of the camera</param>
        /// <param name="aspectRatio">Aspect ratio of the screen</param>
        public Camera(Vector3 position, float aspectRatio)
        {
            _position = position;
            _aspectRatio = aspectRatio;
        }
        
        /// <summary>
        /// Update the eye direction
        /// </summary>
        /// <param name="mouseInput">Mouse state since last update</param>
        /// <param name="keyboardInput">Keyboard state since last update</param>
        /// <param name="deltaTime">Time elapsed since last update</param>
        public void Update(MouseState mouseInput, KeyboardState keyboardInput, float deltaTime)
        {
            
            if (keyboardInput[Keys.LeftAlt])
            {
                if (mouseInput[MouseButton.Left])
                {
                    if (!_leftWasDown)
                    {
                        _clickedMousePos = mouseInput.Position;
                        _leftWasDown = true;
                    } else if (_leftWasDown)
                    {
                        var dragDelta = _clickedMousePos - mouseInput.Position;
                        _clickedMousePos = mouseInput.Position;

                        dragDelta.X = -dragDelta.X;
                        var deltaDegrees = dragDelta * _sensitivity;

                        var q1 = Quaternion.FromAxisAngle(Vector3.UnitY, deltaDegrees.X * 0.01f);
                        var q2 = Quaternion.FromAxisAngle(Vector3.UnitX, deltaDegrees.Y * 0.01f);
                        var newDirection =  q1 * q2;
                        _position = Vector3.Transform(_position, newDirection);
                    }
                }
                else if (!mouseInput[MouseButton.Left])
                {
                    _clickedMousePos = Vector2.Zero;
                }
                
                if (mouseInput.ScrollDelta.Length > 0.0f)
                {
                    //needs to be changed to a point along the lookat vector
                    //FOV += mouseInput.ScrollDelta.Y;

                    _zoomPercentage = Math.Clamp(_zoomPercentage + 0.0001f * mouseInput.ScrollDelta.Y, 0.01f, 1.0f);
                    
                }
                
            }
            
        }

        /// <summary>
        /// Update the aspect ratio of the camera
        /// </summary>
        /// <param name="width">Screen width</param>
        /// <param name="height">Screen height</param>
        public void UpdateAspectRatio(int width, int height)
        {
            _aspectRatio = width / (float) height;
        }

        Vector3 GetSphericalCoordinates(Vector3 cartesian)
        {
            float r = cartesian.Length;
 
            float phi = MathF.Atan2(cartesian.Z / cartesian.X, cartesian.X);
            float theta = MathF.Acos(cartesian.Y / r);
 
            if (cartesian.X < 0)
                phi += MathF.PI;
 
            return new Vector3 (r, phi, theta);
        }
        
        Vector3 GetCartesianCoordinates(Vector3 spherical)
        {
            Vector3 ret = new Vector3 ();
 
            ret.X = spherical.X * MathF.Cos (spherical.Z) * MathF.Cos (spherical.Y);
            ret.Y = spherical.X * MathF.Sin (spherical.Z);
            ret.Z = spherical.X * MathF.Cos (spherical.Z) * MathF.Sin (spherical.Y);
 
            return ret;
        }
        
    }
}