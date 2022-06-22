using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColladaRender.RenderEngine.Core
{

    /// <summary>
    /// Input class to register functions to keys and buttons
    /// 
    /// this will also eventually hold data for how long buttons are held
    /// </summary>
    public class GlobalInputManager
    {
        #region Variables and Initialization
        public struct WindowArgs
        {
            public bool window_focused { get; set; }
            
            public bool cursor_grabbed { get; set; }

            public double delta_time { get; set; }
        }

        public struct GlobalInputContext
        {
            public MouseState mouse { get; set; }
            public KeyboardState keyboard { get; set; }
        }

        private GlobalInputContext _globalInputContext = new GlobalInputContext();
        private WindowArgs _windowArgs = new WindowArgs();

        public delegate void RegisteredFunction(WindowArgs args, GlobalInputContext context);

        private Dictionary<Keys, HashSet<RegisteredFunction>> RegisteredKeyUpFunctions = null;
        private Dictionary<Keys, HashSet<RegisteredFunction>> RegisteredKeyDownFunctions = null;
        private Dictionary<Keys, HashSet<RegisteredFunction>> RegisteredKeyHeldFunctions = null;

        private Dictionary<Keys, double> _lengthKeysHeld = new Dictionary<Keys, double>();
        private Dictionary<Keys, bool> _keysHeldFromLastUpdate = new Dictionary<Keys, bool>();

        private Dictionary<MouseButton, HashSet<RegisteredFunction>> RegisteredMouseUpFunctions = null;
        private Dictionary<MouseButton, HashSet<RegisteredFunction>> RegisteredMouseDownFunctions = null;
        private Dictionary<MouseButton, HashSet<RegisteredFunction>> RegisteredMouseHeldFunctions = null;

        private HashSet<RegisteredFunction> RegisteredMouseScrollFunctions = new HashSet<RegisteredFunction>();
        private HashSet<RegisteredFunction> RegisteredMouseMoveFunctions = new HashSet<RegisteredFunction>();

        private Dictionary<MouseButton, bool> _mouseButtonsHeldFromLastUpdate = new Dictionary<MouseButton, bool>();

        private double _timeSinceLastFrame = 0.0;
        public double DeltaTime => _timeSinceLastFrame;

        public static void Init()
        {
            Instance = new GlobalInputManager();
        }
        public static GlobalInputManager Instance { get; private set; }
        private GlobalInputManager() {

            RegisteredKeyUpFunctions = new Dictionary<Keys, HashSet<RegisteredFunction>>();
            RegisteredKeyDownFunctions = new Dictionary<Keys, HashSet<RegisteredFunction>>();
            RegisteredKeyHeldFunctions = new Dictionary<Keys, HashSet<RegisteredFunction>>();

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (!RegisteredKeyUpFunctions.ContainsKey(key)) RegisteredKeyUpFunctions.Add(key, null);
                if (!RegisteredKeyDownFunctions.ContainsKey(key)) RegisteredKeyDownFunctions.Add(key, null);
                if (!RegisteredKeyHeldFunctions.ContainsKey(key)) RegisteredKeyHeldFunctions.Add(key, null);
                if (!_keysHeldFromLastUpdate.ContainsKey(key)) _keysHeldFromLastUpdate.Add(key, false);
                if (!_lengthKeysHeld.ContainsKey(key)) _lengthKeysHeld.Add(key, 0.0);
            }

            RegisteredMouseUpFunctions = new Dictionary<MouseButton, HashSet<RegisteredFunction>>();
            RegisteredMouseDownFunctions = new Dictionary<MouseButton, HashSet<RegisteredFunction>>();
            RegisteredMouseHeldFunctions = new Dictionary<MouseButton, HashSet<RegisteredFunction>>();

            foreach (MouseButton mb in Enum.GetValues(typeof(MouseButton)))
            {
                if (!RegisteredMouseUpFunctions.ContainsKey(mb)) RegisteredMouseUpFunctions.Add(mb, null);
                if (!RegisteredMouseDownFunctions.ContainsKey(mb)) RegisteredMouseDownFunctions.Add(mb, null);
                if (!RegisteredMouseHeldFunctions.ContainsKey(mb)) RegisteredMouseHeldFunctions.Add(mb, null);
                if (!_mouseButtonsHeldFromLastUpdate.ContainsKey(mb)) _mouseButtonsHeldFromLastUpdate.Add(mb, false);
            }

        }
        #endregion

        #region Processing Input

        //process input
        internal void Update(WindowArgs args)
        {
            _windowArgs = args;
            if (!_windowArgs.window_focused) return;
            _timeSinceLastFrame = _windowArgs.delta_time;
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (_keysHeldFromLastUpdate[key]) _lengthKeysHeld[key] += _windowArgs.delta_time;
                if (RegisteredKeyHeldFunctions[key] != null) RunAllRegisteredFunctions(ref RegisteredKeyHeldFunctions, key, true);
            }

            foreach (MouseButton mb in Enum.GetValues(typeof(MouseButton)))
            {
                if (RegisteredMouseHeldFunctions[mb] != null) RunAllRegisteredFunctions(ref RegisteredMouseHeldFunctions, mb, true);
            }
        }


        private void RunAllRegisteredFunctions(ref Dictionary<Keys, HashSet<RegisteredFunction>> rfs, Keys k, bool careIfHeld = false)
        {

            if (rfs[k] != null)
            {
                if (careIfHeld && !_keysHeldFromLastUpdate[k]) return;
                foreach (RegisteredFunction fn in rfs[k])
                {
                    fn(_windowArgs, _globalInputContext);
                }
            }

        }
        private void RunAllRegisteredFunctions(ref Dictionary<MouseButton, HashSet<RegisteredFunction>> rfs, MouseButton mb, bool careIfHeld = false)
        {
            if (rfs[mb] != null)
            {
                if (careIfHeld && !_mouseButtonsHeldFromLastUpdate[mb]) return;
                foreach (RegisteredFunction fn in rfs[mb])
                {
                    fn(_windowArgs, _globalInputContext);
                }
            }
        }

        private void RunAllRegisteredFunctions(ref HashSet<RegisteredFunction> rfs)
        {
            foreach (var fn in rfs)
            {
                fn(_windowArgs, _globalInputContext);
            }
        }

        #endregion

        #region Registering Input

        #region Keyboard
        public static void RegisterKeyUp(RegisteredFunction rf, Keys k)
        {
            Instance.RegisterUpKey(rf, k);
        }

        private void RegisterUpKey(RegisteredFunction rf, Keys k)
        {
            if (RegisteredKeyUpFunctions[k] == null) RegisteredKeyUpFunctions[k] = new HashSet<RegisteredFunction>();
            RegisteredKeyUpFunctions[k].Add(rf);
        }

        public static void RegisterKeyDown(RegisteredFunction rf, Keys k)
        {
            Instance.RegisterDownKey(rf, k);
        }

        private void RegisterDownKey(RegisteredFunction rf, Keys k)
        {
            if (RegisteredKeyDownFunctions[k] == null) RegisteredKeyDownFunctions[k] = new HashSet<RegisteredFunction>();
            RegisteredKeyDownFunctions[k].Add(rf);
        }

        public static void RegisterKeyHeld(RegisteredFunction rf, Keys k)
        {
            Instance.RegisterHeldKey(rf, k);
        }

        private void RegisterHeldKey(RegisteredFunction rf, Keys k)
        {
            if (RegisteredKeyHeldFunctions[k] == null) RegisteredKeyHeldFunctions[k] = new HashSet<RegisteredFunction>();
            RegisteredKeyHeldFunctions[k].Add(rf);
        }

        #endregion

        #region Mouse

        #region Clicking

        public static void RegisterButtonUp(RegisteredFunction rf, MouseButton mb)
        {
            Instance.RegisterMouseUp(rf, mb);
        }

        private void RegisterMouseUp(RegisteredFunction rf, MouseButton mb)
        {
            if (RegisteredMouseUpFunctions[mb] == null) RegisteredMouseUpFunctions[mb] = new HashSet<RegisteredFunction>();
            RegisteredMouseUpFunctions[mb].Add(rf);
        }

        public static void RegisterButtonDown(RegisteredFunction rf, MouseButton mb)
        {
            Instance.RegisterMouseDown(rf, mb);
        }

        private void RegisterMouseDown(RegisteredFunction rf, MouseButton mb)
        {
            if (RegisteredMouseDownFunctions[mb] == null) RegisteredMouseDownFunctions[mb] = new HashSet<RegisteredFunction>();
            RegisteredMouseDownFunctions[mb].Add(rf);
        }

        public static void RegisterButtonHeld(RegisteredFunction rf, MouseButton mb)
        {
            Instance.RegisterMouseHeld(rf, mb);
        }

        private void RegisterMouseHeld(RegisteredFunction rf, MouseButton mb)
        {
            if (RegisteredMouseHeldFunctions[mb] == null) RegisteredMouseHeldFunctions[mb] = new HashSet<RegisteredFunction>();
            RegisteredMouseHeldFunctions[mb].Add(rf);
        }

        #endregion

        #region Moving

        public static void RegisterMouseMovement(RegisteredFunction rf)
        {
            Instance.RegisterMouseMove(rf);
        }

        private void RegisterMouseMove(RegisteredFunction rf)
        {
            RegisteredMouseMoveFunctions.Add(rf);
        }

        public static void RegisterMouseScroll(RegisteredFunction rf)
        {
            Instance.RegisterMouseWheel(rf);
        }

        private void RegisterMouseWheel(RegisteredFunction rf)
        {
            RegisteredMouseScrollFunctions.Add(rf);
        }

        #endregion

        #endregion

        #endregion

        #region Grabbing Window Handles
        //set flags using the registered window callbacks
        internal static void OnKeyDown(KeyboardKeyEventArgs e, GlobalInputContext ctx)
        {
            Instance._keysHeldFromLastUpdate[e.Key] = true;
            Instance._globalInputContext = ctx;
            Instance.RunAllRegisteredFunctions(ref Instance.RegisteredKeyDownFunctions, e.Key);
        }

        internal static void OnKeyUp(KeyboardKeyEventArgs e, GlobalInputContext ctx)
        {
            Instance._keysHeldFromLastUpdate[e.Key] = false;
            Instance._globalInputContext = ctx;
            Instance.RunAllRegisteredFunctions(ref Instance.RegisteredKeyUpFunctions, e.Key);
        }

        internal static void OnMouseDown(MouseButtonEventArgs e, GlobalInputContext ctx)
        {
            Instance._mouseButtonsHeldFromLastUpdate[e.Button] = true;
            Instance._globalInputContext = ctx;
            Instance.RunAllRegisteredFunctions(ref Instance.RegisteredMouseDownFunctions, e.Button);
        }

        internal static void OnMouseUp(MouseButtonEventArgs e, GlobalInputContext ctx)
        {
            Instance._mouseButtonsHeldFromLastUpdate[e.Button] = false;
            Instance._globalInputContext = ctx;
            Instance.RunAllRegisteredFunctions(ref Instance.RegisteredMouseUpFunctions, e.Button);
        }

        internal static void OnMouseWheel(MouseWheelEventArgs e, GlobalInputContext ctx)
        {
            Instance._globalInputContext = ctx;
            Instance.RunAllRegisteredFunctions(ref Instance.RegisteredMouseScrollFunctions);
        }

        internal static void OnMouseMove(MouseMoveEventArgs e, GlobalInputContext ctx)
        {
            Instance._globalInputContext = ctx;
            Instance.RunAllRegisteredFunctions(ref Instance.RegisteredMouseMoveFunctions);
        }
        #endregion
    }
}
