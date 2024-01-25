namespace SolarSystemTest
{
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    public class Planet
    {
        private int _elementBufferObject;
        private int _vertexBufferObject;
        public int _vertexArrayObject;

        private List<int> indices = new List<int>();
        private List<float> vertices = new List<float>();
                
        public Planet(float radius, int slices, int stacks)
        {
            for (int i = 0; i <= stacks; i++)
            {
                float phi = i * MathHelper.Pi / stacks;

                for (int j = 0; j <= slices; j++)
                {
                    float theta = j * 2.0f * MathHelper.Pi / slices;

                    float x = (float)(radius * Math.Sin(phi) * Math.Cos(theta));
                    float y = (float)(radius * Math.Cos(phi));
                    float z = (float)(radius * Math.Sin(phi) * Math.Sin(theta));

                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);
                }
            }

            for (int i = 0; i < stacks; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    indices.Add(i * (slices + 1) + j);
                    indices.Add((i + 1) * (slices + 1) + j);
                    indices.Add(i * (slices + 1) + j + 1);

                    indices.Add((i + 1) * (slices + 1) + j);
                    indices.Add((i + 1) * (slices + 1) + j + 1);
                    indices.Add(i * (slices + 1) + j + 1);
                }
            }

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);
        }

        public virtual void Render()
        {
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
