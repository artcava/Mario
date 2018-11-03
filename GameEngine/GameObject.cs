using System;
using SFML.Window;
using SFML.Graphics;

namespace GameEngine
{
    public class GameObject : IDisposable
    {
        private RenderWindow _window;

        public RenderWindow Window { get { return _window; } }
        public SceneManager SceneManager = new SceneManager();
        

        public GameObject(string Title)
        {
            // Initialize values
            _window = new RenderWindow(new VideoMode(1024u, 768u), Title, Styles.Default);

            _window.SetVisible(true);
            _window.SetVerticalSyncEnabled(true);
            _window.SetFramerateLimit(30);
            

            // Set up event handlers
            _window.Closed += _window_Closed;
            _window.KeyPressed += _window_KeyPressed;
            _window.KeyReleased += _window_KeyReleased;
        }

        void _window_KeyPressed(object sender, KeyEventArgs e)
        {
            SceneManager.CurrentScene.HandleKeyPress(e);
        }

        void _window_KeyReleased(object sender, KeyEventArgs e)
        {
            SceneManager.CurrentScene.HandleKeyReleased(e);
        }

        void _window_Closed(object sender, EventArgs e)
        {
            _window.Close();
        }

        public void Close()
        {
            _window.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _window.Dispose();
            }
        }
    }
}
