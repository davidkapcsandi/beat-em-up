using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace CodeRedCat.ResponsiveScroller.Core.Scripts
{
    [RequireComponent(typeof(ParallaxRectTransformGenerator))]
    public class BackgroundParallaxScroller : MonoBehaviour
    {
        [Tooltip("How fast the image will scroll. This will be in the direction opposite to the camera movement.")]
        public float xShiftingSpeed = 5f, yShiftingSpeed = 5f;
        [Tooltip("Will make the image move by itself, independent from camera movement. Used for e.g. moving clouds.")]
        [SerializeField] private Vector2 autoScrollSpeed = Vector2.zero;

        [NonSerialized] public BackgroundManager BackgroundManager;
        
        //used to have values for xShiftingSpeed and yShiftingSpeed, that are next to 1
        private const float SpeedAdjust = 0.01f;
        private float UsedXShiftingSpeed => xShiftingSpeed * SpeedAdjust;
        private float UsedYShiftingSpeed => yShiftingSpeed * SpeedAdjust;
        private Vector2 _originalPosition, _originalCamPosition;
        private Camera _cameraRef;
        private Transform _cameraTransformRef;
        private ParallaxRectTransformGenerator _parallaxRectTransformGenerator;

        //reference for better code
        private List<List<RectMatrixTile>> BgLayersRows
        {
            set => _parallaxRectTransformGenerator.BgLayerMatrix = value;
            get => _parallaxRectTransformGenerator.BgLayerMatrix;
        }
    
        public void InitializeBackgroundParallaxEffect()
        {
            _parallaxRectTransformGenerator = GetComponent<ParallaxRectTransformGenerator>();
        
            Debug.Assert(Camera.main != null, "Camera.main != null");
            _cameraRef = Camera.main;
            _cameraTransformRef = _cameraRef.transform;
        
            //construct bordering images
            _parallaxRectTransformGenerator.Init();
        
            _originalPosition = transform.position;
            _originalCamPosition = _cameraTransformRef.position;
        }

        //the main method which is supposed to update the Images each frame
        public void ScrollImageMatrix()
        {
            Vector2 currentCamPosition = _cameraTransformRef.position;

            _parallaxRectTransformGenerator.ImagesToScreen(false);
            
            CheckImageSwapping();
            
            ShiftImages(currentCamPosition);
        }
        
        
        private void ShiftImages(Vector3 currentCamPosition)
        {
            //add additional height to the rect, to simulate three-dimensional zoom
            float zoomYAddition = BackgroundManager.threeDimensionalZoom ?
                ((float)Math.Pow(UsedYShiftingSpeed, BackgroundManager.GetThreeDimensionalZoomFactor()) * BackgroundManager.threeDimensionalEffectMultiplier) - UsedYShiftingSpeed * BackgroundManager.threeDimensionalEffectMultiplier
                : 0;
            
            Vector2 distanceTraveledFromStart = new Vector2(
                currentCamPosition.x - _originalCamPosition.x,
                currentCamPosition.y - _originalCamPosition.y
                );

            Vector2 autoMoveDistance = new Vector2(Time.time * -autoScrollSpeed.x, Time.time * -autoScrollSpeed.y);
            
            //shift the anchorPoints to the left or right, will only be used for the image on the top left, which will then be referenced by the other images
            float shiftX = _originalPosition.x + distanceTraveledFromStart.x - distanceTraveledFromStart.x * UsedXShiftingSpeed - autoMoveDistance.x * transform.lossyScale.x;
            float shiftY = _originalPosition.y + distanceTraveledFromStart.y - distanceTraveledFromStart.y * UsedYShiftingSpeed - zoomYAddition + autoMoveDistance.y;

            
            transform.position = new Vector3(shiftX, shiftY, transform.position.z);
        }
    
        private void CheckImageSwapping()
        {
            for (int i = 0; i < BgLayersRows.Count; i++)
            {
                if (!CheckOutOfCameraFrame()) return;
            }
            
            //check if they need to be scrolled
            bool  CheckOutOfCameraFrame()
            {
                bool isOutOfFrame = false;
                //get the Transform in the center
                int centerElement = (BgLayersRows.Count - 1) / 2;
                RectMatrixTile centerTile = BgLayersRows[centerElement][centerElement];
                RectTransform centerTransform = centerTile.Rect;
                
                Vector3[] fourCornersArray = new Vector3[4];
                // The returned array of 4 vertices is clockwise. It starts bottom left and rotates to top left,
                // then top right, and finally bottom right. Note that bottom left, for example, is an (x, y, z) vector with x being left and y being bottom.
                centerTransform.GetWorldCorners(fourCornersArray);

                Vector3 centerPosition = fourCornersArray[0]+ ((fourCornersArray[2] - fourCornersArray[0])/2);

                float halfWidth = (fourCornersArray[2].x - fourCornersArray[0].x) / 2;
                float halfHeight = (fourCornersArray[2].y - fourCornersArray[0].y) / 2;
                Vector3 topRightCorner = _cameraTransformRef.position + new Vector3(halfWidth, halfHeight, 0);
                Vector3 bottomLeftCorner = _cameraTransformRef.position - new Vector3(halfWidth, halfHeight, 0);
                
                
                if (centerPosition.x > topRightCorner.x)
                {
                    SwapImages(Vector2.left);
                    centerTransform = BgLayersRows[(BgLayersRows.Count-1)/2][centerElement].Rect;
                    centerTransform.GetWorldCorners(fourCornersArray);
                    isOutOfFrame = true;
                }
                if (centerPosition.y > topRightCorner.y)
                {
                    isOutOfFrame = true;
                    SwapImages(Vector2.down);
                }
                if (centerPosition.x < bottomLeftCorner.x)
                {
                    SwapImages(Vector2.right);
                    centerTransform = BgLayersRows[(BgLayersRows.Count-1)/2][centerElement].Rect;
                    centerTransform.GetWorldCorners(fourCornersArray);
                    isOutOfFrame = true;
                }
                if (centerPosition.y < bottomLeftCorner.y)
                {
                    isOutOfFrame = true;
                    SwapImages(Vector2.up);
                }
                    
                return isOutOfFrame;
            }
        }

        private void SwapImages(Vector2 directionOfSwap)
        {
            RearrangeRectLists();
            ReconstructRectAnchors();

            void RearrangeRectLists()
            {
                List<List<RectMatrixTile>> newSortedBgLayerRows = new();

                if (directionOfSwap == Vector2.right || directionOfSwap == Vector2.left)
                {
                    foreach (List<RectMatrixTile> bgLayersRow in BgLayersRows)
                    {
                        int layerToBeSwappedIndex;
                        RectMatrixTile editedTile;
                        
                        List<RectMatrixTile> newSortedBgLayerRow = new();
                        
                        switch (directionOfSwap.x, directionOfSwap.y)
                        {
                            case (1,0): //right
                                layerToBeSwappedIndex = 0;
                
                                //prepare a new List with the sorted bgLayers
                                for (int i = 1; i < bgLayersRow.Count; i++)
                                {
                                    newSortedBgLayerRow.Add(bgLayersRow[i]);
                                }
                                editedTile = bgLayersRow[layerToBeSwappedIndex];
                                editedTile.Column = newSortedBgLayerRow[^1].Column + 1;
                                editedTile.GameObject.name = _parallaxRectTransformGenerator.OriginalName + " Row " + editedTile.Row + " Column " + editedTile.Column;
                                editedTile.IsPositioned = false;
                                newSortedBgLayerRow.Add(editedTile);
                                break;
                            case (-1,0): //left
                                layerToBeSwappedIndex = bgLayersRow.Count -1;
                
                                editedTile = bgLayersRow[layerToBeSwappedIndex];
                                editedTile.Column = bgLayersRow[0].Column - 1;
                                editedTile.GameObject.name = _parallaxRectTransformGenerator.OriginalName + " Row " + editedTile.Row + " Column " + editedTile.Column;
                                editedTile.IsPositioned = false;
                                newSortedBgLayerRow.Add(editedTile);
                                
                                //prepare a new List with the sorted bgLayers
                                for (int i = 0; i < bgLayersRow.Count-1; i++)
                                {
                                    newSortedBgLayerRow.Add(bgLayersRow[i]);
                                }
                                break;
                        }

                        newSortedBgLayerRows.Add(newSortedBgLayerRow);
                    }
                }

                if (directionOfSwap == Vector2.up || directionOfSwap == Vector2.down)
                {
                    switch (directionOfSwap.x, directionOfSwap.y)
                    {
                        case (0,1): //up
                            //prepare a new List with the sorted bgLayers
                            List<RectMatrixTile> newUpperRow = new List<RectMatrixTile>();
                            foreach (RectMatrixTile rectMatrixTile in BgLayersRows[^1])
                            {
                                RectMatrixTile matrixTile = rectMatrixTile;
                                matrixTile.Row = BgLayersRows[0][0].Row + 1;
                                matrixTile.GameObject.name = _parallaxRectTransformGenerator.OriginalName + " Row " + matrixTile.Row + " Column " + matrixTile.Column;
                                matrixTile.IsPositioned = false;
                                newUpperRow.Add(matrixTile);
                            }
                            newSortedBgLayerRows.Add(newUpperRow);
                            
                            for (int i = 0; i < BgLayersRows.Count-1; i++)
                            {
                                newSortedBgLayerRows.Add(BgLayersRows[i]);
                            }
                            
                            //check what kind of image should be displayed for now added first row
                            AssignRowImage(newSortedBgLayerRows[0]);
                            
                            break;
                        case (0,-1): //down
                            //prepare a new List with the sorted bgLayers
                            for (int i = 1; i < BgLayersRows.Count; i++)
                            {
                                newSortedBgLayerRows.Add(BgLayersRows[i]);
                            }
                            
                            
                            List<RectMatrixTile> newLowerRow = new List<RectMatrixTile>();
                            foreach (RectMatrixTile rectMatrixTile in BgLayersRows[0])
                            {
                                RectMatrixTile matrixTile = rectMatrixTile;
                                matrixTile.Row = BgLayersRows[^1][0].Row - 1;
                                matrixTile.GameObject.name = _parallaxRectTransformGenerator.OriginalName + " Row " + matrixTile.Row + " Column " + matrixTile.Column;
                                matrixTile.IsPositioned = false;
                                newLowerRow.Add(matrixTile);
                            }
                            newSortedBgLayerRows.Add(newLowerRow);
                            
                            //check what kind of image should be displayed for now added last row
                            AssignRowImage(newSortedBgLayerRows[^1]);
                            
                            break;
                    }
                }
                BgLayersRows = newSortedBgLayerRows;

                void AssignRowImage(List<RectMatrixTile> tileRow)
                {
                    Sprite imageToAssign = _parallaxRectTransformGenerator.CenterSprite;
                    
                    if(tileRow[0].Row < 0) imageToAssign = _parallaxRectTransformGenerator.bottomSprite;
                    if(tileRow[0].Row > 0) imageToAssign = _parallaxRectTransformGenerator.topSprite;

                    float alphaValue = _parallaxRectTransformGenerator.OriginalImageAlphaValue;
                    if (tileRow[0].Row > 0 && !_parallaxRectTransformGenerator.repeatUpwards)
                    {
                        alphaValue = 0;
                    }
                    if (tileRow[0].Row < 0 && !_parallaxRectTransformGenerator.repeatDownwards)
                    {
                        alphaValue = 0;
                    }

                    foreach (RectMatrixTile tile in tileRow)
                    {
                        Image selectedImage = tile.GameObject.GetComponent<Image>();
                        selectedImage.sprite = imageToAssign;
                        selectedImage.color = new Color(selectedImage.color.r, selectedImage.color.g, selectedImage.color.b, alphaValue);
                    }
                }
            }

            void ReconstructRectAnchors()
            {
                List<List<RectMatrixTile>> newSortedList = new List<List<RectMatrixTile>>();
                RectTransform firstRect = BgLayersRows[0][0].Rect;
                
                //width of a Rect
                float anchorXDistance = firstRect.anchorMax.x - firstRect.anchorMin.x;
                //height of a Rect
                float anchorYDistance = firstRect.anchorMax.y - firstRect.anchorMin.y;
                
                foreach (List<RectMatrixTile> bgLayersRow in BgLayersRows)
                {
                    List<RectMatrixTile> newRow = new List<RectMatrixTile>();
                    foreach (RectMatrixTile bgLayers in bgLayersRow)
                    {
                        RectMatrixTile tile = bgLayers;
                        if (!bgLayers.IsPositioned)
                        {
                            tile.Rect.anchorMin = new Vector2(tile.Column * anchorXDistance, tile.Row * anchorYDistance);
                            tile.Rect.anchorMax = new Vector2(anchorXDistance + tile.Column * anchorXDistance, anchorYDistance + tile.Row * anchorYDistance);
                            tile.IsPositioned = true;
                        }
                        newRow.Add(tile);
                    }
                    newSortedList.Add(newRow);
                }

                BgLayersRows = newSortedList;
            }
        }
    }
}