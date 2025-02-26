using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeRedCat.ResponsiveScroller.Core.Scripts.Editor
{
    public enum AspectModeOptions
    {
        Overstretch = 0,
        Fill = 1
    }
    
    [CustomEditor(typeof(BackgroundManager))]
    public class BackgroundManagerInspector : UnityEditor.Editor
    {
        public AspectModeOptions aspectMode;
        private Texture2D _logo;

        public override void OnInspectorGUI()
        {
            //Draw Logo
            if (_logo == null)
                _logo = Resources.Load<Texture2D>("CodeRedCat_logo");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUILayout.Label(_logo);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            
            BackgroundManager backgroundManager = (BackgroundManager)target;
            
            
            DrawDefaultInspector();
            
            //set execution order
            if (!Application.isPlaying)
            {
                GUIContent overwriteExecutionOrderContent = new GUIContent
                {
                    text = "Overwrite Execution Order",
                    tooltip =
                        "If true, this script will write itself into the execution order."
                };
                bool newOverwriteExecutionOrder = EditorGUILayout.Toggle(overwriteExecutionOrderContent, backgroundManager.overwriteExecutionOrder);
                if (newOverwriteExecutionOrder != backgroundManager.overwriteExecutionOrder)
                {
                    EditorUtility.SetDirty(backgroundManager);
                    Undo.RecordObject(backgroundManager, "Modified Overwrite Execution Order in " + backgroundManager.name);
                    backgroundManager.overwriteExecutionOrder = newOverwriteExecutionOrder;
                }

                if (backgroundManager.overwriteExecutionOrder)
                {
                    GUIContent executionOrderContent = new GUIContent
                    {
                        text = "Execution Order",
                        tooltip = "Sets the execution order to this value, which makes the scroller much smoother in practice. With an execution order of >0 the background will start to show some jitter or flicker."
                    };
                    
                    int newExecutionOrder = EditorGUILayout.IntField(executionOrderContent, backgroundManager.executionOrder);
                    if (newExecutionOrder != backgroundManager.executionOrder)
                    {
                        backgroundManager.executionOrder = newExecutionOrder;
                        EditorUtility.SetDirty(backgroundManager);
                        Undo.RecordObject(backgroundManager, "Modified Fill mode in " + backgroundManager.name);
                    }

                    MonoScript monoScript = MonoScript.FromMonoBehaviour(backgroundManager);
                    if (MonoImporter.GetExecutionOrder(monoScript) != backgroundManager.executionOrder)
                    {
                        MonoImporter.SetExecutionOrder(monoScript, backgroundManager.executionOrder);
                    }
                }
            }
            
            GUIContent autoDetermineContent = new GUIContent
            {
                text = "Auto Determine Bordering Images",
                tooltip = "Determine automatically at runtime, how many Images are constructed, to fill out the full screen" +
                          " and also have enough images, to hide image swapping."
            };
            bool newAutoDetermineBorderingImages = EditorGUILayout.Toggle(autoDetermineContent, backgroundManager.AutoDetermineBorderingImages);
            if (newAutoDetermineBorderingImages != backgroundManager.AutoDetermineBorderingImages)
            {
                EditorUtility.SetDirty(backgroundManager);
                Undo.RecordObject(backgroundManager, "Modified Auto Determine Bordering Images in " + backgroundManager.name);
                backgroundManager.AutoDetermineBorderingImages = newAutoDetermineBorderingImages;
            }

            if (backgroundManager.AutoDetermineBorderingImages)
            {
                GUIContent sliderContent = new GUIContent
                {
                    text = "Bordering Images Max",
                    tooltip = "The maximum of images that can be automatically generated. The amount of images is capped to this amount, for performance reasons against stability issues."
                };
                int newAutoDetermineBorderingImagesMax = EditorGUILayout.IntSlider(sliderContent, backgroundManager.autoDetermineBorderingImagesMax, 1, 10);
                if (newAutoDetermineBorderingImagesMax != backgroundManager.autoDetermineBorderingImagesMax)
                {
                    EditorUtility.SetDirty(backgroundManager);
                    Undo.RecordObject(backgroundManager, "Modified Bordering Images Max in " + backgroundManager.name);
                    backgroundManager.autoDetermineBorderingImagesMax = newAutoDetermineBorderingImagesMax;
                }
            }
            else
            {
                GUIContent sliderContent = new GUIContent
                {
                    text = "Bordering RectTransform Amount",
                    tooltip = "Used to override the 'BorderingRectTransforms' value in all 'AdditionalParallaxImagesGenerator'" +
                              " children. The 'BorderingRectTransforms' describes the amount of RectTransforms bordering the main" +
                              " Image to each side. This will always be 1 left and right. If repetition up and/or down has been enabled," +
                              " there will also be above and below."
                };
                int newBorderingRectTransformsOverride = EditorGUILayout.IntSlider(sliderContent,backgroundManager.BorderingRectTransformsOverride,1,10);
                if (newBorderingRectTransformsOverride != backgroundManager.BorderingRectTransformsOverride)
                {
                    EditorUtility.SetDirty(backgroundManager);
                    Undo.RecordObject(backgroundManager, "Modified Bordering RectTransform Amount in " + backgroundManager.name);
                    backgroundManager.BorderingRectTransformsOverride = newBorderingRectTransformsOverride;
                }
            }
            
            EditorGUILayout.Space();
            backgroundManager.showStretchOptions = EditorGUILayout.Foldout(backgroundManager.showStretchOptions, "Stretch & Aspect Ratio Options");
            if (backgroundManager.showStretchOptions)
            {
                if (Selection.activeTransform)
                {
                    GUIContent sliderContent = new GUIContent
                    {
                        text = "Preserve Aspect<->Stretch to Frame",
                        tooltip =
                            "Forces the image to always be stretched to the camera frame, regardless of the aspect ratio." +
                            " With a value of 1, the RectTransforms will be fully stretched to the edges of the screen." +
                            " With a value of 0, the images will be unaffected. A value in between 0 and 1 will" +
                            " stretch the images to the screen edges, but only by the given factor."
                    };
                    float newStretchFactor =
                        EditorGUILayout.Slider(sliderContent, backgroundManager.StretchFactor, 0, 1);
                    if (newStretchFactor != backgroundManager.StretchFactor)
                    {
                        backgroundManager.StretchFactor = newStretchFactor;
                        Undo.RecordObject(backgroundManager, "Modified Stretch Factor in " + backgroundManager.name);
                        EditorUtility.SetDirty(backgroundManager);
                    }
                    
                    GUIContent preserveAspectContent = new GUIContent
                    {
                        text = "Preserve Aspect, but fill Frame",
                        tooltip =
                            "If true, the base image be fit into the camera frame, while preserving the aspect ratio."
                    };
                    bool newPreserveAspectStretch = EditorGUILayout.Toggle(preserveAspectContent,
                        backgroundManager.PreserveAspectStretch);
                    if (newPreserveAspectStretch != backgroundManager.PreserveAspectStretch)
                    {
                        EditorUtility.SetDirty(backgroundManager);
                        Undo.RecordObject(backgroundManager, "Modified Preserve Aspect in " + backgroundManager.name);
                        backgroundManager.PreserveAspectStretch = newPreserveAspectStretch;
                    }

                    if (backgroundManager.PreserveAspectStretch)
                    {
                        GUIContent preserveAspectModeContent = new GUIContent
                        {
                            text = "Fill mode",
                            tooltip = "Modifies the method, how the aspect ratio is preserved." +
                                      "If Stretch is selected, the image will be bigger than the camera frame. The smaller side will fill out the camera height or width." +
                                      "If Fill is selected, the image will be smaller than the camera frame. The bigger side will fill out the camera height or width."
                        };
                        
                        //get current aspectMode setting
                        aspectMode = backgroundManager.PreserveAspectStretchOut == true ? AspectModeOptions.Overstretch : AspectModeOptions.Fill;
                        
                        aspectMode = (AspectModeOptions)EditorGUILayout.EnumPopup(preserveAspectModeContent, aspectMode);
                        bool newPreserveAspectStretchOut = aspectMode == AspectModeOptions.Overstretch;
                        if (newPreserveAspectStretchOut != backgroundManager.PreserveAspectStretchOut)
                        {
                            backgroundManager.PreserveAspectStretchOut = newPreserveAspectStretchOut;
                            EditorUtility.SetDirty(backgroundManager);
                            Undo.RecordObject(backgroundManager, "Modified Fill mode in " + backgroundManager.name);
                        }
                    }
                }
            }

            //do not show preview Buttons during runtime
            if (Application.isPlaying) return;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            GUIContent previewContent = new GUIContent
            {
                text = "Create Preview for all Children",
                tooltip = "Will preview all bordering RectTransforms of 'AdditionalParallaxImagesGenerator' children."
            };
            
            if(GUILayout.Button(previewContent))
            {
                ParallaxRectTransformGenerator[] parallaxRectTGenerators = backgroundManager.InstantiateBorderingImagesInChildren();
                foreach (ParallaxRectTransformGenerator parallaxRectTGenerator in parallaxRectTGenerators)
                {
                    foreach (List<RectMatrixTile> bgLayerRow in parallaxRectTGenerator.BgLayerMatrix)
                    {
                        foreach (RectMatrixTile bgLayer in bgLayerRow)
                        {
                            if(bgLayer.Rect == parallaxRectTGenerator.baseRectTransform) continue;
                            Undo.RegisterCreatedObjectUndo(bgLayer.GameObject, "Create Preview for all Children");
                        }
                    }
                }
            }
            
            
            GUIContent destroyContent = new GUIContent
            {
                text = "Destroy Previews for all Children",
                tooltip = "Will destroy all bordering RectTransforms of 'AdditionalParallaxImagesGenerator' children."
            };
            
            if(GUILayout.Button(destroyContent))
            {
                ParallaxRectTransformGenerator[] parallaxRectTGenerators = backgroundManager.GetComponentsInChildren<ParallaxRectTransformGenerator>();
                
                foreach (ParallaxRectTransformGenerator parallaxRectTGenerator in parallaxRectTGenerators)
                {
                    foreach (List<RectMatrixTile> bgLayerRow in parallaxRectTGenerator.BgLayerMatrix)
                    {
                        foreach (RectMatrixTile bgLayer in bgLayerRow)
                        {
                            if(bgLayer.Rect == parallaxRectTGenerator.baseRectTransform) continue;
                            Undo.DestroyObjectImmediate(bgLayer.GameObject);
                        }
                    }
                }
                backgroundManager.ClearBgLayers();
                foreach (ParallaxRectTransformGenerator parallaxRectTGenerator in parallaxRectTGenerators)
                {
                    Undo.RecordObject(parallaxRectTGenerator, "Destroy Previews for all Children");
                }
            }
        }
    }
}