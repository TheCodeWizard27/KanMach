using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace KanMach.Veldrid.Input
{
    public class Keyboard
    {

        private HashSet<Key> _previousKeys = new HashSet<Key>();
        private HashSet<Key> _currentKeys = new HashSet<Key>();

        public bool IsButtonDown(Key button)
            => IsDown(_currentKeys, button);
        public bool IsButtonUp(Key button)
            => !IsDown(_currentKeys, button);
        public bool IsButtonPressed(Key button)
            => IsDown(_previousKeys, button) && IsDown(_currentKeys, button);
        public bool IsButtonClicked(Key button)
            => !IsDown(_previousKeys, button) && IsDown(_currentKeys, button);
        public bool IsButtonReleased(Key button)
            => IsDown(_previousKeys, button) && !IsDown(_currentKeys, button);

        public delegate void OnButtonEventHandler(Key button);
        public event OnButtonEventHandler OnButtonDown;
        public event OnButtonEventHandler OnButtonUp;
        public event OnButtonEventHandler OnButtonPressed;
        public event OnButtonEventHandler OnButtonClicked;
        public event OnButtonEventHandler OnButtonReleased;

        public void Update(InputSnapshot snapshot)
        {
            _previousKeys = _currentKeys.ToHashSet();
            foreach(var keyEvent in snapshot.KeyEvents)
            {
                if (keyEvent.Down) _currentKeys.Add(keyEvent.Key);
                else _currentKeys.Remove(keyEvent.Key);
            }

            InvokeButtonEvents();
        }

        private bool IsDown(HashSet<Key> keys, Key button)
        {
            return keys.Contains(button);
        }

        private void InvokeButtonEvents()
        {
            foreach (var button in Enum.GetValues(typeof(Key)).Cast<Key>())
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
