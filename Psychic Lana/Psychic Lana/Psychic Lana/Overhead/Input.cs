using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Psychic_Lana.Overhead
{
	/// <summary>
	/// Basic control types
	/// </summary>
	public enum Controls { Up, Right, Down, Left, Confirm, Cancel, Start, Console, AnyKey, Abort, Fullscreen};
	/// <summary>
	/// Public class used for detecting inputs such as mouse clicks, or various keyboard key presses
	/// </summary>
	public static class Input
	{
		// States
		static KeyboardState previousKeyboard;
		static KeyboardState currentKeyboard;
		static MouseState previousMouse;
		static MouseState currentMouse;

		// Controls
		static Keys upKey = Keys.W;
		static Keys rightKey = Keys.D;
		static Keys downKey = Keys.S;
		static Keys leftKey = Keys.A;
		static Keys confirmKey = Keys.J;
		static Keys cancelKey = Keys.K;
		static Keys startKey = Keys.Space;
		static Keys consoleKey = Keys.OemTilde;
		static Keys abortKey = Keys.Escape;
		static Keys fullscreenKey = Keys.F5;
		// Alternate
		static Keys upKey2 = Keys.Up;
		static Keys rightKey2 = Keys.Right;
		static Keys downKey2 = Keys.Down;
		static Keys leftKey2 = Keys.Left;
		static Keys confirmKey2 = Keys.X;
		static Keys cancelKey2 = Keys.Z;
		static Keys startKey2 = Keys.Enter;

		public static void Update(GameTime gametime)
		{
			// Set previous
			previousKeyboard = currentKeyboard;
			previousMouse = currentMouse;

			currentKeyboard = Keyboard.GetState();
			currentMouse = Mouse.GetState();
		}
		/// <summary>
		/// Checks if a specific input is currently down
		/// </summary>
		/// <param name="input">Input to check</param>
		/// <returns>True if input is down, or alternate input is down</returns>
		public static bool Down(Controls input)
		{
			switch (input)
			{
				case Controls.Up:
					if(currentKeyboard.IsKeyDown(upKey) || currentKeyboard.IsKeyDown(upKey2))
						return true;
					return false;
				case Controls.Right:
					if (currentKeyboard.IsKeyDown(rightKey) || currentKeyboard.IsKeyDown(rightKey2))
						return true;
					return false;
				case Controls.Down:
					if (currentKeyboard.IsKeyDown(downKey) || currentKeyboard.IsKeyDown(downKey2))
						return true;
					return false;
				case Controls.Left:
					if (currentKeyboard.IsKeyDown(leftKey) || currentKeyboard.IsKeyDown(leftKey2))
						return true;
					return false;
				case Controls.Confirm:
					if (currentKeyboard.IsKeyDown(confirmKey) || currentKeyboard.IsKeyDown(confirmKey2))
						return true;
					return false;
				case Controls.Cancel:
					if (currentKeyboard.IsKeyDown(cancelKey) || currentKeyboard.IsKeyDown(cancelKey2))
						return true;
					return false;
				case Controls.Start:
					if (currentKeyboard.IsKeyDown(startKey) || currentKeyboard.IsKeyDown(startKey2))
						return true;
					return false;
				case Controls.Console:
					if (currentKeyboard.IsKeyDown(consoleKey))
						return true;
					return false;
				case Controls.AnyKey:
					if (currentKeyboard.GetPressedKeys().Length > 0)
						return true;
					return false;
				case Controls.Abort:
					if (currentKeyboard.IsKeyDown(abortKey))
						return true;
					return false;
				case Controls.Fullscreen:
					if (currentKeyboard.IsKeyDown(fullscreenKey))
						return true;
					return false;
				default:
					return false;
			}
		}
		/// <summary>
		/// Checks if a specific input has been pressed
		/// </summary>
		/// <param name="input">Input to check</param>
		/// <returns>True if input was pressed (once)</returns>
		public static bool Pressed(Controls input)
		{
			switch (input)
			{
				case Controls.Up:
					if ((currentKeyboard.IsKeyDown(upKey) && previousKeyboard.IsKeyUp(upKey)) ||
						(currentKeyboard.IsKeyDown(upKey2) && previousKeyboard.IsKeyUp(upKey2)))
						return true;
					return false;
				case Controls.Right:
					if ((currentKeyboard.IsKeyDown(rightKey) && previousKeyboard.IsKeyUp(rightKey)) ||
						(currentKeyboard.IsKeyDown(rightKey2) && previousKeyboard.IsKeyUp(rightKey2)))
						return true;
					return false;
				case Controls.Down:
					if ((currentKeyboard.IsKeyDown(downKey) && previousKeyboard.IsKeyUp(downKey)) ||
						(currentKeyboard.IsKeyDown(downKey2) && previousKeyboard.IsKeyUp(downKey2)))
						return true;
					return false;
				case Controls.Left:
					if ((currentKeyboard.IsKeyDown(leftKey) && previousKeyboard.IsKeyUp(leftKey)) ||
						(currentKeyboard.IsKeyDown(leftKey2) && previousKeyboard.IsKeyUp(leftKey2)))
						return true;
					return false;
				case Controls.Confirm:
					if ((currentKeyboard.IsKeyDown(confirmKey) && previousKeyboard.IsKeyUp(confirmKey)) ||
						(currentKeyboard.IsKeyDown(confirmKey2) && previousKeyboard.IsKeyUp(confirmKey2)))
						return true;
					return false;
				case Controls.Cancel:
					if ((currentKeyboard.IsKeyDown(cancelKey) && previousKeyboard.IsKeyUp(cancelKey)) ||
						(currentKeyboard.IsKeyDown(cancelKey2) && previousKeyboard.IsKeyUp(cancelKey2)))
						return true;
					return false;
				case Controls.Start:
					if ((currentKeyboard.IsKeyDown(startKey) && previousKeyboard.IsKeyUp(startKey)) ||
						(currentKeyboard.IsKeyDown(startKey2) && previousKeyboard.IsKeyUp(startKey2)))
						return true;
					return false;
				case Controls.Console:
					if ((currentKeyboard.IsKeyDown(consoleKey) && previousKeyboard.IsKeyUp(consoleKey)))
						return true;
					return false;
				case Controls.AnyKey:
					if (currentKeyboard.GetPressedKeys().Length > 0 && previousKeyboard.GetPressedKeys().Length == 0)
						return true;
					return false;
				case Controls.Abort:
					if (currentKeyboard.IsKeyDown(abortKey) && previousKeyboard.IsKeyUp(abortKey))
						return true;
					return false;
				case Controls.Fullscreen:
					if (currentKeyboard.IsKeyDown(fullscreenKey) && previousKeyboard.IsKeyUp(fullscreenKey))
						return true;
					return false;
				default:
					return false;
			}
		}
	}
}
