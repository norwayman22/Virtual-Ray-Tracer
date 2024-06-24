using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

    [SerializeField]
    private Animator transition;

    [SerializeField]
    private float transitionTime = 1f;

    [SerializeField]
    UnityEvent OnSceneLoad;

    /// <summary>
    /// Get the current SceneLoader instance.
    /// </summary>
    /// <returns>The current SceneLoader instance</returns>
    public static SceneLoader Get()
    {
        return instance;
    }

    public void LoadScene(int index)
    {
        OnSceneLoad?.Invoke();
        StartCoroutine(LoadSceneInternal(index));
    }

    IEnumerator LoadSceneInternal(int index)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadSceneAsync(index);
    }

    private void Awake()
    {
        instance = this;
    }
}
