using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using Unity.Properties;

public sealed class DepthVisualizer : MonoBehaviour
{
    #region Scene object references

    [SerializeField] AROcclusionManager _manager = null;
    [SerializeField] UIDocument _ui = null;

    #endregion

    #region Project asset reference

    [SerializeField] Material _material = null;

    #endregion

    #region Dynamic properties

    [CreateProperty]
    public Vector2 DepthRange { get; set; } = new Vector2(0.2f, 2);

    [CreateProperty]
    public string DepthRangeLabel
      => $"({DepthRange.x:0.0} - {DepthRange.y:0.0})";

    #endregion

    #region Private members

    RenderTexture _converted;

    #endregion

    #region MonoBehaviour implementation

    async void Start()
    {
        var root = _ui.rootVisualElement;

        // Data binding for the controller UI
        root.Q("controller").dataSource = this;

        // Depth image warming up
        while (_manager.environmentDepthTexture == null)
            await Awaitable.NextFrameAsync();

        // Target render texture allocation
        var source = _manager.environmentDepthTexture;
        _converted = new RenderTexture(source.width, source.height, 0);

        // UI: Depth image view
        root.Q("depth-image").style.backgroundImage
          = Background.FromRenderTexture(_converted);

        // Main loop
        while (true)
        {
            _material.SetVector("_DepthRange", DepthRange);
            _material.SetTexture("_DepthMap", _manager.environmentDepthTexture);
            _material.SetTexture("_ConfidenceMap", _manager.environmentDepthConfidenceTexture);
            Graphics.Blit(null, _converted, _material);
            await Awaitable.NextFrameAsync();
        }
    }

    void OnDestroy()
    {
        if (_converted != null)
        {
            Destroy(_converted);
            _converted = null;
        }
    }

    #endregion
}
