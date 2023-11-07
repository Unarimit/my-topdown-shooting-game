using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    public class AnimeHelper : MonoBehaviour
    {
        public static AnimeHelper Instance;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

        public void ApplyRagdoll(Transform transform)
        {
            var prefab = Resources.Load<GameObject>("Effects/RagDoll");
            var go = Instantiate(prefab, CombatContextManager.Instance.Enviorment);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
        }
        private Dictionary<Transform, DamageNumEffectController> hitTextDic = new Dictionary<Transform, DamageNumEffectController>();
        public void DamageTextEffect(int dmg, Transform hitted)
        {
            if(hitTextDic.ContainsKey(hitted) && hitTextDic[hitted] != null && hitTextDic[hitted].InDestroy == false)
            {
                hitTextDic[hitted].AppendDamageNum(dmg);
                hitTextDic[hitted].transform.position = hitted.position + new Vector3(0, 1.5f, 0);
            }
            else
            {
                var prefab = Resources.Load<GameObject>("Effects/DamageTextEffect");
                var go = Instantiate(prefab, CombatContextManager.Instance.Enviorment);
                hitTextDic[hitted] = go.GetComponent<DamageNumEffectController>();
                hitTextDic[hitted].DamageNum = dmg;

                go.transform.position = hitted.position + new Vector3(0, 1.5f, 0);
            }
            
        }
        List<AudioClip> gunshotAudioAudioClips = null;
        public List<AudioClip> GetGunshot()
        {
            if (gunshotAudioAudioClips == null)
            {
                gunshotAudioAudioClips = SplitGunSound(Resources.Load<AudioClip>("Sounds/Spray")).ToList();
            }
            return gunshotAudioAudioClips;
        }
        private List<AudioClip> SplitGunSound(AudioClip gunSound)
        {
            float clipLength = 1.2f; // 分割后的长度为1.2秒
            float[] PeekSeconds = new float[] {
                0.07f, 0.143f, 0.227f, 0.307f, 0.391f,
                0.471f, 0.554f, 0.636f, 0.711f, 0.794f,
                0.88f, 0.96f, 1.03f, 1.12f, 1.2f  };

            var splitClips = new List<AudioClip>(15);

            for (int i = 0; i < 15; i++)
            {
                // 计算开始位置
                int startSample = 0;
                if (i != 0)
                {
                    startSample = Mathf.CeilToInt(PeekSeconds[i-1] * gunSound.frequency);
                    startSample -= Mathf.CeilToInt(0.04f * gunSound.frequency);
                }

                // 计算持续时间
                int cliplength = Mathf.CeilToInt((PeekSeconds[i]) * gunSound.frequency) - startSample;
                clipLength += Mathf.CeilToInt(0.04f * gunSound.frequency);
                float[] splitSamplesData = new float[cliplength];
                gunSound.GetData(splitSamplesData, startSample);

                // 创建audioclip，填入数据
                splitClips.Add(AudioClip.Create("SplitClip" + i, cliplength, gunSound.channels, gunSound.frequency, false));
                splitClips[i].SetData(splitSamplesData, 0);
            }

            // 返回第一个分割后的片段
            return splitClips;
        }
    }
}
