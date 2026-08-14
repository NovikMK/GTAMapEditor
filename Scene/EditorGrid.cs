using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using GtaEditor.Rendering;

namespace GtaEditor.Scene
{
    public class EditorGrid : IDisposable
    {
        private int _vao, _vbo;
        private readonly int _vertexCount;

        public EditorGrid(int size = 100, float step = 1.0f)
        {
            var vertices = new List<float>();
            float half = (size * step) / 2f;

            // Формат вершины: X, Y, Z, R, G, B
            void AddLine(float x1, float y1, float z1, float x2, float y2, float z2, Color4 color)
            {
                vertices.AddRange(new[] { x1, y1, z1, color.R, color.G, color.B });
                vertices.AddRange(new[] { x2, y2, z2, color.R, color.G, color.B });
            }

            // Оси координат
            AddLine(-half, 0, 0, half, 0, 0, Color4.Red);   // X
            AddLine(0, 0, -half, 0, 0, half, Color4.Blue);  // Z
            AddLine(0, -half, 0, 0, half, 0, Color4.Green); // Y

            // Сетка
            Color4 gridColor = new Color4(0.3f, 0.3f, 0.3f, 1.0f);
            for (int i = 0; i <= size; i++)
            {
                float pos = -half + (i * step);
                AddLine(pos, 0, -half, pos, 0, half, gridColor);
                AddLine(-half, 0, pos, half, 0, pos, gridColor);
            }

            _vertexCount = vertices.Count / 6;

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                vertices.Count * sizeof(float),
                vertices.ToArray(),
                BufferUsageHint.StaticDraw);

            // Position (location = 0)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Color (location = 1)
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        public void Draw(ShaderProgram shader)
        {
            shader.Use();
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Lines, 0, _vertexCount);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
        }
    }
}