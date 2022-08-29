using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Input
{
    public class Mouse
    {

        private Vector2 _previousPosition;
        private Vector2 _currentPosition;

        private HashSet<MouseButton> _previousMouseButtons = new HashSet<MouseButton>();
        private HashSet<MouseButton> _currentMouseButtons = new HashSet<MouseButton>();

        private InputSnapshot _currentState { get; set; } = new EmptyInputSnapshot();

        public Vector2 Position { get => _currentPosition; }
        public Vector2 Movement { get => _currentPosition - _previousPosition; }

        public float Scroll { get => _currentState.WheelDelta; }

        public bool IsButtonDown(MouseButton button)
            => IsDown(_currentMouseButtons, button);
        public bool IsButtonUp(MouseButton button)
            => !IsDown(_currentMouseButtons, button);
        public bool IsButtonPressed(MouseButton button)
            => IsDown(_previousMouseButtons, button) && IsDown(_currentMouseButtons, button);
        public bool IsButtonClicked(MouseButton button)
            => !IsDown(_previousMouseButtons, button) && IsDown(_currentMouseButtons, button);
        public bool IsButtonReleased(MouseButton button)
            => IsDown(_previousMouseButtons, button) && !IsDown(_currentMouseButtons, button);

        public delegate void OnButtonEventHandler(MouseButton button);
        public event OnButtonEventHandler OnButtonDown;
        public event OnButtonEventHandler OnButtonUp;
        public event OnButtonEventHandler OnButtonPressed;
        public event OnButtonEventHandler OnButtonClicked;
        public event OnButtonEventHandler OnButtonReleased;

        public void Update(InputSnapshot snapshot)
        {
            _previousPosition = _currentPosition;
            _currentPosition = snapshot.MousePosition;

            _previousMouseButtons = _currentMouseButtons.ToHashSet();
            foreach (var mouseEvent in snapshot.MouseEvents)
            {
                if (mouseEvent.Down) _currentMouseButtons.Add(mouseEvent.MouseButton);
                else _currentMouseButtons.Remove(mouseEvent.MouseButton);
            }

            InvokeButtonEvents();
        }

        private bool IsDown(HashSet<MouseButton> buttonEvents, MouseButton button)
        {
            return buttonEvents.Contains(button);
        }

        private void InvokeButtonEvents()
        {
            foreach (var button in Enum.GetValues(typeof(MouseButton)).Cast<MouseButton>())
            {
                if (IsButtonDown(button))
                {
                    OnButtonDown?.Invoke(button);
                    if (IsButtonPressed(button)) OnButtonPressed?.Invoke(button);
                    if (IsButtonClicked(button)) OnButtonClicked?.Invoke(button);
                }
                else
                {
                    OnButtonUp?.Invoke(button);
                    if (IsButtonReleased(button)) OnButtonReleased?.Invoke(button);
                }
            }
        }

    }
}
