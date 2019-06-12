/*
*プログラム: 
*   最終更新日:
*       11.11.2016
*
*   説明:
*       テキスト管理をします
*       これを使うと多言語表示に対応できます
*
*   パラメータ説明:
*       dontDestoroyObjectOnLoad:
*           true: シーン間でデストロイされませんA
*           false: シーン間でデストロイされます
*
*       LanguageID:
*           表示言語を変えるときに用います
*
*       coreLanguagePackFileList:
*           ここで登録された言語パックファイルはシーン間でデストロイされません
*
*           packFileName:
*               packFileが存在するパス
*               'Assets/'から始まるパス
*
*       sceneList:
*           各シーンごとで使用する言語パックをここで登録します
*           シーン間でデストロイされます
*
*           sceneName:
*               シーンの名前
*
*           languagePackFileList:
*               packFileName:
*                   packFileが存在するパス
*                   'Assets/'から始まるパス
*
*   LanguagePackFileの作り方:
*       Textファイルを用意します
*       Label名を書きます
*           例:
                <Label>Mes0
*
*       Listを書きます
*           例:
                <Label>Mes0
                <List>Test
                <List>テスト

*       上二つをLabel数繰り返します
*           例:
                <Label>Mes0
                <List>Test
                <List>テスト

                <Label>Mes1
                <List>Hello
                <List>やあ

*       UTF8形式で保存します
*           
*      補足:
*           上の場合languageIDが0のとき英語, LanguageIDが1のとき日本語です
*
*   使用例:
*       上のPackFileをロードしているとします
*       LanguageID = 0とします
*       あるスクリプト内:
            Debug.Log(LanguagePackManager.instance.GetString("Mes0"));
*       結果:
*           Test
*
*   更新履歴:
*       4.5.2016:
*           プログラムの完成
*
*       4.11.2016; 4.12.2016; 5.5.2016
*           スクリプト修正
*
*       5.17.2016:
*           仕様変更
*               言語パックのロードのタイミングを変更
*
*       7.9.2016:
*           同じシーンを再読み込みしたときそのシーンで登録される言語パックが破棄される問題を修正
*           
*       11.11.2016:
*           UnityUpdateに伴う修正; OnLevelWasLoadedの代わりにSceneManagerを使用
*/



/*
 *連絡
 *  2018-2-15(Ken):
 *   IDを非静的
 *   理由:
 *    inspectorで変更できない.
 * 
 *  20180-5-02(Dio)
 *   LanguageIDを更新するときのイベントを用意した
 */


using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System;


public class LanguagePackManager : MonoBehaviour
{
    static readonly string LabelToken = "# ";
    static readonly string ListToken = "* ";
    static readonly string NewLineToken = "\\";

    public static LanguagePackManager Instance { get; private set; }


    [System.Serializable]
    public class SceneSetting
    {
        public string sceneName;
        public List<LanguagePackFile> languagePackFileList;

    }

    [System.Serializable]
    public class LanguagePackFile
    {
        public string packFileName = "";
        public LanguagePackFile(string fileName)
        {
            packFileName = fileName;
        }
    }

    class LanguagePack
    {
        public bool dontDestroyOnLoad = false;
        public Dictionary<string, List<string>> labelList = new Dictionary<string, List<string>>();

        public LanguagePack(bool dontDestroyOnLoad)
        {
            this.dontDestroyOnLoad = dontDestroyOnLoad;
        }

        public void AddLable(string key, List<string> textList)
        {
            if (labelList.ContainsKey(key))
            {
                labelList[key] = textList;
            }
            else
            {
                labelList.Add(key, textList);
            }
        }
    }

    public delegate void LanguegeChanged();
    public static event LanguegeChanged OnLanguegeChanged;
    // === 外部パラメータ(inspector表示) =============================
    public int LanguageID
    {
        get { return m_languageID; }
        set
        {
            m_languageID = value;
            if (Instance != null)
            {
                OnLanguegeChanged?.Invoke();
            }
        }
    }


    public List<LanguagePackFile> coreLanguagePackFileList;
    public List<SceneSetting> sceneList;
    
    // === 内部パラメータ ============================================
    List<LanguagePack> languagePackList = new List<LanguagePack>();

    [SerializeField]
    private int m_languageID = 0;


    void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }        
    }

    void Awake()
    {
        foreach (LanguagePackFile file in coreLanguagePackFileList)
        {
            LoadFile(file, true);
        }

        SceneManager.sceneLoaded += OnSceneWasLoaded;
        DontDestroyOnLoad(this);
        Instance = this;
    }

    void Update()
    {

        //RuntimeDebug.Log("files: " + languagePackList.Count);

        //if (Input.GetKey(KeyCode.Q))
        //{
        //    RuntimeDebug.Log("packs: " + languagePackList.Count);

        //    foreach (LanguagePack pack in languagePackList)
        //    {
        //        RuntimeDebug.Log("labels: " + pack.labelList.Count);
        //        foreach (KeyValuePair<string, List<string>> label in pack.labelList)
        //        {
        //            RuntimeDebug.Log("label: " + label.Key);

        //            foreach (var str in label.Value)
        //            {
        //                RuntimeDebug.Log("text: " + str);
        //            }
        //        }
        //    }
        //}


        //if (Input.GetKey(KeyCode.Q))
        //{
        //    foreach (LanguagePack pack in languagePackList)
        //    {
        //        foreach (var label in pack.labelList)
        //        {
        //            Debug.Log("label: " + label.Key);

        //            foreach (var str in label.Value)
        //            {
        //                Debug.Log("text: " + str);
        //            }
        //        }
        //    }
        //}
    }

    //シーンがロードされたとき
    void OnSceneWasLoaded(Scene scenename, LoadSceneMode SceneMode)
    {
        SetManager();
    }

    //シーンに応じてManagerの情報を更新します
    void SetManager()
    {
        UnLoadFiles();

        string sceneName = SceneManager.GetActiveScene().name;

        foreach (SceneSetting scene in sceneList)
        {
            if (scene.sceneName == sceneName)
            {
                foreach (LanguagePackFile file in scene.languagePackFileList)
                {
                    LoadFile(file, false);
                }
            }
        }
    }

    //PackFileをロードします
    public bool LoadFile(LanguagePackFile file, bool dontDestroyOnLoad)
    {

        LanguagePack pack = new LanguagePack(dontDestroyOnLoad);

        string labelKey = null;
        List<string> textList = null;
        string text = null;
        
        try
        {
            string filePath = Application.dataPath + "/" + file.packFileName;

            //Debug.Log(filePath);
            //FileInfo fileInfo = new FileInfo(filePath);
            //StreamReader streamReader = new StreamReader(fileInfo.Open(FileMode.Open, FileAccess.Read), Encoding.UTF8);

            using (StreamReader reader = new StreamReader(@filePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    
                    if (line.StartsWith(LabelToken))
                    {
                        if (text != null)
                        {
                            textList.Add(text);
                        }

                        if (textList != null)
                        {
                            pack.AddLable(labelKey, textList);
                        }

                        labelKey = line.Substring(LabelToken.Length).Trim();
                        textList = new List<string>();
                        text = null;
                        continue;
                    }

                    if (line.StartsWith(ListToken))
                    {
                        if(text != null)
                        {
                            textList.Add(text);
                        }

                        text = DecodeLine(line.Substring(ListToken.Length));
                        
                        continue;
                    }

                    text += DecodeLine(line);
                }

                if(text != null)
                {
                    textList.Add(text);
                }

                if(textList != null)
                {
                    pack.AddLable(labelKey, textList);
                }

                
                reader.Close();
            }

            languagePackList.Add(pack);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("LanguagePackManager.LoadFiles >> Failed: " + e.Message);

            return false;
        }
    }


    private string DecodeLine(string line)
    {
        if (line.EndsWith(NewLineToken))
        {
            return line.Substring(0, line.Length - NewLineToken.Length) + Environment.NewLine;
        }

        return line;
    }


    private void UnLoadFiles()
    {
        //languagePackListを整理
        List<LanguagePack> languagePackListRearrange = new List<LanguagePack>();
        foreach (LanguagePack pack in languagePackList)
        {
            if (pack.dontDestroyOnLoad)
            {
                languagePackListRearrange.Add(pack);
            }
        }
        languagePackList = languagePackListRearrange;
    }
    
  
    /// <summary>
    ///     指定したlabelに対応づけられているテキストを返します.
    ///     labelの前後についている空白文字は自動で取り除かれます.
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    public string GetString(string label)
    {
        label = label.Trim();
        foreach (LanguagePack pack in languagePackList)
        {
            if (pack.labelList.ContainsKey(label))
            {
                if (m_languageID < pack.labelList[label].Count)
                {
                    return pack.labelList[label][m_languageID];
                }
            }
        }

        Debug.LogWarning("LanguagePackManager.GetString >> label '" + label + "' is not found.");
        return "";
    }
}
