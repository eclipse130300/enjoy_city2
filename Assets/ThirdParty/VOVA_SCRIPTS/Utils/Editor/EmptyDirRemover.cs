using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public static class EmptyDirRemover
{
    private static DateTime? _cleanTime;

    static EmptyDirRemover()
    {
        RemoveEmptyDirsCleanTimeSet();
        EditorApplication.playModeStateChanged += RemoveEmptyDirsCleanTimeSet;
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (_cleanTime.HasValue && _cleanTime.Value < DateTime.Now)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                ThreadPool.QueueUserWorkItem(o => RemoveEmptyDirs(new DirectoryInfo("Assets")));
            _cleanTime = null;
        }
    }
    static void RemoveEmptyDirsCleanTimeSet(PlayModeStateChange change)
    {
        RemoveEmptyDirsCleanTimeSet();
    }

    [MenuItem("Custom Editor/Remove empty dirs")]
    static void RemoveEmptyDirsCleanTimeSet()
    {
        _cleanTime = DateTime.Now + TimeSpan.FromSeconds(5);
    }

    static void RemoveEmptyDirs(DirectoryInfo rootDir)
    {
        var dirs = rootDir.GetDirectories();
        var files = rootDir.GetFiles();

        if (dirs.Length == 0 && files.Length == 0)
        {
            var metaName = Path.Combine(rootDir.Parent.FullName, rootDir.Name + ".meta");
            Debug.Log("Empty directory removed \n" + rootDir.FullName + "\n" + metaName);
            rootDir.Delete();
            File.Delete(metaName);
        }
        else if (dirs.Length != 0)
        {
            foreach (var dir in dirs)
                RemoveEmptyDirs(dir);
        }
    }
}
