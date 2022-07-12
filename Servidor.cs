using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


public static class JsonHelper {
    public static T[] FromJson<T>(string json) {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T> {
        public T[] Items;
    }
}

[System.Serializable]
class Agent {
    public int x;
    public int y;
    public string tipo;
    public bool horizontal;
}

public class Servidor : MonoBehaviour {
    string simulationURL = null;
    private float waitTime = 0.1f;
    private float timer = 0.0f;

    public Quaternion Semaforo;

    public GameObject prefabAgente;
    private int NumObjetos = 2;
    List<GameObject> listaObj;

    public GameObject prefabSemaforo;
    private int NumSemaforos = 4;
    List<GameObject> listaSemaforos;

    void Start(){
        listaObj = new List<GameObject>();
        for (int i = 0; i < NumObjetos; i++){
            float x = 0.1f;
            float y = 0f;
            float z = 0.1f;
            listaObj.Add(Instantiate(prefabAgente, new Vector3(x, y, z), Quaternion.identity));
        }

        listaSemaforos = new List<GameObject>();
        for (int i = 0; i < NumSemaforos; i++){
            float x = 0.02f;
            float y = 0f;
            float z = 0.02f;
            listaSemaforos.Add(Instantiate(prefabSemaforo, new Vector3(x, y, z), Quaternion.identity));
        }
        StartCoroutine(ConnectToMesa());
    }

    IEnumerator ConnectToMesa() {
        WWWForm form = new WWWForm();
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:8000/reto", form)) {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.error);
            }
            else {
                simulationURL = www.GetResponseHeader("Location");
                Debug.Log("Connected to simulation through Web API");
                Debug.Log(simulationURL);
            }
        }
    }

    IEnumerator UpdatePositions() {
        using (UnityWebRequest www = UnityWebRequest.Get(simulationURL)) {
            if (simulationURL != null) {
                yield return www.SendWebRequest();

                Debug.Log("Data has been processed");
                Agent[] agents = JsonHelper.FromJson<Agent>(www.downloadHandler.text);
                int i = 0;
                int j = 0;
                foreach(Agent agent in agents) {
                    if (agent.tipo == "Auto") {
                        listaObj[i].transform.position = new Vector3(agent.x, 0.6f, agent.y);
                        if (agent.horizontal == false)
                        {
                            listaObj[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                        }
                        else
                        {
                            listaObj[i].transform.rotation = Quaternion.Euler(0, -90, 0);
                        }
                        i++;
                    }else {
                        listaSemaforos[j].transform.position = new Vector3(agent.x, 0.6f, agent.y);
                        listaSemaforos[j].transform.rotation = Quaternion.Euler(-90, 0, 0);
                        if (agent.tipo == "trafficLightGreen")
                        {
                            listaSemaforos[j].GetComponent<Semaforo>().Rojo = false;
                        }
                        else
                            listaSemaforos[j].GetComponent<Semaforo>().Rojo = true;
                        j++;
                    }
                }
            }
        }
    }

    void Update() {
        timer += Time.deltaTime;
        if (timer > waitTime) {
            StartCoroutine(UpdatePositions());
            timer = timer - waitTime;
        }   
    }
}
