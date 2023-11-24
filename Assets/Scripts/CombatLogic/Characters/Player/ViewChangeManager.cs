using Cinemachine;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.Characters.Player
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
