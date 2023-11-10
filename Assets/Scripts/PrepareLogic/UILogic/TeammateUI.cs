using Assets.Scripts.PrepareLogic;
using Assets.Scripts.PrepareLogic.EffectLogic;
using Assets.Scripts.PrepareLogic.UILogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeammateUI : PrepareUIBase
{
    public GameObject CharacterPortraitPrefab;
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
        int width = 150;
        int height = 250;
        int i = 0;
        int j = 0;
        foreach (var op in ops)
        {
            var go = Instantiate(CharacterPortraitPrefab, transform);

            // set position
            var rect = go.GetComponent<RectTransform>();
            rect.Translate(new Vector3(i * width, -(j * height), 0));
            i += 1;
            if (i == 8)
            {
                i = 0;
                j += 1;
            }

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
