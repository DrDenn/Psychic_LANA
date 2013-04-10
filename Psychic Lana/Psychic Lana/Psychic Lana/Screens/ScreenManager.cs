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
// Added
using Psychic_Lana.Overhead;

namespace Psychic_Lana.Screens
{
	/// <summary> Transition out types </summary>
	public enum TransOut { Instant, Fade }
	/// <summary> Transition in types </summary>
	public enum TransIn { Instant, Fade }
	/// <summary>
	/// Screen Manager class that keeps track of the various screens, and handles transitions between them and also draws and updates the current screen and the transitions between screens.
	/// </summary>
	public class ScreenManager
	{
		// Screen Dictionary of available screens
		Dictionary<String, Screen> screens;
		
		Screen previous;
		Screen current;
		Screen next;

		// Transitions
		bool transitionIn;
		bool transitionOut;
		float transInVariable;
		float transOutVariable;

		public ScreenManager()
		{
			screens = new Dictionary<String, Screen>();
			transitionIn = false;
			previous = null;
			current = null;
			next = null;
		}

		/// <summary>
		/// Adds a new screen to the Dictionary
		/// </summary>
		/// <param name="name">Name of the screen for referencing</param>
		/// <param name="screen">The screen being stored</param>
		public void AddScreen(String name, Screen screen)
		{
			screen.Initialize();
			screens.Add(name, screen);
			screen.LoadContent();
		}
		/// <summary>
		/// Transition to a new screen
		/// </summary>
		/// <param name="name">Name of the next screen</param>
		public void Transition(String name)
		{
			Screen get = null;
			screens.TryGetValue(name, out get);
			if (get == null)
				return;

			if (current != null)
			{
				next = get;
				transitionOut = true;
				transInVariable = 0.0f;
				transOutVariable = 0.0f;
			}
			else
			{
				current = get;
				transitionOut = false;
			}
			
		}
		/// <summary>
		/// Updates the Current Screen or transition
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{
			if (current == null)
				return;
			if (transitionOut) // Transition to next screen
			{
				switch (current.TransitionOut)
				{
					case TransOut.Instant:
						transitionOut = false;
						transitionIn = true;
						break;
					case TransOut.Fade:
						transOutVariable += ProcessFade(1.0f, gameTime);
						if (transOutVariable >= 1.0)
						{
							transOutVariable = 1.0f;
							transitionOut = false;
							transitionIn = true;
						}
						break;
					default:
						break;
				}
				if (current.ControlTransOut)
					current.Update(gameTime);
			}
			else if (transitionIn) // Transition from last screen
			{
				switch (next.TransitionIn)
				{
					case TransIn.Instant:
						transInVariable = 0.0f;
						transOutVariable = 0.0f;
						previous = current;
						current = next;
						break;
					case TransIn.Fade:
						if (transInVariable <= 0.0)
						{
							previous = current;
							current = next;
						}
						transInVariable += ProcessFade(1.0f, gameTime);
						if (transInVariable >= 1.0)
						{
							transInVariable = 1.0f;
							transitionIn = false;
						}
						break;
					default:
						transInVariable = 0.0f;
						transOutVariable = 0.0f;
						previous = current;
						current = next;
						break;
				}
				if(current.ControlTransIn)
					current.Update(gameTime);
			}
			else // Update current Screen
			{
				current.Update(gameTime);
			}
		}

		/// <summary>
		/// Draw the current screen
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (current == null)
				return;
			if (transitionOut) // Transition to next screen
			{
				switch (current.TransitionOut)
				{
					case TransOut.Instant:
						current.Draw(spriteBatch);
						break;
					case TransOut.Fade:
						current.Draw(spriteBatch);
						GlobalReference.DrawFilledScreen(spriteBatch, Color.Black, 0.0f + transOutVariable);
						break;
					default:
						break;
				}
			}
			else if (transitionIn) // Transition from last screen
			{
				switch (current.TransitionIn)
				{
					case TransIn.Instant:
						current.Draw(spriteBatch);
						break;
					case TransIn.Fade:
						current.Draw(spriteBatch);
						GlobalReference.DrawFilledScreen(spriteBatch, Color.Black, 1.0f - transInVariable);
						break;
					default:
						break;
				}
			}
			else // Update current Screen
				current.Draw(spriteBatch);

		}
		private float ProcessFade(float fadeTime, GameTime gameTime)
		{ return (float)gameTime.ElapsedGameTime.TotalSeconds / fadeTime; }
	}
}
