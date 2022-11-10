using System.Drawing;
using ColladaRender.RenderEngine.Core.EncapsulatedTypes;
using ColladaRender.RenderEngine.Core;
using ColladaRender.RenderEngine.Core.RenderableObjects;
using OpenTK.Graphics.OpenGL;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;
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

            //Press the escape key to close
            GlobalInputManager.RegisterKeyDown(
                (context, context2) => 
                    Close(),
                Keys.Escape
                );

            //Toggle the texture with the single white pixel
            GlobalInputManager.RegisterKeyDown(
                (context, context2) =>
                {
                    if (_model.TextureName == TextureManager.Default)
                    {
                        _model.TextureName = "skin";
                    }
                    else
                    {
                        _model.TextureName = TextureManager.Default;
                    }
                },
                Keys.T                
                );

            //Toggle cursor for file drop
            GlobalInputManager.RegisterKeyDown(
                (context, context2) =>
                {
                    if (CursorState == CursorState.Grabbed)
                    {
                        CursorState = CursorState.Normal;
                    }
                    else
                    {
                        CursorState = CursorState.Grabbed;
                    }
                }, 
                Keys.F
                );

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            TextureManager.LoadTexture("skin", "RenderEngine/res/dae/textures/T_Armour_Clean_Metal_BaseColorOpacity.png", TextureUnit.Texture0);
            TextureManager.LoadTexture("normals", "RenderEngine/res/dae/textures/T_Armour_Clean_Metal_NOpenGL.png", TextureUnit.Texture1);
            TextureManager.LoadTexture("metallic", "RenderEngine/res/dae/textures/T_Armour_Clean_Metal_Metallic.png", TextureUnit.Texture2);
            TextureManager.LoadTexture("roughness", "RenderEngine/res/dae/textures/T_Armour_Clean_Metal_Roughness.png", TextureUnit.Texture3);
            TextureManager.LoadTexture("ao", "RenderEngine/res/dae/textures/T_Armour_Clean_Metal_AO.png", TextureUnit.Texture4);

            _model = Model.Load(COLLADA.Load("RenderEngine/res/dae/Soi_Armour_A.dae"));
            //_model.Rotate(Quaternion.FromAxisAngle(Vector3.UnitX, 90f));
            _light = new Light(Vector3.UnitY - Vector3.UnitZ);
            _camera = new Camera(Vector3.One - Vector3.UnitX, Size.X / (float)Size.Y);

            CursorState = CursorState.Grabbed;

            GL.ClearColor(Color.FromArgb(0, 0, 0));

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //code using my objects
            _model.Render(_camera.ViewMatrix, _camera.ProjectionMatrix, _camera._position, _light);
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
                cursor_grabbed = CursorState,
                delta_time = args.Time
            };

            if (!IsFocused)
            {
                return;
            }

            GlobalInputManager.Instance.Update(winArgs);
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