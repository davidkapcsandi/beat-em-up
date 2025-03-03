using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CodeRedCat.ResponsiveScroller.Core.Scripts.Editor
{
    [CustomEditor(typeof(ParallaxRectTransformGenerator))]
    public class ParallaxRectTransformGeneratorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ParallaxRectTransformGenerator parallaxRectTransformGenerator = (ParallaxRectTransformGenerator)target;
            BackgroundManager backgroundManager = parallaxRectTransformGenerator.BackgroundManager;

            DrawDefaultInspector();

            if (backgroundManager == null || !backgroundManager.AutoDetermineBorderingImages)
            {
                GUIContent inheritContent = new GUIContent
                {
                    text = "Inherit Bordering RectTransforms",
                    tooltip = "Set 'borderingRectTransforms' to the value from the BackgroundManager."
                };
                bool newInheritBorderingRectTransforms = EditorGUILayout.Toggle(inheritContent, parallaxRectTransformGenerator.inheritBorderingRectTransforms);
                if (newInheritBorderingRectTransforms != parallaxRectTransformGenerator.inheritBorderingRectTransforms)
                {
                    EditorUtility.SetDirty(parallaxRectTransformGenerator);
                    Undo.RecordObject(parallaxRectTransformGenerator, "Modified Inherit Bordering RectTransforms in " + parallaxRectTransformGenerator.name);
                    parallaxRectTransformGenerator.inheritBorderingRectTransforms = newInheritBorderingRectTransforms;
                }
            }

            if (backgroundManager != null && backgroundManager.AutoDetermineBorderingImages)
            {
                string label = "";
                if (Application.isPlaying) label += parallaxRectTransformGenerator.BorderingRectTransforms + " ";

                label += "(Auto Determined)";
                    
                EditorGUILayout.LabelField("Bordering RectTransforms Amount ", label);
            }
            else if(!parallaxRectTransformGenerator.inheritBorderingRectTransforms)
            {
                GUIContent sliderContent = new GUIContent
                {
                    text = "Bordering RectTransforms Amount",
                    tooltip = "The amount of images in each direction that surround the center image."
                };
                int newBorderingRectTransforms = EditorGUILayout.IntSlider(sliderContent,parallaxRectTransformGenerator.BorderingRectTransforms, 1, 10);
                if (newBorderingRectTransforms != parallaxRectTransformGenerator.BorderingRectTransforms)
                {
                    EditorUtility.SetDirty(parallaxRectTransformGenerator);
                    Undo.RecordObject(parallaxRectTransformGenerator, "Modified Bordering RectTransforms Amount in " + parallaxRectTransformGenerator.name);
                    parallaxRectTransformGenerator.BorderingRectTransforms = newBorderingRectTransforms;
                }
            }
            else
            {
                EditorGUILayout.LabelField("Bordering RectTransforms Amount", parallaxRectTransformGenerator.BorderingRectTransforms + " (inherited)");
            }

            EditorGUILayout.Space();
            if (parallaxRectTransformGenerator.repeatUpwards && !parallaxRectTransformGenerator.repeatDownwards)
            {
                GUIContent topImageContent = new GUIContent
                {
                    text = "Repeating Top Sprite",
                    tooltip = "(Optional) Will be repeated above the center sprite. If none is selected will automatically be the same as the center sprite."
                };
                Sprite newTopSprite = (Sprite) EditorGUILayout.ObjectField(topImageContent, parallaxRectTransformGenerator.topSprite, typeof(Sprite), true);
                if (newTopSprite != parallaxRectTransformGenerator.topSprite)
                {
                    EditorUtility.SetDirty(parallaxRectTransformGenerator);
                    Undo.RecordObject(parallaxRectTransformGenerator, "Modified TopSprite in " + parallaxRectTransformGenerator.name);
                    parallaxRectTransformGenerator.topSprite = newTopSprite;
                }
            }
            
            GUIContent centerImageContent = new GUIContent
            {
                text = "Repeating Center Sprite",
                tooltip = "Will be in the center of the repeating sprites. If null this will automatically be the first found child sprite."
            };
            Sprite newCenterSprite = (Sprite) EditorGUILayout.ObjectField(centerImageContent, parallaxRectTransformGenerator.CenterSprite, typeof(Sprite), true);
            if (newCenterSprite != parallaxRectTransformGenerator.CenterSprite)
            {
                EditorUtility.SetDirty(parallaxRectTransformGenerator);
                EditorUtility.SetDirty(parallaxRectTransformGenerator.baseRectTransform.GetComponent<Image>());
                Undo.RecordObject(parallaxRectTransformGenerator, "Modified Center Sprite in " + parallaxRectTransformGenerator.name);
                Undo.RecordObject(parallaxRectTransformGenerator.baseRectTransform.GetComponent<Image>(), "Modified Center Sprite in " + parallaxRectTransformGenerator.name);
                parallaxRectTransformGenerator.CenterSprite = newCenterSprite;
            }
            
            if (parallaxRectTransformGenerator.repeatDownwards && !parallaxRectTransformGenerator.repeatUpwards)
            {
                GUIContent bottomImageContent = new GUIContent
                {
                    text = "Repeating Bot Sprite",
                    tooltip = "(Optional) Will be repeated below the center sprite. If none is selected will automatically be the same as the center sprite."
                };
                Sprite newBottomSprite = (Sprite) EditorGUILayout.ObjectField(bottomImageContent, parallaxRectTransformGenerator.bottomSprite, typeof(Sprite), true);
                if (newBottomSprite != parallaxRectTransformGenerator.bottomSprite)
                {
                    EditorUtility.SetDirty(parallaxRectTransformGenerator);
                    Undo.RecordObject(parallaxRectTransformGenerator, "Modified Bottom Sprite in " + parallaxRectTransformGenerator.name);
                    parallaxRectTransformGenerator.bottomSprite = newBottomSprite;
                }
            }
            
            //do not show preview Buttons during runtime
            if (Application.isPlaying) return;
            
            EditorGUILayout.Space();
            
            GUIContent previewContent = new GUIContent
            {
                text = "Preview Bordering RectTransforms.",
                tooltip = "Will preview all bordering RectTransforms."
            };
            
            if(GUILayout.Button(previewContent))
            {
                parallaxRectTransformGenerator.Init();
                foreach (List<RectMatrixTile> bgLayerRow in parallaxRectTransformGenerator.BgLayerMatrix)
                {
                    foreach (RectMatrixTile bgLayer in bgLayerRow)
                    {
                        if(bgLayer.Rect == parallaxRectTransformGenerator.baseRectTransform) continue;
                        Undo.RegisterCreatedObjectUndo(bgLayer.GameObject, "Preview Bordering RectTransforms");
                    }
                }
                Undo.RecordObject(parallaxRectTransformGenerator, "Preview Bordering RectTransforms in " + parallaxRectTransformGenerator.name);
            }
            
            GUIContent destroyContent = new GUIContent
            {
                text = "Destroy Bordering RectTransforms Preview.",
                tooltip = "Will destroy all bordering RectTransforms of 'AdditionalParallaxImagesGenerator' children."
            };
            
            if(GUILayout.Button(destroyContent))
            {
                foreach (List<RectMatrixTile> bgLayerRow in parallaxRectTransformGenerator.BgLayerMatrix)
                {
                    foreach (RectMatrixTile bgLayer in bgLayerRow)
                    {
                        if(bgLayer.Rect == parallaxRectTransformGenerator.baseRectTransform) continue;
                        Undo.DestroyObjectImmediate(bgLayer.GameObject);
                    }
                }
                parallaxRectTransformGenerator.ClearBgLayers();
                Undo.RecordObject(parallaxRectTransformGenerator, "Destroy Bordering RectTransforms in " + parallaxRectTransformGenerator.name);
            }
        }
    }
}
