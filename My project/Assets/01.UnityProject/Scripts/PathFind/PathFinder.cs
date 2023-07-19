using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : GSingleton<PathFinder>
{
    #region ���� Ž���� ���� ����
    public GameObject sourceObj = default;
    public GameObject destinationObj = default;
    public MapBoard mapBoard = default;
    public const float PATHFIND_DELAY = 0.1f;
    #endregion      // ���� Ž���� ���� ����

    #region A Star �˰������� �ִܰŸ��� ã�� ���� ����
    private List<AStarNode> aStarResultPath = default;
    private List<AStarNode> aStarOpenPath = default;
    private List<AStarNode> aStarClosePath = default;
    #endregion      // �˰������� �ִܰŸ��� ã�� ���� ����
    
    //! ������� ������ ������ ���� ã�� �Լ�
    public void FindPath_Astar()
    {
        StartCoroutine(DelayFindPath_AStar(PATHFIND_DELAY));
    }       // FIndPath_Astar()

    //! Ž�� �˰��� �����̸� �Ǵ�.
    private IEnumerator DelayFindPath_AStar(float delay_)
    {
        // A star �˰����� ����ϱ� ���ؼ� �н� ����Ʈ�� �ʱ�ȭ�Ѵ�.
        aStarOpenPath = new List<AStarNode>();
        aStarClosePath = new List<AStarNode>();
        aStarResultPath = new List<AStarNode>();

        TerrainController targetTerrain = default;

        // ������� �ε����� ���ؼ�, ����� ��带 ã�ƿ´�.
        string[] sourceObjNameParts = sourceObj.name.Split('_');
        int sourceIdx1D = -1;
        int.TryParse(
            sourceObjNameParts[sourceObjNameParts.Length - 1], out sourceIdx1D);
        targetTerrain = mapBoard.GetTerrain(sourceIdx1D);
        // ã�ƿ� ����� ��带 Open ����Ʈ�� �߰��Ѵ�.
        AStarNode targetNode = new AStarNode(targetTerrain, destinationObj);
        Add_AStarOpenList(targetNode);

        int loopIdx = 0;
        bool isFoundDestination = false;
        bool isNowayTogo = false;
        // TODO: �˰��� �����۵� Ȯ�� �� ���ǹ� ������ ����.
        // while (loopIdx < 10)
        while (isFoundDestination == false && isNowayTogo == false)
        {
            // { Open ����Ʈ�� ��ȸ�ؼ� ���� �ڽ�Ʈ�� ���� ��带 �����Ѵ�.
            AStarNode minCostNode = default;
            foreach (var terrainNode  in aStarOpenPath)
            {
                if (minCostNode == default)
                {
                    minCostNode = terrainNode;
                }       // if: ���� ���� �ڽ�Ʈ�� ��尡 ��� �ִ� ���
                else
                {
                    // terrainNode �� �� ���� �ڽ�Ʈ�� ������ ���
                    // minCostNode �� ������Ʈ �Ѵ�.
                    if (terrainNode.AstarF < minCostNode.AstarF)
                    {
                        minCostNode = terrainNode;
                    }
                    else { continue; }
                }       // else: ���� ���� �ڽ�Ʈ�� ��尡 ĳ�̵Ǿ� �ִ� ���

            }       // loop: ���� �ڽ�Ʈ�� ���� ��带 ã�� ����

            // } Open ����Ʈ�� ��ȸ�ؼ� ���� �ڽ�Ʈ�� ���� ��带 �����Ѵ�.

            minCostNode.ShowCost_Astar();
            minCostNode.Terrain.SetTileActiveColor(RDefine.TileStatusColor.SEARCH);

            // ������ ��尡 �������� �����ߴ��� Ȯ���Ѵ�.
            bool isArriveDest = mapBoard.GetDistance2D(
                minCostNode.Terrain.gameObject, destinationObj).
                Equals(Vector2Int.zero);

            if (isArriveDest)
            {
                // { �������� �����ߴٸ� AStarResultPath ����Ʈ�� �����Ѵ�.
                AStarNode resultNode = minCostNode;
                bool isSet_aStarResultPathok = false;
                while (isSet_aStarResultPathok)
                {
                    aStarResultPath.Add(resultNode);
                    if (resultNode.AStarPrevNode == default ||
                        resultNode.AStarPrevNode == null)
                    {
                        isSet_aStarResultPathok = true;
                        break;
                    }
                    else { /* Do nothing */}

                    resultNode = resultNode.AStarPrevNode;
                }       // loop: ���� ��带 ã�� ���� ������ ��ȸ�ϴ� ����
                // } �������� �����ߴٸ� AStarResultPath ����Ʈ�� �����Ѵ�.

                // Open list �� Clost list �� �����Ѵ�.
                aStarOpenPath.Clear();
                aStarClosePath.Clear();
                isFoundDestination = true;
                break;
            }       // if: ������ ��尡 �������� ������ ���
            else
            {
                // { �������� �ʾҴٸ� ���� Ÿ���� �������� 4 ���� ��带 ã�ƿ´�.
                List<int> nextSearchIdx1Ds = mapBoard.
                    GetTileIdx2D_Around4ways(minCostNode.Terrain.TileIdx2D);

                // ã�ƿ� ��� �߿��� �̵� ������ ���� Open list�� �߰��Ѵ�.
                AStarNode nextNode = default;
                foreach(var nextIdx1D in nextSearchIdx1Ds)
                {
                    nextNode = new AStarNode(
                        mapBoard.GetTerrain(nextIdx1D), destinationObj);

                    if (nextNode.Terrain.IsPassable == false) { continue; }

                    Add_AStarOpenList(nextNode, minCostNode);
                }       // loop: �̵� ������ ��带 Open list�� �߰��ϴ� ����
                // } �������� �ʾҴٸ� ���� Ÿ���� �������� 4 ���� ��带 ã�ƿ´�.

                // Ž���� ���� ���� Close list �� �߰��ϰ�, Open list ���� �����Ѵ�.
                // �� ��, Open list �� ��� �ִٸ� �� �̻� Ž���� �� �ִ� ����
                // �������� �ʴ� ���̴�.
                aStarClosePath.Add(minCostNode);
                aStarOpenPath.Remove(minCostNode);
                if (aStarOpenPath.IsValid() == false)
                {
                    GFunc.LogWarning("[Warning] There are no more tiles to explore.");
                    isNowayTogo = true;
                }       // if: �������� �������� ���ߴµ�, �� �̻� Ž���� �� �ִ� ���� ���� ���

                foreach(var tempNode in aStarOpenPath)
                {
                    GFunc.Log($"Idx: {tempNode.Terrain.TileIdx1D}, " +
                        $"Cost: {tempNode.AstarF}");
                }

            }       // else: ������ ��尡 �������� �������� ���� ���

            loopIdx++;
            yield return new WaitForSeconds(delay_);
        }       // loop: A Star �˰������� ���� ã�� ���� ����
    }       // DelayFindPath_AStar()

    //! ����� ������ ��带 Open ����Ʈ�� �߰��Ѵ�.
    private void Add_AStarOpenList(
        AStarNode targetTerrain_, AStarNode prevNode = default)
    {
        // Open ����Ʈ�� �߰��ϱ� ���� �˰��� ����� �����Ѵ�.
        Update_AStarCostToTerrain(targetTerrain_, prevNode);

        AStarNode closeNode = aStarClosePath.FindNode(targetTerrain_);
        if(closeNode != default && closeNode != null)
        {
            // �̹� Ž���� ���� ��ǥ�� ��尡 �����ϴ� ��쿡��
            // Open list �� �߰����� �ʴ´�.
            /* Do Nothing */
        }       // if: Close list�� �̹� Ž���� ���� ��ǥ�� ��尡 �����ϴ� ���
        else
        {
            AStarNode openedNode = aStarOpenPath.FindNode(targetTerrain_);
            if (openedNode != default && openedNode != null)
            {
                // Ÿ�� ����� �ڽ�Ʈ�� �� ���� ��쿡�� Open list ���� ��带 ��ü�Ѵ�.
                // Ÿ�� ����� �ڽ�Ʈ�� �� ū ��쿡�� Open list �� �߰����� �ʴ´�.
                if (targetTerrain_.AstarF < openedNode.AstarF)
                {
                    aStarOpenPath.Remove(openedNode);
                    aStarOpenPath.Add(targetTerrain_);
                }
                else { /* Do nothing */ }
            }       // if: Open list �� ���� �߰��� ���� ���� ��ǥ�� ��尡 �����ϴ� ���
            else
            {
                aStarOpenPath.Add(targetTerrain_);
            }       // else: Open list �� ���� �߰��� ���� ���� ��ǥ�� ��尡 ���� ���

        }       // else: ���� Ž���� ������ ���� ����� ���
    }       // Add_AStarOpenList()

    //! Target ���� ������ Destination ���� ������ Distance �� Heuristic �� �����ϴ� �Լ�
    private void Update_AStarCostToTerrain(
        AStarNode targetNode, AStarNode prevNode)
    {
        // { Target �������� Destinaton ������ 2D Ÿ�� �Ÿ��� ����ϴ� ����
        Vector2Int distance2D = mapBoard.GetDistance2D(
            targetNode.Terrain.gameObject, destinationObj);
        int totalDistance2D = distance2D.x + distance2D.y;

        // Heuristic �� �����Ÿ��� �����Ѵ�.
        Vector2 localDistance = destinationObj.transform.localPosition -
            targetNode.Terrain.transform.localPosition;
        float heuristic = Mathf.Abs(localDistance.magnitude);
        // } Target �������� Destinaton ������ 2D Ÿ�� �Ÿ��� ����ϴ� ����

        // { ���� ��尡 �����ϴ� ���, ���� ����� �ڽ�Ʈ�� �߰��ؼ� �����Ѵ�.
        if (prevNode == default || prevNode == null) { /* Do nothing */ }
        else
        {
            totalDistance2D = Mathf.RoundToInt(prevNode.AstarG + 1.0f);
        }
        targetNode.UpdateCost_Astar(
            totalDistance2D, heuristic, prevNode);
        // } ���� ��尡 �����ϴ� ���, ���� ����� �ڽ�Ʈ�� �߰��ؼ� �����Ѵ�.
       
    }       // Update_AStarCostToTerrain()
}       // class PathFInder
