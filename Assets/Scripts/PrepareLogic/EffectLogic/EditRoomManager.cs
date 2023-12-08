using Assets.Scripts.CombatLogic;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Mechas;
using DG.Tweening;
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
        private GameObject[] _lastFightersGO;
        static private readonly Vector3[] fighterPos = new Vector3[] { new Vector3(1.1f, 2, 0.2f), new Vector3(-0.2f, 2f, 1f), 
            new Vector3(0.4f, 2f, -0.4f) };
        public void SetCharacterModel(Operator op)
        {
            // clear old
            if(_lastGO != null) Destroy(_lastGO);
            if(_lastFightersGO != null)
            {
                foreach (var x in _lastFightersGO) Destroy(x);
            }
            _lastFightersGO = null;

            // open camera
            m_Camera.gameObject.SetActive(true);

            // load op
            {
                var prefab = ResourceManager.Load<GameObject>("Characters/Displayer");
                var go = Instantiate(prefab, _characterTrans);
                PrepareContextManager.Instance.GetComponent<FbxLoadManager>().LoadModel(op.ModelResourceUrl, go.transform, false);
                _lastGO = go;
            }
            

            // load fighters
            if(op.Fighters != null && op.Fighters.Count != 0)
            {
                _lastFightersGO = new GameObject[op.Fighters.Count];
                for(int i = 0; i < op.Fighters.Count; i++)
                {
                    _lastFightersGO[i] = Instantiate(ResourceManager.Load<GameObject>("Characters/FighterDisplayer"), _characterTrans);
                    PrepareContextManager.Instance.GetComponent<FbxLoadManager>()
                        .LoadModel(op.Fighters[i].Operator.ModelResourceUrl, _lastFightersGO[i].transform.Find("modelroot"), _lastFightersGO[i].transform, false);
                    _lastFightersGO[i].transform.LookAt(m_Camera.transform);
                    _lastFightersGO[i].transform.eulerAngles = new Vector3(0, _lastFightersGO[i].transform.eulerAngles.y, 0);
                    _lastFightersGO[i].GetComponent<EditRoomFighterController>().Inject(fighterPos[i]);

                }
            }
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
