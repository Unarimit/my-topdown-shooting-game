using Assets.Scripts.ComputerControllers;
using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class ViewChangeManager : MonoBehaviour
    {
        public static ViewChangeManager Instance;
        public GameObject MainCamera;
        public Transform SubCameraAnchor;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

        private bool _mainView = true;
        public void ChangeVew()
        {
            if (_mainView)
            {
                MainCamera.GetComponent<CinemachineBrain>().enabled = false;
                MainCamera.transform.position = SubCameraAnchor.position;
                MainCamera.transform.rotation = SubCameraAnchor.rotation;
            }
            else
            {
                MainCamera.GetComponent<CinemachineBrain>().enabled = true;

            }

            _mainView = !_mainView;
        }
    }
}
