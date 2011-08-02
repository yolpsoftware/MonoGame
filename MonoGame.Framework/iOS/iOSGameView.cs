using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using MonoTouch.OpenGLES;
using OpenTK.Graphics;
using MonoTouch.ObjCRuntime;

using All  = OpenTK.Graphics.ES11.All;
using ES11 = OpenTK.Graphics.ES11;
using ES20 = OpenTK.Graphics.ES20;
using MonoTouch.Foundation;

namespace Microsoft.Xna.Framework
{
	
    sealed class GLCalls {
        public delegate void glBindFramebuffer(All target, int framebuffer);
        public delegate void glBindRenderbuffer(All target, int renderbuffer);
        public delegate void glDeleteFramebuffers(int n, ref int framebuffers);
        public delegate void glDeleteRenderbuffers(int n, ref int renderbuffers);
        public delegate void glFramebufferRenderbuffer(All target, All attachment, All renderbuffertarget, int renderbuffer);
        public delegate void glGenFramebuffers(int n, ref int framebuffers);
        public delegate void glGenRenderbuffers(int n, ref int renderbuffers);
        public delegate void glGetInteger(All name, ref int value);
        public delegate void glScissor(int x, int y, int width, int height);
        public delegate void glViewport(int x, int y, int width, int height);

        public glBindFramebuffer BindFramebuffer;
        public glBindRenderbuffer BindRenderbuffer;
        public glDeleteFramebuffers DeleteFramebuffers;
        public glDeleteRenderbuffers DeleteRenderbuffers;
        public glFramebufferRenderbuffer FramebufferRenderbuffer;
        public glGenFramebuffers GenFramebuffers;
        public glGenRenderbuffers GenRenderbuffers;
        public glGetInteger GetInteger;
        public glScissor Scissor;
        public glViewport Viewport;

        public static GLCalls GetGLCalls(EAGLRenderingAPI api)
        {
            switch (api) {
                case EAGLRenderingAPI.OpenGLES1: return CreateES1();
                case EAGLRenderingAPI.OpenGLES2: return CreateES2();
            }
            throw new ArgumentException("api");
        }

        static GLCalls CreateES1()
        {
            return new GLCalls() {
                BindFramebuffer         = (t, f)              => ES11.GL.Oes.BindFramebuffer(t, f),
                BindRenderbuffer        = (t, r)              => ES11.GL.Oes.BindRenderbuffer(t, r),
                DeleteFramebuffers      = (int n, ref int f)  => ES11.GL.Oes.DeleteFramebuffers(n, ref f),
                DeleteRenderbuffers     = (int n, ref int r)  => ES11.GL.Oes.DeleteRenderbuffers(n, ref r),
                FramebufferRenderbuffer = (t, a, rt, rb)      => ES11.GL.Oes.FramebufferRenderbuffer(t, a, rt, rb),
                GenFramebuffers         = (int n, ref int f)  => ES11.GL.Oes.GenFramebuffers(n, ref f),
                GenRenderbuffers        = (int n, ref int r)  => ES11.GL.Oes.GenRenderbuffers(n, ref r),
                GetInteger              = (All n, ref int v)  => ES11.GL.GetInteger(n, ref v),
                Scissor                 = (x, y, w, h)        => ES11.GL.Scissor(x, y, w, h),
                Viewport                = (x, y, w, h)        => ES11.GL.Viewport(x, y, w, h),
            };
        }

        static GLCalls CreateES2()
        {
            return new GLCalls() {
                BindFramebuffer         = (t, f)              => ES20.GL.BindFramebuffer((ES20.All) t, f),
                BindRenderbuffer        = (t, r)              => ES20.GL.BindRenderbuffer((ES20.All) t, r),
                DeleteFramebuffers      = (int n, ref int f)  => ES20.GL.DeleteFramebuffers(n, ref f),
                DeleteRenderbuffers     = (int n, ref int r)  => ES20.GL.DeleteRenderbuffers(n, ref r),
                FramebufferRenderbuffer = (t, a, rt, rb)      => ES20.GL.FramebufferRenderbuffer((ES20.All) t, (ES20.All) a, (ES20.All) rt, rb),
                GenFramebuffers         = (int n, ref int f)  => ES20.GL.GenFramebuffers(n, ref f),
                GenRenderbuffers        = (int n, ref int r)  => ES20.GL.GenRenderbuffers(n, ref r),
                GetInteger              = (All n, ref int v)  => ES20.GL.GetInteger((ES20.All) n, ref v),
                Scissor                 = (x, y, w, h)        => ES20.GL.Scissor(x, y, w, h),
                Viewport                = (x, y, w, h)        => ES20.GL.Viewport(x, y, w, h),
            };
        }
    }
	
	public class iOSGameView : UIViewController
	{
		public iOSGameView ()
		{

		}

		public override void ViewWillAppear (bool animated)
		{
			// Prepare to start the game appear
			base.ViewWillAppear (animated);   
		}

		public override void ViewDidAppear (bool animated)
		{
			// Start the game loop
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			// Prepare to pause the game
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			// Pause the game loop
			base.ViewWillDisappear (animated);
		}

		// NSTimer will cause latency in rendering and may reduce frame rate.
		//iOS 4 onwards...
		// public CADisplaLink DisplayLinkWithTarget 

		// Called whenever the bounds of the view changes
		/* public override void LayoutSubviews()
		{
			// deleteFrameBuffer 

			// createFrameBuffer at the right size
		}*/

		// Copied from your game stuff Clancey
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{

			var manager = GameWindow.game.graphicsDeviceManager as GraphicsDeviceManager;
			Console.WriteLine (manager == null);
			if (manager == null)
				return true;
			DisplayOrientation supportedOrientations = manager.SupportedOrientations;
			switch (toInterfaceOrientation) {
			case UIInterfaceOrientation.LandscapeLeft :
				return (supportedOrientations & DisplayOrientation.LandscapeLeft) != 0;
			case UIInterfaceOrientation.LandscapeRight:
				return (supportedOrientations & DisplayOrientation.LandscapeRight) != 0;
			case UIInterfaceOrientation.Portrait:
				return (supportedOrientations & DisplayOrientation.Portrait) != 0;
			case UIInterfaceOrientation.PortraitUpsideDown :
				return (supportedOrientations & DisplayOrientation.PortraitUpsideDown) != 0;
			default :
				return false;
			}
			return true;

		}

		//Code from iphoneGameView
		 bool disposed;

        int framebuffer, renderbuffer;

        GLCalls gl;
		
		[Export ("layerClass")]
        public static Class GetLayerClass ()
        {
            return new Class (typeof (CAEAGLLayer));
        }
		
		protected virtual void ConfigureLayer (CAEAGLLayer eaglLayer)
		{
		}


		//end of code from iphone game view
	}
}
