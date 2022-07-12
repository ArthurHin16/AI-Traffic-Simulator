using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Semaforo : MonoBehaviour
{
    public GameObject luz;
    public Transform posVerde;
    public Transform posAmarilla;
    public Transform posRoja;
    public bool Rojo;



    private bool verde;
    private bool amarilloDesdeVerde;
    private bool verdeDesdeRoja;
    private bool roja;


    private void Start()
    {
        verde = true;
    }

    void Update()
    {

        if (Rojo == false)
        {
            luz.transform.position = posVerde.position;
            luz.GetComponent<Light>().color = new Color32(61, 161, 27, 255);
            verdeDesdeRoja = false;
        }

         if (Rojo == true)
        {
            luz.transform.position = posRoja.position;
            luz.GetComponent<Light>().color = Color.red;
            amarilloDesdeVerde = false;
        }
       
    }
}

