using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public sealed class DepthVisualizer : MonoBehaviour
{
    [SerializeField] AROcclusionManager _manager = null;
    [SerializeField] Material _material = null;
    [SerializeField] UIDocument _uiRoot = null;

    RenderTexture _converted;

    async void Start()
    {
        while (_manager.environmentDepthTexture == null)
            await Awaitable.NextFrameAsync();

        var source = _manager.environmentDepthTexture;
        _converted = new RenderTexture(source.width, source.height, 0);

        _uiRoot.rootVisualElement.Q("depth-image").style.backgroundImage = Background.FromRenderTexture(_converted);

        while (true)
        {
            source = _manager.environmentDepthTexture;
            Graphics.Blit(source, _converted, _material);
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
}
