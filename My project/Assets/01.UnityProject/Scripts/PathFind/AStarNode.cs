using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public TerrainController Terrain { get; private set; }
    public GameObject DestinationObj { get; private set; }

    // A Star Algorithm
    public float AstarF { get; private set; } = float.MaxValue;
    public float AstarG { get; private set; } = float.MaxValue;
    public float AstarH { get; private set; } = float.MaxValue;

    public AStarNode AStarPrevNode { get; private set; } = default;
    public AStarNode (TerrainController terrain_, GameObject destinationObj_)
    {
        Terrain = terrain_;
        DestinationObj = destinationObj_;
    }       // AstarNode() 

    //! Astar �˰����� ����� ����� �����Ѵ�.
    public void UpdateCost_Astar(float gCost, float heuristic,
        AStarNode prevNode)
    {
        float aStarF = gCost + heuristic;

        if (aStarF < AstarF)
        {
            AstarG = gCost;
            AstarH = heuristic;
            AstarF = aStarF;

            AStarPrevNode = prevNode;
        }       // if: ���� ����� ����� �� ���� ��쿡�� ������Ʈ �Ѵ�.
        else { /* Do nothing */ }
    }       // UpdateCost_Astar()

    //! ������ ����� ����Ѵ�.
    public void ShowCost_Astar()
    {
        GFunc.Log($"TileIdx1D: {Terrain.TileIdx1D}, " +
            $"F: {AstarF}, G: {AstarG}, H: {AstarH}");
    }
}