
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
//using UnityEditor.iOS.Xcode;

public class LanguagePackManagerEditor {

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
#if UNITY_IOS
        string folderpath = Path.Combine(path, "Data/LanguagePack");
#elif UNITY_STANDALONE
        string folderpath = Path.Combine(Path.GetDirectoryName(path), UnityEngine.Application.productName + "_Data/LanguagePacks");
#endif
        string filepath = "./Assets/LanguagePacks";

        UnityEngine.Debug.Log("LanguagePack Copy path: " + folderpath);

        CopyAndReplace(filepath, folderpath);
        //File.Copy(filepath, folderpath);
    }

    public static void CopyAndReplace(string sourcePath, string copyPath)
    {
        //既にディレクトリがある場合は削除し、新たにディレクトリ作成
        Delete(copyPath);
        Directory.CreateDirectory(copyPath);

        //ファイルをコピー
        foreach (var file in Directory.GetFiles(sourcePath))
        {
            File.Copy(file, Path.Combine(copyPath, Path.GetFileName(file)));
        }

        //ディレクトリの中のディレクトリも再帰的にコピー
        foreach (var dir in Directory.GetDirectories(sourcePath))
        {
            CopyAndReplace(dir, Path.Combine(copyPath, Path.GetFileName(dir)));
        }
    }

    public static void Delete(string targetDirectoryPath)
    {
        if (!Directory.Exists(targetDirectoryPath))
        {
            return;
        }

        //ディレクトリ以外の全ファイルを削除
        string[] filePaths = Directory.GetFiles(targetDirectoryPath);
        foreach (string filePath in filePaths)
        {
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }

        //ディレクトリの中のディレクトリも再帰的に削除
        string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
        foreach (string directoryPath in directoryPaths)
        {
            Delete(directoryPath);
        }

        //中が空になったらディレクトリ自身も削除
        Directory.Delete(targetDirectoryPath, false);
    }
}
