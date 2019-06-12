using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontSelector : MonoBehaviour {
	[System.Serializable]
	public class FontSetting
	{
		public List<Font> fontList;
		public List<Text> texts;
	}

	public List<FontSetting> fontSettings;

	async void Start () {
        await new WaitForEndOfFrame();

		foreach (var it in fontSettings)
		{
			foreach (var text in it.texts)
			{
				text.font = it.fontList[LanguagePackManager.Instance.LanguageID];
			}
		}
	}
}
