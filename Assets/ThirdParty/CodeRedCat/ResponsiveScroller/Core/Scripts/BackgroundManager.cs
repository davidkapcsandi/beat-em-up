using UnityEngine;

namespace CodeRedCat.ResponsiveScroller.Core.Scripts
{
    public class BackgroundManager : MonoBehaviour
    {
        [Tooltip("A parent with all the parallax backgrounds. If none is selected, it is assumed that it is the first child gameObject.")]
        [SerializeField] private GameObject sceneryParent;
        [Tooltip("Will effect, how much zooming in and out affects the individual layer size. WIll be react with changes on the Camera.orthographicSize. To get a flat zoom effect, set the value to 1 and disable threeDimensional zoom.")]
        [SerializeField] public float zoomMultiplier = 0.75f;
        [Tooltip("React on changing Orthographic Camera Size. If true, zooming in and out will also simulate a parallax effect. Without this option, zooming in and out will not effect layers.  To get a flat zoom effect, set the zoomMultiplier to 1 and disable threeDimensional zoom.")]
        [SerializeField] public bool threeDimensionalZoom = true;
        [Tooltip("How much zooming will effect the y height, where time 1 is going to be the start orthographic size of the mainCamera and value 1 is the start scale of the scenery Parent. Always use a key with time 1, value 1, so that nothing happens, when you use the startOrthographic size.")]
        [SerializeField] private AnimationCurve threeDimensionalEffectFactor = AnimationCurve.Linear(0,1,25,3);
        [Tooltip("Will be multiplied with the curve of 'Three Dimensional Effect Factor'.")]
        [Range (0, 1000)] [SerializeField] public float threeDimensionalEffectMultiplier = 50;
        [Tooltip("Will be added to the scale of each RectTransform, to prevent a fine line of pixels between the background images. This will make the RectTransforms overlap slightly.")]
        [SerializeField] public float scaleAddition = 0.0005f;
        
        [Range(1,10)] [HideInInspector, SerializeField] private int borderingRectTransformsOverride = 1;
        
        //ToolTip in BackgroundManagerInspector
        //Dev Note: I think so that with an execution order near 0 the images can update, before the UI refreshes its frame. But that's just a theory...
        [HideInInspector, SerializeField] public bool overwriteExecutionOrder = true;
        [HideInInspector, SerializeField] public int executionOrder = -500;
        
        [HideInInspector, SerializeField] public bool showStretchOptions = true;
        [HideInInspector, SerializeField] public int autoDetermineBorderingImagesMax = 10;
        
        [HideInInspector, SerializeField] private bool autoDetermineBorderingImages = true;
        [HideInInspector, SerializeField] private bool preserveAspectStretch = true ;
        [HideInInspector, SerializeField] private bool preserveAspectStretchOut = true; //false = fill in, true = stretch out
        [Range(0,1)] [HideInInspector, SerializeField] private float stretchFactor;
        
        private BackgroundParallaxScroller[] _backgroundParallaxScroller;

        public bool AutoDetermineBorderingImages
        {
            get => autoDetermineBorderingImages;
            set
            {
                if (value != autoDetermineBorderingImages)
                {
                    autoDetermineBorderingImages = value;
                    AutoBorderingImagesChanged();
                }
            }
        }
        
        public int BorderingRectTransformsOverride
        {
            get => borderingRectTransformsOverride;
            set
            {
                if (value != borderingRectTransformsOverride)
                {
                    int oldAmount = borderingRectTransformsOverride;
                    borderingRectTransformsOverride = value;
                    BorderingImagesAmountChanged(oldAmount, value);
                }
            }
        }
        
        public bool PreserveAspectStretch
        {
            get => preserveAspectStretch;
            set
            {
                if (value != preserveAspectStretch)
                {
                    preserveAspectStretch = value;
                    StretchOptionChanged();
                }
            }
        }
        
        public bool PreserveAspectStretchOut
        {
            get => preserveAspectStretchOut;
            set
            {
                if (value != preserveAspectStretchOut)
                {
                    preserveAspectStretchOut = value;
                    StretchOptionChanged();
                }
            }
        }
        public float StretchFactor
        {
            get => stretchFactor;
            set
            {
                if (value != stretchFactor)
                {
                    stretchFactor = value;
                    StretchOptionChanged();
                }
            }
        }

        private Camera _cameraRef;
        private float _startCameraSize;
        private Vector3 startSceneryParentLocalScale;

        private void StretchOptionChanged()
        {
            //don't mess up the values, the user has set up before
            if (!Application.isPlaying) return;
            
            foreach (ParallaxRectTransformGenerator parallaxRTGenerator in sceneryParent.GetComponentsInChildren<ParallaxRectTransformGenerator>())
            {
                parallaxRTGenerator.ImagesToScreen(true);
            }
        }
        
        private void AutoBorderingImagesChanged()
        {
            if (!Application.isPlaying) return;
            
            foreach (ParallaxRectTransformGenerator parallaxRTGenerator in sceneryParent.GetComponentsInChildren<ParallaxRectTransformGenerator>())
            {
                parallaxRTGenerator.ImagesToScreen(true);
            }
        }
        
        private void BorderingImagesAmountChanged(int oldAmount, int newAmount)
        {
            if (!Application.isPlaying) return;
            
            foreach (ParallaxRectTransformGenerator parallaxRTGenerator in sceneryParent.GetComponentsInChildren<ParallaxRectTransformGenerator>())
            {
                parallaxRTGenerator.UpdateAmountOfBorderingImages(oldAmount, newAmount);
            }
        }
        
        void Awake()
        {
            if(sceneryParent == null) sceneryParent = transform.GetChild(0).gameObject;

            _backgroundParallaxScroller = sceneryParent.GetComponentsInChildren<BackgroundParallaxScroller>();
            
            //initialize each BackgroundParallaxScroller
            foreach (BackgroundParallaxScroller backgroundParallaxScroller in _backgroundParallaxScroller)
            {
                backgroundParallaxScroller.BackgroundManager = this;
                backgroundParallaxScroller.InitializeBackgroundParallaxEffect();
            }
        
            Debug.Assert(Camera.main != null, "Camera.main != null");
            _cameraRef = Camera.main;
            _startCameraSize = _cameraRef.orthographicSize;
            startSceneryParentLocalScale = sceneryParent.transform.localScale;
        }

        private void Update()
        {
            //check if the camera is zooming
            if (_cameraRef.orthographicSize < 0) return;

            ApplyNewCameraZoomScale();

            foreach (BackgroundParallaxScroller backgroundParallaxScroller in _backgroundParallaxScroller)
            {
                backgroundParallaxScroller.ScrollImageMatrix();
            }

            void ApplyNewCameraZoomScale()
            {
                float zoomChange = GetCurrentMultipliedZoomChange();
                
                Vector3 newLocalScale = new Vector3(
                    startSceneryParentLocalScale.x * zoomChange,
                    startSceneryParentLocalScale.y * zoomChange,
                    startSceneryParentLocalScale.z * zoomChange
                );
                
                sceneryParent.transform.localScale = newLocalScale;
            }
        }

        private float GetCurrentMultipliedZoomChange()
        {
            float newOrthographicSize = _cameraRef.orthographicSize;
            if (newOrthographicSize <= 0)
            {
                newOrthographicSize = 0.01f;
            }
            
            float zoomChange = 1 + (-1+(_startCameraSize / newOrthographicSize)) * zoomMultiplier;

            return zoomChange;
        }

        public float GetThreeDimensionalZoomFactor()
        {
            return threeDimensionalEffectFactor.Evaluate(_cameraRef.orthographicSize / _startCameraSize);
        }

        public ParallaxRectTransformGenerator[] InstantiateBorderingImagesInChildren()
        {
            ParallaxRectTransformGenerator[] parallaxRectTGenerators = GetComponentsInChildren<ParallaxRectTransformGenerator>();
            foreach (ParallaxRectTransformGenerator imageGen in parallaxRectTGenerators)
            {
                imageGen.Init();
            }
            return parallaxRectTGenerators;
        }

        public void ClearBgLayers()
        {
            foreach (ParallaxRectTransformGenerator imageGen in GetComponentsInChildren<ParallaxRectTransformGenerator>())
            {
                imageGen.ClearBgLayers();
            }
        }
    }
}