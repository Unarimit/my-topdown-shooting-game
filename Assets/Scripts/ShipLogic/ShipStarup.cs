using Assets.Scripts.HomeLogic.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ShipLogic
{
    public class ShipStarup : MonoBehaviour
    {

        [SerializeField]
        LightManager lightManager;
        // Start is called before the first frame update
        void Start()
        {
            lightManager.InSpace();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
