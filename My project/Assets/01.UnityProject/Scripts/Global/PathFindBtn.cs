
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindBtn : MonoBehaviour
{
    //! A Star find 버튼을 누른 경우
    public void OnClickAStarFindBtn()
    {
        PathFinder.Instance.FindPath_Astar();
    }       // OnClickAStarFindBtn()
}
