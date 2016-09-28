using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using VenLight;

namespace Cocos2DGame1
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                using (VenLight06 game = new VenLight06()) { game.Run(); }
            }
            catch (NullReferenceException)
            {

            }
            
            //using (Game1 game = new Game1()) { game.Run(); }
        }
    }


}

