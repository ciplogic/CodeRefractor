using System;
using Tao.OpenGl;
using Tao.Sdl;

namespace Game
{
    public class App
    {
        //App sets up external libraries. GameClass handles events and game code
        private GameClass game;
        private int w = 1024;
        private int h = 768;
        private int bpp = 16;
        private int flags = Sdl.SDL_OPENGL | Sdl.SDL_GL_DOUBLEBUFFER | Sdl.SDL_HWSURFACE; // | Sdl.SDL_FULLSCREEN;
        private static float rtri;
        private static float rquad;

        public App()
        {
            //SDL is comprised of 8 subsystems. Here we initialize the video
            if (Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO) < 0)
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
                game.pollEvents();
                Tick(); //updates the game object
                Sdl.SDL_Delay(1); //release the thread
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
            Sdl.SDL_putenv("SDL_VIDEO_CENTERED=center");

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
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_SWAP_CONTROL, 1); //enable vsync
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_RED_SIZE, 8);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_GREEN_SIZE, 8);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_BLUE_SIZE, 8);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DEPTH_SIZE, 16);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_MULTISAMPLEBUFFERS, 1);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_MULTISAMPLESAMPLES, 2);
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
            InitGL();
            ReSizeGLScene(w, h);
        }


        #region bool DrawGLScene()

        /// <summary>
        ///     Here's where we do all the drawing.
        /// </summary>
        /// <returns>
        ///     <c>true</c> on successful drawing, otherwise <c>false</c>.
        /// </returns>
        private static bool DrawGLScene()
        {
            Gl.glClearColor(1, 1, 0, 0);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT); // Clear Screen And Depth Buffer
            Gl.glLoadIdentity(); // Reset The Current Modelview Matrix
            Gl.glTranslatef(-1.5f, 0, -6); // Move Left 1.5 Units And Into The Screen 6.0
            Gl.glRotatef(rtri, 0, 1, 0); // Rotate The Triangle On The Y axis ( NEW )
            Gl.glBegin(Gl.GL_TRIANGLES); // Drawing Using Triangles
            Gl.glColor3f(1, 0, 0); // Red
            Gl.glVertex3f(0, 1, 0); // Top Of Triangle (Front)
            Gl.glColor3f(0, 1, 0); // Green
            Gl.glVertex3f(-1, -1, 1); // Left Of Triangle (Front)
            Gl.glColor3f(0, 0, 1); // Blue
            Gl.glVertex3f(1, -1, 1); // Right Of Triangle (Front)
            Gl.glColor3f(1, 0, 0); // Red
            Gl.glVertex3f(0, 1, 0); // Top Of Triangle (Right)
            Gl.glColor3f(0, 0, 1); // Blue
            Gl.glVertex3f(1, -1, 1); // Left Of Triangle (Right)
            Gl.glColor3f(0, 1, 0); // Green
            Gl.glVertex3f(1, -1, -1); // Right Of Triangle (Right)
            Gl.glColor3f(1, 0, 0); // Red
            Gl.glVertex3f(0, 1, 0); // Top Of Triangle (Back)
            Gl.glColor3f(0, 1, 0); // Green
            Gl.glVertex3f(1, -1, -1); // Left Of Triangle (Back)
            Gl.glColor3f(0, 0, 1); // Blue
            Gl.glVertex3f(-1, -1, -1); // Right Of Triangle (Back)
            Gl.glColor3f(1, 0, 0); // Red
            Gl.glVertex3f(0, 1, 0); // Top Of Triangle (Left)
            Gl.glColor3f(0, 0, 1); // Blue
            Gl.glVertex3f(-1, -1, -1); // Left Of Triangle (Left)
            Gl.glColor3f(0, 1, 0); // Green
            Gl.glVertex3f(-1, -1, 1); // Right Of Triangle (Left)
            Gl.glEnd(); // Finished Drawing The Triangle
            Gl.glLoadIdentity(); // Reset The Current Modelview Matrix
            Gl.glTranslatef(1.5f, 0, -7); // Move Right 1.5 Units And Into The Screen 7.0
            Gl.glRotatef(rquad, 1, 1, 1); // Rotate The Quad On The X, Y, and Z Axis ( NEW )
            Gl.glColor3f(0.5f, 0.5f, 1); // Set The Color To Blue One Time Only
            Gl.glBegin(Gl.GL_QUADS); // Draw A Quad
            Gl.glColor3f(0, 1, 0); // Set The Color To Green
            Gl.glVertex3f(1, 1, -1); // Top Right Of The Quad (Top)
            Gl.glVertex3f(-1, 1, -1); // Top Left Of The Quad (Top)
            Gl.glVertex3f(-1, 1, 1); // Bottom Left Of The Quad (Top)
            Gl.glVertex3f(1, 1, 1); // Bottom Right Of The Quad (Top)
            Gl.glColor3f(1, 0.5f, 0); // Set The Color To Orange
            Gl.glVertex3f(1, -1, 1); // Top Right Of The Quad (Bottom)
            Gl.glVertex3f(-1, -1, 1); // Top Left Of The Quad (Bottom)
            Gl.glVertex3f(-1, -1, -1); // Bottom Left Of The Quad (Bottom)
            Gl.glVertex3f(1, -1, -1); // Bottom Right Of The Quad (Bottom)
            Gl.glColor3f(1, 0, 0); // Set The Color To Red
            Gl.glVertex3f(1, 1, 1); // Top Right Of The Quad (Front)
            Gl.glVertex3f(-1, 1, 1); // Top Left Of The Quad (Front)
            Gl.glVertex3f(-1, -1, 1); // Bottom Left Of The Quad (Front)
            Gl.glVertex3f(1, -1, 1); // Bottom Right Of The Quad (Front)
            Gl.glColor3f(1, 1, 0); // Set The Color To Yellow
            Gl.glVertex3f(1, -1, -1); // Top Right Of The Quad (Back)
            Gl.glVertex3f(-1, -1, -1); // Top Left Of The Quad (Back)
            Gl.glVertex3f(-1, 1, -1); // Bottom Left Of The Quad (Back)
            Gl.glVertex3f(1, 1, -1); // Bottom Right Of The Quad (Back)
            Gl.glColor3f(0, 0, 1); // Set The Color To Blue
            Gl.glVertex3f(-1, 1, 1); // Top Right Of The Quad (Left)
            Gl.glVertex3f(-1, 1, -1); // Top Left Of The Quad (Left)
            Gl.glVertex3f(-1, -1, -1); // Bottom Left Of The Quad (Left)
            Gl.glVertex3f(-1, -1, 1); // Bottom Right Of The Quad (Left)
            Gl.glColor3f(1, 0, 1); // Set The Color To Violet
            Gl.glVertex3f(1, 1, -1); // Top Right Of The Quad (Right)
            Gl.glVertex3f(1, 1, 1); // Top Left Of The Quad (Right)
            Gl.glVertex3f(1, -1, 1); // Bottom Left Of The Quad (Right)
            Gl.glVertex3f(1, -1, -1); // Bottom Right Of The Quad (Right)
            Gl.glEnd(); // Done Drawing The Quad

            rtri += 0.2f; // Increase The Rotation Variable For The Triangle ( NEW )
            rquad -= 0.15f; // Decrease The Rotation Variable For The Quad ( NEW )
            return true;
        }

        #endregion bool DrawGLScene()

        #region bool InitGL()

        /// <summary>
        ///     All setup for OpenGL goes here.
        /// </summary>
        /// <returns>
        ///     <c>true</c> on successful initialization, otherwise <c>false</c>.
        /// </returns>
        private static bool InitGL()
        {
            Gl.glShadeModel(Gl.GL_SMOOTH); // Enable Smooth Shading
            Gl.glClearColor(0, 0, 0, 0.5f); // Black Background
            Gl.glClearDepth(1); // Depth Buffer Setup
            Gl.glEnable(Gl.GL_DEPTH_TEST); // Enables Depth Testing
            Gl.glDepthFunc(Gl.GL_LEQUAL); // The Type Of Depth Testing To Do
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST); // Really Nice Perspective Calculations
            return true;
        }

        #endregion bool InitGL()

        #region ReSizeGLScene(int width, int height)

        /// <summary>
        ///     Resizes and initializes the GL window.
        /// </summary>
        /// <param name="width">
        ///     The new window width.
        /// </param>
        /// <param name="height">
        ///     The new window height.
        /// </param>
        private static void ReSizeGLScene(int width, int height)
        {
            if (height == 0)
            {
                // Prevent A Divide By Zero...
                height = 1; // By Making Height Equal To One
            }

            Gl.glViewport(0, 0, width, height); // Reset The Current Viewport
            Gl.glMatrixMode(Gl.GL_PROJECTION); // Select The Projection Matrix
            Gl.glLoadIdentity(); // Reset The Projection Matrix
            Glu.gluPerspective(45, width/(double) height, 0.1, 100); // Calculate The Aspect Ratio Of The Window
            Gl.glMatrixMode(Gl.GL_MODELVIEW); // Select The Modelview Matrix
            Gl.glLoadIdentity(); // Reset The Modelview Matrix
        }

        #endregion

        /* tick is called once every loop. Here it paints the screen
           the glClearColor defined in the setOpenGL function then calls
           the game class. The game class decides what will be drawn and
           gets the drawing done. All the drawing is done into the back buffer.
           Once the drawing is finished the buffers are swapped.
        */

        private static void Tick()
        {
            DrawGLScene();
            Sdl.SDL_GL_SwapBuffers();

        }

        [STAThread]
        public static void Main()
        {
            App app = new App();
        }
    }
}