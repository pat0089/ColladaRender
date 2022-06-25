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
    public class Camera: SceneObject
    {

        private Vector2 _clickedMousePos = Vector2.Zero;

        private float _fieldOfView = MathHelper.PiOver3;
        
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
        
        /// <summary>
        /// Position of the camera
        /// </summary>

        private float _aspectRatio;

        private float _sensitivity = 0.2f;
        
        private float _cameraSpeed = 1.5f;

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
                return Matrix4.LookAt(position * (Vector3.One * _zoomPercentage), Vector3.Zero, Vector3.UnitY);
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
            this.position = position;
            _aspectRatio = aspectRatio;
            GlobalInputManager.RegisterMouseScroll(Zoom);
            GlobalInputManager.RegisterButtonUp(ResetMovement, MouseButton.Left);
            GlobalInputManager.RegisterButtonDown(StartMoveCamera, MouseButton.Left);
            GlobalInputManager.RegisterButtonHeld(MoveCamera, MouseButton.Left);
        }

        public void Zoom(GlobalInputManager.WindowArgs w, GlobalInputManager.GlobalInputContext ctx)
        {
            _zoomPercentage = Math.Clamp(_zoomPercentage + -0.1f * ctx.mouse.ScrollDelta.Y, 0.01f, 1.0f);
        }

        public void ResetMovement(GlobalInputManager.WindowArgs w, GlobalInputManager.GlobalInputContext ctx)
        {
            _clickedMousePos = Vector2.Zero;
        }

        public void StartMoveCamera(GlobalInputManager.WindowArgs w, GlobalInputManager.GlobalInputContext ctx)
        {
            _clickedMousePos = ctx.mouse.Position;
        }

        public void MoveCamera(GlobalInputManager.WindowArgs w, GlobalInputManager.GlobalInputContext ctx)
        {
            var dragDelta = _clickedMousePos - ctx.mouse.Position;
            _clickedMousePos = ctx.mouse.Position;

            var deltaDegrees = dragDelta * _sensitivity; 

            var q1 = Quaternion.FromAxisAngle(Vector3.UnitY, deltaDegrees.X * 0.01f);
            var q2 = Quaternion.FromAxisAngle(Vector3.UnitX, deltaDegrees.Y * 0.01f);
            var newDirection = q1 * q2;
            position = Vector3.Transform(position, newDirection);
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
        
    }
}