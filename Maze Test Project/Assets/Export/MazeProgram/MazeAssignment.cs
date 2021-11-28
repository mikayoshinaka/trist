using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeAssignment : MonoBehaviour
{
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
    public List<GameObject> bookstandList = new List<GameObject>();
    GameObject[] targetDeskPos;
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

    public void MazeAssign()
    {
        AnchorSaveBox = collectBoxPost.BoxSpots[collectBoxPost.saveBoxAnchor];

        enemyList.Clear();
        bookstandList.Clear();
        deskList.Clear();
        chairList.Clear();

        int saveBoxPost = collectBoxPost.saveBoxAnchor;
        int playerPost = collectBoxPost.playerAnchor;

        //int saveBoxPost = 0;
        //int playerPost = 1;

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


        // CLEAN ROAD PROGRAM
    }

    void AssignRole(List<GameObject> furnitureList, GameObject targetPos, LayerMask furnitureMask, float range)
    {
        // 家具
        GameObject furniture = ObjectCollider(furnitureList, targetPos, furnitureMask, range);

        // 敵
        GameObject enemy = ObjectCollider(enemyList, furniture, enemyMask, range);

        if (furniture != null && enemy != null)
        {
            enemy.GetComponent<EnemyBehaviour>().MazeGimmick(furniture, targetPos, furnitureMask);
        }
    }

    GameObject ObjectCollider(List<GameObject> objList, GameObject targetPos, LayerMask mask, float range)
    {
        int maxColliders = 25;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(targetPos.transform.position, range, hitColliders, mask);
        float distance = range;
        GameObject target = null;
        for (int i = 0; i < numColliders; i++)
        {
            float targetDistance = Vector3.Distance(targetPos.transform.position, hitColliders[i].transform.position);
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

    void CleanRoad(string post)
    {
        transform.Find("RoadCleaner").Find(post).gameObject.SetActive(true);
    }
}
