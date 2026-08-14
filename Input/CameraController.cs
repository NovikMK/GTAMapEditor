using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GtaEditor.Input
{
    /// <summary>
    /// Свободная игровая камера редактора.
    ///
    /// Управление:
    ///
    /// RMB       - захват камеры и обзор мышью.
    /// W         - вперёд.
    /// S         - назад.
    /// A         - влево.
    /// D         - вправо.
    /// Q         - вниз.
    /// E         - вверх.
    /// Shift     - ускорение.
    /// Ctrl      - медленное движение.
    /// Колесо    - изменение скорости камеры.
    /// </summary>
    public class CameraController
    {
        // ============================================================
        // ПОЛОЖЕНИЕ КАМЕРЫ
        // ============================================================

        private Vector3 _position = new Vector3(
            0.0f,
            5.0f,
            10.0f
        );

        // ============================================================
        // НАПРАВЛЕНИЕ КАМЕРЫ
        // ============================================================

        private float _yaw = -90.0f;

        private float _pitch = -15.0f;


        // ============================================================
        // СКОРОСТЬ
        // ============================================================

        private float _moveSpeed = 10.0f;

        private const float MinMoveSpeed = 0.1f;

        private const float MaxMoveSpeed = 5000.0f;

        private const float SpeedStep = 1.25f;


        // ============================================================
        // НАСТРОЙКИ МЫШИ
        // ============================================================

        private const float MouseSensitivity = 0.01f;


        // ============================================================
        // ПУБЛИЧНЫЕ СВОЙСТВА
        // ============================================================

        /// <summary>
        /// Текущая позиция камеры.
        /// </summary>
        public Vector3 Position => _position;


        /// <summary>
        /// Текущая скорость камеры.
        /// </summary>
        public float MoveSpeed => _moveSpeed;


        /// <summary>
        /// Направление взгляда камеры.
        /// </summary>
        public Vector3 Forward => GetForwardVector();


        /// <summary>
        /// Матрица камеры.
        /// </summary>
        public Matrix4 ViewMatrix
        {
            get
            {
                return Matrix4.LookAt(
                    _position,
                    _position + GetForwardVector(),
                    Vector3.UnitY
                );
            }
        }


        // ============================================================
        // ВРАЩЕНИЕ МЫШЬЮ
        // ============================================================

        /// <summary>
        /// Вращает свободную камеру мышью.
        /// </summary>
        public void Look(
            float deltaX,
            float deltaY)
        {
            _yaw += deltaX * MouseSensitivity;

            _pitch -= deltaY * MouseSensitivity;

            // Не позволяем камере перевернуться.
            _pitch = MathHelper.Clamp(
                _pitch,
                -89.0f,
                89.0f
            );
        }


        // ============================================================
        // ДВИЖЕНИЕ
        // ============================================================

        /// <summary>
        /// Свободное перемещение камеры.
        /// </summary>
        public void Move(
            KeyboardState keyboard,
            float deltaTime)
        {
            Vector3 forward = GetForwardVector();

            Vector3 right = GetRightVector();

            Vector3 movement = Vector3.Zero;


            // ========================================================
            // ВПЕРЁД / НАЗАД
            // ========================================================

            if (keyboard.IsKeyDown(Keys.W))
            {
                movement += forward;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                movement -= forward;
            }


            // ========================================================
            // ВЛЕВО / ВПРАВО
            // ========================================================

            if (keyboard.IsKeyDown(Keys.A))
            {
                movement -= right;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                movement += right;
            }


            // ========================================================
            // ВВЕРХ / ВНИЗ
            // ========================================================

            if (keyboard.IsKeyDown(Keys.E))
            {
                movement += Vector3.UnitY;
            }

            if (keyboard.IsKeyDown(Keys.Q))
            {
                movement -= Vector3.UnitY;
            }


            // ========================================================
            // ЕСЛИ НЕТ ДВИЖЕНИЯ
            // ========================================================

            if (movement.LengthSquared <= 0.000001f)
            {
                return;
            }


            // ========================================================
            // НОРМАЛИЗАЦИЯ
            // ========================================================

            movement = Vector3.Normalize(movement);


            // ========================================================
            // СКОРОСТЬ
            // ========================================================

            float speed = _moveSpeed;


            // Shift = быстро
            if (keyboard.IsKeyDown(Keys.LeftShift) ||
                keyboard.IsKeyDown(Keys.RightShift))
            {
                speed *= 5.0f;
            }


            // Ctrl = медленно
            if (keyboard.IsKeyDown(Keys.LeftControl) ||
                keyboard.IsKeyDown(Keys.RightControl))
            {
                speed *= 0.2f;
            }


            // ========================================================
            // ПЕРЕМЕЩЕНИЕ
            // ========================================================

            _position += movement * speed * deltaTime;
        }


        // ============================================================
        // ИЗМЕНЕНИЕ СКОРОСТИ КОЛЁСИКОМ
        // ============================================================

        /// <summary>
        /// Изменяет скорость свободного полёта.
        ///
        /// Это удобно для редактора:
        /// далеко от карты можно лететь быстро,
        /// возле объекта — медленно.
        /// </summary>
        public void ChangeMoveSpeed(
            float scroll)
        {
            if (scroll > 0.0f)
            {
                _moveSpeed *= SpeedStep;
            }
            else if (scroll < 0.0f)
            {
                _moveSpeed /= SpeedStep;
            }

            _moveSpeed = MathHelper.Clamp(
                _moveSpeed,
                MinMoveSpeed,
                MaxMoveSpeed
            );
        }


        // ============================================================
        // УСТАНОВКА ПОЗИЦИИ
        // ============================================================

        public void SetPosition(
            Vector3 position)
        {
            _position = position;
        }


        // ============================================================
        // НАПРАВЛЕНИЕ ВПЕРЁД
        // ============================================================

        private Vector3 GetForwardVector()
        {
            float yaw =
                MathHelper.DegreesToRadians(_yaw);

            float pitch =
                MathHelper.DegreesToRadians(_pitch);


            Vector3 direction = new Vector3(
                MathF.Cos(pitch) * MathF.Cos(yaw),
                MathF.Sin(pitch),
                MathF.Cos(pitch) * MathF.Sin(yaw)
            );


            return Vector3.Normalize(
                direction
            );
        }


        // ============================================================
        // НАПРАВЛЕНИЕ ВПРАВО
        // ============================================================

        private Vector3 GetRightVector()
        {
            Vector3 forward =
                GetForwardVector();

            return Vector3.Normalize(
                Vector3.Cross(
                    forward,
                    Vector3.UnitY
                )
            );
        }
    }
}