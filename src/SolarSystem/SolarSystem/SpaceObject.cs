namespace SolarSystem
{
    using OpenTK.Graphics.OpenGL4;

    public class SpaceObject : Sphere
    {
        private Texture _textureObejct;

        public float OrbitVelocity { get; set; }

        public float Offset { get; set; }

        public SpaceObject(
            float radius,
            int slices,
            int stacks,
            float orbitVelocity,
            float offset,
            string texturePath)
            : base(radius, slices, stacks)
        {
            Offset = offset;
            OrbitVelocity = orbitVelocity;
            _textureObejct = Texture.LoadFromFile(texturePath);
        }

        public override void Render()
        {
            _textureObejct.Use(TextureUnit.Texture0);
            base.Render();
        }
    }
}
