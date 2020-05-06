using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public float transitionTime = 1f;

    public Animator transition;

    public void OnStartButton()
    {
        StartCoroutine(Load(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void OnRestartButton()
    {
        StartCoroutine(Load(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator Load(int lvl)
    {
        transition.SetTrigger("Load");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(lvl);
    }

}
