using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CodeRedCat.ResponsiveScroller.Core.Scripts
{
    public class ParallaxRectTransformGenerator : MonoBehaviour
    {
        [Tooltip("The background Manager of this generator.")]
        [SerializeField] private BackgroundManager backgroundManager;
        [Tooltip("Does not have to bet set manually. The base RectTransform with the intended middle content. This will automatically be the first child if left as 'none'.")]
        [SerializeField] public RectTransform baseRectTransform;
        [Tooltip("The original transform scale of the base RectTransform.")]
        [HideInInspector,SerializeField] public Vector3 originalBaseScale = Vector3.one;
        [Tooltip("Repeat the image, when the camera is higher, than the center sprite.")]
        [SerializeField] public bool repeatUpwards = true;
        [Tooltip("Repeat the image, when the camera is lower, than the center sprite.")]
        [SerializeField] public bool repeatDownwards = true;
        
        [HideInInspector, SerializeField] public bool inheritBorderingRectTransforms = true;
        [HideInInspector, SerializeField] public Sprite topSprite, bottomSprite;
        [HideInInspector, SerializeField] private Sprite centerSprite;

        public Sprite CenterSprite
        {
            get => centerSprite;
            set
            {
                centerSprite = value;
                if (baseRectTransform != null && baseRectTransform.GetComponent<Image>() != null)
                {
                    baseRectTransform.GetComponent<Image>().sprite = value;
                }
            }
        }
        
        [Tooltip("The amount of RectTransforms bordering the main Image to each side. This will always be 1 left and right. If repetition up and/or down has been enabled, there will also be a row above and below.")]
        public int BorderingRectTransforms
        {
            get
            {
                if (inheritBorderingRectTransforms && BackgroundManager != null && !BackgroundManager.AutoDetermineBorderingImages)
                {
                    return BackgroundManager.BorderingRectTransformsOverride;
                }
                return _borderingRectTransforms;
            }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value) + " on _borderingRectTransforms= " + value);
                _borderingRectTransforms = value;
                if (Application.isPlaying)
                {
                    UpdateImageMatrix(_borderingRectTransforms);
                }

            }
        }
        
        public BackgroundManager BackgroundManager
        {
            get
            {
                if (backgroundManager == null)
                {
                    try
                    {
                        if(backgroundManager == null) backgroundManager = GetComponentInParent<BackgroundManager>();
                    }
                    catch (NullReferenceException)
                    {
                        Debug.LogError("No BackgroundManager found as parent for " + gameObject.name + "! Intended use for the Parallax Scroller Images  is within the BackgroundManager Hierarchy.", gameObject);
                        //inefficient call
                        backgroundManager = FindObjectOfType<BackgroundManager>();
                    }
                }
                return backgroundManager;
            }
            set => backgroundManager = value;
        }

        private readonly Vector2[] _originalAnchorValues = new Vector2[2];
        [NonSerialized] public float OriginalImageAlphaValue;
        [NonSerialized] public String OriginalName;
        private int _borderingRectTransforms = 1;
        private Vector2 _lastScreenSize;
        private Camera _cameraRef;
        private float _lastOrthographicSize;

        public List<List<RectMatrixTile>> BgLayerMatrix = new();

        public void Init()
        {
            _cameraRef = Camera.main;
            System.Diagnostics.Debug.Assert(_cameraRef != null, nameof(_cameraRef) + " != null");
            _lastOrthographicSize = _cameraRef.orthographicSize;
            
            if (baseRectTransform == null)
            {
                baseRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
                if (baseRectTransform == null)
                {
                    Debug.LogError("No RectTransform was found as a child. Please add a RectTransform as a child");
                    return;
                }
            }

            //init Sprite Variables
            if (CenterSprite == null) CenterSprite = baseRectTransform.GetComponent<Image>().sprite;
            else baseRectTransform.GetComponent<Image>().sprite = CenterSprite;
            if (topSprite == null || (repeatUpwards && repeatDownwards)) topSprite = CenterSprite;
            if (bottomSprite == null || (repeatUpwards && repeatDownwards)) bottomSprite = CenterSprite;
            
            //if the user has previewed the Matrix, clear it
            if (transform.childCount > 1)
            {
                foreach (Transform transChild in transform.GetComponentsInChildren<Transform>())
                {
                    if (transChild.gameObject != baseRectTransform.gameObject && transChild != transform)
                    {
                        DestroyImmediate(transChild.gameObject);
                    }
                }
                DestroyBorderingImages(BgLayerMatrix);
            }
            BgLayerMatrix.Clear();
            
            originalBaseScale = baseRectTransform.localScale;
            OriginalName = baseRectTransform.name;
            
            _originalAnchorValues[0] = baseRectTransform.anchorMin;
            _originalAnchorValues[1] = baseRectTransform.anchorMax;
            OriginalImageAlphaValue = baseRectTransform.GetComponent<Image>().color.a;
            
            baseRectTransform.localScale = new Vector3(
                baseRectTransform.localScale.x + backgroundManager.scaleAddition,
                baseRectTransform.localScale.y + backgroundManager.scaleAddition,
                baseRectTransform.localScale.z + backgroundManager.scaleAddition);
            
            //construct a 1x1 matrix with only the current image. This will then be used be the UpdateImageMatrix function
            RectMatrixTile baseMatrixTile = new RectMatrixTile(baseRectTransform.gameObject, baseRectTransform, 0, 0, true);
            List<RectMatrixTile> centerRow = new List<RectMatrixTile>();
            centerRow.Add(baseMatrixTile);
            BgLayerMatrix.Add(centerRow);

            UpdateImageMatrix(BorderingRectTransforms);
        }

        private void UpdateImageMatrix(int newBorderingRectTransforms)
        {
            //get the old centerRow, which should be right in the middle
            
            List<List<RectMatrixTile>> newBgLayersMatrix = ConstructEmptyImageMatrix(newBorderingRectTransforms);
            //construct new Empty Image Matrix
            
            //Mark all old Tiles for Destruction
            MarkOldMatrixForDestruction(BgLayerMatrix);
            
            //fill new Matrix with old Matrix gameObjects or Instantiate new gameObjects
            newBgLayersMatrix = FillMatrixWithGameObjects(newBgLayersMatrix);
            
            //set position of each Tile of the matrix
            newBgLayersMatrix = SetMatrixPositions(newBgLayersMatrix);
            
            //select image sprites for each object
            newBgLayersMatrix = SetMatrixSprites(newBgLayersMatrix);

            CheckIfNewMatrixContainsBaseRect(newBgLayersMatrix);
            
            //Destroy GameObject, if new Matrix is smaller than old one
            DestroyMarkedTiles(BgLayerMatrix);

            BgLayerMatrix = newBgLayersMatrix;
            
            List<List<RectMatrixTile>> ConstructEmptyImageMatrix(int borderingRectTransforms)
            {
                //select old centerRow and shift it up by the new amount of bordering images
                int centerRow = BgLayerMatrix[(BgLayerMatrix.Count - 1) / 2][(BgLayerMatrix.Count - 1) / 2].Row;
                int firstRow = centerRow + borderingRectTransforms;
                
                int centerColumnAnchor = BgLayerMatrix[(BgLayerMatrix.Count - 1) / 2][(BgLayerMatrix.Count - 1) / 2].Column;
                
                List<List<RectMatrixTile>> emptyRectMatrix = new List<List<RectMatrixTile>>();
                //row and column are calculaed in a way, that make it easy to determine their anchorPositions later
                for (int row = firstRow; row >= firstRow-borderingRectTransforms*2; row--)
                {
                    List<RectMatrixTile> newRectMatrixTileRow = new();

                    for (int column = 0 - borderingRectTransforms; column <= borderingRectTransforms; column++)
                    {
                        RectMatrixTile newRectMatrixTile = new RectMatrixTile(
                            null, null, row, centerColumnAnchor - column, false);
                        newRectMatrixTileRow.Add(newRectMatrixTile);
                    }
                    
                    emptyRectMatrix.Add(newRectMatrixTileRow);
                }
                return emptyRectMatrix;
            }
            
            List<List<RectMatrixTile>> FillMatrixWithGameObjects(List<List<RectMatrixTile>> bgLayersMatrixRef)
            {
                List<List<RectMatrixTile>> filledRectMatrix = new List<List<RectMatrixTile>>();
                foreach (List<RectMatrixTile> bgLayersMatrixRefRow in bgLayersMatrixRef)
                {
                    List<RectMatrixTile> newRectMatrixTileRow = new();
                    for (int j = 0; j < bgLayersMatrixRef.Count; j++)
                    {
                        RectMatrixTile existingTile;
                        if (BgLayerMatrix.Any(x => x.Any(y => y.Row == bgLayersMatrixRefRow[j].Row && y.Column == bgLayersMatrixRefRow[j].Column)))
                        {
                            List<RectMatrixTile> existingRow = BgLayerMatrix.Find(x => x.Any(y => y.Row == bgLayersMatrixRefRow[j].Row && y.Column == bgLayersMatrixRefRow[j].Column));
                            existingTile = existingRow.Find(y => y.Row == bgLayersMatrixRefRow[j].Row && y.Column == bgLayersMatrixRefRow[j].Column);
                            BgLayerMatrix.Find(x => x == existingRow).Remove(existingTile);
                            existingTile.Destroy = false;
                            BgLayerMatrix.Find(x => x == existingRow).Add(existingTile);
                        }
                        else
                        {
                            existingTile = bgLayersMatrixRefRow[j];
                            GameObject newTile = Instantiate(baseRectTransform.gameObject, transform);
                            newTile.name = OriginalName + " Row " + existingTile.Row + " Column " + existingTile.Column;
                            existingTile.GameObject = newTile;
                            existingTile.Rect = newTile.GetComponent<RectTransform>();
                        }
                        newRectMatrixTileRow.Add(existingTile);
                    }
                    filledRectMatrix.Add(newRectMatrixTileRow);
                }
                return filledRectMatrix;
            }


            List<List<RectMatrixTile>> SetMatrixPositions(List<List<RectMatrixTile>> bgLayersMatrixRef)
            {
                List<List<RectMatrixTile>> positionedRectMatrix = new List<List<RectMatrixTile>>();
                foreach (List<RectMatrixTile> bgLayersMatrixRefRow in bgLayersMatrixRef)
                {
                    List<RectMatrixTile> newRectMatrixTileRow = new();
                    foreach (RectMatrixTile rectMatrixTile in bgLayersMatrixRefRow)
                    {
                        RectMatrixTile existingTile = rectMatrixTile;
                        if (!existingTile.IsPositioned)
                        {
                            int row = existingTile.Row;
                            int column = existingTile.Column;
                            Vector2 anchorDistances = baseRectTransform.anchorMax - baseRectTransform.anchorMin;
                            existingTile.Rect.anchorMin = new Vector2(
                                anchorDistances.x * column,
                                anchorDistances.y * row);
                            existingTile.Rect.anchorMax = new Vector2(
                                anchorDistances.x + anchorDistances.x * column,
                                anchorDistances.y + anchorDistances.y * row);
                            existingTile.IsPositioned = true;
                        }

                        newRectMatrixTileRow.Add(existingTile);
                    }

                    positionedRectMatrix.Add(newRectMatrixTileRow);
                }

                return positionedRectMatrix;
            }
            
            
            List<List<RectMatrixTile>> SetMatrixSprites(List<List<RectMatrixTile>> bgLayersMatrixRef)
            {
                List<List<RectMatrixTile>> rectMatrixWithCorrectImages = new List<List<RectMatrixTile>>();
                foreach (List<RectMatrixTile> bgLayersMatrixRefRow in bgLayersMatrixRef)
                {
                    Sprite rowSprite = CenterSprite;
                    if (bgLayersMatrixRefRow[0].Row > 0)
                    {
                        rowSprite = topSprite;
                    }
                    if (bgLayersMatrixRefRow[0].Row < 0)
                    {
                        rowSprite = bottomSprite;
                    }

                    float alphaValue = OriginalImageAlphaValue;
                    if (bgLayersMatrixRefRow[0].Row > 0 && !repeatUpwards)
                    {
                        alphaValue = 0;
                    }
                    if (bgLayersMatrixRefRow[0].Row < 0 && !repeatDownwards)
                    {
                        alphaValue = 0;
                    }
                    
                    List<RectMatrixTile> newRectMatrixTileRow = new();
                    foreach (RectMatrixTile rectMatrixTile in bgLayersMatrixRefRow)
                    {
                        RectMatrixTile existingTile = rectMatrixTile;
                        Image selectedImage = existingTile.GameObject.GetComponent<Image>();
                        selectedImage.sprite = rowSprite;
                        selectedImage.color = new Color(selectedImage.color.r, selectedImage.color.g, selectedImage.color.b, alphaValue);
                        newRectMatrixTileRow.Add(existingTile);
                    }
                    rectMatrixWithCorrectImages.Add(newRectMatrixTileRow);
                }

                return rectMatrixWithCorrectImages;
            }

            void CheckIfNewMatrixContainsBaseRect(List<List<RectMatrixTile>> bgLayersMatrixRef)
            {
                bool containsBaseRect = bgLayersMatrixRef.Any(x => x.Any(y => y.Rect == baseRectTransform));
                    
                if (!containsBaseRect)
                {
                    if(!Application.isPlaying) return;
                    baseRectTransform = bgLayersMatrixRef[(bgLayersMatrixRef.Count-1)/2][(bgLayersMatrixRef.Count-1)/2].Rect;
                }
            }
        }
        
        private void MarkOldMatrixForDestruction(List<List<RectMatrixTile>> bgLayersMatrixRef)
        {
            foreach (List<RectMatrixTile> bgLayersMatrixRefRow in bgLayersMatrixRef)
            {
                for (int j = 0; j < bgLayersMatrixRef.Count; j++)
                {
                    RectMatrixTile rectMatrixTile = bgLayersMatrixRefRow[j];
                    rectMatrixTile.Destroy = true;
                    bgLayersMatrixRefRow[j] = rectMatrixTile;
                }
            }
        }

        private void DestroyMarkedTiles(List<List<RectMatrixTile>> bgLayersMatrix)
        {
            foreach (List<RectMatrixTile> bgLayersMatrixRefRow in bgLayersMatrix)
            {
                foreach (RectMatrixTile rectMatrixTile in bgLayersMatrixRefRow)
                {
                    RectMatrixTile existingTile = rectMatrixTile;
                    if (existingTile.Destroy)
                    {
                        if(!Application.isPlaying && existingTile.Rect == baseRectTransform) continue;
                        DestroyImmediate(existingTile.GameObject);
                    }
                }
            }
        }
    
        public void DestroyBorderingImages(List<List<RectMatrixTile>> bgLayersMatrixRef)
        {
            if (baseRectTransform == null)
            {
                Debug.LogError("BaseRectTransform has not been assigned. Operation aborted to prevent loss of BaseRectTransform.");
                return;
            }

            MarkOldMatrixForDestruction(bgLayersMatrixRef);
            DestroyMarkedTiles(bgLayersMatrixRef);
        
            bgLayersMatrixRef.Clear();
            
            baseRectTransform.localScale = originalBaseScale;
        }

        public void ClearBgLayers()
        {
            if (baseRectTransform == null)
            {
                Debug.LogError("BaseRectTransform has not been assigned. Operation aborted to prevent loss of BaseRectTransform.");
                return;
            }
            
            //if the user has previewed the Matrix, clear it
            if (!Application.isPlaying && transform.childCount > 1)
            {
                foreach (Transform transChild in transform.GetComponentsInChildren<Transform>())
                {
                    if (transChild.gameObject != baseRectTransform.gameObject && transChild != transform)
                    {
                        DestroyImmediate(transChild.gameObject);
                    }
                }
            }
        
            BgLayerMatrix.Clear();
            
            baseRectTransform.localScale = originalBaseScale;
        }

        //Handles the image to screen aspect ratio, stretching options and how many images are needed to fill the screen
        public void ImagesToScreen(bool optionChanged)
        {
            Vector2 screenResolution = new Vector2(Screen.width, Screen.height);
#if UNITY_EDITOR
            screenResolution = GetMainGameViewSize();
#endif

            if(_cameraRef == null) _cameraRef = Camera.main;
            
            //Check if resolution changed or backgroundManager stretch options
            System.Diagnostics.Debug.Assert(_cameraRef != null, nameof(_cameraRef) + " != null");
            if (_lastScreenSize == screenResolution && !optionChanged && _cameraRef.orthographicSize == _lastOrthographicSize) return;
            _lastScreenSize = screenResolution;
            _lastOrthographicSize = _cameraRef.orthographicSize;
            
            RectTransform rectTransform = GetComponent<RectTransform>();
            Image sourceImage = baseRectTransform.GetComponent<Image>();

            
            HandleStretchOptions();

            Vector3[] rectCorners = new Vector3[4];
            baseRectTransform.GetWorldCorners(rectCorners);
            Vector2 rectSize = _cameraRef.WorldToScreenPoint(rectCorners[2]) - _cameraRef.WorldToScreenPoint(rectCorners[0]);
            if(backgroundManager.AutoDetermineBorderingImages) DetermineAmountOfBorderingImages(screenResolution, rectSize);
            
            void DetermineAmountOfBorderingImages(Vector2 screenRes, Vector2 currentRectRes)
            {
                //calc how many images would fit into frame and still have an image outside for swapping
                Vector2 amountOfImagesNeeded = new Vector2(screenRes.x/currentRectRes.x + 1.5f,screenRes.y/currentRectRes.y + 1.5f);
                //calc amount of neighbors
                amountOfImagesNeeded = new Vector2(Mathf.Ceil((amountOfImagesNeeded.x - 1)/2),Mathf.Ceil((amountOfImagesNeeded.y - 1)/2));
                //always have at least 1 neighbor
                amountOfImagesNeeded = new Vector2(Mathf.Max(amountOfImagesNeeded.x, 1), Mathf.Max(amountOfImagesNeeded.y, 1));

                //get the least needed amount of images, which is the bigger value
                int needAmountOfImages = (int)amountOfImagesNeeded.x > (int)amountOfImagesNeeded.y ?
                    (int)amountOfImagesNeeded.x : (int)amountOfImagesNeeded.y;

                needAmountOfImages = Mathf.Min(needAmountOfImages, backgroundManager.autoDetermineBorderingImagesMax);
                needAmountOfImages = Mathf.Max(needAmountOfImages, 1);
                
                if (needAmountOfImages != BorderingRectTransforms)
                {
                    BorderingRectTransforms = needAmountOfImages;
                }
            }
            
#if UNITY_EDITOR
            Vector2 GetMainGameViewSize()
            {
                Type T = Type.GetType("UnityEditor.GameView,UnityEditor");
                System.Diagnostics.Debug.Assert(T != null, nameof(T) + " != null");
                System.Reflection.MethodInfo getSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Object res = getSizeOfMainGameView?.Invoke(null,null);
                System.Diagnostics.Debug.Assert(res != null, nameof(res) + " != null");
                return (Vector2) res;
            }
#endif
            
            void HandleStretchOptions()
            {
                Vector2 imageToDisplayRatio = new Vector2(
                    sourceImage.sprite.rect.size.x / screenResolution.x,
                    sourceImage.sprite.rect.size.y / screenResolution.y
                );

                Vector2 anchorLengths = new Vector2(
                    _originalAnchorValues[1].x - _originalAnchorValues[0].x,
                    _originalAnchorValues[1].y - _originalAnchorValues[0].y
                );

                Vector2 anchorAdditions;
                if (backgroundManager.PreserveAspectStretch)
                {
                    //determine stretching method
                    float leftComparison = imageToDisplayRatio.x;
                    float rightComparison = imageToDisplayRatio.y;

                    if (!backgroundManager.PreserveAspectStretchOut)
                    {
                        leftComparison = imageToDisplayRatio.y;
                        rightComparison = imageToDisplayRatio.x;
                    }

                    if (leftComparison > rightComparison)
                    {
                        float yToXUpscale =
                            (sourceImage.sprite.rect.size.x * screenResolution.y / sourceImage.sprite.rect.size.y) /
                            screenResolution.x;
                        anchorAdditions = new Vector2((1 - yToXUpscale) / 2 * (1 - backgroundManager.StretchFactor), 0);
                    }
                    else
                    {
                        float xToYUpscale =
                            (sourceImage.sprite.rect.size.y * screenResolution.x / sourceImage.sprite.rect.size.x) /
                            screenResolution.y;
                        anchorAdditions = new Vector2(0, (1 - xToYUpscale) / 2 * (1 - backgroundManager.StretchFactor));
                    }
                }
                else
                {
                    anchorAdditions = new Vector2(
                        ((1 - imageToDisplayRatio.x) * anchorLengths.x * (1 - backgroundManager.StretchFactor)) / 2,
                        ((1 - imageToDisplayRatio.y) * anchorLengths.y * (1 - backgroundManager.StretchFactor)) / 2
                    );
                }


                rectTransform.anchorMin = new Vector2(
                    _originalAnchorValues[0].x + anchorAdditions.x,
                    _originalAnchorValues[0].y + anchorAdditions.y
                );
                rectTransform.anchorMax = new Vector2(
                    _originalAnchorValues[1].x - anchorAdditions.x,
                    _originalAnchorValues[1].y - anchorAdditions.y
                );
            }
        }

        public void UpdateAmountOfBorderingImages(int oldAmount, int newAmount)
        {
            if (newAmount != oldAmount)
            {
                UpdateImageMatrix(newAmount);
            }
        }
    }

    public struct RectMatrixTile
    {
        public GameObject GameObject;
        public RectTransform Rect;
        public int Row;
        public int Column;
        public bool IsPositioned;
        public bool Destroy;

        public RectMatrixTile(GameObject gameObject, RectTransform rect, int row, int column, bool isPositioned)
        {
            GameObject = gameObject;
            Rect = rect;
            Row = row;
            Column = column;
            IsPositioned = isPositioned;
            Destroy = false;
        }
    }
}
