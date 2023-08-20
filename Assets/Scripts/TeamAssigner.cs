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
    public static TeamAssigner Instance;
    public int team = 1;
    public Dictionary<int, int> Teams = new Dictionary<int, int>();
    public TextMeshProUGUI teamText;
    public Dictionary<GridCell, CellPopUp> TeamUi = new Dictionary<GridCell, CellPopUp>();
    public bool showingUi;
    private CellPopUp _teamUiPrefab;

    private void Start()
    {
        SwitchShowingUI();
        Instance = this;
        teamText.text = team.ToString();

        _teamUiPrefab = GameData.Instance.cellPopUpPrefab;
        GridManager.GridGenerated += InitShowingUI;
        GridEditor.Instance.OnGridDestroyed += OnGridDestroyed;
    }

    public void SpawnUI()
    {
        if (PlayerInteraction.SelectedArea.Count > 1)
        {
            foreach (var gridCell in PlayerInteraction.SelectedArea)
            {
                AddTeamOnCell(gridCell);
            }
        }
        else
        {
            if (PlayerInteraction.SelectedCell == null) return;

            AddTeamOnCell(PlayerInteraction.SelectedCell);
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
            
            if (AllTeamsAreOk("Adding team"))
            {
                team++;
                Teams.Add(team, 0);
                teamText.text = team.ToString();
                return;
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
        
        //if current team is in Teams dictionary and is last team and value is 0 then remove it
        if (Teams.ContainsKey(team))
        {
            if (Teams.Last().Key == team && Teams.Last().Value == 0)
            {
                Teams.Remove(team);
            }
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

            if (Teams.ContainsKey(cell.teamId))
            {
                if ((Teams[cell.teamId] - 1) == 0)
                {
                    if (cell.teamId == Teams.Last().Key)
                    {
                        Teams[cell.teamId] -= 1;
                        Teams.Remove(cell.teamId);
                    }
                    else
                    {
                        ErrorManager.ShowError("Error removing team",
                            "Removing team from cell" + cell.gridPosition +
                            " would leave a gap in the team numbers.\n" +
                            "You cant remove team " + cell.teamId + " because it is not the last team.\n" +
                            "Remove team " + Teams.Last().Key + " first.");
                        return;
                    }
                }
                else
                {
                    Teams[cell.teamId] -= 1;
                }
            }

            if (!Teams.ContainsKey(teamId))
            {
                Teams.Add(teamId, 1);
            }
            else
            {
                Teams[teamId] += 1;
            }

            ui.gameObject.SetActive(showingUi);
            ui.SetText(teamId.ToString());
            cell.teamId = teamId;
        }
        else
        {
            if (Teams.ContainsKey(cell.teamId))
            {
                if ((Teams[cell.teamId] - 1) == 0)
                {
                    if (cell.teamId == Teams.Last().Key)
                    {
                        Teams[cell.teamId] -= 1;
                        Teams.Remove(cell.teamId);
                    }
                    else
                    {
                        ErrorManager.ShowError("Error removing team",
                            "Removing team from cell" + cell.gridPosition +
                            " would leave a gap in the team numbers.\n" +
                            "You cant remove team " + cell.teamId + " because it is not the last team.\n" +
                            "Remove team " + Teams.Last().Key + " first.");
                        return;
                    }
                }
                else
                {
                    Teams[cell.teamId] -= 1;
                }
            }

            if (!Teams.ContainsKey(teamId))
            {
                Teams.Add(teamId, 1);
            }
            else
            {
                Teams[teamId] += 1;
            }

            TeamUi[cell].SetText(teamId.ToString());
            cell.teamId = teamId;
        }
    }

    public bool AllTeamsAreOk(string checkType)
    {
        string error = checkType + " error";
        if (Teams.Count == 0)
        {
            ErrorManager.ShowError(error, "There are no teams on the grid.");
            Teams.TryAdd(1, 0);
            Teams.TryAdd(2, 0);
            return false;
        }

        int teamCount = Teams.First().Value;
        foreach (var team in Teams)
        {
            if (team.Value != teamCount)
            {
                string s = "";
                foreach (var team1 in Teams)
                {
                    s += team1.Key + " " + team1.Value + "\n";
                }
                ErrorManager.ShowError(error, "All teams must have same amount of start spaces." +
                                                  "\n" + s);
                return false;
            }
        }
        
        if (Teams.First().Value == 0)
        {
            string s = "";
            foreach (var team in Teams)
            {
                s += team.Key + " " + team.Value + "\n";
            }
            ErrorManager.ShowError(error, "All teams must have at least one start space.\n" + s);
            return false;
        }

        return true;
    }

    private void RemoveTeamFromCell(GridCell cell)
    {
        
        if (TeamUi.ContainsKey(cell))
        {
            if (Teams.ContainsKey(cell.teamId))
            {
                if ((Teams[cell.teamId] - 1) == 0)
                {
                    if (cell.teamId == Teams.Last().Key)
                    {
                        Teams[cell.teamId] -= 1;
                        Teams.Remove(cell.teamId);
                    }
                    else
                    {
                        ErrorManager.ShowError("Error removing team",
                            "Removing team from cell" + cell.gridPosition +
                            " would leave a gap in the team numbers.\n" +
                            "You cant remove team " + cell.teamId + " because it is not the last team.\n" +
                            "Remove team " + Teams.Last().Key + " first.");
                        return;
                    }
                }
                else
                {
                    Teams[cell.teamId] -= 1;
                }
            }
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
    
    private void OnGridDestroyed()
    {
        Teams.Clear();
        TeamUi.Clear();
        team = 1;
        teamText.text = team.ToString();
    }
}