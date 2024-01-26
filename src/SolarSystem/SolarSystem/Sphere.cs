namespace SolarSystem
{
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    public class Sphere
    {
        private float _radius;
        private int _slices;
        private int _stacks;

        private Vector3[] _vertices;
        private Vector3[] _normals;
        private Vector2[] _textureCoords;
        private int[] _indices;
        private int _indexCount;

        private int _vaoModel;
        private int _vertexBufferObject;
        private int _normalBufferObject;
        private int _indexBufferObject;
        private int _texCoordBufferObject;        

        public float Radius 
        {
            get => _radius;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException(
                        $"Радиус должен быть больше нуля");
                }

                _radius = value;
            }
        }

        public int Slices
        {
            get => _slices; 
            set
            {
                if (value <= 15)
                {
                    throw new ArgumentException(
                        $"Количество вертикальных сегментов должно быть больше 15");
                }

                _slices = value;
            }
        }

        public int Stacks
        {
            get => _stacks;
            set
            {
                if (value <= 15)
                {
                    throw new ArgumentException(
                        $"Количество горизонтальных сегментов должно быть больше 15");
                }

                _stacks = value;
            }
        }

        public Sphere(float radius, int slices, int stacks)
        {
            Radius = radius;
            Slices = slices;
            Stacks = stacks;

            _vertices = CalculateSphereVertices();
            _normals = CalculateSphereNormals();
            _indices = CalculateSphereIndices();
            _textureCoords = CalculateSphereTextureCoordinates();
            _indexCount = _indices.Length;

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * _vertices.Length,
                _vertices, BufferUsageHint.StaticDraw);

            _normalBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _normalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * _vertices.Length,
                _normals, BufferUsageHint.StaticDraw);

            _texCoordBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _texCoordBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * _textureCoords.Length,
                _textureCoords, BufferUsageHint.StaticDraw);

            _indexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * _indices.Length,
                _indices, BufferUsageHint.StaticDraw);


            _vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(_vaoModel);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
                Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _normalBufferObject);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
                Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _texCoordBufferObject);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false,
                Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
            GL.BindVertexArray(0);            
        }

        private Vector3[] CalculateSphereVertices()
        {
            int numVertices = (Slices + 1) * (Stacks + 1);
            Vector3[] vertices = new Vector3[numVertices];

            for (int i = 0; i <= Stacks; i++)
            {
                float stackAngle = (float)Math.PI / 2 - (float)i * (float)Math.PI / Stacks;
                float xy = Radius * (float)Math.Cos(stackAngle);
                float z = Radius * (float)Math.Sin(stackAngle);

                for (int j = 0; j <= Slices; j++)
                {
                    float sectorAngle = (float)j * 2 * (float)Math.PI / Slices;
                    float x = xy * (float)Math.Cos(sectorAngle);
                    float y = xy * (float)Math.Sin(sectorAngle);

                    vertices[i * (Slices + 1) + j] = new Vector3(x, y, z);
                }
            }

            return vertices;
        }

        private Vector3[] CalculateSphereNormals()
        {
            Vector3[] normals = new Vector3[_vertices.Length];

            for (int i = 0; i < _vertices.Length; i++)
            {
                normals[i] = Vector3.Normalize(_vertices[i]);
            }

            return normals;
        }

        private Vector2[] CalculateSphereTextureCoordinates()
        {
            int numVertices = (Slices + 1) * (Stacks + 1);
            Vector2[] textureCoordinates = new Vector2[numVertices];

            for (int i = 0; i <= Stacks; i++)
            {
                for (int j = 0; j <= Slices; j++)
                {
                    float s = (float)j / Slices;
                    float t = (float)i / Stacks;
                    textureCoordinates[i * (Slices + 1) + j] = new Vector2(s, t);
                }
            }

            return textureCoordinates;
        }

        private int[] CalculateSphereIndices()
        {
            int numIndices = Slices * Stacks * 6;
            int[] indices = new int[numIndices];
            int index = 0;

            for (int i = 0; i < Stacks; i++)
            {
                for (int j = 0; j < Slices; j++)
                {
                    int k1 = i * (Slices + 1) + j;
                    int k2 = k1 + Slices + 1;

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

        public virtual void Render()
        {
            GL.BindVertexArray(_vaoModel);
            GL.DrawElements(
                BeginMode.Triangles, _indexCount,
                DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }    
}
