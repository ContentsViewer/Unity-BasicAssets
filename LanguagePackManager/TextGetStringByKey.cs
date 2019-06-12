using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextGetStringByKey : MonoBehaviour
{
    private Text text;
    [SerializeField]
    private string key;

    void Awake()
    {
        text = GetComponent<Text>();

        LanguagePackManager.OnLanguegeChanged += UpdateText;
    }

    void Start()
    {
        text.text = LanguagePackManager.Instance.GetString(key);
    }

    public void UpdateText()
    {
        text.text = LanguagePackManager.Instance.GetString(key);
    }

    private void OnDestroy()
    {
        LanguagePackManager.OnLanguegeChanged -= UpdateText;
    }
}
