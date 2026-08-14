using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GtaEditor.Rendering
{
    public class ShaderProgram : IDisposable
    {
        public int Handle { get; private set; }

        /// <summary>
        /// Загружает шейдеры из Embedded Resources.
        /// Путь указывается как "Namespace.Folder.FileName", например:
        /// "GtaEditor.Assets.shader.vert"
        /// </summary>
        public ShaderProgram(string vertexResourceName, string fragmentResourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            string vertSource = LoadEmbeddedResource(assembly, vertexResourceName);
            string fragSource = LoadEmbeddedResource(assembly, fragmentResourceName);

            int vertShader = CompileShader(ShaderType.VertexShader, vertSource);
            int fragShader = CompileShader(ShaderType.FragmentShader, fragSource);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertShader);
            GL.AttachShader(Handle, fragShader);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
                throw new Exception($"Shader link error: {GL.GetProgramInfoLog(Handle)}");

            GL.DeleteShader(vertShader);
            GL.DeleteShader(fragShader);
        }

        private static string LoadEmbeddedResource(Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException(
                    $"Embedded resource '{resourceName}' not found. " +
                    $"Available resources: [{string.Join(", ", assembly.GetManifestResourceNames())}]");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public void Use() => GL.UseProgram(Handle);

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public void SetVector3(string name, Vector3 vector)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform3(location, vector);
        }

        private static int CompileShader(ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
                throw new Exception($"{type} compile error: {GL.GetShaderInfoLog(shader)}");

            return shader;
        }

        public void Dispose() => GL.DeleteProgram(Handle);
    }
}