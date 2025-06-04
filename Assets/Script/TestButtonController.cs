using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestButtonController : MonoBehaviour
{
    public void GameStartButtonAction()
    {
        SceneManager.LoadScene("TestScene");
    }
}
