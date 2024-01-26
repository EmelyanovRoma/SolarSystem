namespace SolarSystem
{
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using OpenTK.Windowing.Desktop;

    public class Game : GameWindow
    {
        private double _time;

        private Shader _lampShader;
        private Shader _lightingShader;

        private Sphere _sun;
        private Dictionary<PlanetType, SpaceObject> _solarSystemPlanets;

        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;

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

            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            _camera = new Camera(Vector3.UnitZ * 30, Size.X / (float)Size.Y);
            CursorState = CursorState.Grabbed;

            _sun = new Sphere(15.0f, 50, 50);
            _solarSystemPlanets = new Dictionary<PlanetType, SpaceObject>()
            {
                {
                    PlanetType.Mercury,
                    new SpaceObject(0.34f, 100, 100, 47.36f, 20.0f, "Resources/mercury_texture.jpg")
                },
                {
                    PlanetType.Venus,
                    new SpaceObject(0.85f, 250, 250, 35.02f, 24.0f, "Resources/venus_texture.jpg")
                },
                {
                    PlanetType.Earth,
                    new SpaceObject(0.91f, 250, 250, 29.78f, 29.0f, "Resources/earth_texture.jpg")
                },
                {
                    PlanetType.Mars,
                    new SpaceObject(0.49f, 125, 125, 24.13f, 33.0f, "Resources/mars_texture.jpg")
                },
                {
                    PlanetType.Jupiter,
                    new SpaceObject(2.0f, 500, 500, 13.07f, 45.0f, "Resources/jupiter_texture.jpg")
                },
                {
                    PlanetType.Saturn,
                    new SpaceObject(1.66f, 450, 450, 9.69f, 58.0f, "Resources/saturn_texture.jpg")
                },
                {
                    PlanetType.Uranus,
                    new SpaceObject(1.42f, 350, 350, 6.81f, 71.0f, "Resources/uranus_texture.jpg")
                },
                {
                    PlanetType.Neptune,
                    new SpaceObject(1.38f, 350, 350, 5.43f, 80.0f, "Resources/neptune_texture.jpg")
                },
            };

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            LightingShaderSetup();

            _time += 0.5 * e.Time;

            // Рендер планет
            foreach (var planet in _solarSystemPlanets)
            {
                var spaceObjectOrbitMatrix = Matrix4.CreateRotationY(
                    (float)MathHelper.DegreesToRadians(
                        _time * planet.Value.OrbitVelocity));

                var spaceObjectModel = Matrix4.Identity *
                    Matrix4.CreateTranslation(Vector3.UnitX * planet.Value.Offset);

                spaceObjectModel *= spaceObjectOrbitMatrix;

                _lightingShader.SetMatrix4("model", spaceObjectModel);
                planet.Value.Render();
            }

            // Рендер Солнца
            LampShaderSetup();            
            _sun.Render();

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

            const float cameraSpeed = 15.0f;
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

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {            
            _camera.Fov -= e.OffsetY;

            base.OnMouseWheel(e);
        }

        private void LightingShaderSetup()
        {
            _lightingShader.Use();
            _lightingShader.SetMatrix4("model", Matrix4.Identity);
            _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _lightingShader.SetVector3("viewPos", _camera.Position);
            _lightingShader.SetInt("material.diffuse", 0);
            _lightingShader.SetInt("material.specular", 1);
            _lightingShader.SetVector3("material.specular", new Vector3(0.5f));
            _lightingShader.SetFloat("material.shininess", 30.0f);

            _lightingShader.SetVector3("light.position", new Vector3(0.0f));
            _lightingShader.SetVector3("light.ambient", new Vector3(0.2f));
            _lightingShader.SetVector3("light.diffuse", new Vector3(0.7f));
            _lightingShader.SetVector3("light.specular", new Vector3(1.0f));
        }

        private void LampShaderSetup()
        {
            _lampShader.Use();
            _lampShader.SetMatrix4("model", Matrix4.Identity);
            _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        }
    }
}
