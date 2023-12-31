﻿using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using Assets.Scripts.PrepareLogic.PrepareEntities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PrepareLogic.UILogic.EnemyUIs
{
    internal class EnemyPortraitScrollViewItemUI : MonoBehaviour
    {
        public RawImage PortraitImage;
        public TextMeshProUGUI CharacterNameTMP;
        public TextMeshProUGUI CharacterTypeTMP;
        private Operator _model;

        public void Inject(Operator model)
        {
            _model = model;
            PortraitImage.texture = PhotographyManager.GetOperatorPortrait(model);
            CharacterNameTMP.text = _model.Name;
            CharacterTypeTMP.text = _model.Type.ToString();
        }

    }
}
