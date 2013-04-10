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
// added
using Psychic_Lana.Overhead;

namespace Psychic_Lana.Maps
{
	/// <summary> Transition out types </summary>
	public enum TransOut { Instant, Fade }
	/// <summary> Transition in types </summary>
	public enum TransIn { Instant, Fade }
	/// <summary>
	/// Map Manager class that keeps track of the various Maps, and handles transitions between them and also draws and updates the current map and the transitions between maps.
	/// </summary>
	public class MapManager
	{
		// Map List of available maps
		List<Map> maps;

		public Map Previous;
		public Map Current;
		public Map Next;

		// Transitions
		bool transitionIn;
		bool transitionOut;
		float transInVariable;
		float transOutVariable;

		public MapManager()
		{
			maps = new List<Map>();
			transitionIn = false;
			Previous = null;
			Current = null;
			Next = null;
		}

		/// <summary>
		/// Adds a new map to the List
		/// </summary>
		/// <param name="map"> The Map being added</param>
		public void AddMap(Map map)
		{
			maps.Add(map);
			map.LoadContent();
		}
		/// <summary>
		/// Transition to a new Map
		/// </summary>
		/// <param name="mapID"> id of the next Map </param>
		public void Transition(int mapID)
		{
			// Invalid Map Case: Ignore for now, throw an error later?
			if (mapID >= maps.Count || mapID < 0)
				return;

			Map get = null;
			get = maps.ElementAt(mapID);


			if (Current != null)
			{
				Next = get;
				transitionOut = true;
				transInVariable = 0.0f;
				transOutVariable = 0.0f;
			}
			else
			{
				Current = get;
				transitionOut = false;
			}
		}
		/// <summary>
		/// Updates the Current Map or transition
		/// </summary>
		/// <param name="gameTime"> Game Time</param>
		public void Update(GameTime gameTime)
		{
			if (Current == null)
				return;
			if (transitionOut) // Transition to next map
			{
				switch (Current.TransitionOut)
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
				if (Current.ControlTransOut)
					Current.Update(gameTime);
			}
			else if (transitionIn) // Transition from last map
			{
				switch (Next.TransitionIn)
				{
					case TransIn.Instant:
						transInVariable = 0.0f;
						transOutVariable = 0.0f;
						Previous = Current;
						Current = Next;
						break;
					case TransIn.Fade:
						if (transInVariable <= 0.0)
						{
							Previous = Current;
							Current = Next;
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
						Previous = Current;
						Current = Next;
						break;
				}
				if (Current.ControlTransIn)
					Current.Update(gameTime);
			}
			else // Update current map
			{
				Current.Update(gameTime);
			}
		}

		/// <summary>
		/// Draw the current map
		/// </summary>
		/// <param name="spriteBatch"> Sprite Batch </param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (Current == null)
				return;
			if (transitionOut) // Transition to next map
			{
				switch (Current.TransitionOut)
				{
					case TransOut.Instant:
						Current.Draw(spriteBatch);
						break;
					case TransOut.Fade:
						Current.Draw(spriteBatch);
						GlobalReference.DrawFilledScreen(spriteBatch, Color.Black, 0.0f + transOutVariable);
						break;
					default:
						break;
				}
			}
			else if (transitionIn) // Transition from last map
			{
				switch (Current.TransitionIn)
				{
					case TransIn.Instant:
						Current.Draw(spriteBatch);
						break;
					case TransIn.Fade:
						Current.Draw(spriteBatch);
						GlobalReference.DrawFilledScreen(spriteBatch, Color.Black, 1.0f - transInVariable);
						break;
					default:
						break;
				}
			}
			else // Update current map
				Current.Draw(spriteBatch);

		}

		private float ProcessFade(float fadeTime, GameTime gameTime)
		{ return (float)gameTime.ElapsedGameTime.TotalSeconds / fadeTime; }
	}
}
