using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HowToRenderTarget2D
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Texture2D dotTexture;

        //
        // Ok so here is our rendertarget reference declaration we declare it at class scope.
        //
        RenderTarget2D renderTargetIsAOffScreenBuffer;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //
            // We have to set up the render target, So we will instantiate it now here were we are sure the device is set up.
            // The method asks for a couple basic variables at least we will fill out the whole thing though.
            //
            renderTargetIsAOffScreenBuffer = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);

            // I should also say that we should also change its size if we are resizing the screen at least ussually people want to do that. 
            // We would do that in a method we add to window.ClientSizeChanged += OnResize;  
            // public void OnResize(){} is a void method you make up and add to game1, i dont want to get off topic though.

            dotTexture = TextureDotCreate(GraphicsDevice);
        }

        //
        // Well just make a dot texture for this example from a color array, which we can draw with.
        // There is also a get data method that will allow for pulling data from a texture to a array.
        //
        // These methods are not typically used in game regularly but for one time specific needs.
        // Typically these are used for creating a texture from scratch or for specific image editing operations.
        // Typically were you need or desire the exact opposite of a pixel shader operation which is faster.
        // The render target way is faster simple reliable and recommended.
        //
        public static Texture2D TextureDotCreate(GraphicsDevice device)
        {
            Color[] data = new Color[1];
            data[0] = new Color(255, 255, 255, 255);
            return TextureFromColorArray(device, data, 1, 1);
        }
        public static Texture2D TextureFromColorArray(GraphicsDevice device, Color[] data, int width, int height)
        {
            Texture2D tex = new Texture2D(device, width, height);
            tex.SetData<Color>(data);
            return tex;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);



            spriteBatch.Begin();

            // Set the drawing target to be a off screen rendering buffer.
            GraphicsDevice.SetRenderTarget(renderTargetIsAOffScreenBuffer);

            // Draw our dot texture on it but streach it out to a big 100x100 pixel area.
            spriteBatch.Draw(dotTexture, new Rectangle(100, 100, 200, 200), Color.White);

            spriteBatch.End();


            // If you were to run the program now without the code below then all you get is a clear screen,
            // Because we have not drawn to the back buffer yet which is what is presented to the screen by the video card.

            // Ok so now lets start drawing from the rendertarget to the backbuffer.

            spriteBatch.Begin();

            // Set the drawing target to be the screens back buffer again.
            GraphicsDevice.SetRenderTarget(null);

            // Treat the render target buffer we drew to before as if it is just a regular texture.
            Texture2D tempTexture = (Texture2D)renderTargetIsAOffScreenBuffer;

            // Draw that texture to the back buffer.
            spriteBatch.Draw(tempTexture, new Rectangle(0, 0, 500, 500), Color.Green);

            // Since its just a texture now you can treat it like one. 
            // For example draw just a portion of it to the backbuffer getting the source rectangle pixels from the render target and setting them as a destination to the backbuffer.
            // take note that draw takes the first rectangle as the pixel destination and the second is the texel source rectangle.
            spriteBatch.Draw(tempTexture, new Rectangle(100, 100, 200, 200), new Rectangle(100, 100, 200, 200), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}