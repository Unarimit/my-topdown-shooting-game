using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    internal class LoadModelRes
    {
        public Transform ModelTransform;
        public Transform GunfireTransform;
        public GameObject GunfireEffect;
    }
    internal class FbxLoadManager : MonoBehaviour
    {
        [Tooltip("泛用的，能代表导入角色的avatar")]
        public Avatar NormalAvatar;

        public LoadModelRes LoadModel(string modelName, Transform parent, bool withGun)
        {
            return LoadModel(modelName, parent, parent, withGun);
        }
        public LoadModelRes LoadModel(string modelName, Transform parent, Transform animatorTrans, bool withGun)
        {
            // 1.读取模型
            var prefab = ResourceManager.Load<GameObject>($"Fbx/{modelName}");
            var go = Instantiate(prefab, parent);
            var go_root = go;

            // 禁用自带的animator
            if (go.GetComponent<Animator>() != null)
            {
                go.GetComponent<Animator>().enabled = false;
            }
            // 拆无用层级
            while (go.transform.childCount == 1)
            {
                go = go.transform.GetChild(0).gameObject;
            }

            Transform boneRoot = null, Body = null, Weapon = null;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var temp = go.transform.GetChild(i);
                if (temp.name.EndsWith("_Shield_Weapon"))
                {
                    temp.gameObject.SetActive(false);
                    Destroy(temp.gameObject);
                }
                else if (temp.name == "bone_root")
                {
                    boneRoot = temp;
                }
                else if (temp.name.EndsWith("_Body"))
                {
                    Body = temp;
                }
                else if (temp.name.EndsWith("_Weapon"))
                {
                    Weapon = temp;
                    if (!withGun) Weapon.gameObject.SetActive(false);
                }
                else
                {
                    Destroy(temp.gameObject);
                }
            }
            
            // 2.读取avatar
            Avatar avatar = AvatarBuilder.BuildHumanAvatar(go, GetHumanDesc());
            animatorTrans.GetComponent<Animator>().avatar = avatar;

            // 3.抢放好
            if (withGun)
            {
                var weaponBone = boneRoot.Find("Bip001").Find("Bip001_Weapon");
                var rhandBone = boneRoot.Find("Bip001").Find("Bip001 Pelvis").Find("Bip001 Spine").Find("Bip001 Spine1").Find("Bip001 R Clavicle")
                    .Find("Bip001 R UpperArm").Find("Bip001 R Forearm").Find("Bip001 R Hand");
                weaponBone.parent = rhandBone;
                weaponBone.localPosition = new Vector3(-0.1f, 0, 0.1f);
                weaponBone.localEulerAngles = new Vector3(-14, -88, 75);

                // 配置枪口位置
                var fireStart = weaponBone.Find("fire_01");
                var gf = Instantiate(ResourceManager.Load<GameObject>("Effects/Gunfire"), fireStart);
                gf.transform.localEulerAngles = new Vector3(0, -90, 0);
                gf.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                return new LoadModelRes { ModelTransform = go_root.transform, GunfireEffect = gf, GunfireTransform = fireStart };

            }
            else
            {
                return new LoadModelRes { ModelTransform = go_root.transform };
            }
        }

        /// <summary>
        /// 获取已经配置过的模型的枪械信息
        /// </summary>
        /// <param name="modelRoot"></param>
        /// <param name="withGun"></param>
        /// <returns></returns>
        public LoadModelRes GetModelConfig(Transform modelRoot, bool withGun)
        {
            Transform boneRoot = modelRoot;
            while (boneRoot.childCount == 1)
            {
                boneRoot = boneRoot.GetChild(0);
            }
            boneRoot = boneRoot.Find("bone_root");
            if (withGun)
            {
                var weaponBone = boneRoot.Find("Bip001").Find("Bip001 Pelvis").Find("Bip001 Spine").Find("Bip001 Spine1").Find("Bip001 R Clavicle")
                    .Find("Bip001 R UpperArm").Find("Bip001 R Forearm").Find("Bip001 R Hand").Find("Bip001_Weapon"); ;

                // 不用配置，直接找
                var fireStart = weaponBone.Find("fire_01");
                var gf = fireStart.Find("Gunfire(Clone)").gameObject;
                return new LoadModelRes { ModelTransform = modelRoot, GunfireEffect = gf, GunfireTransform = fireStart };

            }
            else return new LoadModelRes { ModelTransform = modelRoot };
        }

        private HumanDescription GetHumanDesc()
        {
            var res = NormalAvatar.humanDescription;
            int t = 0;
            for(; t < res.skeleton.Length; t++)
            {
                if (res.skeleton[t].name == "bone_root") break;
            }
            res.skeleton = res.skeleton.Skip(t-1).Take(115).ToArray(); // 保留mesh当作根，我也不知道为什么要这样，反正这样是对的
            
            // delete parent
            res.skeleton[0] = new SkeletonBone { name = "this_is_root", position = res.skeleton[0].position, rotation = res.skeleton[0].rotation, scale = res.skeleton[0].scale };
            res.skeleton[1] = new SkeletonBone { name = res.skeleton[1].name, position = res.skeleton[1].position, rotation = res.skeleton[1].rotation, scale = res.skeleton[1].scale };
            return res;
        }
        
    }
}
