using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;

public class splashCTRL : MonoBehaviour
{
    public VideoPlayer vp;
    public TextMeshProUGUI tmp;
    public int last;
    int next;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Count());

        // 
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            last--;
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Next(last);
        }
    }

    IEnumerator Count()
    {
        yield return new WaitForSeconds(3);
        Next(next);
        next++;
        StartCoroutine(Count());
    }

    void Next(int which)
    {

        if (which == last)
        {
            SceneManager.LoadScene("mainMenu");
        }
        else if (which == 0)
        {
            vp.enabled = false;
            tmp.gameObject.SetActive(true);
        }
    }
}
