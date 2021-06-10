using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    [Range(0f, 1f)]
    //============================= auto collect system variable ============================================
    public float AutoCollectPercentage = 0.1f;
    public float SaveDelay = 5f;
    public ResourceConfig[] ResourcesConfigs;
    public Sprite[] ResourceSprite;

    public Transform ResourcesParent;
    public ResourceController ResourcePrefab;

    public Text GoldInfo;
    public Text AutoCollectInfo;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private float _collectSecond;
    private float _saveDelayCounter;

    public double TotalGold { get; private set; }

    //================ variable memunculkan text pada posisi tap dan menganimasikan coin ====================
    public TapText TapTextPrefab;
    public Transform CoinIcon;
    private List<TapText> _tapTextPool = new List<TapText>();

    private bool _isClick;

    // Start is called before the first frame update
    void Start()
    {
        AddAllResource();

        GoldInfo.text = $"Gold: { UserDataManager.Progress.Gold.ToString("0") }";
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;
        _saveDelayCounter -= deltaTime;

        //Collect / second
        _collectSecond += deltaTime;

        if (_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }

        CheckResourceCost();

        //Animasi coin
        if(_isClick == true)
        {
            CoinIcon.transform.localScale = Vector3.LerpUnclamped(CoinIcon.transform.localScale, Vector3.one * 2f, 0.4f);
        }
        CoinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100);
    }

    //============================= collect system ============================================

    //menambah semua resource atau list upgrade/unlock
    private void AddAllResource()
    {
        bool showResource = true;

        int index = 0;

        foreach (ResourceConfig config in ResourcesConfigs)
        {
            GameObject obj = Instantiate(ResourcePrefab.gameObject, ResourcesParent, false);
            ResourceController resource = obj.GetComponent<ResourceController>();

            resource.SetConfig(index, config);

            obj.gameObject.SetActive(showResource);

            if(showResource && !resource.isUnloked)
            {
                showResource = false;
            }

            _activeResources.Add(resource);
            index++;
        }
    }

    public void ShowNextResource()
    {
        foreach(ResourceController resource in _activeResources)
        {
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;
            }
        }
    }

    //cek reource
    private void CheckResourceCost()
    {
        foreach(ResourceController resource in _activeResources)
        {
            bool _isBuyable = false;

            if (resource.isUnloked)
            {
                _isBuyable = UserDataManager.Progress.Gold >= resource.GetUpgradeCost();
            }
            else
            {
                _isBuyable = UserDataManager.Progress.Gold >= resource.GetUnlockCost();
            }

            resource.ResourceImage.sprite = ResourceSprite[_isBuyable ? 1 : 0];
        }
    }

    //collect / second
    private void CollectPerSecond()
    {

        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.isUnloked)
            {
                output += resource.GetOutPut();
            }
        }
        output *= AutoCollectPercentage;
        
        AutoCollectInfo.text = $"Auto Collect: { output.ToString("F1") } / second";
        AddGold(output);
    }

    //Add gold
    public void AddGold(double value)
    {
        UserDataManager.Progress.Gold += value;
        GoldInfo.text = $"Gold: { UserDataManager.Progress.Gold.ToString("0") }";

        UserDataManager.Save(_saveDelayCounter < 0f);

        if (_saveDelayCounter < 0f)
        {
            _saveDelayCounter = SaveDelay;
        }
    }

    //============================= Tap to collect ============================================
    public void CollectbyTap(Vector3 tapPosition, Transform parent)
    {

        _isClick = true;

        double output = 0;
        foreach(ResourceController resource in _activeResources)
        {
            if (resource.isUnloked)
            {
                output += resource.GetOutPut();
            }
        }

        TapText tapText = GetOreCreatedTapText();
        tapText.transform.SetParent(parent,false);
        tapText.transform.position = tapPosition;

        tapText.text.text = $"+{output.ToString("0")}";
        tapText.gameObject.SetActive(true);
        tapText.transform.localScale = Vector3.one * 1.75f;

        AddGold(output);
    }

    public void IsCliCked()
    {
        _isClick = false;
        CoinIcon.transform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.one, 0.4f);
    }

    private TapText GetOreCreatedTapText()
    {
        TapText tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);
        if(tapText == null)
        {
            tapText = Instantiate(TapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }
        return tapText;
    }


    //resource valuee

    [System.Serializable]

    public struct ResourceConfig

    {

        public string Name;

        public double UnlockCost;

        public double UpgradeCost;

        public double Output;

    }
}
