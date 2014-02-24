//~~~~~~App.cs~~~~~~

using System;
using System.Reflection;
using Tao.Sdl;

/* App sets up the SDL window and a timer, as well as basic openGL
   state. Much of the code has been inspired from a German page:
   http://www.mono-project.de/artikel/entwickler/tao-tutorial/seite/3/.
   Unfortunately I cannot read German and am unable to better
   attribute the code.
 
   Author Stephen Jones
*/


//______!!______!!______
//~~~~~~GameClass.cs

/* GameClass is the brains behind the game; it handles events and decides
   what to draw etc.
 
   Author Stephen Jones
*/

namespace Game
{
    public class GameClass
    {
        /* fin is set true when the escape key is pressed. Every loop
           the App class checks this variable to see if it should terminate
           the game loop.
        */
        private bool fin = false;
        private Sdl.SDL_Event e;
        // used to rotate the cube
        private float xrot = 0.0f;
        private float yrot = 0.0f;

        /* The constructor is called from the App constructor prior to the
           setting up of the game loop. Here we use it to hide the cursor.
           The first two functions should be called together if you want
           to both hide the cursor, and keep it centered on screen. If the
           second function is omitted the cursor will still move about the
           screen, it will not be seen, but when it hits a screen edge and can
           go no further, it will stop firing events that would push it off the
           screen; not such a good idea when the monster is behind you and you
           need to be able to move the mouse in that direction and have your
           avatar turn around.
           A new event structure is initialized here and re-used for all events
           rather than constructing a new event every game loop
        */

        public GameClass()
        {
            Sdl.SDL_ShowCursor(Sdl.SDL_DISABLE);
            Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_ON);
            e = new Sdl.SDL_Event();
        }

        /* Use a C# property to get and set the loop variable fin. Properties,
           as oppesed to methods are compiled as assignments rather than pass
           by value functions. Kinda cooler even than Java.
        */

        public bool finP
        {
            get { return fin; }
            set { fin = value; }
        }

        /* pollEvents is called from the tick function in this class.
           It will pop events off the queue untill it is empty. Any event of
           interest will be caught in the switch statement and its case
           handled. Actually, the SDL_QUIT will never eventuate because the window
           is frameless and therefore has no quit button. You need to press
           escape
 
           At this stage the events, when triggered, write to the console at the
           bottom of the MonoDevelop IDE. To make better sense of the output
           you might like to comment the first two Console.WriteLine statements
           pertaining to dx and dy because thay are called every loop and somewhat
           swamp the key and mouse button presses in the Console. Also note that
           I do not use wasd, I use efa and space.
        */

        public void pollEvents()
        {
            while (Sdl.SDL_PollEvent(out e) == 1)
            {
                switch (e.type)
                {
                    case Sdl.SDL_QUIT:
                        fin = true;
                        break;
                    case Sdl.SDL_KEYDOWN:
                        if (e.key.keysym.sym == Sdl.SDLK_ESCAPE) finP = true;
                        if (e.key.keysym.sym == Sdl.SDLK_e) Console.WriteLine("Moving ford");
                        if (e.key.keysym.sym == Sdl.SDLK_f) Console.WriteLine("Moving back");
                        if (e.key.keysym.sym == Sdl.SDLK_a) Console.WriteLine("Strafing left");
                        if (e.key.keysym.sym == Sdl.SDLK_SPACE) Console.WriteLine("Strafing right");
                        break;
                    case Sdl.SDL_MOUSEBUTTONDOWN:
                        if (e.button.button == Sdl.SDL_BUTTON_LEFT) Console.WriteLine("Let the shoosting begin");
                        if (e.button.button == Sdl.SDL_BUTTON_RIGHT) Console.WriteLine("Using an object");
                        break;
                    case Sdl.SDL_MOUSEMOTION:
                        Console.WriteLine("Mouse motion");
                        break;
                }
            }
        }
    }
}
