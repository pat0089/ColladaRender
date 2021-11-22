using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes
{
    /// <summary>
    /// Describes a camera in 3-D space
    /// </summary>
    public class Camera
    {
        private static Vector3 Front = -Vector3.UnitZ;
        private static Vector3 Up = Vector3.UnitY;
        private static Vector3 Right = Vector3.UnitX;
        
        /// <summary>
        /// Direction for the camera to move
        /// </summary>
        public enum Direction
        {
            Up, Down, Left, Right, Forward, Backward
        }

        private float _pitch;
        
        /// <summary>
        /// Pitch in degrees
        /// </summary>
        public float Pitch {
            get
            {
                return MathHelper.RadiansToDegrees(_pitch);
            }
            set
            {
                var degs = MathHelper.Clamp(value, -89.9f, 89.9f);
                _pitch = MathHelper.DegreesToRadians(degs);
                UpdateViewVectors();
            }
        }
        
        private float _yaw = -MathHelper.PiOver2;
        
        /// <summary>
        /// Yaw in degrees
        /// </summary>
        public float Yaw
        {
            get
            {
                return MathHelper.RadiansToDegrees(_yaw);
            }
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateViewVectors();
            }
        }
        
        
        private float _fieldOfView = MathHelper.PiOver2;
        
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
                return Matrix4.LookAt(_position, _position + Front, Up);
            }
        }

        /// <summary>
        /// Projection matrix of the camera
        /// </summary>
        /// <returns>Matrix representation of the perspective FOV of the camera</returns>
        public Matrix4 ProjectionMatrix
        { 
            get {
                return Matrix4.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, 0.01f, 100f);
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
        /// Move the camera based on elapsed time
        /// </summary>
        /// <param name="toMove">Direction to move</param>
        /// <param name="elapsedTime">Time elapsed since last update</param>
        public void Move(Direction toMove, float elapsedTime)
        {
            switch (toMove)
            {
                case Direction.Up:
                    _position += Vector3.UnitY * _cameraSpeed * elapsedTime;
                    break;
                case Direction.Down:
                    _position += -Vector3.UnitY * _cameraSpeed * elapsedTime;
                    break;
                case Direction.Backward:
                    _position -= Front * Speed * elapsedTime;
                    break;
                case Direction.Forward:
                    _position += Front * Speed * elapsedTime;
                    break;
                case Direction.Right:
                    _position += Right * Speed * elapsedTime;
                    break;
                case Direction.Left:
                    _position -= Right * Speed * elapsedTime;
                    break;
            }
        }
        
        /// <summary>
        /// Update the eye direction
        /// </summary>
        /// <param name="mouseInput">Mouse state since last update</param>
        public void Update(MouseState mouseInput)
        {
            if (mouseInput.Delta.Length > 0.0f)
            {
                Yaw += mouseInput.Delta.X * _sensitivity;
                Pitch -= mouseInput.Delta.Y * _sensitivity;
            }

            if (mouseInput.ScrollDelta.Length > 0.0f)
            {
                FOV += mouseInput.ScrollDelta.Y;
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
        
        /// <summary>
        /// Update the internal eye view vectors
        /// </summary>
        public void UpdateViewVectors()
        {
            Front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            Front.Y = MathF.Sin(_pitch);
            Front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            Front.Normalize();

            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}