using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class DownloadPageMono_AsIpPort : MonoBehaviour
{

    public string m_pageURL = "https://raw.githubusercontent.com/EloiStree/IP/main/DroneSoccerXR/SERVER.md";
    public string m_downloadContent;

    public UnityEvent<string> m_onLoaded;
    public UnityEvent m_onFail;
    public bool m_useTrim = true;


    public string m_ipFound;
    public int m_portFound;

    public UnityEvent<string> m_onIpFound;
    public UnityEvent<int> m_onPortFound;

  

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

                string ipFound= "";
                int portFound = 0;

                string[] split = m_downloadContent.Split(':');
                if (split.Length == 2)
                {
                    if (int.TryParse(split[1], out int port))
                    {
                        ipFound = split[0];
                        portFound = port;
                    }
                }
                else { 
                
                    string[] lines = m_downloadContent.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = lines[i].Trim().ToUpper();
                    }
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("IP:"))
                        {
                            ipFound = line.Replace("IP:", "").Trim();
                        }
                        if (line.StartsWith("PORT:"))
                        {
                            string portStr = line.Substring("PORT:".Length).Trim();
                            if (int.TryParse(portStr, out int port))
                            {
                                portFound = port;
                            }
                        }
                    }
                    
                    foreach (string line in lines)
                    {
                        if( line.Length > 0 )
                        {
                            if( int.TryParse(line,out int port))
                            {
                                if (portFound == 0)
                                    portFound = port;
                            }
                            else
                            {
                                if(ipFound.Length == 0)
                                    ipFound = line;
                            }   
                            
                        }
                    }

                }

                m_ipFound = ipFound;
                m_portFound = portFound;

                m_onIpFound.Invoke(ipFound);
                m_onPortFound.Invoke(portFound);
                m_onLoaded.Invoke(m_downloadContent);
            }
            else
            {
                m_onFail.Invoke();
                Debug.LogError("Failed to download file: " + webRequest.error);
            }
        }
    }
}