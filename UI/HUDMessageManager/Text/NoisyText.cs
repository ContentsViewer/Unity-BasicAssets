using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.Text;

using System.Text.RegularExpressions;

[RequireComponent(typeof(Text))]
public class NoisyText : MonoBehaviour
{
    public bool resetTextOnUpdate = true;
    public int noiseRate = 10;

    [Space(10)]
    public string noiseChars = "-+#@./?$%&^*";
    

    public Text Text { get; private set; }


    string orgText;

    void Awake()
    {
        Text = GetComponent<Text>();

        orgText = Text.text;
    }

    void Update()
    {
        if (resetTextOnUpdate)
        {
            Text.text = orgText;
        }
    }

    void LateUpdate()
    {
        StringBuilder stringBuilder = new StringBuilder();
        var splits = Regex.Split(Text.text, "(</?.*?>)");
        
        foreach (var split in splits)
        {
            if (split.Length >= 2 && split[0] == '<' && split[split.Length - 1] == '>')
            {
                stringBuilder.Append(split);
            }
            else
            {
                for (var i = 0; i < split.Length; i++)
                {
                    if (Random.Range(0, 100) < noiseRate)
                        stringBuilder.Append(noiseChars[Random.Range(0, noiseChars.Length)]);

                    else
                        stringBuilder.Append(split[i]);

                }
            }
        }

        Text.text = stringBuilder.ToString();
    }

    void OnDisable()
    {
        Text.text = orgText;
    }
}
