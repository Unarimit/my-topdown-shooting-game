using Assets.Scripts.CombatLogic;
using Assets.Scripts.Entities.Mechas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.EffectLogic
{
    public class EditRoomManager : MonoBehaviour
    {
        public Camera m_Camera;

        public Transform m_MechaTrans;

        public static EditRoomManager Instance;
        private Transform _characterTrans;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");

            _characterTrans = transform.Find("CharacterTrans");
        }
        private GameObject _lastGO;
        public void SetCharacterModel(string modelUrl)
        {
            if(_lastGO != null) Destroy(_lastGO);

            m_Camera.gameObject.SetActive(true);
            var prefab = ResourceManager.Load<GameObject>("Characters/Displayer");
            var go = Instantiate(prefab, _characterTrans);
            PrepareContextManager.Instance.GetComponent<FbxLoadManager>().LoadModel(modelUrl, go.transform, false);


            _lastGO = go;
        }

        GameObject headGO;
        GameObject bodyGO;
        GameObject legGO;
        public void LoadMechaPart(MechaBase mecha)
        {
            if(mecha is MechaHead)
            {
                if(headGO != null) Destroy(headGO);
                headGO = Instantiate(ResourceManager.Load<GameObject>("Mechas/" + mecha.IconUrl), m_MechaTrans);
            }
            else if(mecha is MechaBody)
            {
                if (bodyGO != null) Destroy(bodyGO);
                bodyGO = Instantiate(ResourceManager.Load<GameObject>("Mechas/" + mecha.IconUrl), m_MechaTrans);
            }
            else
            {
                if (legGO != null) Destroy(legGO);
                legGO = Instantiate(ResourceManager.Load<GameObject>("Mechas/" + mecha.IconUrl), m_MechaTrans);
            }
        }
    }
}
