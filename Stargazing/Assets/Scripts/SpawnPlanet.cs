using System.Collections.Generic;
using UnityEngine;

public class SpawnPlanet : MonoBehaviour
{
    public List<GameObject> prefabObj;
    public float distanciaDaCamera = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main != null && prefabObj != null)
        {

            foreach (var i in prefabObj)
            {
                Vector3 posicaoAlvo = Camera.main.transform.position 
                    + Camera.main.transform.forward * (Random.Range(0, 10));

                Instantiate(i, posicaoAlvo, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
