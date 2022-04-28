using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Animation clip target remapper.
/// This script allows animation curves to be moved from one target to another.
/// 
/// Usage:
///     1) Open the Animation Clip Target Renamer from the Window menu in the Unity UI.
///     2) Select the animation clip whose curves you wish to move.
///     3) Change the names in the textboxes on the right side of the window to the names of the objects you wish to move the animations to.
///     4) Press Apply.
/// </summary>
public class AnimationClipTargetRemapper : EditorWindow
{

    public AnimationClip selectedClip;
    public List<RemapperCurveData> CurveDatas;
    private bool initialized;

    [MenuItem("Window/Animation Clip Target Renamer")]
    public static void OpenWindow ()
    {
        AnimationClipTargetRemapper renamer = GetWindow<AnimationClipTargetRemapper>("Animation Clip Target Renamer");
        renamer.Clear();
    }

    private void Initialize ()
    {
        CurveDatas = new List<RemapperCurveData>();
        var curveBindings = AnimationUtility.GetCurveBindings(selectedClip);

        foreach (EditorCurveBinding curveBinding in curveBindings)
        {
            RemapperCurveData cd = new RemapperCurveData();
            cd.Binding = curveBinding;
            cd.OldPath = curveBinding.path + "";
            cd.NewPath = curveBinding.path + "";
            cd.Curve = new AnimationCurve(AnimationUtility.GetEditorCurve(selectedClip, curveBinding).keys);
            CurveDatas.Add(cd);
        }
        initialized = true;
    }

    private void Clear ()
    {
        CurveDatas = null;
        initialized = false;
    }

    void OnGUIShowTargetsList ()
    {

        if (CurveDatas == null) Initialize();

        var uniqueCurveDatas = CurveDatas.Where(x => x.Binding.path != "").GroupBy(g => g.Binding.path).Select(g => g.First()).ToList();

        if (uniqueCurveDatas != null && uniqueCurveDatas.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUIUtility.labelWidth = 250;

            for (int i = 0; i < uniqueCurveDatas.Count; i++)
            {
                string newName = EditorGUILayout.TextField(uniqueCurveDatas[i].OldPath, uniqueCurveDatas[i].NewPath);

                if (uniqueCurveDatas[i].OldPath != newName)
                {
                    var j = i;
                    CurveDatas.ForEach(x =>
                    {
                        if (x.OldPath == uniqueCurveDatas[j].OldPath)
                        {
                            x.NewPath = newName;
                        }
                    });
                }
            }
        }


    }

    private void RenameTargets ()
    {
        CurveDatas.ForEach(x =>
        {
            if (x.Binding.path != "" && x.OldPath != x.NewPath)
            {
                x.Binding.path = x.NewPath;
                x.OldPath = x.NewPath;
            }
        });

        selectedClip.ClearCurves();

        foreach (var curveData in CurveDatas)
        {
            selectedClip.SetCurve(curveData.Binding.path, curveData.Binding.type, curveData.Binding.propertyName, curveData.Curve);
        }

        Clear();
        Initialize();
    }


    void OnGUI ()
    {
        AnimationClip previous = selectedClip;
        selectedClip = EditorGUILayout.ObjectField("Animation Clip", selectedClip, typeof(AnimationClip), true) as AnimationClip;

        if (selectedClip != previous)
        {
            Clear();
        }

        if (selectedClip != null)
        {
            if (!initialized)
            {
                Initialize();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh"))
            {
                Clear();
                Initialize();
            }
            EditorGUILayout.EndHorizontal();

            OnGUIShowTargetsList();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply"))
            {
                RenameTargets();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public class RemapperCurveData
    {
        public EditorCurveBinding Binding;
        public AnimationCurve Curve;
        public string OldPath;
        public string NewPath;
    }
}