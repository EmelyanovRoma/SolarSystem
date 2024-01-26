namespace SolarSystemTest
{
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using System.Drawing.Imaging;
    using System.Drawing;
    using SolarSystem;
    using System.Reflection.Metadata;

    class Sphere
    {
        private Vector3[] _vertices;
        private Vector3[] _normals;
        private int[] _indices;
        private Vector2[] _textureCoordinates;

        private int _vaoModel;
        private int _vertexBufferObject;
        private int _normalBufferObject;
        private int _indexBufferObject;
        private int _texCoordBufferObject;
        private int _indexCount;

        private Texture _textureObejct;

        public float OrbitVelocity { get; set; }
        public float Offset { get; set; }

        public Sphere(float radius, int slices, int stacks, float orbitVelocity, float offset, string texturePath)
        {
            Offset = offset;
            OrbitVelocity = orbitVelocity;
            _vertices = CalculateSphereVertices(radius, slices, stacks);
            _normals = CalculateSphereNormals(_vertices, slices, stacks);
            _indices = CalculateSphereIndices(slices, stacks);
            _textureCoordinates = CalculateSphereTextureCoordinates(slices, stacks);
            _indexCount = _indices.Length;

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vector3.SizeInBytes * _vertices.Length), _vertices, BufferUsageHint.StaticDraw);

            _normalBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _normalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * _vertices.Length, _normals, BufferUsageHint.StaticDraw);

            _texCoordBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _texCoordBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vector2.SizeInBytes * _textureCoordinates.Length), _textureCoordinates, BufferUsageHint.StaticDraw);

            _indexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(int) * _indices.Length), _indices, BufferUsageHint.StaticDraw);


            _vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(_vaoModel);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _normalBufferObject);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _texCoordBufferObject);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
            GL.BindVertexArray(0);

            _textureObejct = Texture.LoadFromFile(texturePath);
        }

        private Vector3[] CalculateSphereVertices(float radius, int slices, int stacks)
        {
            int numVertices = (slices + 1) * (stacks + 1);
            Vector3[] vertices = new Vector3[numVertices];

            for (int i = 0; i <= stacks; i++)
            {
                float stackAngle = (float)Math.PI / 2 - (float)i * (float)Math.PI / stacks;
                float xy = radius * (float)Math.Cos(stackAngle);
                float z = radius * (float)Math.Sin(stackAngle);

                for (int j = 0; j <= slices; j++)
                {
                    float sectorAngle = (float)j * 2 * (float)Math.PI / slices;
                    float x = xy * (float)Math.Cos(sectorAngle);
                    float y = xy * (float)Math.Sin(sectorAngle);

                    vertices[i * (slices + 1) + j] = new Vector3(x, y, z);
                }
            }

            return vertices;
        }

        private Vector3[] CalculateSphereNormals(Vector3[] vertices, int slices, int stacks)
        {
            Vector3[] normals = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                normals[i] = Vector3.Normalize(vertices[i]);
            }

            return normals;
        }

        private Vector2[] CalculateSphereTextureCoordinates(int slices, int stacks)
        {
            int numVertices = (slices + 1) * (stacks + 1);
            Vector2[] textureCoordinates = new Vector2[numVertices];

            for (int i = 0; i <= stacks; i++)
            {
                for (int j = 0; j <= slices; j++)
                {
                    float s = (float)j / slices;
                    float t = (float)i / stacks;
                    textureCoordinates[i * (slices + 1) + j] = new Vector2(s, t);
                }
            }

            return textureCoordinates;
        }

        private int[] CalculateSphereIndices(int slices, int stacks)
        {
            int numIndices = slices * stacks * 6;
            int[] indices = new int[numIndices];
            int index = 0;

            for (int i = 0; i < stacks; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    int k1 = i * (slices + 1) + j;
                    int k2 = k1 + slices + 1;

                    indices[index++] = k1;
                    indices[index++] = k2;
                    indices[index++] = k1 + 1;

                    indices[index++] = k1 + 1;
                    indices[index++] = k2;
                    indices[index++] = k2 + 1;
                }
            }

            return indices;
        }

        public void Render()
        {
            GL.BindVertexArray(_vaoModel);
            _textureObejct.Use(TextureUnit.Texture0);
            GL.DrawElements(BeginMode.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }    
}
