using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class MainMenu : MonoBehaviour
    {
        public void GotoGame()
        {
            SceneManager.LoadSceneAsync("Game");
        }
    }
}
