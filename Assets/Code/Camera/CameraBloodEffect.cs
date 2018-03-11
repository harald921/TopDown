using UnityEngine;

public class CameraBloodEffect : MonoBehaviour
{
    [SerializeField] Texture2D _image;     
    [SerializeField] Texture2D _normals;   
    [SerializeField] Shader    _shader;

    [Space(5)]
    [SerializeField] [Range(0, 1)] float _maxBloodAmount = 1.0f;
    [SerializeField] [Range(0, 1)] float _edgeSharpness  = 1.0f;
    [SerializeField] [Range(0, 1)] float _distortion     = 0.2f;

    float _playerMaxHealth;
    float  _trauma;        
	
	Material _material;


	void Awake()
	{
        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            Shader.EnableKeyword("LINEAR_COLORSPACE");
        else
            Debug.LogError("Please enable Linear Colorspace");

        _material = new Material(_shader);
        _material.SetTexture("_BlendTex", _image);
        _material.SetTexture("_BumpMap", _normals);
    }

    public void Initialize(PlayerHealthComponent inHealthComponent, float inMaxHealth)
    {
        _playerMaxHealth = inMaxHealth;
        inHealthComponent.OnHealthChange += SetTraumaRelativeToHealth;
    }

    void SetTraumaRelativeToHealth(float inPreviousHealth, float inCurrentHealth)
    {
        float healthPercentage = inCurrentHealth / _playerMaxHealth;
        _trauma = Mathf.Lerp(_maxBloodAmount, 0, healthPercentage);
    }

	void OnRenderImage(RenderTexture inSource, RenderTexture inDestination)
    {
        _material.SetFloat("_BlendAmount",   _trauma);
        _material.SetFloat("_EdgeSharpness", _edgeSharpness);
        _material.SetFloat("_Distortion",    _distortion);

		Graphics.Blit(inSource, inDestination, _material);
	}
}