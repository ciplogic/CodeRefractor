//~~~~~~App.cs~~~~~~

using System;
using System.Runtime.InteropServices;
using Tao.OpenGl;
using Tao.Sdl;

static class SdlWrapper
{

    [DllImport("SDL.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_Init")]
    public static extern int __SDL_Init(int flags);
}

/* App sets up the SDL window and a timer, as well as basic openGL
   state. Much of the code has been inspired from a German page:
   http://www.mono-project.de/artikel/entwickler/tao-tutorial/seite/3/.
   Unfortunately I cannot read German and am unable to better
   attribute the code.
 
   Author Stephen Jones
*/


namespace Game
{
    public class App
    {
        //App sets up external libraries. GameClass handles events and game code
        private GameClass game;
        private int w = 800;
        private int h = 500;
        private int bpp = 16;
        private int flags = Sdl.SDL_OPENGL | Sdl.SDL_GL_DOUBLEBUFFER | Sdl.SDL_NOFRAME | Sdl.SDL_HWSURFACE;// | Sdl.SDL_FULLSCREEN;
        private int timeCatcher;//used for the timing mechanism

        public App()
        {
            //SDL is comprised of 8 subsystems. Here we initialize the video
            if (SdlWrapper.__SDL_Init(Sdl.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine("Error initializing SDL");
                Sdl.SDL_Quit();
                return;
            }

            /* Rather than set the video properties up in the constructor, I set
               them in setVideo. The reason for this is that 2 pointers are used
               to interact with SDL structures. Once used they convert their
               handles into vidInfo and surface tamer variables. That this
               occurs inside the function means the pointers will release
               their memory on function exit.
            */
            setSDLVideo();
            /* openGL is not part of SDL, rather it runs in a window handled
               by SDL. here we set up some openGL state
            */
            //initialize game
            game = new GameClass();
            /* finP is the property get/setter for the boolean fin in the game
               class. It is held in the game class because the game class handles
               events. When escape is pressed, fin is set to true and the following
               loop
               terminates.
               The function tick is called every loop. It is passed the time
               taken for each loop
            */
            while (!game.finP)
            {
                tick(Sdl.SDL_GetTicks() - timeCatcher);//updates the game object
                timeCatcher = Sdl.SDL_GetTicks();//stores the current time
                Sdl.SDL_Delay(1);//release the thread
            }

            /* When the loop ends the code drops down to here. SDL_Quit shuts
               down the SDL subsystems initialized by SDL_Init
            */
            Sdl.SDL_Quit();
            return;
        }


        private void setSDLVideo()
        {
            /* To center a non-fullscreen window we need to set an environment
               variable
            */
            //Sdl.SDL_putenv("SDL_VIDEO_CENTERED=center");

            /* the video info structure contains the current video mode. Prior to
               calling setVideoMode, it contains the best available mode
               for your system. Post setting the video mode, it contains
               whatever values you set the video mode with.
               First we point at the SDL structure, then test to see that the
               point is right. Then we copy the data from the structure to
               the safer vidInfo variable.
            */

            /* according to the SDL documentaion, the flags parameter passed to setVideoMode
               affects only the 2D SDL surface, not the openGL. To set their properties
               use the syntax below. We enable vsync because we are running the loop
               unfettered and we don't want the loop redrawing the buffer
               while it is being written to screen
            */
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_SWAP_CONTROL, 1);//enable vsync
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_RED_SIZE, 8);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_GREEN_SIZE, 8);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_BLUE_SIZE, 8);
            //Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DEPTH_SIZE, 16);
            //Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_MULTISAMPLEBUFFERS, 1);
            //Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_MULTISAMPLESAMPLES, 2);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DOUBLEBUFFER, 1);

            /* the setVideoMode function returns the current frame buffer as an
               SDL_Surface. Again, we grab a pointer to it, then place its
               content into the non pointery surface variable. I say 'non-pointery',
               but this SDL variable must have a pointer in it because it can
               access the current pixels in the framebuffer.
            */
            var ptr = Sdl.SDL_SetVideoMode(w, h, bpp, flags);
            if (ptr == IntPtr.Zero)
            {
                Console.WriteLine("Error qsetting the video mode");
                Sdl.SDL_Quit();
            }
        }


        /* tick is called once every loop. Here it paints the screen
           the glClearColor defined in the setOpenGL function then calls
           the game class. The game class decides what will be drawn and
           gets the drawing done. All the drawing is done into the back buffer.
           Once the drawing is finished the buffers are swapped.
        */
        private void tick(long delay)
        {

            game.tick(delay);

        }

        [STAThread]
        public static void Main()
        {
            App app = new App();
        }
    }
}


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
        private void pollEvents()
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
                        //Console.WriteLine("Heading to {0}", e.motion.xrel);
                        //Console.WriteLine("Pitch to {0}", e.motion.yrel);
                        break;
                }
            }
        }


        /* Thank You Mr NeHe, not just for your GL cube, but for opening the
           path to openGL, and indeed, a freer and more helpful community of programmers
        */
        private void draw()
        {
            Gl.glClearColor(1, 1, 0, 0);

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

            Sdl.SDL_GL_SwapBuffers();
        }


        /* Events will basically drive the program logic, which in
           turn determines what the game class decides to draw. Once the
           logic is in place, the drawing can begin.
        */
        public void tick(long delay)
        {
            pollEvents();
            draw();
        }
    }
}