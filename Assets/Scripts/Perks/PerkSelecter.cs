using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PerkSelecter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI perkLabel;
    [SerializeField] private Image perkIcon;
    [SerializeField] private GameObject perkOwnedTextObject;

    [Header("Perks")]
    [SerializeField] private List<PerkSO> All_Perks;
    [SerializeField] private PerkSO currentPerkSelected;
    [SerializeField] private PerkSO currentPerkHighlighted;
    private int perkIndex = 0;

    private PerkMachineData perkMachineData;
    private PerkMachineSO perkMachine;

    void Awake()
    {
        perkMachine = PerksManager.Singleton.GetPerkMachine();
        perkMachineData = ShopSaveSystem.GetPerkMachineData();
    }

    void Start()
    {
        All_Perks.Clear();
        All_Perks.Add(null);

        PerkSO[] perksInData = perkMachine.GetAvailablePerks();
        foreach(PerkSO perk in perksInData)
        {
            All_Perks.Add(perk);
        }

        perkIcon.gameObject.SetActive(false);

        CheckForActivePerk();
    }

    public void SwitchPerk(int direction)
    {
        perkIndex += direction;
        if(perkIndex >= All_Perks.Count)
            perkIndex = 0;
        else if(perkIndex < 0)
            perkIndex = All_Perks.Count - 1;

        currentPerkHighlighted = All_Perks[perkIndex];

        currentPerkSelected = null;
        if(currentPerkHighlighted != null)
        {
            if(ShopSaveSystem.IsPerkUnlocked(currentPerkHighlighted.name))
            {
                currentPerkSelected = currentPerkHighlighted;
            }
        }

        UpdatePerkSelectUI(currentPerkHighlighted);
    }

    public void UpdatePerkSelectUI(PerkSO perk)
    {
        if(perk == null)
        {
            perkLabel.SetText("None");
            perkIcon.gameObject.SetActive(false);

            perkOwnedTextObject.SetActive(false);
        }
        else
        {
            perkLabel.SetText(perk.perkName);

            perkIcon.gameObject.SetActive(true);
            perkIcon.sprite = perk.icon;

            perkOwnedTextObject.SetActive(!ShopSaveSystem.IsPerkUnlocked(perk.name));
        }
    }

    public PerkSO GetSelectedPerk()
    {
        return(currentPerkSelected);
    }

    private void CheckForActivePerk()
    {
        PerkSO _perk = PerksManager.Singleton.GetActivePerk();
        if(_perk != null)
        {
            int _perkIndex = All_Perks.IndexOf(_perk);
            SwitchPerk(_perkIndex);
        }
        
    }
}
