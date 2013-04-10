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
using Drawing;
using Psychic_Lana.Overhead;

namespace Psychic_Lana.Screens
{
	/// <summary>
	/// Super class for the screens
	/// </summary>
	public class Screen
	{
		public TransIn TransitionIn = TransIn.Instant;
		public TransOut TransitionOut = TransOut.Instant;
		public bool ControlTransIn = false;
		public bool ControlTransOut = false;

		public virtual void Initialize()
		{

		}
		public virtual void LoadContent()
		{

		}
		public virtual void UnloadContent()
		{

		}
		public virtual void Update(GameTime gametime)
		{

		}
		public virtual void Draw(SpriteBatch spriteBatch)
		{

		}
	}
}
