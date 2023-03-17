using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Kraft.ApplicationLifecycle
{
    public class ApplicationController : LifetimeScope
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("World");
        }
    }
}
