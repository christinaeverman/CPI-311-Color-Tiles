using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public static class InputManager
    {
        static KeyboardState PreviousKeyboardState { get; set; }
        static KeyboardState CurrentKeyboardState { get; set; }
        static MouseState PreviousMouseState { get; set; }
        static MouseState CurrentMouseState { get; set; }

        public static void Initialize()
        {
            PreviousKeyboardState = CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState = Mouse.GetState();
        }

        public static void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public static bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key);
        }

        // ************************ Lab 11 ****************************************
        public static bool IsKeyReleased(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key) && PreviousKeyboardState.IsKeyDown(key);
        }
        // *************************************************************************

        // ******************** Lab08 Update ***********************************
        public static Vector2 GetMousePosition()
        {
            return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
        }
        // *********************************************************************

        // *************************** Assignment02 ****************************
        public static bool IsMousePressed(int MouseButton)
        {
            return PreviousMouseState.LeftButton == ButtonState.Released &&
                CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        // ********************** Lab 11 ************************************
        public static bool IsMouseReleased(int mouseButton)
        {
            return PreviousMouseState.LeftButton == ButtonState.Pressed && 
                CurrentMouseState.LeftButton == ButtonState.Released;
        }
        // *******************************************************************

        public static bool IsCursorMovedRight()
        {
            if (CurrentMouseState.Position.X >= PreviousMouseState.Position.X)
                return true;
            return false;
        }

        public static bool IsCursorMovedLeft()
        {
            if (CurrentMouseState.Position.X <= PreviousMouseState.Position.X)
                return true;
            return false;
        }
        // *********************************************************************
    }
}
