using Assets.Scripts.PrepareLogic;
using Assets.Scripts.PrepareLogic.EffectLogic;
using Assets.Scripts.PrepareLogic.UILogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeammatePortraitUI : PrepareUIBase
{
    public GameObject CharacterPortraitPrefab;
    public Transform PortraitsContentTrans;
    private PrepareContextManager _context => PrepareContextManager.Instance;
    private RectTransform _rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();


        generatePortrait();

    }
    private void generatePortrait()
    {
        var ops = _context.GetOperators();
        foreach (var op in ops)
        {
            var go = Instantiate(CharacterPortraitPrefab, PortraitsContentTrans);

            // set content
            var port = go.GetComponent<CharacterPortraitUI>();
            port.PortraitImage.texture = PhotographyManager.Instance.GetCharacterPortrait(op.ModelResourceUrl);
            port.CharacterNameTMP.text = op.Name;

            // set event
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                Debug.Log("click");
            });

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
