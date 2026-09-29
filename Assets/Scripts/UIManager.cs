using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public InputSystem_Actions inputActions;
    public CanvasGroup managerMenu;
    public CanvasGroup managerMenuButton;
    public CanvasGroup unitActionMenu;

    [Header("Unit Info Panel")]
    public CanvasGroup unitInfoPanel;
    public TextMeshProUGUI unitNameText;
    // public TextMeshProUGUI unitLevelText;
    public TextMeshProUGUI unitHealthText;
    // public TextMeshProUGUI unitItemText;
    public Image unitHealthBar;
    public Image unitPortrait;

    [Header("Hover Info Panel")]
    public CanvasGroup hoverInfoPanel;
    public Image hoverInfoImage;
    public TextMeshProUGUI hoverNameText;
    // public TextMeshProUGUI hoverLevelText;
    public TextMeshProUGUI hoverHealthText;
    public Image hoverHealthBar;
    public Image hoverPortrait;

    [Header("StatWindow")]
    public Image healthBar;
    // public Image XPBar;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    // public TextMeshProUGUI XPText;
    public TextMeshProUGUI movRangeText;
    public TextMeshProUGUI attRangeText;
    public TextMeshProUGUI attDamageText;
    public TextMeshProUGUI defText;
    // public TextMeshProUGUI luckText;
    // public TextMeshProUGUI intellectText;
    // public TextMeshProUGUI spiritText;
    // public TextMeshProUGUI masteryText;
    public TextMeshProUGUI critMethod;

    [Header("EndScreen")]
    public CanvasGroup endScreen;
    public CanvasGroup winBanner;
    public CanvasGroup loseBanner;




    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputActions.Player.ManagerMenu.WasPressedThisFrame())
        {
            if (CheckIfMenuOpen(managerMenu))
            {
                CloseMenu(managerMenu);
                OpenMenu(managerMenuButton);
            }
            else
            {
                CloseMenu(managerMenuButton);
                OpenMenu(managerMenu);
            }
        }

        if (Player.selectedUnit && !CheckIfMenuOpen(managerMenu))
        {
            if (!Player.selectedUnit.isMoving)
            {
                OpenMenu(unitActionMenu);
            }
            else
            {
                CloseMenu(unitActionMenu);
            }

            OpenMenu(unitInfoPanel);
            DisplayUnitInfo();
        }
        else
        {
            CloseMenu(unitInfoPanel);
            CloseMenu(unitActionMenu);
        }

        if (Player.hoverUnit && !CheckIfMenuOpen(managerMenu) && Player.hoverUnit != Player.selectedUnit)
        {
            OpenMenu(hoverInfoPanel);
            DisplayHoverInfo();
        }
        else
        {
            CloseMenu(hoverInfoPanel);
        }
    }

    public void CloseMenu(CanvasGroup menu)
    {
        menu.alpha = 0;
        menu.interactable = false;
        menu.blocksRaycasts = false;
    }

    public void OpenMenu(CanvasGroup menu)
    {
        menu.alpha = 1;
        menu.interactable = true;
        menu.blocksRaycasts = true;

    }

    public bool CheckIfMenuOpen(CanvasGroup menu)
    {
        if (menu.alpha == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DisplayUnitInfo()
    {
        Unit unit = Player.selectedUnit;
        unitNameText.text = unit.unitName;
        unitPortrait.sprite = unit.unitPortrait;
        unitPortrait.color = unit.unitColor;
        // unitLevelText.text = $"Lvl {unit.level}";
        unitHealthText.text = $"{Mathf.RoundToInt(unit.health * 0.1f)} / {Mathf.RoundToInt(unit.maxHealth * 0.1f)}";
        unitHealthBar.fillAmount = (float)unit.health / unit.maxHealth;
    }

    public void WaitButton()
    {
        if (Player.selectedUnit)
        {
            Player.selectedUnit.ExpendUnit();
            Player.selectedUnit = null;
        }
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGameButton()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void DisplayHoverInfo()
    {
        Unit hoverUnit = Player.hoverUnit;
        hoverInfoImage.color = hoverUnit.owner.negativeColor;
        hoverNameText.text = hoverUnit.unitName;
        hoverNameText.color = hoverUnit.owner.playerColor;
        hoverPortrait.sprite = hoverUnit.unitPortrait;
        hoverPortrait.color = hoverUnit.unitColor;
        // hoverLevelText.text = $"Lvl {hoverUnit.level}";
        hoverHealthText.text = $"{Mathf.RoundToInt(hoverUnit.health * 0.1f)} / {Mathf.RoundToInt(hoverUnit.maxHealth * 0.1f)}";
        hoverHealthBar.fillAmount = (float)hoverUnit.health / hoverUnit.maxHealth;
    }

    public void PopulateStatWindow()
    {
        Unit unit = Player.selectedUnit;

        healthBar.fillAmount = (float)unit.health / unit.maxHealth;
        // XPBar.fillAmount = (float)unit.experience / unit.experienceToLevel;
        nameText.text = $"{unit.unitName}: {unit.unitClass.classDescription}";    // nameText.text = $"{unit.unitName}: Level {unit.level} {unit.unitClass}";
        healthText.text = $"HP: {Mathf.RoundToInt(unit.health * 0.1f)} / {Mathf.RoundToInt(unit.maxHealth * 0.1f)}";
        // XPText.text = $"XP: {unit.experience} / {unit.experienceToLevel}";
        movRangeText.text = $"Movement: {unit.stats.moveRange}";
        attRangeText.text = $"Range: {unit.stats.attackRange}";
        attDamageText.text = $"Attack: {Mathf.RoundToInt(unit.stats.attackDamage * 0.1f)}";
        defText.text = $"Defense: {unit.stats.defense}%";
        // luckText.text = $"Luck: {unit.stats.defense}";
        // intellectText.text = $"Intellect: {unit.stats.intellect}";
        // spiritText.text = $"Spirit: {unit.stats.spirit}";
        // masteryText.text = $"Mastery: {unit.stats.mastery}";
        critMethod.text = $"{unit.unitClass.critCondition}";
    }

    public void EndScreen(bool win)
    {
        endScreen.alpha = 1;
        endScreen.interactable = true;
        endScreen.blocksRaycasts = true;

        if (win)
        {
            winBanner.alpha = 1;
        }
        else
        {
            loseBanner.alpha = 1;
        }
    }
}
