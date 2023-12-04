using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic.MiniMap
{
    [RequireComponent(typeof(bl_MiniMapEntity))]
    internal class MiniMapMarkUI : MonoBehaviour
    {
        bl_MiniMapEntity mapEntity;
        private void Awake()
        {
            mapEntity = GetComponent<bl_MiniMapEntity>();
        }
        public void Inject(int team, Entities.OperatorType type)
        {
            if (team == 0)
            {
                mapEntity.IconColor = TestDB.TeamColor;
            }
            else if (team == 1)
            {
                mapEntity.IconColor = TestDB.EnemyColor;
            }
            if (type == Entities.OperatorType.CA)
            {

            }
            else if (type == Entities.OperatorType.CV)
            {

            }
        }
        public void FighterInject(int team)
        {
            if (team == 0)
            {
                mapEntity.IconColor = TestDB.TeamColor;
            }
            else if (team == 1)
            {
                mapEntity.IconColor = TestDB.EnemyColor;
            }
        }

        private void OnEnable()
        {
            // 有可能他还没准备好
            if(mapEntity.IconInstance != null) mapEntity.ShowItem();
        }
        private void OnDisable()
        {
            // 有可能他还没准备好
            if (mapEntity.IconInstance != null) mapEntity.HideItem();
        }
    }
}
