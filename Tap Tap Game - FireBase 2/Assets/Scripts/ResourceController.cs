using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class ResourceController : MonoBehaviour
{
    public Button ResourceButton;
    public Image ResourceImage;

    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    private ResourceConfig _config;

    private int _index;
    private int _level
    {
        set
        {
            UserDataManager.Progress.ResourcesLevels[_index] = value;
            UserDataManager.Save(true);
        }
        get
        {
            if (!UserDataManager.HasResources(_index))
            {
                return 1;
            }

            return UserDataManager.Progress.ResourcesLevels[_index];
        }
    }

    public bool isUnloked { get; private set; }

    private void Start()
    {
        ResourceButton.onClick.AddListener(() =>
        {
            if (isUnloked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }

    public void SetConfig(int index, ResourceConfig config)
    {
        _index = index;
        _config = config;

        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutPut().ToString("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{_config.UnlockCost}";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";

        SetUnlocked(_config.UnlockCost == 0 || UserDataManager.HasResources(_index));
    }

    public double GetOutPut()
    {
        return _config.Output * _level;
    }

    public double GetUpgradeCost()
    {
        return _config.UpgradeCost * _level;
    }

    public double GetUnlockCost()
    {
        return _config.UnlockCost;
    }

    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();
        if(UserDataManager.Progress.Gold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold(-upgradeCost);
        _level++;

        ResourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";
        ResourceDescription.text = $"{_config.Name} Lv. {_level}\n{GetOutPut().ToString("0")}";

        AnalyticsManager.LogUpgradeEvent(_index, _level);
    }

    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();
        if (UserDataManager.Progress.Gold < unlockCost)
        {
            return;
        }

        SetUnlocked(true);
        GameManager.Instance.ShowNextResource();

        AchievmentController.Instance.UnlockAchievment(AchievmentController.AchievmentType.UnlockResource, _config.Name);
        AnalyticsManager.LogUnlockEvent(_index);
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnloked = unlocked;

        if (unlocked)
        {
            if (!UserDataManager.HasResources(_index))
            {
                UserDataManager.Progress.ResourcesLevels.Add(_level);
                UserDataManager.Save(true);
            }
        }
        ResourceImage.color = isUnloked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive(!unlocked);
        ResourceUpgradeCost.gameObject.SetActive(unlocked);
    }
}
