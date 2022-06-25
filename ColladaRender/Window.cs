using System.Drawing;
using ColladaRender.RenderEngine.Core.EncapsulatedTypes;
using ColladaRender.RenderEngine.Core;
using ColladaRender.RenderEngine.Core.RenderableObjects;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ColladaRender
{
    /// <summary>
    /// Extends OpenTK Gamewindow to perform a game loop with OpenGL context
    /// </summary>
    class Window : GameWindow
    {
        private Camera _camera;
        private Model _model;
        private Light _light;
        public Window(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
            GlobalInputManager.Init();
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            
            GL.Enable(EnableCap.DepthTest);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            TextureManager.LoadTexture("crate", "RenderEngine/res/dae/textures/T_Armour_Worn_Metal_BaseColorOpacity.png");

            _model = Model.Load(COLLADA.Load("RenderEngine/res/dae/Soi_Armour_A.dae"));
            _light = new Light(Vector3.One * 3);
            _camera = new Camera(Vector3.One, Size.X / (float)Size.Y);
            
            CursorGrabbed = true;
            
            GL.ClearColor(Color.FromArgb(240, 240, 240));

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //code using my objects
            _model.Render(_camera.ViewMatrix, _camera.ProjectionMatrix, _camera.position, _light);
            GL.Flush();
            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            var context = new GlobalInputManager.GlobalInputContext()
            {
                keyboard = KeyboardState,
                mouse = MouseState
            };
            GlobalInputManager.OnKeyDown(e, context);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            var context = new GlobalInputManager.GlobalInputContext()
            {
                keyboard = KeyboardState,
                mouse = MouseState
            };
            GlobalInputManager.OnKeyUp(e, context);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            var context = new GlobalInputManager.GlobalInputContext()
            {
                keyboard = KeyboardState,
                mouse = MouseState
            };
            GlobalInputManager.OnMouseDown(e, context);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            var context = new GlobalInputManager.GlobalInputContext()
            {
                keyboard = KeyboardState,
                mouse = MouseState
            };
            GlobalInputManager.OnMouseUp(e, context);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            var context = new GlobalInputManager.GlobalInputContext()
            {
                keyboard = KeyboardState,
                mouse = MouseState
            };
            GlobalInputManager.OnMouseMove(e, context);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var context = new GlobalInputManager.GlobalInputContext()
            {
                keyboard = KeyboardState,
                mouse = MouseState
            };
            GlobalInputManager.OnMouseWheel(e, context);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            var winArgs = new GlobalInputManager.WindowArgs
            {
                window_focused = IsFocused,
                cursor_grabbed = CursorGrabbed,
                delta_time = args.Time
            };
            GlobalInputManager.Instance.Update(winArgs);

            if (!IsFocused)
            {
                return;
            }

            var timeElapsed = (float)args.Time;
            var keyboard = KeyboardState;
            var mouse = MouseState;



            if (keyboard[Keys.Escape])
            {
                Close();
            }
            
            if (KeyboardState.IsKeyPressed(Keys.T))
            {
                if (_model.TextureName == TextureManager.Default)
                {
                    _model.TextureName = "crate";
                }
                else
                {
                    _model.TextureName = TextureManager.Default;
                }
            }

            if (KeyboardState.IsKeyPressed(Keys.F))
            {
                CursorGrabbed = false;
            }
            
            if (keyboard[Keys.L])
            {
                if (keyboard[Keys.Up])
                {
                    _light.Position += new Vector3(timeElapsed, 0.0f, 0.0f);
                } else if (keyboard[Keys.Down])
                {
                    _light.Position -= new Vector3(timeElapsed, 0.0f, 0.0f);
                } else if (keyboard[Keys.Right])
                {
                    _light.Position += new Vector3(0.0f, 0.0f, timeElapsed);
                } else if (keyboard[Keys.Left])
                {
                    _light.Position -= new Vector3(0.0f, 0.0f, timeElapsed);
                } else if (keyboard[Keys.Period])
                {
                    _light.Position += new Vector3(0.0f, timeElapsed, 0.0f);
                } else if (keyboard[Keys.Comma])
                {
                    _light.Position -= new Vector3(0.0f, timeElapsed, 0.0f);
                }

                if (keyboard[Keys.I])
                {
                    _light.Intensity += timeElapsed;
                } else if (keyboard[Keys.O])
                {
                    _light.Intensity -= timeElapsed;
                }
            }
            
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            
            GL.Viewport(0,0,Size.X, Size.Y);
            
            _camera.UpdateAspectRatio(Size.X, Size.Y);
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            _model = Model.Load(COLLADA.Load(e.FileNames[0]));
            base.OnFileDrop(e);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}