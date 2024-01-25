namespace SolarSystemTest
{
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using OpenTK.Windowing.Desktop;

    public class Game : GameWindow
    {
        private Shader _shader;
        private Sphere _sphere;
        private double _time;

        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        private readonly Vector3 _lightPos = new Vector3(1.0f, 1.0f, 2.0f);
        private int _vertexBufferObject;

        private int _vaoModel;
        private int _vaoLamp;
        private Shader _lampShader;
        private Shader _lightingShader;

        private Texture _diffuseMap;
        private Texture _specularMap;

        public Game(
            GameWindowSettings gameWindowSettings,
            NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _sphere = new Sphere(0.5f, 500, 500);
            //_shader = new Shader(
            //    "../../../Shaders/shader.vert",
            //    "../../../Shaders/shader.frag");
            //_shader.Use();

            _lightingShader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/lighting.frag");
            _lampShader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/shader.frag");

            {
                // Initialize the vao for the model
                //_vaoModel = GL.GenVertexArray();
                //GL.BindVertexArray(_vaoModel);

                var vertexLocation = _lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            }

            {
                // Initialize the vao for the lamp, this is mostly the same as the code for the model cube
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                // Set the vertex attributes (only position data for our lamp)
                var vertexLocation = _lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            }

            //{            
            //    var vertexLocation = _shader.GetAttribLocation("aPosition");
            //    GL.EnableVertexAttribArray(vertexLocation);
            //    GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);                
            //
            //    var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            //    GL.EnableVertexAttribArray(texCoordLocation);
            //    GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            //}

            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            CursorState = CursorState.Grabbed;

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {            
            _time += 4.0 * e.Time;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
           
            //_sphere.SetLight(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
            //var model = Matrix4.Identity;
            //model *= Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));
            ////model *= Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time));
            ////model *= Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(_time));
            //
            //_shader.Use();
            //_shader.SetMatrix4("model", model);
            //_shader.SetMatrix4("view", _camera.GetViewMatrix());
            //_shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _lightingShader.Use();

            _lightingShader.SetMatrix4("model", Matrix4.Identity);
            _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _lightingShader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.6f));
            _lightingShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));

            // Отрисовка сферы
            _sphere.Render();

            GL.BindVertexArray(_vaoLamp);

            _lampShader.Use();

            Matrix4 lampMatrix = Matrix4.CreateScale(0.3f); // We scale the lamp cube down a bit to make it less dominant
            lampMatrix = lampMatrix * Matrix4.CreateTranslation(_lightPos);

            _lampShader.SetMatrix4("model", lampMatrix);
            _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _sphere.Render();

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time;
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time;
            }

            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {            
            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;

            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindVertexArray(0);
            //GL.UseProgram(0);
            //
            //GL.DeleteProgram(_shader.Handle);

            base.OnUnload();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {            
            _camera.Fov -= e.OffsetY;

            base.OnMouseWheel(e);
        }
    }
}
