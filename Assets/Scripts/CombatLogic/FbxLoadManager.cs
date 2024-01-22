using Assets.Scripts.Common.Test;
using Assets.Scripts.Services;
using MagicaCloth2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    internal class LoadModelRes
    {
        public Transform ModelTransform;
        public Transform GunfireTransform;
        public GameObject GunfireEffect;
    }
    /// <summary>
    /// TODO: 做成静态类，实例放到DontDestroyOnLoad
    /// </summary>
    internal class FbxLoadManager : MonoBehaviour
    {
        /// <summary> NPR材质 </summary>
        Material default_face, default_hair, default_body, default_eyeMouth;
        /// <summary> magica cloth预设 </summary>
        GameObject magicaSkirt, magicaHair, magicaTail;
        /// <summary> skirt预设Collider </summary>
        GameObject lThighCollider, rThighCollider, pelvisCollider;
        private void Awake()
        {
            default_face = ResourceManager.Load<Material>("Fbx/Materials/default_face");
            default_hair = ResourceManager.Load<Material>("Fbx/Materials/default_hair");
            default_body = ResourceManager.Load<Material>("Fbx/Materials/default_body");
            default_eyeMouth = ResourceManager.Load<Material>("Fbx/Materials/default_eyeMouth");

            magicaSkirt = ResourceManager.Load<GameObject>("Fbx/Magica/MagicaSkirt");
            magicaHair = ResourceManager.Load<GameObject>("Fbx/Magica/MagicaHair");
            magicaTail = ResourceManager.Load<GameObject>("Fbx/Magica/MagicaTail");

            lThighCollider = ResourceManager.Load<GameObject>("Fbx/Magica/LThighCollider");
            rThighCollider = ResourceManager.Load<GameObject>("Fbx/Magica/RThighCollider");
            pelvisCollider = ResourceManager.Load<GameObject>("Fbx/Magica/PelvisCollider");

        }

        [MyTest]
        public void TestLoadModel()
        {
            var model = LoadModel("aru", transform, false);

        }


        [Tooltip("泛用的，能代表导入角色的avatar")]
        public Avatar NormalAvatar;

        public LoadModelRes LoadModel(string modelName, Transform parent, bool withGun, ModelShaderLayer msLayer = ModelShaderLayer.Default)
        {
            return LoadModel(modelName, parent, parent, withGun, msLayer);
        }
        public LoadModelRes LoadModel(string modelName, Transform parent, Transform animatorTrans, bool withGun, ModelShaderLayer msLayer = ModelShaderLayer.Default)
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

            // -- 1.1 配置Material
            changeSkinMaterial(findSkinMesh(go_root.transform));

            // -- 1.2 配置Magica Cloth
            if (GamePreference.UseMagicaBone is true) addMagicaBone(go_root.transform);


            // 2.读取avatar
            Avatar avatar = AvatarBuilder.BuildHumanAvatar(go, GetHumanDesc());
            animatorTrans.GetComponent<Animator>().avatar = avatar;


            // 3 配置 layer
            changeLayer(go_root, msLayer);

            // 4.抢放好
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
        /// 找到BA格式的body mesh位置
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private SkinnedMeshRenderer findSkinMesh(Transform root)
        {
            var renders = root.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            int skinMeshIndex = -1;
            for (int i = 0; i < renders.Length; i++)
            {
                if (renders[i].gameObject.name.Contains("Body"))
                {
                    if (skinMeshIndex == -1) skinMeshIndex = i;
                    else throw new ArgumentException($"too much \"Body\" render when load model {root.gameObject.name}");
                }
            }
            if (skinMeshIndex == -1) throw new ArgumentException($"cannot find \"Body\" render when load model {root.gameObject.name}");
            return renders[skinMeshIndex];
        }

        /// <summary>
        /// 改变导入的fbx material为NPR风格
        /// </summary>
        /// <param name="skinRender"></param>
        private void changeSkinMaterial(SkinnedMeshRenderer skinRender)
        {
            var oldM = skinRender.materials;
            var newM = new Material[oldM.Length];
            for (int i = 0; i < oldM.Length; i++)
            {
                var x = oldM[i];
                //if (x.mainTexture != null) Debug.Log($"{x}, {x.mainTexture.name}");
                //else Debug.Log($"{x}, null texture");
                if(x.mainTexture == null)
                {
                    newM[i] = x;
                }
                else if (x.name.Contains("_Face"))
                {
                    newM[i] = Instantiate(default_face);
                    newM[i].SetTexture("_BaseMap", x.mainTexture);
                }
                else if (x.name.Contains("_Hair"))
                {
                    newM[i] = Instantiate(default_hair);
                    newM[i].SetTexture("_BaseMap", x.mainTexture);
                }
                else if (x.name.Contains("_Body"))
                {
                    newM[i] = Instantiate(default_body);
                    newM[i].SetTexture("_BaseMap", x.mainTexture);

                }
                else if (x.name.Contains("_EyeMouth"))
                {
                    newM[i] = Instantiate(default_eyeMouth);
                    newM[i].SetTexture("_BaseMap", x.mainTexture);

                }
                else
                {
                    newM[i] = x;
                }
            }
            skinRender.sharedMaterials = newM;
        }

        private void addMagicaBone(Transform root)
        {
            var nodes = root.GetComponentsInChildren<Transform>();
            // hair (short front hair)
            var hairParent = nodes.Where(x => x.gameObject.name.Contains("hair") && x.parent.name.Contains("hair") is false).ToList();
            if (hairParent.Count() != 0)
            {
                var go = Instantiate(magicaHair, root);
                go.GetComponent<MagicaCloth>().SerializeData.rootBones = hairParent;
                go.GetComponent<MagicaCloth>().BuildAndRun();
            }
            // hair - tail (long back hair)
            var tailParent = nodes.Where(x => x.gameObject.name.Contains("tail") && x.parent.name.Contains("tail") is false).ToList();
            if (tailParent.Count() != 0)
            {
                var go = Instantiate(magicaTail, root);
                go.GetComponent<MagicaCloth>().SerializeData.rootBones = tailParent;
                go.GetComponent<MagicaCloth>().BuildAndRun();

            }

            // skirt: find parent
            var skirtParent = nodes.Where(x => x.gameObject.name.Contains("skirt") && x.parent.name.Contains("skirt") is false).ToList();
            if (skirtParent.Count() != 0)
            {
                // 找到叶节点并延长一段
                var skirtLeafs = nodes.Where(x => x.gameObject.name.Contains("skirt") && x.childCount == 0).ToList();
                foreach(var leaf in skirtLeafs)
                {
                    var left_ex = new GameObject(leaf.name + "left_ex");
                    left_ex.transform.parent = leaf.transform;
                    left_ex.transform.position = leaf.position + (leaf.position - leaf.parent.position).normalized * 0.08f;
                }

                // bones
                var go = Instantiate(magicaSkirt, root);
                var comp = go.GetComponent<MagicaCloth>();
                comp.SerializeData.rootBones = skirtParent;
                var colliders = new List<ColliderComponent>();
                // colliders
                // -- 右腿
                {
                    var rThighTrans = nodes.Where(x => x.gameObject.name.Contains("R Thigh"));
                    if (rThighTrans.Count() != 1)
                        throw new ArgumentException($"{root.name} have {rThighTrans.Count()} 'R Thigh' Bone, expected to be 1");
                    var rTrans = Instantiate(rThighCollider, rThighTrans.First());
                    colliders.Add(rTrans.GetComponent<MagicaCapsuleCollider>());
                }
                // -- 左腿
                {
                    var lThighTrans = nodes.Where(x => x.gameObject.name.Contains("L Thigh"));
                    if (lThighTrans.Count() != 1)
                        throw new ArgumentException($"{root.name} have {lThighTrans.Count()} 'L Thigh' Bone, expected to be 1");
                    var thTrans = Instantiate(lThighCollider, lThighTrans.First());
                    colliders.Add(thTrans.GetComponent<MagicaCapsuleCollider>());
                }
                // -- 盆骨
                {
                    var pelvisTrans = nodes.Where(x => x.gameObject.name.Contains("Pelvis"));
                    if (pelvisTrans.Count() != 1)
                        throw new ArgumentException($"{root.name} have {pelvisTrans.Count()} 'Pelvis' Bone, expected to be 1");
                    var thTrans = Instantiate(pelvisCollider, pelvisTrans.First());
                    colliders.Add(thTrans.GetComponent<MagicaSphereCollider>());
                }

                comp.SerializeData.colliderCollisionConstraint.colliderList = colliders;
                go.GetComponent<MagicaCloth>().BuildAndRun();

                // 冻结裙子根骨骼运动，直到MagicaCloth build完成
                int id = _ID++;
                freezeSkirtId.Add(id);
                StartCoroutine(freezeSkirt(skirtParent, id));
                go.GetComponent<MagicaCloth>().OnBuildComplete += (flag) => { freezeSkirtId.Remove(id); };
            }
        }

        static int _ID = 0;
        HashSet<int> freezeSkirtId = new HashSet<int>();
        private IEnumerator freezeSkirt(IList<Transform> skirtTopNode, int id)
        {
            var points = skirtTopNode.Select(x => x.position).ToList();
            yield return null;
            while (freezeSkirtId.Contains(id) && skirtTopNode[0] != null) // 可能加载完的时候，skirt已经销毁了（出现这种情况是因为被dontdestroyonload调用
            {
                for(int i = 0; i < skirtTopNode.Count; i++) skirtTopNode[i].position = points[i];
                yield return null;
            }
            yield return null;
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
        public enum ModelShaderLayer
        {
            Default,
            BackgroundShading,
        }
        private void changeLayer(GameObject go, ModelShaderLayer layerEnum)
        {
            if (layerEnum == ModelShaderLayer.Default) return;

            var queue = new Queue<GameObject>();
            var layer = LayerMask.NameToLayer(layerEnum.ToString());
            queue.Enqueue(go);
            int deep = 0;
            while (queue.Count != 0)
            {
                int len = queue.Count;
                for (int _ = 0; _ < len; _++)
                {
                    var t = queue.Dequeue();
                    t.layer = layer;
                    for (int i = 0; i < t.transform.childCount; i++)
                    {
                        queue.Enqueue(t.transform.GetChild(i).gameObject);
                    }
                }

                deep += 1;
                if (deep == 4) break;
            }
        }

    }
}
