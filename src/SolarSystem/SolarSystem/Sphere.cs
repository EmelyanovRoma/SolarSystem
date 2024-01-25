namespace SolarSystemTest
{
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using System.Drawing.Imaging;
    using System.Drawing;

    class Sphere
    {
        public int vaoHandle;
        public int vertexBufferHandle;
        public int indexBufferHandle;
        public int textureCoordBufferHandle;
        public int indexCount;
        public int textureHandle;
        public int textureSpecularHandle;
        public Vector3 LightPosition { get; set; }
        public Vector3 LightColor { get; set; }

        public Sphere(float radius, int slices, int stacks)
        {           
            Vector3[] vertices = CalculateSphereVertices(radius, slices, stacks);
            int[] indices = CalculateSphereIndices(slices, stacks);
            Vector2[] textureCoordinates = CalculateSphereTextureCoordinates(slices, stacks);
            
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vector3.SizeInBytes * vertices.Length), vertices, BufferUsageHint.StaticDraw);
            
            indexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(int) * indices.Length), indices, BufferUsageHint.StaticDraw);
            indexCount = indices.Length;
            
            textureCoordBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureCoordBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vector2.SizeInBytes * textureCoordinates.Length), textureCoordinates, BufferUsageHint.StaticDraw);
            
            vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(vaoHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureCoordBufferHandle);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
            GL.BindVertexArray(0);                        
            
            textureHandle = LoadTexture("../../../Resources/earth8k.jpg");
            textureSpecularHandle = LoadTexture("../../../Resources/earth8k_specular.jpg");
        }

        public void SetLight(Vector3 position, Vector3 color)
        {
            LightPosition = position;
            LightColor = color;
        }

        private int LoadTexture(string path)
        {
            Bitmap bitmap = new Bitmap(path);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return textureHandle;
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
            GL.BindVertexArray(vaoHandle);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, textureSpecularHandle);
            GL.DrawElements(BeginMode.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }    
}
