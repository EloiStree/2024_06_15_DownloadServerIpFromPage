using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class DownloadPageMono_AsText : MonoBehaviour
{

    public string m_pageURL = "https://raw.githubusercontent.com/EloiStree/IP/main/DroneSoccerXR/SERVER.md";
    [TextArea(0,10)]
    public string m_downloadContent;

    public UnityEvent<string> m_onLoadedText;
    public UnityEvent m_onFail;
    public bool m_useTrim = true;

    IEnumerator Start()
    {
        yield return Coroutine_LoadFromWeb();
    }

    [ContextMenu("Load from Web")]
    public void LoadFromWeb()
    {
        StartCoroutine(Coroutine_LoadFromWeb());
    }

    private IEnumerator Coroutine_LoadFromWeb()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(m_pageURL))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                m_downloadContent = webRequest.downloadHandler.text;
                if (m_useTrim)
                    m_downloadContent = m_downloadContent.Trim();
                m_onLoadedText.Invoke(m_downloadContent);
            }
            else
            {
                m_onFail.Invoke();
                Debug.LogError("Failed to download file: " + webRequest.error);
            }
        }
    }
}