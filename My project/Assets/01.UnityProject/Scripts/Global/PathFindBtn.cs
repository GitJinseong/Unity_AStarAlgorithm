
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindBtn : MonoBehaviour
{
    //! A Star find ��ư�� ���� ���
    public void OnClickAStarFindBtn()
    {
        PathFinder.Instance.FindPath_Astar();
    }       // OnClickAStarFindBtn()
}
