using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public List<Obj> objPrefabList;
    public Transform spawnPoint;
    public Button playButton;
    public Text scoreLabel;
    public AudioSource collideAudio;
    public AudioSource winAudio;
    public AudioSource loseAudio;
    public int score;
    // public int cubeLeft;

    private Obj obj;
    private int objId;
    private bool isGameOver;
    // private float highestY;

    private List<Obj> objs = new List<Obj>();
    // Start is called before the first frame update
    void Start()
    {
        // obj = SpawnNextObj();
        // highestY = -10f;

    }

    // Update is called once per frame
    void Update()
    {
        if(isGameOver)
        {
            return;
        }

        if(score == -1)
        {
            OnGameLose();
        }

        // check whether collapse
        // var tempY = -10f;
        for (int i = 0; i < objs.Count; i++)
        {
            var objPosY = objs[i].rigid.position.y;
            
            if (objPosY < -6.5f) 
            {
                collideAudio.Play();
                
                Destroy(objs[i].gameObject);
                objs.RemoveAt(i);
            }
            // tempY = Mathf.Max(tempY, objPosY);
            // Debug.Log(objPosY);
        }
        // if (tempY < highestY)
        // {
        //     Debug.Log("collapse");
        // }

        if (Input.GetMouseButtonUp(0))
        {
            var mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            
            var objPos = new Vector3(worldPos.x, spawnPoint.position.y, -1);
            obj.gameObject.transform.position = objPos;

            obj.SetSimulated(true);
            // obj.rigid.velocity = new Vector3(0, -5, 0);
            // for (int i = 0; i < objs.Count-2; i++)
            // {
            //     var objPosY = objs[i].rigid.position.y;
            //     highestY = Mathf.Max(highestY, objPosY);
            // }
            obj = SpawnNextObj();
        }
    }

    private Obj SpawnNextObj()
    {
        var prob = Random.Range(0f, 1f);
        var rand = 0;
        if (prob > 0.9)
        {
            rand = Random.Range(objPrefabList.Count-2, objPrefabList.Count);
        }
        else
        {
            rand = Random.Range(0, objPrefabList.Count-2);
        }
        Debug.Log(rand);
        // var rand = Random.Range(0, objPrefabList.Count-2);
        var prefab = objPrefabList[rand].gameObject;
        var pos = spawnPoint.position;
        ChangeScore();

        return SpawnObj(prefab, pos);
    }

    private Obj SpawnObj(GameObject prefab, Vector3 pos)
    {

        var obj = Instantiate(prefab, pos, Quaternion.identity);
        var f = obj.GetComponent<Obj>();
        f.SetSimulated(false);
        f.id = objId++;
        // f.OnLevelUp = (a, b) =>
        // {
        //     if (isObjExist(a) && isObjExist(b))
        //     {
        //         var pos1 = a.gameObject.transform.position;
        //         var pos2 = b.gameObject.transform.position;
        //         var pos_new = (pos1 + pos2) * 0.5f;
        //         RemoveObj(a);
        //         RemoveObj(b);
        //         // AddScore(a.score);
        //         // Instantiate(a.nextLevelPrefab, pos_new, Quaternion.identity);
        //         collideAudio.Play();
        //         var fr = SpawnObj(a.nextLevelPrefab, pos_new);
        //         fr.SetSimulated(true);
        //     }
        // };

        f.OnGameWin = () =>
        {
            if (isGameOver == true)
            {
                return;
            }
            OnGameWin();
        };
        objs.Add(f);



        return f;
    }

    public void Restart()
    {

        for (int i = 0; i < objs.Count; i++)
        {
            objs[i].SetSimulated(false);
            Destroy(objs[i].gameObject);
        }

        objs.Clear();

        playButton.gameObject.SetActive(false);
        obj = SpawnNextObj();
        Debug.Log("Restart");
        score = 50;
        scoreLabel.text = "50";

        isGameOver = false;
    }

    private void OnGameWin()
    {
        isGameOver = true;
        playButton.gameObject.SetActive(true);
        winAudio.Play();
    }

    private void OnGameLose()
    {
        isGameOver = true;
        playButton.gameObject.SetActive(true);
        loseAudio.Play();
        // for (int i = 0; i < objs.Count; i++)
        // {
        //     objs[i].SetSimulated(false);
        //     Destroy(objs[i].gameObject);
        // }

        // objs.Clear();
    }

    private void RemoveObj(Obj f)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            if (objs[i].id == f.id)
            {
                objs.Remove(f);
                Destroy(f.gameObject);
                return;
            }
        }
    }

    private bool isObjExist(Obj f)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            if (objs[i].id == f.id)
            {
                return true;
            }
        }
        return false;
    }

    // private void AddScore(int score)
    // {
    //     this.score += score;
    //     scoreLabel.text = $"{this.score}";
    // }

    private void ChangeScore()
    {
        this.score -= 1;
        scoreLabel.text = $"{this.score}";
    }

}
