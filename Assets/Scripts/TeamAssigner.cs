using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Managers;
using ScriptableObjects;
using TMPro;
using UnityEngine;

public class TeamAssigner : MonoBehaviour
{
    public int team = 1;
    public Dictionary<int, int> Teams = new Dictionary<int, int>();
    public TextMeshProUGUI teamText;
    public Dictionary<GridCell, CellPopUp> TeamUi = new Dictionary<GridCell, CellPopUp>();
    public bool showingUi = false;
    private CellPopUp _teamUiPrefab;

    private void Start()
    {
        teamText.text = team.ToString();
        
        _teamUiPrefab = GameData.Instance.cellPopUpPrefab;
        GridManager.GridGenerated += InitShowingUI;
    }
    
    public void SpawnUI()
    {
        if (PlayerInteraction.SelectedCell == null) return;
        
        AddTeamOnCell(PlayerInteraction.SelectedCell);
        
        if (PlayerInteraction.SelectedArea.Count <= 0) return;
        foreach (var gridCell in PlayerInteraction.SelectedArea)
        {
            AddTeamOnCell(gridCell);
        }

    }

    public void HighTeam()
    {
        if (Teams.Count == 0)
        {
            Teams.Add(1, 0);
            Teams.Add(2, 0);
        }
        
        if (Teams.Count == 1)
        {
            Teams.Add(2, 0);
        }
        
        if (team < 1)
        {
            team = 1;
            teamText.text = team.ToString();
            return;
        }
        
        int highestTeam = 0;
        foreach (var team in Teams)
        {
            if (team.Key > highestTeam)
            {
                highestTeam = team.Key;
            }
        }

        if (highestTeam == team)
        {
            bool allTeamsEven = true;
            int teamCount = Teams.First().Value;
            foreach (var team in Teams)
            {
                if (team.Value != teamCount)
                {
                    allTeamsEven = false;
                }
            }

            if (allTeamsEven && Teams.First().Value == 0)
            {
                string s = "";
                foreach (var team in Teams)
                {
                    s += team.Key + " " + team.Value + "\n";
                }
                ErrorManager.ShowError("Error creating new team",
                    "Other teams must have at least one start space before creating new team\n" + s);
                return;
            }
            
            if (allTeamsEven)
            {
                team++;
                Teams.Add(team, 0);
                teamText.text = team.ToString();
                return;
            }
            else
            {
                string s = "";
                foreach (var team in Teams)
                {
                    s += team.Key + " " + team.Value + "\n";
                }
                ErrorManager.ShowError("Error creating new team",
                    "All other teams must have same amount start spaces before creating new team.\n" + s);
            }

        }
        else
        {
            //find next team in teams dictionary
            int nextTeam = team + 1;
            while (!Teams.ContainsKey(nextTeam))
            {
                nextTeam++;
            }
            
            team = nextTeam;
        }
        
        teamText.text = team.ToString();
    }
    
    public void LowTeam()
    {
        if (team == 1)
        {
            return;
        }
        
        if (team == -1)
        {
            team = 1;
            teamText.text = team.ToString();
            return;
        }
        
        //find next team in teams dictionary
        int nextTeam = team - 1;
        while (!Teams.ContainsKey(nextTeam))
        {
            nextTeam--;
            if (nextTeam == 0)
            {
                Teams.TryAdd(1, 0);
                team = 1;
                return;
            }
        }
        
        team = nextTeam;
        teamText.text = team.ToString();
    }

    public void RemoveTeams()
    {
        team = -1;
        teamText.text = team.ToString();
    }

   
 
    
    private void AddTeamOnCell(GridCell cell)
    {
        AddTeamOnCell(cell, team);
    }
    
    private void AddTeamOnCell(GridCell cell, int teamId)
    {
        if (teamId < 1)
        {
            RemoveTeamFromCell(cell);
            return;
        }
        
        if (!TeamUi.ContainsKey(cell))
        {
            var ui = Instantiate(_teamUiPrefab, cell.transform);
            ui.transform.SetParent(cell.transform);
            TeamUi.Add(cell, ui);
            if (!Teams.ContainsKey(cell.teamId))
            {
                Teams.Add(cell.teamId, 1);
            }
            else
            {
                Teams[cell.teamId] += 1;
            }
            ui.gameObject.SetActive(showingUi);
            ui.SetText(teamId.ToString());
            cell.teamId = teamId;
        }
        else
        {
            Teams[cell.teamId] -= 1;
            Teams[teamId] += 1;
            TeamUi[cell].SetText(team.ToString());
            cell.teamId = teamId;
        }
    }
    
    private void RemoveTeamFromCell(GridCell cell)
    {
        if (TeamUi.ContainsKey(cell))
        {
            Teams[cell.teamId] -= 1;
            Destroy(TeamUi[cell].gameObject);
            TeamUi.Remove(cell);
        }
        cell.teamId = -1;
    }
    
    public void SwitchShowingUI()
    {
        showingUi = !showingUi;
        
        if (TeamUi.Count > 0)
        {   
            foreach (var ui in TeamUi)
            {
                ui.Value.gameObject.SetActive(showingUi);
            }
        }
        
    }

    private void InitShowingUI()
    {
        Teams.Clear();
        foreach (var ui in TeamUi)
        {
            Destroy(ui.Value.gameObject);
            TeamUi.Remove(ui.Key);
        }
        TeamUi.Clear();
        
        
        foreach (var gridCell in GridManager.Instance.gridCells)
        {
            if (gridCell.teamId > 0)
            {
                AddTeamOnCell(gridCell, gridCell.teamId);
                if (Teams.ContainsKey(gridCell.teamId))
                {
                    Teams[gridCell.teamId] += 1;
                }
                else
                {
                    Teams.Add(gridCell.teamId, 1);
                }
            }
        }

        if (Teams.Count == 0)
        {
            Teams.TryAdd(1, 0);
            Teams.TryAdd(2, 0);
        } 
        
        if (Teams.Count == 1)
        {
            Teams.TryAdd(2, 0);
            if (Teams.Count == 1)
            {
                Teams.TryAdd(1, 0);
            }
        }
    }

  
    
    

}
