using System;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data;
using System.Collections;

abstract class AI
{
    protected LogicCell FindCellPair(LogicCell cell, Grid grid, int valueToFind, List<LogicCell> checkedCells, int x, int y)
    {
        foreach (LogicCell adjacentCell in cell.AdjacentCells)
        {
            if (adjacentCell.Value == valueToFind && !checkedCells.Contains(adjacentCell))
            {
                int[] directions = cell.GetDirection(grid, adjacentCell, x, y);
                int xDirection = directions[0];
                int yDirection = directions[1];

                if (xDirection == 0 || yDirection == 0)
                {
                    return adjacentCell;
                }
            }
        }

        return null;
    }

    protected List<LogicCell> FindPerpendicularCellsFromAdjacents(LogicCell cell, Grid grid, int x, int y, int xDirection, int yDirection)
    {
        List<LogicCell> perpCels = new List<LogicCell>();

        perpCels.Add(grid.GetLogicCell(x + yDirection, y + xDirection));
        perpCels.Add(grid.GetLogicCell(x - yDirection, y - xDirection));

        return perpCels;
    }

    protected int[] FindLayout3x3(Grid grid, LogicCell mainCell, int[,] layout, int x, int y)
    {
        const int xLayout = 1;
        const int yLayout = 1;

        int[] returnValue = new int[2];

        if (!IntMatchesLogicCell(grid.GetLogicCell(x, y), layout[xLayout, yLayout])) return null;

        bool matches = true;

        int xOffset;
        int yOffset;
        int count = 0;

        for (int mirror = 0; mirror < 2; mirror++)
        {
            for (int rotation = 0; rotation < 4; rotation++)
            {
                xOffset = -1;
                yOffset = -1;

                do
                {
                    if (xOffset != 0 || yOffset != 0)
                    {
                        returnValue = GetRotatedIndex(xOffset, yOffset, rotation, mirror == 1);
                        int xValue = returnValue[0];
                        int yValue = returnValue[1];

                        if (IntMatchesLogicCell(grid.GetLogicCell(x + xValue, y + yValue), layout[xLayout + xValue, yLayout + yValue]))
                        {
                            matches = false;
                        }
                    }

                    if (yOffset == 1)
                    {
                        yOffset = -2;
                        xOffset++;
                    }
                    yOffset++;

                    count++;
                }
                while (matches && count < 9);

                if (matches)
                {
                    if (mirror == 0) returnValue[1] = 1;
                    else returnValue[1] = 1;

                    returnValue[0] = rotation;

                    return returnValue;
                }
            }
        }

        return null;
    }
    
    private bool IntMatchesLogicCell(LogicCell cell, int num)
    {
        // -2 means hidden unflagged cell
        // -5 means a flagged or unflagged hidden cell
        // -6 means anything
        // -7 means any revealed cell
        // 

        if (num == -6) return true;

        if (cell == null)
        {
            if (num == -4)
            {
                return true;
            }
            return false;
        }

        if (!cell.IsHidden)
        {
            if (cell.Value == num) return true;
            if (num == -7) return true;
            return false;
        }

        if (num == -5) return true;

        if (cell.IsFlagged)
        {
            if (num == -3) return true;
            return false;
        }

        if (num == -2) return true;
        return false;
    }
    
    protected int[] GetRotatedIndex(int x, int y, int rotation, bool mirror)
    {
        int[] returnValue = { 0, 0 };
        if (x == 0 && y == 0) return returnValue;

        switch (x)
        {
            case 1:
                {
                    switch (y)
                    {
                        case 1: y = -1; break;
                        case 0: y = -1; x = 0; break;
                        case -1: x = -1; break;
                    }
                    break;
                }
            case 0:
                {
                    switch (y)
                    {
                        case 1: y = 0; x = 1; break;
                        case -1: y = 0; x = -1; break;
                    }
                    break;
                }
            case -1:
                {
                    switch (y)
                    {
                        case 1: x = 1; break;
                        case 0: y = 1; x = 0; break;
                        case -1: y = 1; x = -1; break;
                    }
                    break;
                }
        }

        rotation--;

        if (rotation > 0)
        {
            returnValue = GetRotatedIndex(x, y, rotation, false);
        }

        if (mirror) x = -x;

        returnValue[0] = x;
        return returnValue;
    }

    protected bool CellsContainsEachother(LogicCell mainCell, LogicCell containedCell)
    {
        foreach (LogicCell cell in containedCell.AdjacentCells)
        {
            if (cell.IsHidden && !mainCell.AdjacentCells.Contains(cell)) return false;
        }

        return true;
    }

    protected List<LogicCell> GetNonOverlapingHiddenCells(LogicCell mainCell, LogicCell containedCell)
    {
        List<LogicCell > cells = new List<LogicCell>();

        foreach (LogicCell adjacentCell in mainCell.AdjacentCells)
        {
            if (adjacentCell.IsHidden && !containedCell.AdjacentCells.Contains(adjacentCell)) cells.Add(adjacentCell);
        }
        

        return cells;
    }
}