using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

class AmateurAI : AI
{
    public bool P2_1(LogicCell currentCell, Grid grid, int x, int y)
    {
        if (currentCell.Value != 2 || currentCell.EffectiveValue != 2) return false;

        using (StreamWriter sw = new StreamWriter("log.txt", true))
        {
            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (adjacentCell.Value == 1 && adjacentCell.EffectiveValue == 1)
                {
                    List<LogicCell> nonOverlappingHiddenCells = GetNonOverlapingHiddenCells(currentCell, adjacentCell);
                    if (nonOverlappingHiddenCells.Count == 1 && !nonOverlappingHiddenCells[0].IsFlagged)
                    {
                        sw.WriteLine("2-1 at " + x + ", " + y);
                        nonOverlappingHiddenCells[0].Flag();
                        return true;
                    }
                }
            }
        }

        return false;
    }
    public bool P1_1C(LogicCell currentCell, Grid grid, int x, int y)
    {
        if (currentCell.Value != 1 || currentCell.EffectiveValue != 1) return false;

        using (StreamWriter sw = new StreamWriter("log.txt", true))
        {
            foreach (LogicCell adjacentCell in currentCell.AdjacentCells)
            {
                if (adjacentCell.Value == 1 && adjacentCell.EffectiveValue == 1 && CellsContainsEachother(currentCell, adjacentCell))
                {
                    List<LogicCell> cells = GetNonOverlapingHiddenCells(currentCell, adjacentCell);

                    if (cells.Count == 1)
                    {
                        sw.WriteLine("1-1C at " + x + ", " + y);
                        cells[0].Open();
                        return true;
                    }
                }
            }
        }

        return false;
    }
}