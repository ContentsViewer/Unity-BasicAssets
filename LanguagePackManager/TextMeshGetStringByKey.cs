using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMesh))]
public class TextMeshGetStringByKey : MonoBehaviour
{
    private TextMesh text;
    [SerializeField]
    private string key;

    void Awake()
    {
        text = GetComponent<TextMesh>();

        LanguagePackManager.OnLanguegeChanged += UpdateText;
    }

    private void Start()
    {
        UpdateText();
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
