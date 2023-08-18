using System.Collections;
using System.Collections.Generic;
using Controllers;
using TMPro;
using UnityEngine;

public class TeamAssigner : MonoBehaviour
{
    public int team = -1;
    public TextMeshProUGUI teamText;
    public Dictionary<GridCell, GameObject> TeamUi = new Dictionary<GridCell, GameObject>();
    public bool showingUi = false;
    public GameObject teamUiPrefab;

    public void AssignTeam()
    {
        if (PlayerInteraction.SelectedCell == null) return;
        PlayerInteraction.SelectedCell.teamId = team;
        if (PlayerInteraction.SelectedArea.Count > 0)
        {
            foreach (var gridCell in PlayerInteraction.SelectedArea)
            {
                gridCell.teamId = team;
            }
        }
    }

    public void SpawnUI()
    {
        if (PlayerInteraction.SelectedCell == null) return;
        
        SpawnUiOnCell(PlayerInteraction.SelectedCell);
        
        if (PlayerInteraction.SelectedArea.Count <= 0) return;
        foreach (var gridCell in PlayerInteraction.SelectedArea)
        {
            SpawnUiOnCell(gridCell);
        }

    }
    
    private void SpawnUiOnCell(GridCell cell)
    {
        var ui = Instantiate(teamUiPrefab, cell.transform);
        TeamUi.Add(cell, ui);
        cell.teamId = team;
        ui.SetActive(showingUi);
    }
    
    public void SwitchShowingUI()
    {
        showingUi = !showingUi;
        
        if (TeamUi.Count > 0)
        {   
            foreach (var ui in TeamUi)
            {
                ui.Value.SetActive(showingUi);
            }
        }
        
    }

  
    
    

}
