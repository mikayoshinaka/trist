using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeAssignment : MonoBehaviour
{
    [Header("テスト用")]
    public bool enableMazeTest;

    CollectBoxPost collectBoxPost;
    Transform AnchorSaveBox;
    
    GameObject furnitures;
    GameObject maze;
    GameObject maze_bookstands;
    GameObject maze_desks;
    LayerMask bookstandMask;
    LayerMask chairMask;
    LayerMask deskMask;
    LayerMask enemyMask;

    // Maze
    List<GameObject> enemyList = new List<GameObject>();
    GameObject[] targetBookstandPos;
    GameObject[] targetDeskPos;
    [Space]
    public List<GameObject> bookstandList = new List<GameObject>();
    public List<GameObject> deskList = new List<GameObject>();
    public List<GameObject> chairList = new List<GameObject>();

    private void Start()
    {
        collectBoxPost = transform.Find("SaveBox").GetComponent<CollectBoxPost>();

        furnitures = GameObject.Find("Furnitures");

        maze = this.gameObject;
        maze_bookstands = maze.transform.Find("Bookstands").gameObject;
        maze_desks = maze.transform.Find("Desks").gameObject;
        bookstandMask = LayerMask.GetMask("Bookstand");
        chairMask = LayerMask.GetMask("Chair");
        deskMask = LayerMask.GetMask("Desk");
        enemyMask = LayerMask.GetMask("Enemy");
    }

    // NavMesh 管理
    public void MazeNavmesh(bool flag)
    {
        // Navmesh
        GameObject bookstands = GameObject.Find("Furnitures").transform.Find("Bookstands").gameObject;
        for (int i = 0; i < bookstands.transform.childCount; i++)
        {
            bookstands.transform.GetChild(i).Find("Navmesh").gameObject.SetActive(flag);
        }

        GameObject desks = GameObject.Find("Furnitures").transform.Find("Desks").gameObject;
        for (int i = 0; i < desks.transform.childCount; i++)
        {
            desks.transform.GetChild(i).Find("Navmesh").gameObject.SetActive(flag);
        }

        GameObject chairs = GameObject.Find("Furnitures").transform.Find("Chairs").gameObject;
        for (int i = 0; i < chairs.transform.childCount; i++)
        {
            chairs.transform.GetChild(i).Find("Navmesh").gameObject.SetActive(flag);
        }

        GameObject.Find("NavMeshPlayer").GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    
    // 全ての家具を SetActive(true)
    public void FurnitureActive()
    {
        furnitures = GameObject.Find("Furnitures");
        GameObject bookstands = furnitures.transform.Find("Bookstands").gameObject;
        GameObject desks = furnitures.transform.Find("Desks").gameObject;
        GameObject chairs = furnitures.transform.Find("Chairs").gameObject;

        for (int i = 0; i < bookstands.transform.childCount; i++)
        {
            bookstands.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < desks.transform.childCount; i++)
        {
            desks.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < chairs.transform.childCount; i++)
        {
            chairs.transform.GetChild(i).gameObject.SetActive(true);
        }

        GameObject roadCleaner = GameObject.Find("Maze").transform.Find("RoadCleaner").gameObject;
        for (int i = 0; i < roadCleaner.transform.childCount; i++)
        {
            roadCleaner.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 迷路開始、パターンを決める処理
    public void MazeAssign()
    {
        AnchorSaveBox = collectBoxPost.BoxSpots[collectBoxPost.saveBoxAnchor];

        enemyList.Clear();
        bookstandList.Clear();
        deskList.Clear();
        chairList.Clear();

        int saveBoxPost = collectBoxPost.saveBoxAnchor;
        int playerPost = collectBoxPost.playerAnchor;

        // テスト用
        if (enableMazeTest)
        {
            saveBoxPost = 9;
            playerPost = 9;
            if (saveBoxPost == 9 && playerPost == 9)
            {
                MazePattern("Post_TEST");
                CleanRoad("Post_TEST");
            }
        }
        
        // パターン　1
        if (saveBoxPost == 0)
        {
            if (playerPost == 1)
            {
                MazePattern("Post_1_A");
                CleanRoad("Post_1_A");
            }
            else if (playerPost == 2)
            {
                MazePattern("Post_1_B");
                CleanRoad("Post_1_B");
            }
            else if (playerPost == 3)
            {
                MazePattern("Post_1_C");
                CleanRoad("Post_1_C");
            }
        }

        // パターン　2
        else if (saveBoxPost == 1)
        {
            if (playerPost == 2)
            {
                MazePattern("Post_2_A");
                CleanRoad("Post_2_A");
            }
            else if (playerPost == 3)
            {
                MazePattern("Post_2_BC");
                CleanRoad("Post_2_BC");
            }
            else if (playerPost == 0)
            {
                MazePattern("Post_2_BC");
                CleanRoad("Post_2_BC");
            }
        }

        // パターン　3
        else if (saveBoxPost == 2)
        {
            if (playerPost == 3)
            {
                MazePattern("Post_3_A");
                CleanRoad("Post_3_A");
            }
            else if (playerPost == 0)
            {
                MazePattern("Post_3_B");
                CleanRoad("Post_3_B");
            }
            else if (playerPost == 1)
            {
                MazePattern("Post_3_C");
                CleanRoad("Post_3_C");
            }
        }

        // パターン　4
        else if (saveBoxPost == 3)
        {
            if (playerPost == 0)
            {
                MazePattern("Post_4_AB");
                CleanRoad("Post_4_AB");
            }
            else if (playerPost == 1)
            {
                MazePattern("Post_4_AB");
                CleanRoad("Post_4_AB");
            }
            else if (playerPost == 2)
            {
                MazePattern("Post_4_C");
                CleanRoad("Post_4_C");
            }
        }
    }

    // 決めたパターンにより、家具を設定する処理
    void MazePattern(string pattern)
    {
        // Bookstandオブジェクト
        GameObject targetParent = maze_bookstands.transform.Find(pattern).gameObject;
        targetBookstandPos = new GameObject[targetParent.transform.childCount];
        for (int i = 0; i < targetBookstandPos.Length; i++)
        {
            targetBookstandPos[i] = targetParent.transform.GetChild(i).gameObject;
            AssignRole(bookstandList, targetBookstandPos[i], bookstandMask, 100f);
        }

        // Deskオブジェクト
        targetParent = maze_desks.transform.Find(pattern).gameObject;
        targetDeskPos = new GameObject[targetParent.transform.childCount];
        for (int i = 0; i < targetDeskPos.Length; i++)
        {
            targetDeskPos[i] = targetParent.transform.GetChild(i).gameObject;
            AssignRole(deskList, targetDeskPos[i], deskMask, 100f);
        }

        // Chairオブジェクト
        for (int i = 0; i < 2; i++)
        {
            AssignRole(chairList, AnchorSaveBox.gameObject, chairMask, 100f);
        }
    }
    
    // 家具とお化けの役割
    void AssignRole(List<GameObject> furnitureList, GameObject targetPos, LayerMask furnitureMask, float range)
    {
        // 家具
        GameObject furniture = ObjectCollider(furnitureList, targetPos, furnitureMask, range);

        // 敵
        GameObject enemy = ObjectCollider(enemyList, furniture, enemyMask, range);

        if (furniture != null && enemy != null)
        {
            if (enemy.tag == "NormalGhost")
            {
                enemy.GetComponent<EnemyBehaviour>().MazeGimmick(furniture, targetPos, furnitureMask);
            }
        }
    }

    // 役割用のコライダー
    GameObject ObjectCollider(List<GameObject> objList, GameObject targetPos, LayerMask mask, float range)
    {
        int maxColliders = 25;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(targetPos.transform.position, range, hitColliders, mask);
        float distance = range;
        GameObject target = null;
        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].transform.parent.tag == "DonyoriGhost")
            {
                continue;
            }

            // Vector3.Distanceによる距離計算
            float targetDistance = Vector3.Distance(targetPos.transform.position, hitColliders[i].transform.position);

            // NavMeshによる距離計算
            //NavMeshPath path = new NavMeshPath();
            //NavMesh.CalculatePath(targetPos.transform.position, hitColliders[i].transform.position, NavMesh.AllAreas, path);
            //float targetDistance = 0f;

            //if ((path.status != NavMeshPathStatus.PathInvalid))
            //{
            //    for (int j = 1; j < path.corners.Length; ++j)
            //    {
            //        targetDistance += Vector3.Distance(path.corners[j - 1], path.corners[j]);
            //    }
            //}

            if (targetDistance < distance && !objList.Contains(hitColliders[i].transform.parent.gameObject))
            {
                distance = targetDistance;
                target = hitColliders[i].transform.parent.gameObject;
            }
        }

        //Debug.Log(target.name);
        objList.Add(target);
        return target;
    }

    // RoadCleaner用
    void CleanRoad(string post)
    {
        transform.Find("RoadCleaner").Find(post).gameObject.SetActive(true);
    }
}
